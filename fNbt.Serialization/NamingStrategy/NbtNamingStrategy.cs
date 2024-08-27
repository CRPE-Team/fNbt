namespace fNbt.Serialization.NamingStrategy {
    public abstract class NbtNamingStrategy {
        public abstract string ResolvePropertyName(string name);

        public override bool Equals(object obj) {
            return GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
