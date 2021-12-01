using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class Dictionary<TKey, TValue>
    {
        private class Entry
        {
            public int next;        // Index of next entry, -1 if last
            public TKey key;           // Key of entry
            public TValue value;         // Value of entry
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
                //_SetValOverwrite(key);
            }
        }

        private void _SetValOverwrite(TKey key)
        {
            Console.WriteLine("_SetValOverwrite not implemented");
        }

        private TValue _getVal(TKey key)
        {
            Console.WriteLine("_getVal not implemented");
            return default(TValue);
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
