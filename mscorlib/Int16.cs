using mscorlib;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public struct Int16
    {
        public override string ToString()
        {
            return FormatUtils.Internal__System_Int16_ToString(); //this will magicly return our value
        }
    }
}
