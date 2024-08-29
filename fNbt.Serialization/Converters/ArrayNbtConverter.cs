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

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.List;
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var array = Array.CreateInstance(type.GetElementType(), length);
            for (var i = 0; i < length; i++) {
                array.SetValue(ElementSerializationCache.Read(stream, string.Empty), i);
            }

            return array;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.List);
            stream.Write(name);

            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var array = (Array)value;

            stream.Write(ElementSerializationCache.GetTagType(array.GetType().GetElementType()));
            stream.Write(array.Length);
            for (var i = 0; i < array.Length; i++) {
                ElementSerializationCache.WriteData(stream, array.GetValue(i));
            }
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            var nbtList = tag as NbtList;
            var array = Array.CreateInstance(type.GetElementType(), nbtList.Count);

            for (var i = 0; i < nbtList.Count; i++) {
                array.SetValue(ElementSerializationCache.FromNbt(nbtList[i]), i);
            }

            return array;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            var array = (Array)value;
            var nbtList = new NbtList(name);

            foreach (var val in array) {
                var tag = ElementSerializationCache.ToNbt(val, null);

                if (tag != null) {
                    nbtList.Add(tag);
                }
            }

            return nbtList;
        }
    }
}
