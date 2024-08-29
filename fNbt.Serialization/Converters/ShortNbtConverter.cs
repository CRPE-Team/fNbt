using System;

namespace fNbt.Serialization.Converters {
    public class ShortNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(short)
                || type == typeof(ushort);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Short;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            return stream.ReadInt16();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.Short);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            stream.Write((short)value);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            return tag.ShortValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtShort(name, (short)value);
        }
    }
}
