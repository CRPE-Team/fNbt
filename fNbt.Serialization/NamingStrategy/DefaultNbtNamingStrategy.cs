namespace fNbt.Serialization.NamingStrategy {
    public class DefaultNbtNamingStrategy : NbtNamingStrategy {
        public override string ResolveMemberName(string name) => name;
    }
}
