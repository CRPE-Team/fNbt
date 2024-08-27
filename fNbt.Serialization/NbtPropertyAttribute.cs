using System;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    [AttributeUsage(AttributeTargets.Property)]
    public class NbtPropertyAttribute : Attribute {
        public string Name { get; }
        public NbtConverter Converter { get; }

        public NbtPropertyAttribute(string name) {
            Name = name;
        }

        public NbtPropertyAttribute(string name, NbtConverter converter) {
            Name = name;
            Converter = converter;
        }
    }
}
