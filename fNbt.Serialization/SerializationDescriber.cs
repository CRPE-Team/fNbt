using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    internal static class SerializationDescriber {
        private static readonly ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache> _cache = new ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache>();

        public static NbtSerializationCache Describe(Type type, NbtSerializerSettings settings, NbtPropertyAttribute attribute = null) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? NbtSerializer.DefaultSettings,
                PropertyConverterType = attribute?.ConverterType
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
                foreach (var c in NbtSerializer.DefaultSettings.Converters) {
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
                    var gen = type.GetElementType();

                    converter = new ArrayNbtConverter() {
                        ElementSerializationCache = Describe(gen, settings)
                    };
                } else if (typeof(IList).IsAssignableFrom(type)) {
                    var gen = type.GetGenericArguments()[0];

                    converter = new ListNbtConverter() {
                        ElementSerializationCache = Describe(gen, settings)
                    };
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

            var cache = new NbtSerializationCache() {
                Converter = converter,
                Type = type,
                Settings = settings
            };

            _cache.TryAdd(profile, cache);

            try {
                if (converter == null) {
                    var properties = type.GetProperties();

                    foreach (var property in properties) {
                        if (property.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                            continue;
                        }
                        if (property.PropertyType.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                            continue;
                        }

                        var atr = property.GetCustomAttribute<NbtPropertyAttribute>();
                        if (settings.NbtPropertyHandling == Handlings.NbtPropertyHandling.MarkedOnly 
                            && atr == null
                            && property.PropertyType.GetCustomAttribute<NbtPropertyAttribute>() == null) {
                            continue;
                        }

                        var name = atr?.Name;

                        if (name == null) {
                            name = (settings.NamingStrategy ?? NbtSerializer.DefaultSettings.NamingStrategy).ResolvePropertyName(property.Name);
                        }

                        var nbtProperty = new NbtSerializationProperty() {
                            Type = property.PropertyType,
                            Name = name,
                            SerializationCache = Describe(property.PropertyType, settings, atr),
                            Settings = settings
                        };

                        if (property.GetMethod != null) {
                            nbtProperty.Get = property.GetMethod.Invoke;
                        }
                        if (property.SetMethod != null) {
                            nbtProperty.Set = property.SetMethod.Invoke;
                        }

                        cache.Properties.Add(name, nbtProperty);
                    }
                }
            } catch {
                // remove bad cache
                _cache.TryRemove(profile, out _);
                throw;
            }

            return cache;
        }
    }
}
