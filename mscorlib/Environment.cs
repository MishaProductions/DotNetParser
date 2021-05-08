using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    class Environment
    {
        public static int get_SystemPageSize()
        {
            //idk weather this will be breaking some app, but this will be the way to deterim if running under dotnetparser
            return 1;
        }
    }
}
