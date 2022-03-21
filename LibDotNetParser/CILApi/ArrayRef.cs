using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDotNetParser.CILApi
{
    public class ArrayRef
    {
        public MethodArgStack[] Items;
        public int Length;
        public int Index { get; set; }
    }
}
