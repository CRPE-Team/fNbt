using System;
using System.Collections.Generic;

namespace fNbt.Serialization {
    internal class NbtSerializationProfile {
        public Type ObjectType { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public Type PropertyConverterType { get; set; }

        public override bool Equals(object obj) {
            return obj is NbtSerializationProfile profile &&
                   EqualityComparer<Type>.Default.Equals(ObjectType, profile.ObjectType) &&
                   EqualityComparer<NbtSerializerSettings>.Default.Equals(Settings, profile.Settings) &&
                   EqualityComparer<Type>.Default.Equals(PropertyConverterType, profile.PropertyConverterType);
        }

        public override int GetHashCode() {
            return HashCode.Combine(ObjectType, Settings, PropertyConverterType);
        }
    }
}
