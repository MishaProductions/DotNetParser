using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public class List<T>
    {
        private const int _defaultCapacity = 4;

        private T[] _items;
        private int _size;

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
            if (capacity == 0)
                _items = _emptyArray;
            else
                _items = new T[capacity];
        }
        public void Add(T item)
        {
            List_AddItem(this, _size++, item);
        }
        public T get_Item(int index)
        {
            return _items[index];
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void List_AddItem(List<T> array, int index, T item);
    }
}
