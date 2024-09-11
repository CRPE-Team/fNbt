using System;
using fNbt.Serialization.NamingStrategy;

namespace fNbt.Serialization {
    [AttributeUsage(AttributeTargets.Property)]
    public class NbtFlatPropertyAttribute : NbtPropertyAttribute {
        public NbtNamingStrategy NamingStrategy { get; }

        public NbtFlatPropertyAttribute() {

        }

        public NbtFlatPropertyAttribute(Type namingStrategyType) {
            CheckNamingStrategyType(namingStrategyType);

            NamingStrategy = (NbtNamingStrategy)Activator.CreateInstance(namingStrategyType);
        }

        private void CheckNamingStrategyType(Type namingStrategyType) {
            if (!typeof(NbtNamingStrategy).IsAssignableFrom(namingStrategyType)) {
                throw new ArgumentException("Must be inherited from NbtNamingStrategy");
            }
        }
    }
}
