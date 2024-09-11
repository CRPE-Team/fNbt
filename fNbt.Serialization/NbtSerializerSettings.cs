using System;
using System.Collections.Generic;
using System.Linq;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;
using fNbt.Serialization.NamingStrategy;

namespace fNbt.Serialization {
    public class NbtSerializerSettings {
        private PropertyGetHandling? _propertyGetHandling;
        private PropertySetHandling? _propertySetHandling;
        private MissingMemberHandling? _missingMemberHandling;
        private NullReferenceHandling? _nullReferenceHandling;
        private LoopReferenceHandling? _loopReferenceHandling;
        private NbtMemberHandling? _nbtMemberHandling;
        private NbtFlavor _flavor;
        private NbtNamingStrategy _namingStrategy;

        public static NbtSerializerSettings DefaultSettings { get; } = new NbtSerializerSettings() {
            PropertyGetHandling = PropertyGetHandling.Default,
            PropertySetHandling = PropertySetHandling.Default,
            MissingMemberHandling = MissingMemberHandling.Default,
            NullReferenceHandling = NullReferenceHandling.Default,
            LoopReferenceHandling = LoopReferenceHandling.Default,
            NbtMemberHandling = NbtMemberHandling.Default,

            NamingStrategy = new DefaultNbtNamingStrategy(),
            Flavor = NbtFlavor.Default,

            Converters = new List<NbtConverter>() {
                new BooleanNbtConverter(),
                new ByteArrayNbtConverter(),
                new ByteNbtConverter(),
                new DoubleNbtConverter(),
                new FloatNbtConverter(),
                new IntArrayNbtConverter(),
                new IntNbtConverter(),
                new LongArrayNbtConverter(),
                new LongNbtConverter(),
                new ShortNbtConverter(),
                new StringNbtConverter(),
                new TagNbtConverter()
            }
        };

        public List<NbtConverter> Converters { get; set; } = new List<NbtConverter>();

        public NbtNamingStrategy NamingStrategy {
            get {
                return _namingStrategy ?? DefaultSettings._namingStrategy;
            }
            set {
                _namingStrategy = value ?? DefaultSettings._namingStrategy;
            }
        }

        public NbtFlavor Flavor {
            get {
                return _flavor ?? DefaultSettings._flavor;
            }
            set {
                _flavor = value ?? DefaultSettings._flavor;
            }
        }

        public PropertyGetHandling PropertyGetHandling {
            get {
                return _propertyGetHandling ?? DefaultSettings._propertyGetHandling.Value;
            }
            set {
                _propertyGetHandling = value;
            }
        }

        public PropertySetHandling PropertySetHandling {
            get {
                return _propertySetHandling ?? DefaultSettings._propertySetHandling.Value;
            }
            set {
                _propertySetHandling = value;
            }
        }

        public MissingMemberHandling MissingMemberHandling {
            get {
                return _missingMemberHandling ?? DefaultSettings._missingMemberHandling.Value;
            }
            set {
                _missingMemberHandling = value;
            }
        }

        public NullReferenceHandling NullReferenceHandling {
            get {
                return _nullReferenceHandling ?? DefaultSettings._nullReferenceHandling.Value;
            }
            set {
                _nullReferenceHandling = value;
            }
        }

        public LoopReferenceHandling LoopReferenceHandling {
            get {
                return _loopReferenceHandling ?? DefaultSettings._loopReferenceHandling.Value;
            }
            set {
                _loopReferenceHandling = value;
            }
        }

        public NbtMemberHandling NbtMemberHandling {
            get {
                return _nbtMemberHandling ?? DefaultSettings._nbtMemberHandling.Value;
            }
            set {
                _nbtMemberHandling = value;
            }
        }

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
                   NbtMemberHandling == settings.NbtMemberHandling;
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
            hash.Add(NbtMemberHandling);

            return hash.ToHashCode();
        }
    }
}
