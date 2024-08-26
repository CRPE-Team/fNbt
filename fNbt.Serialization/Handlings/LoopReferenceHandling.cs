namespace fNbt.Serialization.Handlings {
    public enum LoopReferenceHandling {
        Ignore = 1,
        Error = 2,
        Handling = 3,

        Default = Error,
    }
}
