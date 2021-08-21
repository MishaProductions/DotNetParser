using mscorlib;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public struct SByte
    {
        public override string ToString()
        {
            return FormatUtils.Internal__System_SByte_ToString(); //this will magicly return our value
        }
    }
}
