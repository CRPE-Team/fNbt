using System;

namespace fNbt.Serialization.Converters {
    internal class FlatMemberConverter : NbtConverter {
        public INbtSerializationMember Member { get; set; }

        public override bool CanConvert(Type type) {
            return Member.Type.IsAssignableFrom(type);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return Member.SerializationCache.GetTagType(type);
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            Member.Read(value ??= Activator.CreateInstance(type), stream);

            return value;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            Member.Write(value, stream);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            throw new NbtSerializationException("Members can't write data without headers");
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            Member.FromNbt(value ??= Activator.CreateInstance(type), tag);

            return value;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return Member.ToNbt(value);
        }
    }
}
