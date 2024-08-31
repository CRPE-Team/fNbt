using System;

namespace fNbt.Serialization.Converters {
    public class ObjectNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return false;
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings, null, true).GetTagType(type);
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings, null, true).Read(stream, value, name);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            SerializationDescriber.Describe(value?.GetType(), settings, null, true).Write(stream, value, name);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            SerializationDescriber.Describe(value?.GetType(), settings, null, true).WriteData(stream, value);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings, null, true).FromNbt(tag, value);
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(value?.GetType(), settings, null, true).ToNbt(value, name);
        }
    }
}
