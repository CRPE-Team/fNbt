namespace fNbt.Serialization.NamingStrategy {
    public class DefaultNbtNamingStrategy : NbtNamingStrategy {
        public override string ResolvePropertyName(string name) => name;
    }
}
