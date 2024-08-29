using System;
using System.Collections.Generic;
using System.IO;
using fNbt.Serialization.Converters;
using fNbt.Serialization.NamingStrategy;

namespace fNbt.Serialization {
    public static class NbtSerializer {
        public static NbtSerializerSettings DefaultSettings { get; } = new NbtSerializerSettings() {
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

        public static T Read<T>(Stream stream, NbtSerializerSettings settings = null) {
            return Read<T>(new NbtBinaryReader(stream, settings?.Flavor ?? DefaultSettings.Flavor), settings);
        }

        public static T Read<T>(NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static object Read(Type type, NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return ReadInternal(type, stream, null, settings);
        }

        public static void Write(object value, Stream stream, NbtSerializerSettings settings = null) {
            WriteInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? DefaultSettings.Flavor), string.Empty, settings);
        }

        public static void Write(object value, NbtBinaryWriter stream, NbtSerializerSettings settings = null) {
            WriteInternal(value, stream, string.Empty, settings);
        }

        internal static NbtTagType GetTagTypeInternal(Type type, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).GetTagType(type);
        }

        internal static object ReadInternal(Type type, NbtBinaryReader stream, string name, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).Read(stream, name);
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

        internal static object FromNbtInternal(Type type, NbtTag tag, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).FromNbt(tag);
        }

        internal static NbtTag ToNbtInternal(object value, string name, NbtSerializerSettings settings) {
            if (value == null) return null;

            return SerializationDescriber.Describe(value.GetType(), settings).ToNbt(value, name);
        }
    }
}
