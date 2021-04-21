using System;
using System.Collections.Generic;
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
        public static string Concat(params string[] s)
        {
            return ClrConcatString(s);
        }

        public extern static bool op_Equality(string a, string b);

        private static extern string ClrConcatString(params string[] s);
    }
}
