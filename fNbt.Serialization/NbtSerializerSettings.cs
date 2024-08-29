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

        public override bool Equals(object obj) {
            return obj is NbtSerializerSettings settings &&
                   EqualityComparer<NbtFlavor>.Default.Equals(Flavor, settings.Flavor) &&
                   Converters.SequenceEqual(settings.Converters) &&
                   EqualityComparer<NbtNamingStrategy>.Default.Equals(NamingStrategy, settings.NamingStrategy) &&
                   PropertyGetHandling == settings.PropertyGetHandling &&
                   PropertySetHandling == settings.PropertySetHandling &&
                   MissingMemberHandling == settings.MissingMemberHandling &&
                   NullReferenceHandling == settings.NullReferenceHandling &&
                   LoopReferenceHandling == settings.LoopReferenceHandling;
        }

        public override int GetHashCode() {
            var hashCode = new HashCode();
            Converters.ForEach(hashCode.Add);

            return HashCode.Combine(Flavor, 
                hashCode.ToHashCode(), 
                NamingStrategy, 
                PropertyGetHandling, 
                PropertySetHandling, 
                MissingMemberHandling, 
                NullReferenceHandling,
                LoopReferenceHandling);
        }
    }
}
