using System.Runtime.CompilerServices;

namespace System
{
    internal class NumberFormatUtils
    {

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Byte_ToString(System.Byte a);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_SByte_ToString(System.SByte a);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_UInt16_ToString(System.UInt16 a);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Int16_ToString(System.Int16 a);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Int32_ToString(System.Int32 a);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_UInt32_ToString(System.UInt32 a);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string Internal__System_Char_ToString(System.Char a);
    }
}
