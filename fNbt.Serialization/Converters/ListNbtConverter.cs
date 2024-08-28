using System;
using System.Collections;

namespace fNbt.Serialization.Converters {
    internal class ListNbtConverter : ArrayNbtConverter {
        public override bool CanConvert(Type type) {
            return typeof(IList).IsAssignableFrom(type)
                && ElementSerializationCache.Type.IsAssignableFrom(type.GetGenericArguments()[0]);
        }

        public override object Read(NbtBinaryReader stream, Type type, string name, NbtSerializerSettings settings) {
            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var list = (IList) Activator.CreateInstance(type);
            for (var i = 0; i < length; i++) {
                list.Add(ElementSerializationCache.Read(stream, string.Empty));
            }

            return list;
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var list = (IList)value;

            stream.Write(ElementSerializationCache.GetTagType(list.GetType().GetGenericArguments()[0]));
            stream.Write(list.Count);

            foreach (var element in list) {
                ElementSerializationCache.WriteData(stream, element);
            }
        }

        public override object FromNbt(NbtTag tag, Type type, NbtSerializerSettings settings) {
            var nbtList = tag as NbtList;
            var list = (IList)Activator.CreateInstance(type);

            for (var i = 0; i < nbtList.Count; i++) {
                list.Add(ElementSerializationCache.FromNbt(nbtList[i]));
            }

            return list;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            var list = (IList)value;
            var nbtList = new NbtList(name);

            foreach (var val in list) {
                var tag = ElementSerializationCache.ToNbt(val, null);

                if (tag != null) {
                    nbtList.Add(tag);
                }
            }

            return nbtList;
        }
    }
}
