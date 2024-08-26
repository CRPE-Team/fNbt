using System;

namespace fNbt.Serialization.Converters {
    public class ByteNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(byte)
                || type == typeof(sbyte);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.Byte;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return stream.ReadByte();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.Byte);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            stream.Write((byte)value);
        }
    }
}
