using LibDotNetParser.CILApi;

namespace LibDotNetParser
{
    public class MethodArgStack
    {
        public static readonly MethodArgStack ldnull = new MethodArgStack() { type = StackItemType.ldnull};
        public StackItemType type;
        public object value;

        public DotNetType ObjectType;
        public DotNetMethod ObjectContructor;

        public int ArrayLen;
        public MethodArgStack[] ArrayItems;

        public override string ToString()
        {
            switch (type)
            {
                case StackItemType.String:
                    return (string)value;
                case StackItemType.Int32:
                    return ((int)value).ToString();
                case StackItemType.Int64:
                    return ((ulong)value).ToString();
                case StackItemType.ldnull:
                    return "NULL";
                case StackItemType.NotImpl:
                    return "Not implemented";
                case StackItemType.Float32:
                    return ((float)value).ToString();
                case StackItemType.Float64:
                    return ((decimal)value).ToString();
                case StackItemType.Object:
                    return "Object: "+ObjectType.FullName;
                case StackItemType.Array:
                    return "Array";
                case StackItemType.Char:
                    return ((char)value).ToString();
                case StackItemType.ObjectRef:
                    return "Object refrence to " + ObjectType.FullName;
                case StackItemType.MethodPtr:
                    return "Method Pointer to " + ((DotNetMethod)value).ToString();
                default:
                    return "Unknown";
            }
        }
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
        Object,
        Array,
        Char,
        ObjectRef,
        MethodPtr
    }
}