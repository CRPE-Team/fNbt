using System;

namespace fNbt.Serialization.Converters {
    public class EnumNbtConverter : NbtConverter {
        private StringNbtConverter _underlyingTypeConverter = new StringNbtConverter();

        public override bool CanConvert(Type type) {
            return type.IsEnum;
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return _underlyingTypeConverter.GetTagType(type, settings);
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            return Enum.Parse(type, (string)_underlyingTypeConverter.Read(stream, type, name, settings));
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            _underlyingTypeConverter.Write(stream, value, name, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            _underlyingTypeConverter.WriteData(stream, value, settings);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            return Enum.Parse(type, (string)_underlyingTypeConverter.FromNbt(tag, type, settings));
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return _underlyingTypeConverter.ToNbt(value, name, settings);
        }
    }
}
