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

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.Compound;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            var dictionary = (IDictionary)Activator.CreateInstance(type);

            while (ReadProperty(stream, out var valueName, out var value)) {
                dictionary.Add(valueName, value);
            }

            return dictionary;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.Compound);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
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
