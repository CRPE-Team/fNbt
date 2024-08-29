using System;
using System.Collections.Generic;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;

namespace fNbt.Serialization {
    internal class NbtSerializationCache {
        [ThreadStatic]
        private static readonly List<object> _trace = new List<object>();

        public Type Type { get; set; }

        public Dictionary<string, NbtSerializationProperty> Properties { get; set; } = new Dictionary<string, NbtSerializationProperty>();

        public NbtConverter Converter { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public NbtTagType GetTagType(Type type) {
            if (Converter != null) {
                return Converter.GetTagType(type, Settings);
            } else if (type != Type) {
                return NbtSerializer.ObjectNbtConverter.GetTagType(type, Settings);
            } else {
                return NbtTagType.Compound;
            }
        }

        public object Read(NbtBinaryReader stream, string name = null) {
            if (name == null) {
                var tagType = stream.ReadTagType();

                if (tagType == NbtTagType.Unknown) {
                    throw new NbtSerializationException($"Readed unknown tag type of [{Type}]");
                }

                name = stream.ReadString();
            }

            if (Converter != null && Converter.CanRead) {
                return Converter.Read(stream, Type, name, Settings);
            } else {
                var obj = Activator.CreateInstance(Type);

                while (ReadProperty(obj, stream)) ;

                return obj;
            }
        }

        public void Write(NbtBinaryWriter stream, object value, string name) {
            if (value == null) {
                if (Settings.NullReferenceHandling == NullReferenceHandling.Error) {
                    throw new NbtSerializationException($"Null reference of [{Type}] detected");
                }

                return;
            }

            bool hasLoop = false;
            if (hasLoop = CheckLoopReference(value, out var release)) {
                return;
            }

            try {
                if (Converter != null && Converter.CanWrite) {
                    Converter.Write(stream, value, name, Settings);
                } else if (value.GetType() != Type) {
                    if (release) Release(value);
                    NbtSerializer.ObjectNbtConverter.Write(stream, value, name, Settings);
                } else {
                    stream.Write(NbtTagType.Compound);
                    stream.Write(name);

                    WriteCompoundData(stream, value);
                }
            } finally {
                if (release) Release(value);
            }
        }

        public void WriteData(NbtBinaryWriter stream, object value) {
            if (value == null) {
                throw new NbtSerializationException($"Unexpected null reference of [{Type}] detected");
            }

            bool hasLoop = false;
            if (hasLoop = CheckLoopReference(value, out var release)) {
                // we can't handle this case
                throw new NbtSerializationException($"Unexpected loop reference in collection on type [{Type}] detected");
            }

            try {
                if (Converter != null && Converter.CanWrite) {
                    Converter.WriteData(stream, value, Settings);
                } else if (value.GetType() != Type) {
                    if (release) Release(value);
                    NbtSerializer.ObjectNbtConverter.WriteData(stream, value, Settings);
                } else {
                    WriteCompoundData(stream, value);
                }
            } finally {
                if (release) Release(value);
            }
        }

        public object FromNbt(NbtTag tag) {
            if (Converter != null && Converter.CanRead) {
                return Converter.FromNbt(tag, Type, Settings);
            } else {
                var obj = Activator.CreateInstance(Type);

                foreach (var child in tag as NbtCompound) {
                    if (!Properties.TryGetValue(child.Name, out var property)
                        && Settings.MissingMemberHandling == MissingMemberHandling.Error) {
                        throw new NbtSerializationException($"Missing member [{child.Name}]");
                    }

                    property.FromNbt(obj, child);
                }

                return obj;
            }
        }

        public NbtTag ToNbt(object value, string name) {
            if (value == null) {
                if (Settings.NullReferenceHandling == NullReferenceHandling.Error) {
                    throw new NbtSerializationException($"Null reference of [{Type}] detected");
                }

                return null;
            }

            bool hasLoop = false;
            if (hasLoop = CheckLoopReference(value, out var release)) {
                value = Default();
            }

            try {
                if (Converter != null && Converter.CanWrite) {
                    return Converter.ToNbt(value, name, Settings);
                } else if (value != null && value.GetType() != Type) {
                    if (release) Release(value);
                    return NbtSerializer.ObjectNbtConverter.ToNbt(value, name, Settings);
                } else {
                    var compound = new NbtCompound(name);

                    foreach (var property in Properties.Values) {
                        var tag = property.ToNbt(value);

                        if (tag != null) {
                            compound.Add(tag);
                        }
                    }

                    return compound;
                }
            } finally {
                if (release) Release(value);
            }
        }

        private bool ReadProperty(object obj, NbtBinaryReader stream) {
            var tagType = stream.ReadTagType();

            if (tagType == NbtTagType.End) return false;

            if (tagType == NbtTagType.Unknown) {
                throw new NbtSerializationException("Readed unknown tag type");
            }

            var name = stream.ReadString();

            if (!Properties.TryGetValue(name, out var property)) {
                if (Settings.MissingMemberHandling == MissingMemberHandling.Error) {
                    throw new NbtSerializationException($"Missing member [{name}]");
                }

                SkipTag(tagType, stream);

                return true;
            }

            property.Read(obj, stream);
            return true;
        }

        private void WriteCompoundData(NbtBinaryWriter stream, object value) {
            if (value != null) {
                foreach (var property in Properties.Values) {
                    property.Write(value, stream);
                }
            }

            stream.Write(NbtTagType.End);
        }

        private bool CheckLoopReference(object value, out bool enabled) {
            //loop reference handling
            enabled = value != null && Settings.LoopReferenceHandling != LoopReferenceHandling.Serialize;

            if (enabled) {
                if ( _trace.Contains(value)) {
                    if (Settings.LoopReferenceHandling == LoopReferenceHandling.Error) {
                        throw new NbtSerializationException($"Loop reference on type [{Type}] detected");
                    }

                    return true;
                }

                _trace.Add(value);
            }

            return false;
        }

        private void SkipTag(NbtTagType tagType, NbtBinaryReader stream) {
            NbtTag newTag;

            switch (tagType) {
                case NbtTagType.End:
                    return;

                case NbtTagType.Byte:
                    newTag = new NbtByte();
                    break;

                case NbtTagType.Short:
                    newTag = new NbtShort();
                    break;

                case NbtTagType.Int:
                    newTag = new NbtInt();
                    break;

                case NbtTagType.Long:
                    newTag = new NbtLong();
                    break;

                case NbtTagType.Float:
                    newTag = new NbtFloat();
                    break;

                case NbtTagType.Double:
                    newTag = new NbtDouble();
                    break;

                case NbtTagType.ByteArray:
                    newTag = new NbtByteArray();
                    break;

                case NbtTagType.String:
                    newTag = new NbtString();
                    break;

                case NbtTagType.List:
                    newTag = new NbtList();
                    break;

                case NbtTagType.Compound:
                    newTag = new NbtCompound();
                    break;

                case NbtTagType.IntArray:
                    newTag = new NbtIntArray();
                    break;

                case NbtTagType.LongArray:
                    newTag = new NbtLongArray();
                    break;

                default:
                    throw new NbtFormatException("Unsupported tag type found in NBT_Compound: " + tagType);
            }
            newTag.SkipTag(stream);
        }

        private void Release(object value) {
            _trace.Remove(value);
        }

        private object Default() {
            return Type.IsValueType ? Activator.CreateInstance(Type) : default;
        }
    }
}
