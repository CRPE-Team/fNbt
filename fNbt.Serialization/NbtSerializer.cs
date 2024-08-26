using System;
using System.Collections.Concurrent;
using System.IO;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    public static class NbtSerializer {
        private static readonly ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache> _cache = new ConcurrentDictionary<NbtSerializationProfile, NbtSerializationCache>();

        public static NbtSerializationSettings DefaultSettings { get; } = new NbtSerializationSettings();

        internal static ObjectNbtConverter ObjectNbtConverter { get; } = new ObjectNbtConverter();

        public static T Read<T>(Stream stream, NbtSerializationSettings settings = null) {
            return Read<T>(new NbtBinaryReader(stream, settings.Flavor), settings);
        }

        public static T Read<T>(NbtBinaryReader stream, NbtSerializationSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static object Read(Type type, NbtBinaryReader stream, NbtSerializationSettings settings = null) {
            return ReadInternal(type, stream, null, settings);
        }

        internal static NbtTagType GetTagTypeInternal(Type type, NbtSerializationSettings settings) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? DefaultSettings
            };

            return _cache.GetOrAdd(profile, SerializationDescriber.Describe)
                .GetTagType(type);
        }

        internal static object ReadInternal(Type type, NbtBinaryReader stream, string name, NbtSerializationSettings settings) {
            var profile = new NbtSerializationProfile() {
                ObjectType = type,
                Settings = settings ?? DefaultSettings
            };

            return _cache.GetOrAdd(profile, SerializationDescriber.Describe)
                .Read(stream, name);
        }

        internal static void WriteInternal(object value, NbtBinaryWriter stream, string name, NbtSerializationSettings settings) {
            var profile = new NbtSerializationProfile() {
                ObjectType = value.GetType(),
                Settings = settings ?? DefaultSettings
            };

            _cache.GetOrAdd(profile, SerializationDescriber.Describe)
                .Write(stream, value, name);
        }

        internal static void WriteDataInternal(object value, NbtBinaryWriter stream, NbtSerializationSettings settings) {
            var profile = new NbtSerializationProfile() {
                ObjectType = value.GetType(),
                Settings = settings ?? DefaultSettings
            };

            _cache.GetOrAdd(profile, SerializationDescriber.Describe)
                .WriteData(stream, value);
        }
    }
}
