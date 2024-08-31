using System;

namespace fNbt.Serialization.Converters {
    public abstract class NbtConverter {
        public virtual bool CanRead => true;
        public virtual bool CanWrite => true;

        public abstract bool CanConvert(Type type);

        public abstract NbtTagType GetTagType(Type type, NbtSerializerSettings settings);

        public abstract object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings);
        public abstract void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings);
        public abstract void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings);

        public abstract object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings);
        public abstract NbtTag ToNbt(object value, string name, NbtSerializerSettings settings);

        public override bool Equals(object obj) {
            return GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
