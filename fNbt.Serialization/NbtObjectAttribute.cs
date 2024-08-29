using System;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class NbtObjectAttribute : Attribute {
        public Type ConverterType { get; }

        public NbtObjectAttribute(Type converterType) {
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
