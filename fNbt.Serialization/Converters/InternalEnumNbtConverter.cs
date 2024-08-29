using System;

namespace fNbt.Serialization.Converters {
    internal class InternalEnumNbtConverter : NbtConverter {
        public NbtConverter UnderlyingTypeConverter { get; set; }

        public override bool CanConvert(Type type) {
            return type.IsEnum && UnderlyingTypeConverter.CanConvert(Enum.GetUnderlyingType(type));
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return UnderlyingTypeConverter.GetTagType(type, settings);
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            return Enum.ToObject(type, UnderlyingTypeConverter.Read(stream, type, name, settings));
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            UnderlyingTypeConverter.Write(stream, value, name, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            UnderlyingTypeConverter.WriteData(stream, value, settings);
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            return Enum.ToObject(type, UnderlyingTypeConverter.FromNbt(tag, type, settings));
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return UnderlyingTypeConverter.ToNbt(value, name, settings);
        }
    }
}
