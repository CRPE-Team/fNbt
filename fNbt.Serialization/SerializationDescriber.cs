using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using fNbt.Serialization.Converters;
using fNbt.Serialization.NbtObject;

namespace fNbt.Serialization {
    internal static class SerializationDescriber {
        private static readonly ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache> _cache = new ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache>();

        public static NbtSerializationCache Describe(Type type, NbtSerializerSettings settings, NbtPropertyAttribute attribute = null) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? NbtSerializer.DefaultSettings
            };

            return _cache.GetOrAdd(profile, p => Describe(p, attribute));
        }

        private static NbtSerializationCache Describe(NbtSerializationProfile profile, NbtPropertyAttribute attribute) {
            NbtConverter converter = null;
            var type = profile.ObjectType;
            var settings = profile.Settings;

            if (attribute?.Converter != null && attribute.Converter.CanConvert(type)) {
                converter = attribute.Converter;
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
                }
            }

            var cache = new NbtSerializationCache() {
                Converter = converter,
                Type = type,
                Settings = settings
            };

            _cache.TryAdd(profile, cache);

            if (converter == null) {
                var properties = type.GetProperties();

                foreach (var property in properties) {
                    if (property.GetCustomAttribute<NbtIgnoreAttribute>() != null) {
                        continue;
                    }

                    var atr = property.GetCustomAttribute<NbtPropertyAttribute>();
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

            return cache;
        }
    }
}
