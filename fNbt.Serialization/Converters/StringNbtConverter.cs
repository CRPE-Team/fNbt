using System;

namespace fNbt.Serialization.Converters {
    public class StringNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(string);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.String;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return stream.ReadString();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.String);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            stream.Write((string)value);
        }
    }
}
