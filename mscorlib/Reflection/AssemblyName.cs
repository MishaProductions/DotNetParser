using System;
using System.Collections.Generic;
using System.Text;

namespace System.Reflection
{
    public class AssemblyName
    {
        public string internal__name;
        public AssemblyName(string name)
        {
            internal__name = name;
        }
    }
}
