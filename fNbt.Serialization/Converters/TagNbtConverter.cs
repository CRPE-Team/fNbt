using System;

namespace fNbt.Serialization.Converters {
    public class TagNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return typeof(NbtTag).IsAssignableFrom(type);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return ((NbtTag)Activator.CreateInstance(type)).TagType;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            return ((NbtTag)Activator.CreateInstance(type)).ReadTag(stream);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            ((NbtTag)value).WriteTag(stream);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            ((NbtTag)value).WriteData(stream);
        }
    }
}
