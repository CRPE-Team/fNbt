using System;

namespace fNbt.Serialization {
    public class NbtSerializationException : Exception {
        public NbtSerializationException() {
        }

        public NbtSerializationException(string message) : base(message) {
        }
    }
}
