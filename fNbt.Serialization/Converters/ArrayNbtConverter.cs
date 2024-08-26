using System;

namespace fNbt.Serialization.Converters {
    internal class ArrayNbtConverter : NbtConverter {
        public NbtSerializationCache ElementSerializationCache { get; set; }

        public override bool CanConvert(Type type) {
            if (!type.IsArray) return false;

            var elementType = type.GetElementType();
            if (!ElementSerializationCache.Type.IsAssignableFrom(elementType)) return false;

            return elementType != typeof(byte)
                && elementType != typeof(int)
                && elementType != typeof(long);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializationSettings settings) {
            return NbtTagType.List;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var array = Array.CreateInstance(type.GetElementType(), length);
            for (var i = 0; i < length; i++) {
                array.SetValue(ElementSerializationCache.Read(stream, string.Empty), i);
            }

            return array;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializationSettings settings) {
            stream.Write(NbtTagType.List);
            stream.Write(name);

            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            var array = (Array)value;

            stream.Write(ElementSerializationCache.GetTagType(array.GetType().GetElementType()));
            stream.Write(array.Length);
            for (var i = 0; i < array.Length; i++) {
                ElementSerializationCache.WriteData(stream, array.GetValue(i));
            }
        }
    }
}
