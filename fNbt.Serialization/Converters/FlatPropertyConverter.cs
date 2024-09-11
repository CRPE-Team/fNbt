using System;

namespace fNbt.Serialization.Converters {
    internal class FlatPropertyConverter : NbtConverter {
        public NbtSerializationProperty Property { get; set; }

        public override bool CanConvert(Type type) {
            return Property.Type.IsAssignableFrom(type);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return Property.SerializationCache.GetTagType(type);
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            Property.Read(value ??= Activator.CreateInstance(type), stream);

            return value;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            Property.Write(value, stream);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            throw new NbtSerializationException("Properties can't write data without headers");
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            Property.FromNbt(value ??= Activator.CreateInstance(type), tag);

            return value;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return Property.ToNbt(value);
        }
    }
}
