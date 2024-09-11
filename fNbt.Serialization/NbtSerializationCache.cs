using System;
using System.Collections.Generic;
using fNbt.Serialization.Converters;
using fNbt.Serialization.Handlings;

namespace fNbt.Serialization {
    internal class NbtSerializationCache {
        [ThreadStatic]
        private static List<object> _trace;

        public Type Type { get; set; }

        public Dictionary<string, INbtSerializationMember> Members { get; set; } = new Dictionary<string, INbtSerializationMember>();

        public NbtConverter Converter { get; set; }

        public NbtSerializerSettings Settings { get; set; }

        public NbtTagType GetTagType(Type type) {
            if (Converter != null) {
                return Converter.GetTagType(type, Settings);
            } else if (type != Type) {
                return NbtSerializer.DynamicNbtConverter.GetTagType(type, Settings);
            } else {
                return NbtTagType.Compound;
            }
        }

        public object Read(NbtBinaryReader stream, object value, string name = null) {
            if (name == null) {
                var tagType = stream.ReadTagType();

                if (tagType == NbtTagType.Unknown) {
                    throw new NbtSerializationException($"Readed unknown tag type of [{Type}]");
                }

                name = stream.ReadString();
            }

            if (Converter != null && Converter.CanRead) {
                return Converter.Read(stream, Type, value, name, Settings);
            } else {
                var obj = value ?? Activator.CreateInstance(Type);

                while (ReadMember(obj, stream)) ;

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
                    NbtSerializer.DynamicNbtConverter.Write(stream, value, name, Settings);
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
                    NbtSerializer.DynamicNbtConverter.WriteData(stream, value, Settings);
                } else {
                    WriteCompoundData(stream, value);
                }
            } finally {
                if (release) Release(value);
            }
        }

        public object FromNbt(NbtTag tag, object value) {
            if (Converter != null && Converter.CanRead) {
                return Converter.FromNbt(tag, Type, value, Settings);
            } else {
                var obj = value ?? Activator.CreateInstance(Type);

                foreach (var child in tag as NbtCompound) {
                    if (!Members.TryGetValue(child.Name, out var member)) {
                        if (Settings.MissingMemberHandling == MissingMemberHandling.Error) {
                            throw new NbtSerializationException($"Missing member [{child.Name}]");
                        }

                        continue;
                    }

                    member.FromNbt(obj, child);
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
                    return NbtSerializer.DynamicNbtConverter.ToNbt(value, name, Settings);
                } else {
                    var compound = new NbtCompound(name);

                    foreach (var member in Members.Values) {
                        var tag = member.ToNbt(value);

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

        private bool ReadMember(object obj, NbtBinaryReader stream) {
            var tagType = stream.ReadTagType();

            if (tagType == NbtTagType.End) return false;

            if (tagType == NbtTagType.Unknown) {
                throw new NbtSerializationException("Readed unknown tag type");
            }

            var name = stream.ReadString();

            if (!Members.TryGetValue(name, out var member)) {
                if (Settings.MissingMemberHandling == MissingMemberHandling.Error) {
                    throw new NbtSerializationException($"Missing member [{name}]");
                }

                SkipTag(tagType, stream);

                return true;
            }

            member.Read(obj, stream);
            return true;
        }

        private void WriteCompoundData(NbtBinaryWriter stream, object value) {
            if (value != null) {
                foreach (var member in Members.Values) {
                    member.Write(value, stream);
                }
            }

            stream.Write(NbtTagType.End);
        }

        private bool CheckLoopReference(object value, out bool enabled) {
            //loop reference handling
            enabled = value != null && Settings.LoopReferenceHandling != LoopReferenceHandling.Serialize;

            if (enabled) {
                _trace ??= new List<object>();
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
            _trace?.Remove(value);
        }

        private object Default() {
            return Type.IsValueType ? Activator.CreateInstance(Type) : default;
        }
    }
}
