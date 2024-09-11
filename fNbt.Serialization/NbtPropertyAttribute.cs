using System;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NbtPropertyAttribute : Attribute {
        public string Name { get; }
        public Type ConverterType { get; }

        public NbtPropertyAttribute() {

        }

        public NbtPropertyAttribute(string name) {
            Name = name;
        }

        public NbtPropertyAttribute(string name, Type converterType) {
            CheckConverterType(converterType);

            Name = name;
            ConverterType = converterType;
        }

        public NbtPropertyAttribute(Type converterType) {
            CheckConverterType(converterType);

            ConverterType = converterType;
        }

        private void CheckConverterType(Type converterType) {
            if (!typeof(NbtConverter).IsAssignableFrom(converterType)) {
                throw new ArgumentException("Must be inherited from NbtConverter");
            }
        }
    }
}
