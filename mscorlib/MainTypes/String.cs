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
        //public string ToUpper()
        //{
        //    return String_ToUpper();
        //}
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
    }
}
