using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic
{
    public class List<T>
    {
        private const int _defaultCapacity = 4;

        private T[] _items;
        private int _size;
        private int _version;
        private Object _syncRoot;

        static readonly T[] _emptyArray = new T[0];
        public int Count
        {
            get { return _size; }
        }
        public List()
        {
            _items = _emptyArray;
        }
        public List(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            if (capacity == 0)
                _items = _emptyArray;
            else
                _items = new T[capacity];
        }

        // Adds the given object to the end of this list. The size of the list is
        // increased by one. If required, the capacity of the list is doubled
        // before adding the new element.
        //
        public void Add(T item)
        {
            _items[_size++] = item;
            _version++;
        }
    }
}
