using System;

namespace fNbt.Serialization.Converters {
    public class ByteNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(byte)
                || type == typeof(sbyte);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Byte;
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return stream.ReadByte();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.Byte);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            stream.Write((byte)value);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.ByteValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtByte(name, (byte)value);
        }
    }
}
