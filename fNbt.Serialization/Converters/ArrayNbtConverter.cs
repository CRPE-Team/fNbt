using System;

namespace fNbt.Serialization.Converters {
    public class ArrayNbtConverter : NbtConverter {
        internal NbtSerializationCache ElementSerializationCache { get; set; }

        public ArrayNbtConverter(Type itemsTime) {
            ElementSerializationCache = itemsTime == null ? null : SerializationDescriber.Describe(itemsTime, null);
        }

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

        public virtual NbtTagType GetItemTagType(Type collectionType, NbtSerializerSettings settings) {
            return ElementSerializationCache.GetTagType(collectionType.GetElementType());
        }

        public override object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var listType = stream.ReadTagType();

            var length = stream.ReadInt32();
            var array = Array.CreateInstance(type.GetElementType(), length);
            for (var i = 0; i < length; i++) {
                var k = i;
                array.SetValue(ReadItem(stream, ref k, settings), k);
            }

            return array;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.List);
            stream.Write(name);

            WriteData(stream, value, settings);
        }

        public override void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var array = (Array)value;

            stream.Write(GetItemTagType(array.GetType(), settings));
            stream.Write(array.Length);
            for (var i = 0; i < array.Length; i++) {
                WriteItem(stream, array.GetValue(i), i, settings);
            }
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var nbtList = tag as NbtList;
            var array = Array.CreateInstance(type.GetElementType(), nbtList.Count);

            for (var i = 0; i < nbtList.Count; i++) {
                var k = i;
                array.SetValue(ItemFromNbt(nbtList[i], ref k, settings), k);
            }

            return array;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            ElementSerializationCache.Settings = settings;

            var array = (Array)value;
            var nbtList = new NbtList(name) { ListType = GetItemTagType(array.GetType(), settings) };

            for (var i = 0; i < array.Length; i++) {
                var tag = ItemToNbt(array.GetValue(i), i, settings);

                if (tag != null) {
                    nbtList.Add(tag);
                }
            }

            return nbtList;
        }

        protected virtual object ReadItem(NbtBinaryReader stream, ref int index, NbtSerializerSettings settings) {
            return ElementSerializationCache.Read(stream, null, string.Empty);
        }

        protected virtual void WriteItem(NbtBinaryWriter stream, object value, int index, NbtSerializerSettings settings) {
            ElementSerializationCache.WriteData(stream, value);
        }

        protected virtual object ItemFromNbt(NbtTag tag, ref int index, NbtSerializerSettings settings) {
            return ElementSerializationCache.FromNbt(tag, null);
        }

        protected virtual NbtTag ItemToNbt(object value, int index, NbtSerializerSettings settings) {
            return ElementSerializationCache.ToNbt(value, null);
        }
    }
}
