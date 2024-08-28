using System;

namespace fNbt.Serialization.Converters {
    public class IntNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(int)
                || type == typeof(uint);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Int;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            return stream.ReadInt32();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.Int);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            stream.Write((int)value);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            return tag.IntValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtInt(name, (int)value);
        }
    }
}
