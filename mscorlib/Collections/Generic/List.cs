using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic
{
    public class List<T>
    {
        private int count;
        public int get_Count()
        {
            return count;
        }
        public List()
        {

        }
        public void Add(T item)
        {
            InternalAddItemToList(this, item);
            count++;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InternalAddItemToList(List<T> list, T item);
    }
}
