using System;
using System.Collections;

namespace fNbt.Serialization.Converters {
    public class ListNbtConverter : ArrayNbtConverter {
        public ListNbtConverter(Type itemsTime) : base(itemsTime) {
        }

        public override NbtTagType GetItemTagType(Type collectionType, NbtSerializerSettings settings) {
            return ElementSerializationCache.GetTagType(collectionType.GetGenericArguments()[0]);
        }

        public override bool CanConvert(Type type) {
            return typeof(IList).IsAssignableFrom(type)
                && ElementSerializationCache.Type.IsAssignableFrom(type.GetGenericArguments()[0]);
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var list = (IList)Activator.CreateInstance(type);
            for (var i = 0; i < length; i++) {
                list.Add(ElementSerializationCache.Read(stream, null, string.Empty));
            }

            return list;
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var list = (IList)value;

            stream.Write(GetItemTagType(list.GetType(), settings));
            stream.Write(list.Count);

            for (var i = 0; i < list.Count; i++) {
                WriteItem(stream, list[i], i, settings);
            }
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var nbtList = tag as NbtList;
            var list = (IList)Activator.CreateInstance(type);

            for (var i = 0; i < nbtList.Count; i++) {
                list.Add(ElementSerializationCache.FromNbt(nbtList[i], null));
            }

            return list;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var list = (IList)value;
            var nbtList = new NbtList(name) { ListType = GetItemTagType(list.GetType(), settings) };

            for (var i = 0; i < list.Count; i++) {
                var tag = ItemToNbt(list[i], i, settings);

                if (tag != null) {
                    nbtList.Add(tag);
                }
            }

            return nbtList;
        }
    }
}
