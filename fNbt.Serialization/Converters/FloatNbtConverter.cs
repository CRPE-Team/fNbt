using System;

namespace fNbt.Serialization.Converters {
    public class FloatNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(float);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.Float;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return stream.ReadSingle();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.Float);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            stream.Write((float)value);
        }
    }
}
