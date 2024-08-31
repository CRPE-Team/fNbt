using System;

namespace fNbt.Serialization.Converters {
    public class LongArrayNbtConverter : NbtConverter {
        public override bool CanConvert(Type type) {
            return type == typeof(long[]);
        }

        public override NbtTagType GetTagType(Type type, NbtSerializerSettings settings) {
            return NbtTagType.LongArray;
        }

        public override unsafe object Read(NbtBinaryReader stream, Type type, object value, string name, NbtSerializerSettings settings) {
            var length = stream.ReadInt32();
            var array = new long[length];

            if (stream.Flavor.BigEndian == BitConverter.IsLittleEndian) {
                for (var i = 0; i < length; i++) {
                    array[i] = stream.ReadInt64();
                }
            } else {
                fixed (long* ptr = array) {
                    stream.Read(new Span<byte>(ptr, array.Length * sizeof(long)));
                }
            }

            return array;
        }

        public override void Write(NbtBinaryWriter stream, object value, string name, NbtSerializerSettings settings) {
            stream.Write(NbtTagType.LongArray);
            stream.Write(name);
            WriteData(stream, value, settings);
        }

        public override unsafe void WriteData(NbtBinaryWriter stream, object value, NbtSerializerSettings settings) {
            var array = (long[])value;
            stream.Write(array.Length);
            if (stream.Flavor.BigEndian == BitConverter.IsLittleEndian) {
                for (var i = 0; i < array.Length; i++) {
                    stream.Write(array[i]);
                }
            } else {
                fixed (long* ptr = array) {
                    stream.BaseStream.Write(new ReadOnlySpan<byte>(ptr, array.Length * sizeof(long)));
                }
            }
        }

        public override object FromNbt(NbtTag tag, Type type, object value, NbtSerializerSettings settings) {
            return tag.LongArrayValue;
        }

        public override NbtTag ToNbt(object value, string name, NbtSerializerSettings settings) {
            return new NbtLongArray(name, (long[])value);
        }
    }
}
