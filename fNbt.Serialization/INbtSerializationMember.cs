using System;
using System.Reflection;

namespace fNbt.Serialization {
    internal interface INbtSerializationMember : ICloneable {
        Type Type { get; set; }
        string Name { get; set; }

        MemberInfo Origin { get; set; }

        NbtSerializationCache SerializationCache { get; set; }
        NbtSerializerSettings Settings { get; set; }

        void Read(object obj, NbtBinaryReader stream);
        void Write(object obj, NbtBinaryWriter stream);

        void FromNbt(object obj, NbtTag tag);
        NbtTag ToNbt(object obj);
    }
}