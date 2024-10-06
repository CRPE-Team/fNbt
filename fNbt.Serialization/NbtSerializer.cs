using System;
using System.IO;
using System.IO.Compression;
using fNbt.Serialization.Converters;

namespace fNbt.Serialization {
    public static class NbtSerializer {
        private const string WrongZLibHeaderMessage = "Unrecognized ZLib header. Expected 0x78";

        internal static DynamicNbtConverter DynamicNbtConverter { get; } = new DynamicNbtConverter();

        public static int BufferSize { get; set; } = NbtFile.DefaultBufferSize;

        public static T Read<T>(Stream stream, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, NbtCompression.AutoDetect, settings);
        }

        public static T Read<T>(Stream stream, NbtCompression compression, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, compression, settings);
        }

        public static object Read(Type type, Stream stream, NbtSerializerSettings settings = null) {
            return Read(type, stream, null, null, NbtCompression.AutoDetect, settings);
        }

        public static object Read(Type type, Stream stream, NbtCompression compression, NbtSerializerSettings settings = null) {
            return Read(type, stream, null, null, compression, settings);
        }

        public static object Read(object value, Stream stream, NbtSerializerSettings settings = null) {
            return Read(value.GetType(), stream, value, null, NbtCompression.AutoDetect, settings);
        }

        public static object Read(object value, Stream stream, NbtCompression compression, NbtSerializerSettings settings = null) {
            return Read(value.GetType(), stream, value, null, compression, settings);
        }

        public static object Read(Type type, Stream stream, object value, string name, NbtSerializerSettings settings = null) {
            return Read(type, stream, value, name, NbtCompression.AutoDetect, settings);
        }

        public static object Read(Type type, Stream stream, object value, string name, NbtCompression compression = NbtCompression.AutoDetect, NbtSerializerSettings settings = null) {
            if (compression == NbtCompression.AutoDetect) {
                compression = NbtFile.DetectCompression(stream);
            }

            long startOffset = 0;
            if (stream.CanSeek) {
                startOffset = stream.Position;
            } else {
                stream = new ByteCountingStream(stream);
            }

            object result;
            switch (compression) {
                case NbtCompression.GZip:
                    using (var decStream = new GZipStream(stream, CompressionMode.Decompress, true)) {
                        result = ReadFromStreamInternal(type, decStream, value, name, settings, true);
                    }
                    break;

                case NbtCompression.None:
                    result = ReadFromStreamInternal(type, stream, value, name, settings, false);
                    break;

                case NbtCompression.ZLib:
                    if (stream.ReadByte() != NbtFile.ZLibMagicNumber) {
                        throw new InvalidDataException(WrongZLibHeaderMessage);
                    }
                    stream.ReadByte();
                    using (var decStream = new DeflateStream(stream, CompressionMode.Decompress, true)) {
                        result = ReadFromStreamInternal(type, decStream, value, name, settings, true);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(compression));
            }

            long bytesRead = 0;
            if (stream.CanSeek) {
                bytesRead =  stream.Position - startOffset;
            } else {
                bytesRead = ((ByteCountingStream)stream).BytesRead;
            }

            return result;
        }

        public static T Read<T>(NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return (T)Read(typeof(T), stream, settings);
        }

        public static object Read(Type type, NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return ReadInternal(type, stream, null, null, settings);
        }

        public static object Read(object value, NbtBinaryReader stream, NbtSerializerSettings settings = null) {
            return ReadInternal(value.GetType(), stream, value, null, settings);
        }

        public static long Write(object value, Stream stream, NbtSerializerSettings settings = null) {
            return WriteToStreamInternal(value, stream, NbtCompression.None, settings, true);
        }

        public static long Write(object value, Stream stream, NbtCompression compression, NbtSerializerSettings settings = null) {
            return WriteToStreamInternal(value, stream, compression, settings, true);
        }

        public static void Write(object value, NbtBinaryWriter stream, NbtSerializerSettings settings = null) {
            WriteInternal(value, stream, string.Empty, settings);
        }

        public static long WriteData(object value, Stream stream, NbtSerializerSettings settings = null) {
            return WriteToStreamInternal(value, stream, NbtCompression.None, settings, false);
        }

        public static long WriteData(object value, Stream stream, NbtCompression compression, NbtSerializerSettings settings = null) {
            return WriteToStreamInternal(value, stream, compression, settings, false);
        }

        public static void WriteData(object value, NbtBinaryWriter stream, NbtSerializerSettings settings = null) {
            WriteDataInternal(value, stream, settings);
        }

        internal static NbtTagType GetTagTypeInternal(Type type, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).GetTagType(type);
        }

        internal static object ReadInternal(Type type, NbtBinaryReader stream, object value, string name, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).Read(stream, value, name);
        }

        internal static void WriteInternal(object value, NbtBinaryWriter stream, string name, NbtSerializerSettings settings) {
            if (value == null) return;

            SerializationDescriber.Describe(value.GetType(), settings).Write(stream, value, name);
        }

        internal static void WriteDataInternal(object value, NbtBinaryWriter stream, NbtSerializerSettings settings) {
            if (value == null) {
                throw new NbtSerializationException($"Unexpected null reference detected");
            }

            SerializationDescriber.Describe(value.GetType(), settings).WriteData(stream, value);
        }

        internal static object FromNbtInternal(Type type, NbtTag tag, object value, NbtSerializerSettings settings) {
            return SerializationDescriber.Describe(type, settings).FromNbt(tag, value);
        }

        internal static NbtTag ToNbtInternal(object value, string name, NbtSerializerSettings settings) {
            if (value == null) return null;

            return SerializationDescriber.Describe(value.GetType(), settings).ToNbt(value, name);
        }

        private static object ReadFromStreamInternal(Type type, Stream stream, object value, string name, NbtSerializerSettings settings, bool useBoffer) {
            if (useBoffer && BufferSize > 0) {
                stream = new BufferedStream(stream, BufferSize);
            }

            return ReadInternal(type, new NbtBinaryReader(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), value, name, settings);
        }

        private static long WriteToStreamInternal(object value, Stream stream, NbtCompression compression, NbtSerializerSettings settings, bool writeHeader) {
            switch (compression) {
                case NbtCompression.AutoDetect:
                    throw new ArgumentException("AutoDetect is not a valid NbtCompression value for saving.");
                case NbtCompression.ZLib:
                case NbtCompression.GZip:
                case NbtCompression.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compression));
            }

            long startOffset = 0;
            if (stream.CanSeek) {
                startOffset = stream.Position;
            } else {
                stream = new ByteCountingStream(stream);
            }

            switch (compression) {
                case NbtCompression.ZLib:
                    stream.WriteByte(NbtFile.ZLibMagicNumber);
                    stream.WriteByte(0x01);
                    int checksum;
                    using (var compressStream = new ZLibStream(stream, CompressionMode.Compress, true)) {
                        var bufferedStream = new BufferedStream(compressStream, NbtFile.WriteBufferSize);
                        WriteToStreamInternal(value, stream, settings, writeHeader);
                        bufferedStream.Flush();
                        checksum = compressStream.Checksum;
                    }
                    byte[] checksumBytes = BitConverter.GetBytes(checksum);
                    if (BitConverter.IsLittleEndian) {
                        // Adler32 checksum is big-endian
                        Array.Reverse(checksumBytes);
                    }
                    stream.Write(checksumBytes, 0, checksumBytes.Length);
                    break;

                case NbtCompression.GZip:
                    using (var compressStream = new GZipStream(stream, CompressionMode.Compress, true)) {
                        // use a buffered stream to avoid GZipping in small increments (which has a lot of overhead)
                        var bufferedStream = new BufferedStream(compressStream, NbtFile.WriteBufferSize);
                        WriteToStreamInternal(value, stream, settings, writeHeader);
                        bufferedStream.Flush();
                    }
                    break;

                case NbtCompression.None:
                    WriteToStreamInternal(value, stream, settings, writeHeader);
                    break;

                    // Can't be AutoDetect or unknown: parameter is already validated
            }

            if (stream.CanSeek) {
                return stream.Position - startOffset;
            } else {
                return ((ByteCountingStream)stream).BytesWritten;
            }
        }

        private static void WriteToStreamInternal(object value, Stream stream, NbtSerializerSettings settings, bool writeHeader) {
            if (writeHeader) {
                WriteInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), string.Empty, settings);
            } else {
                WriteDataInternal(value, new NbtBinaryWriter(stream, settings?.Flavor ?? NbtSerializerSettings.DefaultSettings.Flavor), settings);
            }
        }
    }
}
