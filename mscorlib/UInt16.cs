using mscorlib;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public struct UInt16
    {
        public override string ToString()
        {
            return FormatUtils.Internal__System_UInt16_ToString(); //this will magicly return our value
        }
    }
}
