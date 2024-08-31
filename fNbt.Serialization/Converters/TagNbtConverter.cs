using System;

namespace fNbt.Serialization.Converters {
    public class TagNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return typeof(NbtTag).IsAssignableFrom(type);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return ((NbtTag)Activator.CreateInstance(type)).TagType;
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            return ((NbtTag)Activator.CreateInstance(type)).ReadTag(stream);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            ((NbtTag)value).WriteTag(stream);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            ((NbtTag)value).WriteData(stream);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.Clone();
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return (NbtTag)((NbtTag)value).Clone();
        }
    }
}
