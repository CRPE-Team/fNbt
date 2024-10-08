﻿using System;
using System.IO;

namespace fNbt.Serialization {
    public static class NbtConvert
    {
        public static T ReadFromFile<T>(string filePath, NbtSerializerSettings settings = null) {
            return (T)ReadFromFile(typeof(T), filePath, NbtCompression.AutoDetect, settings);
        }

        public static T ReadFromFile<T>(string filePath, NbtCompression compression, NbtSerializerSettings settings = null) {
            return (T)ReadFromFile(typeof(T), filePath, compression, settings);
        }

        public static object ReadFromFile(Type type, string filePath, NbtSerializerSettings settings = null) {
            return ReadFromFile(type, filePath, NbtCompression.AutoDetect, settings);
        }

        public static object ReadFromFile(Type type, string filePath, NbtCompression compression, NbtSerializerSettings settings = null) {
            using (
                var stream = new FileStream(filePath,
                                                    FileMode.Open,
                                                    FileAccess.Read,
                                                    FileShare.Read,
                                                    NbtFile.FileStreamBufferSize,
                                                    FileOptions.SequentialScan)) {
                return NbtSerializer.Read(type, stream, compression, settings);
            }
        }

        public static void SaveToFile(object value, string filePath, NbtSerializerSettings settings = null) {
            SaveToFile(value, filePath, NbtCompression.None, settings);
        }

        public static void SaveToFile(object value, string filePath, NbtCompression compression, NbtSerializerSettings settings = null) {
            using (
                var stream = new FileStream(filePath,
                                              FileMode.Create,
                                              FileAccess.Write,
                                              FileShare.None,
                                              NbtFile.FileStreamBufferSize,
                                              FileOptions.SequentialScan)) {
                NbtSerializer.Write(value, stream, compression, settings);
            }
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
            return NbtSerializer.FromNbtInternal(type, tag, null, settings);
        }

        public static object FromNbt(object value, NbtTag tag, NbtSerializerSettings settings = null) {
            return NbtSerializer.FromNbtInternal(value.GetType(), tag, value, settings);
        }

        public static NbtFile ToNbtFile(object value, NbtSerializerSettings settings = null) {
            return new NbtFile() { RootTag = ToNbt<NbtCompound>(value, settings) };
        }

        public static TTag ToNbt<TTag>(object value, NbtSerializerSettings settings = null) where TTag : NbtTag {
            return ToNbt<TTag>(value, string.Empty, settings);
        }

        public static TTag ToNbt<TTag>(object value, string name, NbtSerializerSettings settings = null) where TTag : NbtTag {
            return (TTag)ToNbt(value, name, settings);
        }

        public static NbtTag ToNbt(object value, NbtSerializerSettings settings = null) {
            return ToNbt(value, string.Empty, settings);
        }

        public static NbtTag ToNbt(object value, string name, NbtSerializerSettings settings = null) {
            return NbtSerializer.ToNbtInternal(value, name, settings);
        }
    }
}
