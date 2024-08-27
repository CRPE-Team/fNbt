using System;

namespace fNbt.Serialization.Converters {
    public abstract class NbtConverter {
        public virtual bool CanRead { get; set; } = true;
        public virtual bool CanWrite { get; set; } = true;

        public abstract bool CanConvert(Type type);

        public abstract NbtTagType GetTagType(Type type, NbtSerializationSettings settings);

        public abstract object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings);
        public abstract void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings);
        public abstract void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings);

        //public abstract object FromNbt(NbtTag tag, Type type, string name, NbtSerializationSettings settings);
        //public abstract NbtTag ToNbt(object value, NbtSerializationSettings settings);

        public override bool Equals(object obj) {
            return GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
