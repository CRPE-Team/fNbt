using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using fNbt.Serialization.Converters;
using fNbt.Serialization.NamingStrategy;

namespace fNbt.Serialization {
    public static class NbtSerializer {
        private static readonly ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache> _cache = new ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache>();

        public static NbtSerializationSettings DefaultSettings { get; } = new NbtSerializationSettings() {
            NamingStrategy = new DefaultNbtNamingStrategy(),
            Converters = new List<NbtConverter>() {
                new ByteArrayNbtConverter(),
                new ByteNbtConverter(),
                new DoubleNbtConverter(),
                new FloatNbtConverter(),
                new IntArrayNbtConverter(),
                new IntNbtConverter(),
                new LongArrayNbtConverter(),
                new LongNbtConverter(),
                new ShortNbtConverter(),
                new StringNbtConverter(),
                new TagNbtConverter()
            }
        };

        internal static ObjectNbtConverter ObjectNbtConverter { get; } = new ObjectNbtConverter();

        public static T Read<T>(Stream stream, NbtSerializationSettings settings = null) {
            return Read<T>(new NbtBinaryReader(stream, settings?.Flavor ?? DefaultSettings.Flavor), settings);
        }

        public static T Read<T>(NbtBinaryReader stream, NbtSerializationSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static object Read(Type type, NbtBinaryReader stream, NbtSerializationSettings settings = null) {
            return ReadInternal(type, stream, null, settings);
        }

        public static void Write(object value, Stream stream, NbtSerializationSettings settings = null) {
            WriteInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? DefaultSettings.Flavor), string.Empty, settings);
        }

        public static void Write(object value, NbtBinaryWriter stream, NbtSerializationSettings settings = null) {
            WriteInternal(value, stream, string.Empty, settings);
        }

        internal static NbtTagType GetTagTypeInternal(Type type, NbtSerializationSettings settings) {
            return GetCache(type, settings).GetTagType(type);
        }

        internal static object ReadInternal(Type type, NbtBinaryReader stream, string name, NbtSerializationSettings settings) {
            return GetCache(type, settings).Read(stream, name);
        }

        internal static void WriteInternal(object value, NbtBinaryWriter stream, string name, NbtSerializationSettings settings) {
            GetCache(value.GetType(), settings).Write(stream, value, name);
        }

        internal static void WriteDataInternal(object value, NbtBinaryWriter stream, NbtSerializationSettings settings) {
            GetCache(value.GetType(), settings).WriteData(stream, value);
        }

        internal static NbtSerializationCache GetCache(Type type, NbtSerializationSettings settings, NbtPropertyAttribute attribute = null) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? DefaultSettings
            };

            return _cache.GetOrAdd(profile, p => SerializationDescriber.Describe(p, attribute));
        }
    }
}
