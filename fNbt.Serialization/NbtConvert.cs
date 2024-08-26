using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("fNbt.Test")]
namespace fNbt.Serialization {
    public static class NbtConvert
    {
        public static T FromNbt<T>(string filePath, NbtSerializationSettings settings = null) {
            return NbtSerializer.Read<T>(File.OpenRead(filePath), settings);
        }
    }
}
