using System;

namespace fNbt.Serialization.Converters {
    public class ByteArrayNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(byte[]);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.ByteArray;
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            var length = stream.ReadInt32();
            return stream.ReadBytes(length);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.ByteArray);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var array = (byte[])value;
            stream.Write(array.Length);
            stream.Write(array, 0, array.Length);
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.ByteArrayValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtByteArray(name, (byte[])value);
        }
    }
}
