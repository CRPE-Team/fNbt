using System;
using System.Reflection;

namespace fNbt.Serialization {
    internal class NbtSerializationField : INbtSerializationMember {
        public Type Type { get; set; }

        public string Name { get; set; }

        public MemberInfo Origin { get; set; }

        public Func<object, object> Get { get; set; }

        public Action<object, object> Set { get; set; }

        public NbtSerializationCache SerializationCache { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public void Read(object obj, NbtBinaryReader stream) {
            var value = SerializationCache.Read(stream, Get(obj), Name);

            Set(obj, value);
        }

        public void Write(object obj, NbtBinaryWriter stream) {
            SerializationCache.Write(stream, Get(obj), Name);
        }

        public void FromNbt(object obj, NbtTag tag) {
            var value = SerializationCache.FromNbt(tag, Get(obj));

            Set(obj, value);
        }

        public NbtTag ToNbt(object obj) {
            return SerializationCache.ToNbt(Get(obj), Name);
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}
