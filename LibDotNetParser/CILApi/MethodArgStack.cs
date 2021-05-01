using LibDotNetParser.CILApi;

namespace LibDotNetParser
{
    public class MethodArgStack
    {
        public StackItemType type;
        public object value;

        public DotNetType ObjectType;
        public DotNetMethod ObjectContructor;
    }

    public enum StackItemType
    {
        String,
        Int32,
        Int64,
        ldnull,
        NotImpl,
        Float32,
        Float64,
        Object
    }
}