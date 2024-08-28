namespace fNbt.Serialization.Handlings {
    public enum LoopReferenceHandling {
        Ignore = 1,
        Error = 2,
        Serialize = 3,

        Default = Error,
    }
}
