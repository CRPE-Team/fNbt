using System;
using System.Collections.Generic;

namespace fNbt.Serialization {
    internal class NbtSerializationProfile {
        public Type ObjectType { get; set; }

        public NbtSerializationSettings Settings { get; set; }

        public override bool Equals(object obj) {
            return obj is NbtSerializationProfile profile &&
                   EqualityComparer<Type>.Default.Equals(ObjectType, profile.ObjectType) &&
                   EqualityComparer<NbtSerializationSettings>.Default.Equals(Settings, profile.Settings);
        }

        public override int GetHashCode() {
            return HashCode.Combine(ObjectType, Settings);
        }
    }
}
