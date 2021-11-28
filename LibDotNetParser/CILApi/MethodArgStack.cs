using LibDotNetParser.CILApi;

namespace LibDotNetParser
{
    public class MethodArgStack
    {
        public static readonly MethodArgStack ldnull = new MethodArgStack() { type = StackItemType.ldnull };
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
                case StackItemType.Float32:
                    return ((float)value).ToString();
                case StackItemType.Float64:
                    return ((decimal)value).ToString();
                case StackItemType.Object:
                    return "Object: " + ObjectType.FullName;
                case StackItemType.Array:
                    return "Array";
                case StackItemType.ObjectRef:
                    return "Object refrence to " + ObjectType.FullName;
                case StackItemType.MethodPtr:
                    return "Method Pointer to " + ((DotNetMethod)value).ToString();
                default:
                    return "Unknown";
            }
        }

        public static MethodArgStack Int32(int value)
        {
            return new MethodArgStack() { type = StackItemType.Int32, value = value };
        }
        public static MethodArgStack Int64(long value)
        {
            return new MethodArgStack() { type = StackItemType.Int64, value = value };
        }
        public static MethodArgStack String(string value)
        {
            return new MethodArgStack() { type = StackItemType.String, value = value };
        }
        public static MethodArgStack Null()
        {
            return ldnull;
        }
        public static MethodArgStack Float32(float value)
        {
            return new MethodArgStack() { type = StackItemType.String, value = value };
        }
        public static MethodArgStack Float64(float value)
        {
            return new MethodArgStack() { type = StackItemType.Float64, value = value };
        }
        public static MethodArgStack ObjectRef(DotNetType type)
        {
            return new MethodArgStack() { type = StackItemType.ObjectRef, ObjectType = type };
        }
        public static MethodArgStack MethodPtr(DotNetMethod method)
        {
            return new MethodArgStack() { type = StackItemType.MethodPtr, value = method };
        }
    }

    public enum StackItemType
    {
        None,
        String,
        Int32,
        Int64,
        ldnull,
        Float32,
        Float64,
        Object,
        Array,
        ObjectRef,
        MethodPtr,
        IntPtr,
        Any
    }
}