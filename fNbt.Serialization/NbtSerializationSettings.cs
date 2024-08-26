using System;
using System.Collections.Generic;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;

namespace fNbt.Serialization {
    public class NbtSerializationSettings {
        public NbtFlavor Flavor { get; set; } = NbtFlavor.Default;
        public List<NbtConverter> Converters { get; set; }

        public PropertyGetHandling PropertyGetHandling { get; set; } = PropertyGetHandling.Default;
        public PropertySetHandling PropertySetHandling { get; set; } = PropertySetHandling.Default;
        public MissingMemberHandling MissingMemberHandling { get; set; } = MissingMemberHandling.Default;
        public NullReferenceHandling NullReferenceHandling { get; set; } = NullReferenceHandling.Default;

        public override bool Equals(object obj) {
            return obj is NbtSerializationSettings settings &&
                   EqualityComparer<NbtFlavor>.Default.Equals(Flavor, settings.Flavor) &&
                   EqualityComparer<List<NbtConverter>>.Default.Equals(Converters, settings.Converters) &&
                   PropertyGetHandling == settings.PropertyGetHandling &&
                   PropertySetHandling == settings.PropertySetHandling &&
                   MissingMemberHandling == settings.MissingMemberHandling &&
                   NullReferenceHandling == settings.NullReferenceHandling;
        }

        public override int GetHashCode() {
            var hashCode = new HashCode();
            Converters.ForEach(hashCode.Add);

            return HashCode.Combine(Flavor, hashCode.ToHashCode(), PropertyGetHandling, PropertySetHandling, MissingMemberHandling, NullReferenceHandling);
        }
    }
}
