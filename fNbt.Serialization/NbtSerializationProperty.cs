using System;

namespace fNbt.Serialization {
    internal class NbtSerializationProperty {
        public Type Type { get; set; }

        public string Name { get; set; }

        public Func<object, object[], object> Get { get; set; }

        public Func<object, object[], object> Set { get; set; }

        public NbtSerializationCache SerializationCache { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public void Read(object obj, NbtBinaryReader stream) {
            if (Set == null) {
                if (Settings.PropertySetHandling == Handlings.PropertySetHandling.Error) {
                    throw new NbtSerializationException($"set method of property [{Type}.{Name}] is not implemented");
                }
            } else {
                Set(obj, [SerializationCache.Read(stream, Name)]);
            }
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
            if (Set == null) {
                if (Settings.PropertySetHandling == Handlings.PropertySetHandling.Error) {
                    throw new NbtSerializationException($"set method of property [{Type}.{Name}] is not implemented");
                }
            } else {
                Set(obj, [SerializationCache.FromNbt(tag)]);
            }
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
    }
}
