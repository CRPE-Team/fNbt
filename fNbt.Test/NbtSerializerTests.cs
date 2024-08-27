using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fNbt.Serialization;
using fNbt.Serialization.Converters;
using NUnit.Framework;

namespace fNbt.Test {
    [TestFixture]
    public sealed class NbtSerializerTests {
        public class TestObject {
            public byte TestByte { get; set; }
            public short TestShort { get; set; }

            public int TestInt { get; set; }

            public long TestLong { get; set; }

            [NbtProperty("test_float")]
            public float TestFloat { get; set; }

            public double TestDecimal { get; set; }

            public byte[] TestByteArray { get; set; }

            public int[] TestIntArray { get; set; }

            public long[] TestLongArray { get; set; }

            public SubObject TestSubObj {  get; set; }

            public Dictionary<string, SubObject> TestDictionary { get; set; }

            public List<SubObject> TestList { get; set; }

            public override bool Equals(object obj) {
                return obj is TestObject @object &&
                       TestByte == @object.TestByte &&
                       TestShort == @object.TestShort &&
                       TestInt == @object.TestInt &&
                       TestLong == @object.TestLong &&
                       TestFloat == @object.TestFloat &&
                       TestDecimal == @object.TestDecimal &&
                       TestByteArray.SequenceEqual(@object.TestByteArray) &&
                       TestIntArray.SequenceEqual(@object.TestIntArray) &&
                       TestLongArray.SequenceEqual(@object.TestLongArray) &&
                       EqualityComparer<SubObject>.Default.Equals(TestSubObj, @object.TestSubObj) &&
                       TestDictionary.SequenceEqual(@object.TestDictionary) &&
                       TestList.SequenceEqual(@object.TestList);
            }

            public override int GetHashCode() {
                HashCode hash = new HashCode();
                hash.Add(TestByte);
                hash.Add(TestShort);
                hash.Add(TestInt);
                hash.Add(TestLong);
                hash.Add(TestFloat);
                hash.Add(TestDecimal);
                hash.Add(TestByteArray);
                hash.Add(TestIntArray);
                hash.Add(TestLongArray);
                hash.Add(TestSubObj);
                hash.Add(TestDictionary);
                hash.Add(TestList);
                return hash.ToHashCode();
            }

            public class SubObject {
                public string TestSubString { get; set; }

                public override bool Equals(object obj) {
                    return obj is SubObject @object &&
                           TestSubString == @object.TestSubString;
                }

                public override int GetHashCode() {
                    return HashCode.Combine(TestSubString);
                }
            }
        }

        [Test]
        public void ObjectSerializationTest() {
            var obj = new TestObject() {
                TestByte = 131,
                TestShort = 13141,
                TestInt = 131412,
                TestLong = 13141231212,
                TestFloat = 123.21f,
                TestDecimal = 123.21,
                TestByteArray = new byte[] { 14, 2, 121 },
                TestIntArray = new int[] { 14, 2, 121 },
                TestLongArray = new long[] { 14, 2, 121 },
                TestSubObj = new TestObject.SubObject() { TestSubString = "test value" },
                TestDictionary = new Dictionary<string, TestObject.SubObject>() {
                    { "test key 0", new TestObject.SubObject() { TestSubString = "test value" } },
                    { "test key 1", new TestObject.SubObject() { TestSubString = "test new value" } }
                },
                TestList = new List<TestObject.SubObject>() {
                    new TestObject.SubObject() { TestSubString = "test val" }
                }
            };

            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream);
            stream.Position = 0;

            var file = new NbtFile();
            file.LoadFromStream(stream, NbtCompression.None);
            stream.Position = 0;

            var newObj = NbtSerializer.Read<TestObject>(stream);

            Assert.AreEqual(obj, newObj);
        }

        [Test]
        public void ByteDeserializeTest() {
            BaseConverterTest(new NbtByte("test", 14), (byte)14, new ByteNbtConverter());
        }

        [Test]
        public void ShortDeserializeTest() {
            BaseConverterTest(new NbtShort("test", 14), (short)14, new ShortNbtConverter());
        }

        [Test]
        public void IntDeserializeTest() {
            BaseConverterTest(new NbtInt("test", 141231), 141231, new IntNbtConverter());
        }

        [Test]
        public void LongDeserializeTest() {
            BaseConverterTest(new NbtLong("test", 14123112312), 14123112312, new LongNbtConverter());
        }

        [Test]
        public void StringDeserializeTest() {
            BaseConverterTest(new NbtString("test", "value"), "value", new StringNbtConverter());
        }

        [Test]
        public void DoubleDeserializeTest() {
            BaseConverterTest(new NbtDouble("test", 1.1233), 1.1233, new DoubleNbtConverter());
        }

        [Test]
        public void FloatDeserializeTest() {
            BaseConverterTest(new NbtFloat("test", 1.1233f), 1.1233f, new FloatNbtConverter());
        }

        [Test]
        public void ByteArrayDeserializeTest() {
            var value = new byte[] { 1, 41, 66 };

            BaseConverterTest(new NbtByteArray("test", value), value, new ByteArrayNbtConverter());
        }

        [Test]
        public void IntArrayDeserializeTest() {
            var value = new int[] { 1920874, 1, -1184931 };

            BaseConverterTest(new NbtIntArray("test", value), value, new IntArrayNbtConverter());
        }

        [Test]
        public void LongArrayDeserializeTest() {
            var value = new long[] { 192087411, 1, -1184911231 };

            BaseConverterTest(new NbtLongArray("test", value), value, new LongArrayNbtConverter());
        }

        [Test]
        public void ListDeserializeTest() {
            var list = new List<int[]>() {
                new int[] { 1, 951, -141 },
                new int[0],
                new int[] { 23, -314, 6551, 1123, 31 },
                new int[] { 9192 }
            };

            var nbtList = new NbtList("test", NbtTagType.IntArray);
            foreach (var i in list) {
                nbtList.Add(new NbtIntArray(i));
            }

            var converter = new ListNbtConverter() {
                ElementSerializationCache = CreateSerializationCache(new IntArrayNbtConverter(), typeof(int[]))
            };

            var dict = BaseDeserialization(nbtList, list.GetType(), converter);

            var val = (IList) dict[nbtList.Name];
            CollectionAssert.AreEquivalent(list, val);
        }

        private void BaseConverterTest(NbtTag tag, object value, NbtConverter converter) {
            var dict = BaseDeserialization(tag, value.GetType(), converter);

            var val = dict[tag.Name];

            Assert.AreEqual(value, val);
        }

        private IDictionary BaseDeserialization(NbtTag tag, Type type, NbtConverter converter) {
            var stream = new MemoryStream();

            var file = new NbtFile() {
                Flavor = NbtFlavor.Default,
                RootTag = new NbtCompound(tag.Name + "_root") { tag }
            }.SaveToStream(stream, NbtCompression.None);
            stream.Position = 0;

            var intCache = CreateSerializationCache(converter, type);
            var dictCache = CreateSerializationCache(new DictionaryNbtConverter() { ElementSerializationCache = intCache },
                typeof(Dictionary<,>).MakeGenericType(typeof(string), type));

            return (IDictionary)dictCache.Read(new NbtBinaryReader(stream, NbtFlavor.Default));
        }

        private NbtSerializationCache CreateSerializationCache(NbtConverter converter, Type type) {
            return new NbtSerializationCache() {
                Converter = converter,
                Settings = NbtSerializer.DefaultSettings,
                Type = type
            };
        }
    }
}
