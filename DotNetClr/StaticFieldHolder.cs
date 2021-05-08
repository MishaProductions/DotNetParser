using LibDotNetParser;
using LibDotNetParser.CILApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace libDotNetClr
{
    internal class StaticFieldHolder
    {
        public static List<StaticField> staticFields = new List<StaticField>();
    }
    internal class StaticField
    {
        public MethodArgStack value;
        public DotNetField theField;
    }
}
