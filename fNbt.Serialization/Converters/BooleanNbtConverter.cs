using System;

namespace fNbt.Serialization.Converters {
    public class BooleanNbtConverter : NbtConverter {
        private ByteNbtConverter _underlyingTypeConverter = new ByteNbtConverter();

        public override bool CanConvert(Type type) {
            return type == typeof(bool);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.Byte;
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return Convert.ToBoolean(_underlyingTypeConverter.Read(stream, type, value, name, settings));
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            _underlyingTypeConverter.Write(stream, Convert.ToByte(value), name, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            _underlyingTypeConverter.WriteData(stream, Convert.ToByte(value), settings);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return Convert.ToBoolean(tag.ByteValue);
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtByte(name, Convert.ToByte(value));
        }
    }
}
