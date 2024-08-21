using System;
using System.Collections;
using System.Collections.Generic;

namespace fNbt {
    public abstract class NbtTagCollection : NbtTag, ICollection<NbtTag>, ICollection {
        public abstract int Count { get; }
        public abstract object SyncRoot { get; }

        public abstract void Add(NbtTag item);
        public abstract void Clear();
        public abstract bool Contains(NbtTag item);
        public abstract void CopyTo(NbtTag[] array, int arrayIndex);
        public abstract IEnumerator<NbtTag> GetEnumerator();
        public abstract bool Remove(NbtTag item);

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        bool ICollection<NbtTag>.IsReadOnly {
            get { return false; }
        }

        void ICollection.CopyTo(Array array, int index) {
            CopyTo((NbtTag[])array, index);
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }
    }
}
