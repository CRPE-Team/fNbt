using System;
using System.IO;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    public static class NbtSerializer {
        internal static DynamicNbtConverter DynamicNbtConverter { get; } = new DynamicNbtConverter();

        public static T Read<T>(Stream stream, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static T Read<T>(NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static object Read(Type type, Stream stream, NbtSerializerSettings settings = null) {
            return ReadInternal(type, new NbtBinaryReader(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), null, null, settings);
        }

        public static object Read(Type type, NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return ReadInternal(type, stream, null, null, settings);
        }

        public static object Read(object value, Stream stream, NbtSerializerSettings settings = null) {
            return Read(value, new NbtBinaryReader(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), settings);
        }

        public static object Read(object value, NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return ReadInternal(value.GetType(), stream, value, null, settings);
        }

        public static void Write(object value, Stream stream, NbtSerializerSettings settings = null) {
            WriteInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), string.Empty, settings);
        }

        public static void Write(object value, NbtBinaryWriter stream, NbtSerializerSettings settings = null) {
            WriteInternal(value, stream, string.Empty, settings);
        }

        public static void WriteData(object value, Stream stream, NbtSerializerSettings settings = null) {
            WriteDataInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), settings);
        }

        public static void WriteData(object value, NbtBinaryWriter stream, NbtSerializerSettings settings = null) {
            WriteDataInternal(value, stream, settings);
        }

        internal static NbtTagType GetTagTypeInternal(Type type, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).GetTagType(type);
        }

        internal static object ReadInternal(Type type, NbtBinaryReader stream, object value, string name, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).Read(stream, value, name);
        }

        internal static void WriteInternal(object value, NbtBinaryWriter stream, string name, NbtSerializerSettings settings) {
            if (value == null) return;

            SerializationDescriber.Describe(value.GetType(), settings).Write(stream, value, name);
        }

        internal static void WriteDataInternal(object value, NbtBinaryWriter stream, NbtSerializerSettings settings) {
            if (value == null) {
                throw new NbtSerializationException($"Unexpected null reference detected");
            }

            SerializationDescriber.Describe(value.GetType(), settings).WriteData(stream, value);
        }

        internal static object FromNbtInternal(Type type, NbtTag tag, object value, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).FromNbt(tag, value);
        }

        internal static NbtTag ToNbtInternal(object value, string name, NbtSerializerSettings settings) {
            if (value == null) return null;

            return SerializationDescriber.Describe(value.GetType(), settings).ToNbt(value, name);
        }
    }
}
