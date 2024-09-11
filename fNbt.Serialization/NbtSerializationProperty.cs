using System;
using System.Reflection;

namespace fNbt.Serialization {
    internal class NbtSerializationProperty : INbtSerializationMember {
        public Type Type { get; set; }

        public string Name { get; set; }

        public MemberInfo Origin { get; set; }

        public Func<object, object[], object> Get { get; set; }

        public Func<object, object[], object> Set { get; set; }

        public NbtSerializationCache SerializationCache { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public void Read(object obj, NbtBinaryReader stream) {
            if (Get == null) {
                if (Settings.PropertyGetHandling == Handlings.PropertyGetHandling.Error) {
                    throw new NbtSerializationException($"get method of property [{Type}.{Name}] is not implemented");
                }
                return;
            }

            var value = Get(obj, []);
            value = SerializationCache.Read(stream, value, Name);

            if (Set == null && value == null) {
                if (Settings.PropertySetHandling == Handlings.PropertySetHandling.Error) {
                    throw new NbtSerializationException($"set method of property [{Type}.{Name}] is not implemented");
                }
                return;
            }

            Set(obj, [value]);
        }

        public void Write(object obj, NbtBinaryWriter stream) {
            if (Get == null) {
                if (Settings.PropertyGetHandling == Handlings.PropertyGetHandling.Error) {
                    throw new NbtSerializationException($"get method of property [{Type}.{Name}] is not implemented");
                }
            } else {
                SerializationCache.Write(stream, Get(obj, []), Name);
            }
        }

        public void FromNbt(object obj, NbtTag tag) {
            if (Get == null) {
                if (Settings.PropertyGetHandling == Handlings.PropertyGetHandling.Error) {
                    throw new NbtSerializationException($"get method of property [{Type}.{Name}] is not implemented");
                }
                return;
            }

            var value = Get(obj, []);
            value = SerializationCache.FromNbt(tag, value);

            if (Set == null) {
                if (Settings.PropertySetHandling == Handlings.PropertySetHandling.Error) {
                    throw new NbtSerializationException($"set method of property [{Type}.{Name}] is not implemented");
                }

                return;
            }

            Set(obj, [value]);
        }

        public NbtTag ToNbt(object obj) {
            if (Get == null) {
                if (Settings.PropertyGetHandling == Handlings.PropertyGetHandling.Error) {
                    throw new NbtSerializationException($"get method of property [{Type}.{Name}] is not implemented");
                }

                return null;
            } else {
                return SerializationCache.ToNbt(Get(obj, []), Name);
            }
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}
