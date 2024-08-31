using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fNbt.Serialization;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;
using NUnit.Framework;

namespace fNbt.Test {
    [TestFixture]
    public sealed class NbtSerializerTests {
        public class TestObject {
            public byte TestByte { get; set; }
            public byte? TestNullableByte { get; set; }

            public short TestShort { get; set; }
            public short? TestNullableShort { get; set; }

            public int TestInt { get; set; }
            public int? TestNullableInt { get; set; }

            public long TestLong { get; set; }
            public long? TestNullableLong { get; set; }

            [NbtProperty("test_float")]
            public float TestFloat { get; set; }
            public float? TestNullableFloat { get; set; }

            public double TestDouble { get; set; }
            public double? TestNullableDouble { get; set; }

            public byte[] TestByteArray { get; set; }

            public int[] TestIntArray { get; set; }

            public long[] TestLongArray { get; set; }

            public SubObject TestSubObj {  get; set; }

            public Dictionary<string, SubObject> TestDictionary { get; set; }

            public List<SubObject> TestList { get; set; }

            public SubObject[] TestArray { get; set; }

            public ObjEnum TestEnum { get; set; }

            [NbtProperty(typeof(EnumNbtConverter))]
            public ObjEnum TestStringEnum { get; set; }

            public ObjEnum? TestNullableEnum { get; set; }

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

            public static TestObject Create() {
                return new TestObject() {
                    TestByte = 131,
                    TestNullableByte = 132,
                    TestShort = 13141,
                    TestNullableShort = 13142,
                    TestInt = 131412,
                    TestNullableInt = 131413,
                    TestLong = 13141231212,
                    TestNullableLong = 13141231213,
                    TestFloat = 123.21f,
                    TestNullableFloat = 123.22f,
                    TestDouble = 124.21,
                    TestNullableDouble = 124.22,
                    TestEnum = ObjEnum.First,
                    TestStringEnum = ObjEnum.Third,
                    TestNullableEnum = ObjEnum.Second,
                    TestByteArray = new byte[] { 14, 2, 121 },
                    TestIntArray = new int[] { 15, 3, 122 },
                    TestLongArray = new long[] { 16, 4, 123 },
                    TestSubObj = new TestObject.SubObject() { TestSubString = "test value" },
                    TestDictionary = new Dictionary<string, TestObject.SubObject>() {
                        { "test key 0", new TestObject.SubObject() { TestSubString = "test value" } },
                        { "test key 1", new TestObject.SubObject() { TestSubString = "test new value" } }
                    },
                    TestList = new List<TestObject.SubObject>() {
                        new TestObject.SubObject() { TestSubString = "test ls val" }
                    },
                    TestArray = new TestObject.SubObject[] {
                        new TestObject.SubObject() { TestSubString = "test ar val" }
                    }
                };
            }

            public override bool Equals(object obj) {
                return obj is TestObject @object &&
                       TestByte == @object.TestByte &&
                       TestNullableByte == @object.TestNullableByte &&
                       TestShort == @object.TestShort &&
                       TestNullableShort == @object.TestNullableShort &&
                       TestInt == @object.TestInt &&
                       TestNullableInt == @object.TestNullableInt &&
                       TestLong == @object.TestLong &&
                       TestNullableLong == @object.TestNullableLong &&
                       TestFloat == @object.TestFloat &&
                       TestNullableFloat == @object.TestNullableFloat &&
                       TestDouble == @object.TestDouble &&
                       TestNullableDouble == @object.TestNullableDouble &&
                       TestEnum == @object.TestEnum &&
                       TestStringEnum == @object.TestStringEnum &&
                       TestNullableEnum == @object.TestNullableEnum &&
                       (TestByteArray == @object.TestByteArray || (TestByteArray != null && TestByteArray.SequenceEqual(@object.TestByteArray))) &&
                       (TestIntArray == @object.TestIntArray || (TestIntArray != null && TestIntArray.SequenceEqual(@object.TestIntArray))) &&
                       (TestLongArray == @object.TestLongArray || (TestLongArray != null && TestLongArray.SequenceEqual(@object.TestLongArray))) &&
                       EqualityComparer<SubObject>.Default.Equals(TestSubObj, @object.TestSubObj) &&
                       (TestDictionary == @object.TestDictionary || (TestDictionary != null && TestDictionary.SequenceEqual(@object.TestDictionary))) &&
                       (TestList == @object.TestList || (TestList != null && TestList.SequenceEqual(@object.TestList)));
            }

            public override int GetHashCode() {
                HashCode hash = new HashCode();
                hash.Add(TestByte);
                hash.Add(TestNullableByte);
                hash.Add(TestShort);
                hash.Add(TestNullableShort);
                hash.Add(TestInt);
                hash.Add(TestNullableInt);
                hash.Add(TestLong);
                hash.Add(TestNullableLong);
                hash.Add(TestFloat);
                hash.Add(TestNullableFloat);
                hash.Add(TestDouble);
                hash.Add(TestNullableDouble);
                hash.Add(TestEnum);
                hash.Add(TestStringEnum);
                hash.Add(TestNullableEnum);
                hash.Add(TestByteArray);
                hash.Add(TestIntArray);
                hash.Add(TestLongArray);
                hash.Add(TestSubObj);
                hash.Add(TestDictionary);
                hash.Add(TestList);
                hash.Add(TestArray);
                return hash.ToHashCode();
            }

            public enum ObjEnum {
                None = 0,
                First = 1,
                Second = 2,
                Third = 3,
            }
        }

        public class LoopObject {
            public LoopObject Loop { get; set; }

            public object Loop2 { get; set; }

            public string TestString { get; set; }
        }

        public class OneMemberObject {

            public string TestString { get; set; }
        }

        public class TwoMembersObject {

            public string TestString { get; set; }

            public string TestString2 { get; set; }
        }

        [Test]
        public void MissingMemberDeserializationTest() {
            var obj = new TwoMembersObject() {
                TestString = "test val",
                TestString2 = "test val 2"
            };

            var stream = new MemoryStream();
            NbtSerializer.Write(obj, stream);
            stream.Position = 0;

            Assert.Throws<NbtSerializationException>(() => NbtSerializer.Read<OneMemberObject>(stream));
        }

        [Test]
        public void MissingMemberIgnoreDeserializationTest() {
            var obj = new TwoMembersObject() {
                TestString = "test val",
                TestString2 = "test val 2"
            };

            var stream = new MemoryStream();
            NbtSerializer.Write(obj, stream);
            stream.Position = 0;

            var newObj = NbtSerializer.Read<OneMemberObject>(stream, new NbtSerializerSettings() { 
                MissingMemberHandling = MissingMemberHandling.Ignore 
            });

            Assert.AreEqual(obj.TestString, newObj.TestString);
        }

        [Test]
        public void LoopReferenceIgnoreSerializationTest() {
            var obj = new LoopObject() {
                TestString = "test val"
            };

            obj.Loop = obj;
            obj.Loop2 = obj;

            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream, new NbtSerializerSettings() {
                NullReferenceHandling = NullReferenceHandling.Ignore,
                LoopReferenceHandling = LoopReferenceHandling.Ignore
            });
            stream.Position = 0;

            var newObj = NbtSerializer.Read<LoopObject>(stream);

            Assert.AreEqual(obj.TestString, newObj.TestString);
            Assert.IsNull(newObj.Loop);
            Assert.IsNull(newObj.Loop2);
        }

        [Test]
        public void LoopReferenceSerializationTest() {
            var obj = new LoopObject() {
                TestString = "test val",
                Loop = new LoopObject() { TestString = "test val 2" },
                Loop2 = new LoopObject() { TestString = "test val 3" },
            };
            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore });
            stream.Position = 0;

            obj.Loop = obj;

            Assert.Throws<NbtSerializationException>(
                () => NbtSerializer.Write(obj, stream, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore }));
            stream.Position = 0;

            obj.Loop = null;
            obj.Loop2 = obj;

            Assert.Throws<NbtSerializationException>(
                () => NbtSerializer.Write(obj, stream, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore }));
        }

        [Test]
        public void ObjectDefaultsSerializationTest() {
            var obj = new TestObject() {
                TestInt = 1,
                TestByte = 2
            };

            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore });
            stream.Position = 0;

            var file = new NbtFile();
            file.LoadFromStream(stream, NbtCompression.None);
            stream.Position = 0;

            var newObj = NbtSerializer.Read<TestObject>(stream);

            Assert.AreEqual(obj, newObj);
        }

        [Test]
        public void ObjectCollectionDefaultsSerializationTest() {
            var obj = new TestObject() {
                TestDictionary = new Dictionary<string, TestObject.SubObject>() {
                    { "null", null },
                    { "test key 1", new TestObject.SubObject() { TestSubString = "test new value" } }
                },
                TestList = new List<TestObject.SubObject>() {
                    new TestObject.SubObject() { TestSubString = "test ls val" }
                },
                TestArray = new TestObject.SubObject[] {
                    new TestObject.SubObject() { TestSubString = "test ar val" }
                }
            };

            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore });
            stream.Position = 0;

            var file = new NbtFile();
            file.LoadFromStream(stream, NbtCompression.None);
            stream.Position = 0;

            var newObj = NbtSerializer.Read<TestObject>(stream);

            obj.TestDictionary.Remove("null");

            Assert.AreEqual(obj, newObj);
        }

        [Test]
        public void ObjectSerializationTest() {
            var obj = TestObject.Create();

            var stream = new MemoryStream();

            NbtSerializer.Write(obj, stream);
            stream.Position = 0;
            NbtSerializer.Write(obj, stream);
            stream.Position = 0;
            NbtSerializer.Write(obj, stream);
            stream.Position = 0;

            var file = new NbtFile();
            file.LoadFromStream(stream, NbtCompression.None);
            stream.Position = 0;

            var newObj = NbtSerializer.Read<TestObject>(stream);

            Assert.AreEqual((int)obj.TestEnum, file.RootTag[nameof(TestObject.TestEnum)].IntValue);
            Assert.AreEqual(obj.TestStringEnum.ToString(), file.RootTag[nameof(TestObject.TestStringEnum)].StringValue);
            Assert.AreEqual(obj, newObj);
        }

        [Test]
        public void ObjectDefaultsConvertTest() {
            var obj = new TestObject() {
                TestInt = 1,
                TestByte = 2,
                TestNullableByte = 3,
            };

            var tag = NbtConvert.ToNbt(obj, new NbtSerializerSettings() { NullReferenceHandling = NullReferenceHandling.Ignore });
            var newObj = NbtConvert.FromNbt<TestObject>(tag);

            Assert.AreEqual(obj, newObj);
        }

        [Test]
        public void ObjectConvertTest() {
            var obj = TestObject.Create();

            var tag = NbtConvert.ToNbt(obj);
            var newObj = NbtConvert.FromNbt<TestObject>(tag);

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

            var converter = new ListNbtConverter(null) {
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

            return (IDictionary)dictCache.Read(new NbtBinaryReader(stream, NbtFlavor.Default), null);
        }

        private NbtSerializationCache CreateSerializationCache(NbtConverter converter, Type type) {
            return new NbtSerializationCache() {
                Converter = converter,
                Settings = NbtSerializerSettings.DefaultSettings,
                Type = type
            };
        }
    }
}
