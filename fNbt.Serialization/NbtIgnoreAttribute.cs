using System;

namespace fNbt.Serialization {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class NbtIgnoreAttribute : Attribute {

    }
}
