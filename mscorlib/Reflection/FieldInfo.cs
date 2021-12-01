using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection
{
    public class FieldInfo
    {
        public string _internalFieldName;

        public string get_Name()
        {
            return _internalFieldName;
        }
    }
}
