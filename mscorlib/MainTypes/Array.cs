using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public abstract class Array
    {
        public static T[] Empty<T>()
        {
            return EmptyArray<T>.Value;
        }
    }
    // Useful in number of places that return an empty byte array to avoid unnecessary memory allocation.
    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
