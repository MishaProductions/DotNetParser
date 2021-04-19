namespace System
{
    /// <summary>
    /// Console class
    /// </summary>
    public static class Console
    {
        //Implemented in the CLR
        public static extern void WriteLine(string str);
        //Implemented in the CLR
        public static extern void WriteLine(int num);
        //Implemented in the CLR
        public static extern void WriteLine(long num);
        //Implemented in the CLR
        public static extern void WriteLine();
        //Implemented in the CLR
        public static extern void Write(string str);
        //Implemented in the CLR
        public static extern void Write(int num);
        //Implemented in the CLR
        public static extern void Write(long num);
        //Implemented in the CLR
        public static extern void Clear();
    }
}
