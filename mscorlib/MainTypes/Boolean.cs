using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public struct Boolean
    {
        public override string ToString()
        {
            if (Boolean_GetValue(this))
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Boolean_GetValue(Boolean a);
    }
}
