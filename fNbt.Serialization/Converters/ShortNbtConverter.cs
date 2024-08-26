using System;

namespace fNbt.Serialization.Converters {
    public class ShortNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(short)
                || type == typeof(ushort);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.Short;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return stream.ReadInt16();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.Short);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            stream.Write((short)value);
        }
    }
}
