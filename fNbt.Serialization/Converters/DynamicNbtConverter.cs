using System;

namespace fNbt.Serialization.Converters {
    public class DynamicNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(object);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtSerializer.GetTagTypeInternal(type, settings);
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return NbtSerializer.ReadInternal(type, stream, value, name, settings);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            NbtSerializer.WriteInternal(value, stream, name, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            NbtSerializer.WriteDataInternal(value, stream, settings);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return NbtSerializer.FromNbtInternal(type, tag, value, settings);
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return NbtSerializer.ToNbtInternal(value, name, settings);
        }
    }
}
