using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public sealed class String
    {
        public readonly int Length;
        public static bool IsNullOrEmpty(string s)
        {
            if (s == null)
                return true;

            if (s == "")
                return true;

            return false;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Concat(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Concat(string a, string b, string c);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool op_Equality(string a, string b);
    }
}
