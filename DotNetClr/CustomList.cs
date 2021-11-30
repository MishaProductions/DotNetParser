//#define STACK_DEBUG
using LibDotNetParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace libDotNetClr
{
    /// <summary>
    /// This class is used for debugging a List<>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CustomList<T>
    {
        public List<T> backend = new List<T>();
        public int Count { get { return backend.Count; } }

        public T this[int index]
        {
            get { return backend[index]; }
            set
            {
                backend[index] = value;
                if (value == null)
                {
                    Console.WriteLine("***Setting a value to null on stack***");
                }
            }
        }

        public CustomList()
        {
        }
        public void Clear()
        {
#if STACK_DEBUG
            Console.WriteLine("Clearing the stack");
            //throw new Exception();
#endif
            backend.Clear();
        }
        public void Add(T a)
        {
            backend.Add(a);
        }

        public void RemoveAt(int m)
        {
            backend.RemoveAt(m);
        }
        public void RemoveRange(int index, int count)
        {
            backend.RemoveRange(index, count);
        }

        public T[] ToArray()
        {
            return backend.ToArray();
        }
        public List<T>.Enumerator GetEnumerator()
        {
            return backend.GetEnumerator();
        }
        public void MoveNext()
        {
            backend.GetEnumerator().MoveNext();

        }
        public T Current { get { return backend.GetEnumerator().Current; } }
    }
}