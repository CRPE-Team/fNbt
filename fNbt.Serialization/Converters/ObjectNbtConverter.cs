using System;

namespace fNbt.Serialization.Converters {
    public class ObjectNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(object);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtSerializer.GetTagTypeInternal(type, settings);
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return NbtSerializer.ReadInternal(type, stream, name, settings);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            NbtSerializer.WriteInternal(value, stream, name, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            NbtSerializer.WriteDataInternal(value, stream, settings);
        }
    }
}
