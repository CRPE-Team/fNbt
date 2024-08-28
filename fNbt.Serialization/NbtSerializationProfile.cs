﻿using System;
using System.Collections.Generic;

namespace fNbt.Serialization {
    internal class NbtSerializationProfile {
        public Type ObjectType { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public override bool Equals(object obj) {
            return obj is NbtSerializationProfile profile &&
                   EqualityComparer<Type>.Default.Equals(ObjectType, profile.ObjectType) &&
                   EqualityComparer<NbtSerializerSettings>.Default.Equals(Settings, profile.Settings);
        }

        public override int GetHashCode() {
            return HashCode.Combine(ObjectType, Settings);
        }
    }
}
