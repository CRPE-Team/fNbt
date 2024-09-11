using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    internal static class SerializationDescriber {
        private static readonly ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache> _cache = new ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache>();

        public static NbtSerializationCache Describe(Type type, NbtSerializerSettings settings, NbtPropertyAttribute attribute = null, bool skipConverter = false) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? NbtSerializerSettings.DefaultSettings,
                PropertyConverterType = attribute?.ConverterType,
                SkipConverter = skipConverter
            };

            return _cache.GetOrAdd(profile, p => Describe(p, attribute));
        }

        private static NbtSerializationCache Describe(NbtSerializationProfile profile, NbtPropertyAttribute attribute) {
            NbtConverter converter = null;
            var type = profile.ObjectType;
            var settings = profile.Settings;

            if (Nullable.GetUnderlyingType(type) != null) {
                type = Nullable.GetUnderlyingType(type);
            }

            if (!profile.SkipConverter) {
                if (attribute?.ConverterType != null) {
                    var propertyConverter = (NbtConverter)Activator.CreateInstance(attribute.ConverterType);

                    if (propertyConverter.CanConvert(type)) {
                        converter = propertyConverter;
                    }
                }

                if (converter == null) {
                    var objectAttribute = type.GetCustomAttribute<NbtObjectAttribute>();
                    if (objectAttribute?.ConverterType != null) {
                        var objectConverter = (NbtConverter)Activator.CreateInstance(objectAttribute.ConverterType);

                        if (objectConverter.CanConvert(type)) {
                            converter = objectConverter;
                        }
                    }
                }

                if (converter == null) {
                    foreach (var c in settings.Converters) {
                        if (c.CanConvert(type)) {
                            converter = c;
                            break;
                        }
                    }
                }

                if (converter == null) {
                    foreach (var c in NbtSerializerSettings.DefaultSettings.Converters) {
                        if (c.CanConvert(type)) {
                            converter = c;
                            break;
                        }
                    }
                }

                if (converter == null) {
                    if (typeof(IDictionary).IsAssignableFrom(type)) {
                        var gen = type.GetGenericArguments()[1];

                        converter = new DictionaryNbtConverter() {
                            ElementSerializationCache = Describe(gen, settings)
                        };
                    } else if (type.IsArray) {
                        converter = new ArrayNbtConverter(type.GetElementType());
                    } else if (typeof(IList).IsAssignableFrom(type)) {
                        converter = new ListNbtConverter(type.GetGenericArguments()[0]);
                    } else if (type.IsEnum) {
                        var c = Describe(Enum.GetUnderlyingType(type), settings);
                        if (c.Converter == null) {
                            return c;
                        }

                        converter = new InternalEnumNbtConverter() {
                            UnderlyingTypeConverter = c.Converter
                        };
                    }
                }
            }

            var cache = new NbtSerializationCache() {
                Converter = converter,
                Type = type,
                Settings = settings
            };

            _cache.TryAdd(profile, cache);

            try {
                var publicInstance = BindingFlags.Public | BindingFlags.Instance;
                var nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

                var publicMembers = GetAndValidateProperties(type, publicInstance)
                    .Cast<MemberInfo>()
                    .Concat(type.GetFields(publicInstance));

                var nonPublicMembers = GetAndValidateProperties(type, nonPublicInstance)
                    .Cast<MemberInfo>()
                    .Concat(type.GetFields(nonPublicInstance))
                    .Where(member => member.GetCustomAttributes().Any(atr => typeof(NbtPropertyAttribute).IsAssignableFrom(atr.GetType())));

                var members = publicMembers.Concat(nonPublicMembers);

                foreach (var member in members) {
                    var memberType = GetMemberType(member);

                    if (member.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                        continue;
                    }
                    if (memberType.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                        continue;
                    }

                    var flatAtr = member.GetCustomAttribute<NbtFlatPropertyAttribute>();
                    var atr = flatAtr ?? member.GetCustomAttribute<NbtPropertyAttribute>();
                    if (settings.NbtMemberHandling == Handlings.NbtMemberHandling.MarkedOnly 
                        && atr == null
                        && type.GetCustomAttribute<NbtObjectAttribute>() == null) {
                        continue;
                    }

                    var namingStrategy = settings.NamingStrategy ?? NbtSerializerSettings.DefaultSettings.NamingStrategy;
                    var memberCache = Describe(memberType, settings, atr);

                    if (flatAtr == null || memberCache.Converter != null) {
                        var name = atr?.Name
                        ?? namingStrategy.ResolveMemberName(member.Name);

                        cache.Members.Add(name, CreateMember(member, memberCache, name, settings));
                    } else {
                        var flatNamingStrategy = flatAtr.NamingStrategy ?? namingStrategy;

                        foreach (var flatMember in memberCache.Members) {
                            var flatNbtMember = (INbtSerializationMember)flatMember.Value.Clone();
                            flatNbtMember.Name = flatNamingStrategy.ResolveMemberName(flatNbtMember.Origin.Name);

                            var flatCache = new NbtSerializationCache() {
                                Type = memberCache.Type,
                                Settings = memberCache.Settings,
                                Converter = new FlatMemberConverter() {
                                    Member = flatNbtMember
                                }
                            };

                            cache.Members.Add(flatNbtMember.Name, CreateMember(member, flatCache, flatNbtMember.Name, settings));
                        }
                    }
                }
            } catch {
                // remove bad cache
                _cache.TryRemove(profile, out _);
                throw;
            }

            return cache;
        }

        private static Type GetMemberType(MemberInfo member) {
            if (member is PropertyInfo propertyInfo) {
                return propertyInfo.PropertyType;
            } else if (member is FieldInfo fieldInfo) {
                return fieldInfo.FieldType;
            }

            throw new NbtSerializationException($"member type [{member.GetType().Name}] is not supported");
        }

        private static INbtSerializationMember CreateMember(MemberInfo member, NbtSerializationCache cache, string name, NbtSerializerSettings settings) {
            if (member is PropertyInfo propertyInfo) {
                return CreateProperty(propertyInfo, cache, name, settings);
            } else if (member is FieldInfo fieldInfo) {
                return CreateField(fieldInfo, cache, name, settings);
            }

            throw new NbtSerializationException($"member type [{member.GetType().Name}] is not supported");
        }

        private static NbtSerializationField CreateField(FieldInfo fieldInfo, NbtSerializationCache cache, string name, NbtSerializerSettings settings) {
            var nbtField = new NbtSerializationField() {
                Type = fieldInfo.FieldType,
                Name = name,
                Origin = fieldInfo,
                SerializationCache = cache,
                Settings = settings,

                Get = fieldInfo.GetValue,
                Set = fieldInfo.SetValue,
            };

            return nbtField;
        }

        private static NbtSerializationProperty CreateProperty(PropertyInfo propertyInfo, NbtSerializationCache cache, string name, NbtSerializerSettings settings) {
            var nbtProperty = new NbtSerializationProperty() {
                Type = propertyInfo.PropertyType,
                Name = name,
                Origin = propertyInfo,
                SerializationCache = cache,
                Settings = settings
            };

            if (propertyInfo.GetMethod != null) {
                nbtProperty.Get = propertyInfo.GetMethod.Invoke;
            }
            if (propertyInfo.SetMethod != null) {
                nbtProperty.Set = propertyInfo.SetMethod.Invoke;
            }

            return nbtProperty;
        }

        private static IEnumerable<PropertyInfo> GetAndValidateProperties(Type type, BindingFlags bindingFlags) {
            // Because GetProperties may return NOT properties

            return type.GetProperties(bindingFlags)
                .Where(p => (p.GetMethod == null || !p.GetMethod.GetParameters().Any())
                    && (p.SetMethod == null || p.SetMethod.GetParameters().SingleOrDefault()?.ParameterType == p.PropertyType));
        }
    }
}
