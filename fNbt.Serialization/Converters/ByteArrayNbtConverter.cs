using System;

namespace fNbt.Serialization.Converters {
    public class ByteArrayNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(byte[]);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.ByteArray;
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            var length = stream.ReadInt32();
            return stream.ReadBytes(length);
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.ByteArray);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            var array = (byte[])value;
            stream.Write(array.Length);
            stream.Write(array, 0, array.Length);
        }
    }
}
