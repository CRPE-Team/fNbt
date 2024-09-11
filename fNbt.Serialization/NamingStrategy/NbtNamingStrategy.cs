namespace fNbt.Serialization.NamingStrategy {
    public abstract class NbtNamingStrategy {
        public abstract string ResolveMemberName(string name);

        public override bool Equals(object obj) {
            return GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
