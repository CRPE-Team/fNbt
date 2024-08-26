using System;

namespace fNbt.Serialization.Converters {
    public abstract class NbtConverter {
        public abstract bool CanConvert(Type type);

        public abstract NbtTagType GetTagType(Type type, NbtSerializationSettings settings);

        public abstract object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings);
        public abstract void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings);
        public abstract void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings);
    }
}
