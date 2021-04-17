using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
    public static class Console
    {
        //Implemented in the CLR
        public static extern void WriteLine(string str);
        //Implemented in the CLR
        public static extern void WriteLine(int num);
    }
}
