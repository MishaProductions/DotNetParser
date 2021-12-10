namespace System.Collections.Generic
{
    public class Dictionary<TKey, TValue>
    {
        private class Entry
        {
            public int next = 0;        // Index of next entry, -1 if last
            public TKey key = default;           // Key of entry
            public TValue value = default;         // Value of entry
        }
        private Entry[] entries;
        private int count = 0;
        private IEqualityComparer<TKey> comparer;

        public TValue this[TKey key]
        {
            get
            {
                return _getVal(key);
            }
            set
            {
                _SetVal(key, true);
            }
        }

        private void _SetVal(TKey key, bool overwrite)
        {
            
        }

        private TValue _getVal(TKey key)
        {
            Console.WriteLine("_getVal not implemented");
            return default;
        }

        public bool TryGetValue(TKey t, out TValue v)
        {
            var i = FindEntry(t);
            if (i != -1)
            {
                v = entries[i].value;
                return true;
            }
            else
            {
                v = default;
                return false;
            }
        }

        private int FindEntry(TKey key)
        {
            for (int i = 0; i < count; i++)
            {
                var v = entries[i];
                if (v.key.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }
        public Dictionary() : this(0, null)
        {

        }
        public Dictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        {
            entries = new Entry[100];
        }
        public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
        }
    }
}
