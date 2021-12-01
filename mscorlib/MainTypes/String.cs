using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public class String
    {
        public char get_Chars(int index)
        {
            return String_get_Chars_1(index);
        }
        public int get_Length()
        {
            return strLen();
        }
        public static bool IsNullOrEmpty(string s)
        {
            if (s == null)
                return true;

            if (s == "")
                return true;

            return false;
        }
        public string ToUpper()
        {
            return String_ToUpper();
        }
        public string ToLower()
        {
            return String_ToLower();
        }
        public static bool Equals(string a, string b, StringComparison comparisonType)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (comparisonType == StringComparison.CurrentCulture | comparisonType == StringComparison.CurrentCultureIgnoreCase)
            {
                Console.WriteLine("String.cs:Equals() CurrentCulture not implemented!");
                return false;
            }
            if (comparisonType == StringComparison.InvariantCulture | comparisonType == StringComparison.InvariantCultureIgnoreCase)
            {
                Console.WriteLine("String.cs:Equals() CurrentCulture not implemented!");
                return false;
            }
            if (comparisonType == StringComparison.Ordinal)
                return EqualsHelper(a, b);

            if (comparisonType == StringComparison.OrdinalIgnoreCase) 
                return String_EqualsOrdinalIgnoreCaseNoLengthCheck(a, b);
            Console.WriteLine("string.cs: end of flow");
            return false;
        }

        private static bool EqualsHelper(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) { return false; }
            }
            return true;
        }

        //Internal calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Concat(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Concat(string a, string b, string c);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Concat(string a, string b, string c, string d);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool op_Equality(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int strLen();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static char String_get_Chars_1(int i);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static string String_ToUpper();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static string String_ToLower();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool String_EqualsOrdinalIgnoreCaseNoLengthCheck(string a, string b);
    }
    public enum StringComparison
    {
        CurrentCulture,
        CurrentCultureIgnoreCase,
        InvariantCulture,
        InvariantCultureIgnoreCase,
        Ordinal,
        OrdinalIgnoreCase
    }
}
