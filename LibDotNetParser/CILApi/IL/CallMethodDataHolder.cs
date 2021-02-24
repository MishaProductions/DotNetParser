using System;
using System.Collections.Generic;
using System.Text;

namespace LibDotNetParser.CILApi.IL
{
    class CallMethodDataHolder
    {
        public string NameSpace { get; set; }
        public string ClassName { get; set; }
        public string FunctionName { get; set; }
        public DotNetMethod ResolvedMethod { get; internal set; }
    }
}
