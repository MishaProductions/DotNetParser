//#define CLR_DEBUG
using LibDotNetParser;
using System.Collections.Generic;

namespace DotNetClr
{
    internal class ObjectValueHolder
    {
        public Dictionary<string, MethodArgStack> Fields = new Dictionary<string, MethodArgStack>();
        public ObjectValueHolder()
        {
        }
    }
}