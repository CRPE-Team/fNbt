using System;

namespace fNbt.Serialization.Converters {
    public class StringNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(string);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.String;
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return stream.ReadString();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.String);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            stream.Write(value.ToString());
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.StringValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtString(name, value.ToString());
        }
    }
}
