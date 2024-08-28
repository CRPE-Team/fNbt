using System;
using System.Collections;

namespace fNbt.Serialization.Converters {
    internal class DictionaryNbtConverter : NbtConverter {
        public NbtSerializationCache ElementSerializationCache { get; set; }

        public override bool CanConvert(Type type) {
            if (!typeof(IDictionary).IsAssignableFrom(type)) return false;

            var generic = type.GetGenericArguments();
            return generic[0] == typeof(string) && generic[1] == ElementSerializationCache.Type;
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Compound;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            var dictionary = (IDictionary)Activator.CreateInstance(type);

            while (ReadProperty(stream, out var valueName, out var value)) {
                dictionary.Add(valueName, value);
            }

            return dictionary;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.Compound);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var dictionary = (IDictionary)value;

            foreach (dynamic pair in dictionary) {
                object pairValue = pair.Value;

                if (pairValue == null) {
                    if (settings.NullReferenceHandling == Handlings.NullReferenceHandling.Error) {
                        throw new NbtSerializationException($"Null reference of [{pairValue.GetType()}] detected");
                    }
                }

                ElementSerializationCache.Write(stream, pairValue, pair.Key.ToString());
            }

            stream.Write(NbtTagType.End);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            var dictionary = (IDictionary)Activator.CreateInstance(type);

            foreach (var child in tag as NbtCompound) {
                dictionary.Add(child.Name, ElementSerializationCache.FromNbt(child));
            }

            return dictionary;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            var dictionary = (IDictionary)value;
            var compound = new NbtCompound(name);

            foreach (dynamic pair in dictionary) {
                object pairValue = pair.Value;

                if (pairValue == null) {
                    if (settings.NullReferenceHandling == Handlings.NullReferenceHandling.Error) {
                        throw new NbtSerializationException($"Null reference of [{pairValue.GetType()}] detected");
                    }
                }

                var tag = ElementSerializationCache.ToNbt(pairValue, pair.Key.ToString());

                if (tag != null) {
                    compound.Add(tag);
                }
            }

            return compound;
        }

        private bool ReadProperty(NbtBinaryReader stream, out string name, out object obj) {
            name = null;
            obj = null;
            var tagType = stream.ReadTagType();

            if (tagType == NbtTagType.End) return false;

            if (tagType == NbtTagType.Unknown) {
                throw new NbtSerializationException("Readed unknown tag type");
            }

            name = stream.ReadString();
            obj = ElementSerializationCache.Read(stream, name);
            return true;
        }
    }
}
