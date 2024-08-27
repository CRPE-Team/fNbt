using System;

namespace fNbt.Serialization.NbtObject {
    internal class NbtSerializationProperty {
        public Type Type {  get; set; }

        public string Name { get; set; }

        public Func<object, object[], object> Get { get; set; }

        public Func<object, object[], object> Set { get; set; }

        public NbtSerializationCache SerializationCache { get; set; }

        public NbtSerializationSettings Settings { get; set; }

        public void Read(object obj, NbtBinaryReader stream) {
            if (Set == null && Settings.PropertySetHandling == Handlings.PropertySetHandling.Error) {
                throw new NbtSerializationException($"set method of property [{Type}.{Name}] is not implemented");
            }

            Set(obj, [SerializationCache.Read(stream, Name)]);
        }

        public void Write(object obj, NbtBinaryWriter stream) {
            if (Get == null && Settings.PropertyGetHandling == Handlings.PropertyGetHandling.Error) {
                throw new NbtSerializationException($"get method of property [{Type}.{Name}] is not implemented");
            }

            SerializationCache.Write(stream, Get(obj, []), Name);
        }
    }
}
