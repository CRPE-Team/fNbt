using System;
using System.IO;

namespace fNbt.Serialization {
    public static class NbtConvert
    {
        public static T FromNbt<T>(string filePath, NbtSerializerSettings settings = null) {
            return NbtSerializer.Read<T>(File.OpenRead(filePath), settings);
        }

        public static T FromNbt<T>(NbtFile nbt, NbtSerializerSettings settings = null) {
            return (T)FromNbt(typeof(T), nbt, settings);
        }

        public static T FromNbt<T>(NbtTag tag, NbtSerializerSettings settings = null) {
            return (T)FromNbt(typeof(T), tag, settings);
        }

        public static object FromNbt(Type type, NbtFile nbt, NbtSerializerSettings settings = null) {
            return FromNbt(type, nbt.RootTag, settings);
        }

        public static object FromNbt(Type type, NbtTag tag, NbtSerializerSettings settings = null) {
            return NbtSerializer.FromNbtInternal(type, tag, settings);
        }

        public static NbtFile ToNbtFile(object value, NbtSerializerSettings settings = null) {
            return new NbtFile() { RootTag = ToNbt<NbtCompound>(value, settings) };
        }

        public static TTag ToNbt<TTag>(object value, NbtSerializerSettings settings = null) where TTag : NbtTag {
            return (TTag) ToNbt(value, string.Empty, settings);
        }

        public static NbtTag ToNbt(object value, NbtSerializerSettings settings = null) {
            return ToNbt(value, string.Empty, settings);
        }

        public static NbtTag ToNbt(object value, string name, NbtSerializerSettings settings = null) {
            return NbtSerializer.ToNbtInternal(value, name, settings);
        }
    }
}
