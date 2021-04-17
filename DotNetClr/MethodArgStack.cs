namespace DotNetClr
{
    internal class MethodArgStack
    {
        public StackItemType type;
        public object value;
    }

    internal enum StackItemType
    {
        String,
        Int32,
        Int64,
        ldnull,
        NotImpl,
    }
}