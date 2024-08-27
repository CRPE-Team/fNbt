using System;
using System.Collections.Generic;
using fNbt.Serialization.Converters;
using fNbt.Serialization.NbtObject;

namespace fNbt.Serialization {
    internal class NbtSerializationCache {
        public Type Type { get; set; }

        public Dictionary<string, NbtSerializationProperty> Properties { get; set; } = new Dictionary<string, NbtSerializationProperty>();

        public NbtConverter Converter { get; set; }

        public NbtSerializationSettings Settings { get; set; }

        public NbtTagType GetTagType(Type type) {
            if (Converter != null) {
                return Converter.GetTagType(type, Settings);
            } else if (type != Type) {
                return NbtSerializer.ObjectNbtConverter.GetTagType(type, Settings);
            } else {
                return NbtTagType.Compound;
            }
        }

        public object Read(NbtBinaryReader stream, string name = null) {
            if (name == null) {
                var tagType = stream.ReadTagType();

                if (tagType == NbtTagType.Unknown) {
                    throw new NbtSerializationException($"Readed unknown tag type of [{Type}]");
                }

                name = stream.ReadString();
            }

            if (Converter != null && Converter.CanRead) {
                return Converter.Read(stream, Type, name, Settings);
            } else {
                var obj = Activator.CreateInstance(Type);

                while (ReadProperty(obj, stream)) ;

                return obj;
            }
        }

        public void Write(NbtBinaryWriter stream, object value, string name) {
            if (true) { //если включена настройка! (по умолчанию включена)
                // проверяем call stack по потоковой переменной List (RuntimeLoopHanling)
            }

            if (Converter != null && Converter.CanWrite) {
                Converter.Write(stream, value, name, Settings);
            } else if (value.GetType() != Type) {
                NbtSerializer.ObjectNbtConverter.Write(stream, value, name, Settings);
            } else {
                if (Settings.NullReferenceHandling != Handlings.NullReferenceHandling.Ignore) {
                    stream.Write(NbtTagType.Compound);
                    stream.Write(name);
                }

                WriteCompoundData(stream, value);
            }
        }

        public void WriteData(NbtBinaryWriter stream, object value) {
            if (Converter != null && Converter.CanWrite) {
                Converter.WriteData(stream, value, Settings);
            } else if (value.GetType() != Type) {
                NbtSerializer.ObjectNbtConverter.WriteData(stream, value, Settings);
            } else {
                WriteCompoundData(stream, value);
            }
        }

        private bool ReadProperty(object obj, NbtBinaryReader stream) {
            var tagType = stream.ReadTagType();

            if (tagType == NbtTagType.End) return false;

            if (tagType == NbtTagType.Unknown) {
                throw new NbtSerializationException("Readed unknown tag type");
            }

            var name = stream.ReadString();

            if (!Properties.TryGetValue(name, out var property)
                    && Settings.MissingMemberHandling == Handlings.MissingMemberHandling.Error) {
                throw new NbtSerializationException($"Missing member [{name}]");
            }

            property.Read(obj, stream);
            return true;
        }

        private void WriteCompoundData(NbtBinaryWriter stream, object value) {
            if (value == null) {
                if (Settings.NullReferenceHandling == Handlings.NullReferenceHandling.Error) {
                    throw new NbtSerializationException($"Null reference of [{Type}] detected");
                }
            } else {
                foreach (var property in Properties.Values) {
                    property.Write(value, stream);
                }
            }

            stream.Write(NbtTagType.End);
        }
    }
}
