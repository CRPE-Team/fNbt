using System;
using System.Collections;
using System.Collections.Concurrent;
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
                var properties = type.GetProperties();

                foreach (var property in properties) {
                    if (property.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                        continue;
                    }
                    if (property.PropertyType.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                        continue;
                    }

                    var flatAtr = property.GetCustomAttribute<NbtFlatPropertyAttribute>();
                    var atr = flatAtr ?? property.GetCustomAttribute<NbtPropertyAttribute>();
                    if (settings.NbtPropertyHandling == Handlings.NbtPropertyHandling.MarkedOnly 
                        && atr == null
                        && type.GetCustomAttribute<NbtObjectAttribute>() == null) {
                        continue;
                    }

                    var namingStrategy = settings.NamingStrategy ?? NbtSerializerSettings.DefaultSettings.NamingStrategy;
                    var propertyCache = Describe(property.PropertyType, settings, atr);

                    if (flatAtr == null || propertyCache.Converter != null) {
                        var name = atr?.Name
                        ?? namingStrategy.ResolvePropertyName(property.Name);

                        cache.Properties.Add(name, CreateProperty(property, propertyCache, name, settings));
                    } else {
                        var flatNamingStrategy = flatAtr.NamingStrategy ?? namingStrategy;

                        foreach (var flatProperty in propertyCache.Properties) {
                            var flatNbtProperty = (NbtSerializationProperty)flatProperty.Value.Clone();
                            flatNbtProperty.Name = flatNamingStrategy.ResolvePropertyName(flatNbtProperty.Origin.Name);

                            var flatCache = new NbtSerializationCache() {
                                Type = propertyCache.Type,
                                Settings = propertyCache.Settings,
                                Converter = new FlatPropertyConverter() {
                                    Property = flatProperty.Value
                                }
                            };

                            cache.Properties.Add(flatNbtProperty.Name, CreateProperty(property, flatCache, flatNbtProperty.Name, settings));
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
    }
}
