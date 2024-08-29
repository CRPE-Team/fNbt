using System;
using System.Collections.Generic;
using System.Linq;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;
using fNbt.Serialization.NamingStrategy;

namespace fNbt.Serialization {
    public class NbtSerializerSettings {
        public NbtFlavor Flavor { get; set; } = NbtFlavor.Default;
        public List<NbtConverter> Converters { get; set; } = new List<NbtConverter>();

        public NbtNamingStrategy NamingStrategy { get; set; }

        public PropertyGetHandling PropertyGetHandling { get; set; } = PropertyGetHandling.Default;
        public PropertySetHandling PropertySetHandling { get; set; } = PropertySetHandling.Default;
        public MissingMemberHandling MissingMemberHandling { get; set; } = MissingMemberHandling.Default;
        public NullReferenceHandling NullReferenceHandling { get; set; } = NullReferenceHandling.Default;
        public LoopReferenceHandling LoopReferenceHandling { get; set; } = LoopReferenceHandling.Default;
        public NbtPropertyHandling NbtPropertyHandling { get; set; } = NbtPropertyHandling.Default;

        public override bool Equals(object obj) {
            return obj is NbtSerializerSettings settings &&
                   EqualityComparer<NbtFlavor>.Default.Equals(Flavor, settings.Flavor) &&
                   Converters.SequenceEqual(settings.Converters) &&
                   EqualityComparer<NbtNamingStrategy>.Default.Equals(NamingStrategy, settings.NamingStrategy) &&
                   PropertyGetHandling == settings.PropertyGetHandling &&
                   PropertySetHandling == settings.PropertySetHandling &&
                   MissingMemberHandling == settings.MissingMemberHandling &&
                   NullReferenceHandling == settings.NullReferenceHandling &&
                   LoopReferenceHandling == settings.LoopReferenceHandling &&
                   NbtPropertyHandling == settings.NbtPropertyHandling;
        }

        public override int GetHashCode() {
            var hash = new HashCode();

            hash.Add(Flavor);
            Converters.ForEach(hash.Add);
            hash.Add(NamingStrategy);
            hash.Add(PropertyGetHandling);
            hash.Add(PropertySetHandling);
            hash.Add(MissingMemberHandling);
            hash.Add(NullReferenceHandling);
            hash.Add(LoopReferenceHandling);
            hash.Add(NbtPropertyHandling);

            return hash.ToHashCode();
        }
    }
}
