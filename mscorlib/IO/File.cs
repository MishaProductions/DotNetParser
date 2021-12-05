using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    public static class File
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Exists(string path);
    }
}
