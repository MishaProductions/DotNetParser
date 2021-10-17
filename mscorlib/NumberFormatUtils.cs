using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    internal class NumberFormatUtils
    {

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Byte_ToString(byte a);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Byte_ToString();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_SByte_ToString();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_UInt16_ToString();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Int16_ToString();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Int32_ToString();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_UInt32_ToString();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Char_ToString();
    }
}
