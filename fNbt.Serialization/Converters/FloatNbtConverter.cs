using System;

namespace fNbt.Serialization.Converters {
    public class FloatNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(float);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Float;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            return stream.ReadSingle();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.Float);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            stream.Write((float)value);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            return tag.FloatValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtFloat(name, (float)value);
        }
    }
}
