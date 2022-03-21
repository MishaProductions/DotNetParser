﻿using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Array
    {
        public extern int Length
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

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
