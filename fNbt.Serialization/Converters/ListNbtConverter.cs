using System;
using System.Collections;

namespace fNbt.Serialization.Converters {
    internal class ListNbtConverter : ArrayNbtConverter {
        public override bool CanConvert(Type type) {
            return typeof(IList).IsAssignableFrom(type)
                && ElementSerializationCache.Type.IsAssignableFrom(type.GetElementType());
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializationSettings settings) {
            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var list = (IList) Activator.CreateInstance(type);
            for (var i = 0; i < length; i++) {
                list.Add(ElementSerializationCache.Read(stream, string.Empty));
            }

            return list;
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializationSettings settings) {
            var list = (IList)value;

            stream.Write(ElementSerializationCache.GetTagType(list.GetType().GetElementType()));
            stream.Write(list.Count);

            foreach (var element in list) {
                ElementSerializationCache.WriteData(stream, element);
            }
        }
    }
}
