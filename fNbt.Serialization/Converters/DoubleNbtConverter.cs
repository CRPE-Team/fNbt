using System;

namespace fNbt.Serialization.Converters {
    public class DoubleNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(double);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.Double;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return stream.ReadDouble();
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.Double);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            stream.Write((double)value);
        }
    }
}
