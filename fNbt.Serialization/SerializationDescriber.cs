using System;
using System.Collections;
using System.Reflection;
using fNbt.Serialization.Converters;
using fNbt.Serialization.NbtObject;

namespace fNbt.Serialization {
    internal static class SerializationDescriber {
        public static NbtSerializationCache Describe(NbtSerializationProfile profile, NbtPropertyAttribute attribute) {
            return Describe(profile.ObjectType, attribute, profile.Settings);
        }

        private static NbtSerializationCache Describe(Type type, NbtPropertyAttribute attribute, NbtSerializationSettings settings) {
            NbtConverter converter = null;

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
                        ElementSerializationCache = NbtSerializer.GetCache(gen, settings)
                    };
                } else if (type.IsArray) {
                    var gen = type.GetElementType();

                    converter = new ArrayNbtConverter() {
                        ElementSerializationCache = NbtSerializer.GetCache(gen, settings)
                    };
                } else if (typeof(IList).IsAssignableFrom(type)) {
                    var gen = type.GetGenericArguments()[0];

                    converter = new ListNbtConverter() {
                        ElementSerializationCache = NbtSerializer.GetCache(gen, settings)
                    };
                }
            }

            var cache = new NbtSerializationCache() {
                Converter = converter,
                Type = type,
                Settings = settings
            };

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
                        SerializationCache = NbtSerializer.GetCache(property.PropertyType, settings, atr),
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
