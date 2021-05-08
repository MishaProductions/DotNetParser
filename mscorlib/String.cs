using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public sealed class String
    {
        public readonly int Length;
        public static bool IsNullOrIsEmpty(string s)
        {
            return s == null || s == "";
        }

        public static string Concat(string a, string b)
        {
            return ClrConcatString(a,b);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool op_Equality(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string ClrConcatString(string a, string b);
    }
}
