using System;

namespace fNbt.Serialization.Converters {
    public class IntArrayNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(int[]);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.IntArray;
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            var length = stream.ReadInt32();
            var array = new int[length];

            if (stream.Flavor.BigEndian == BitConverter.IsLittleEndian) {
                for (var i = 0; i < length; i++) {
                    array[i] = stream.ReadInt32();
                }
            } else {
                fixed (int* ptr = array) {
                    stream.Read(new Span<byte>(ptr, array.Length * sizeof(int)));
                }
            }

            return array;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.IntArray);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var array = (int[])value;
            stream.Write(array.Length);
            if (stream.Flavor.BigEndian == BitConverter.IsLittleEndian) {
                for (var i = 0; i < array.Length; i++) {
                    stream.Write(array[i]);
                }
            } else {
                fixed (int* ptr = array) {
                    stream.BaseStream.Write(new ReadOnlySpan<byte>(ptr, array.Length * sizeof(int)));
                }
            }
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.IntArrayValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtIntArray(name, (int[])value);
        }
    }
}
