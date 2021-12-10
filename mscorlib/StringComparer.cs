namespace System
{
    public abstract class StringComparer
    {
        public static StringComparer OrdinalIgnoreCase
        {
            get
            {
                return OrdinalIgnoreCaseComparer.Instance;
            }
        }


    }
    internal sealed class OrdinalIgnoreCaseComparer : StringComparer
    {
        internal static readonly OrdinalIgnoreCaseComparer Instance = new OrdinalIgnoreCaseComparer();
        public int Compare(string x, string y)
        {
            Console.WriteLine("TODO: int Compare(string, string) in OrdinalIgnoreCaseComparer");
            return 0;
        }

        // Token: 0x06001801 RID: 6145 RVA: 0x000F2B1D File Offset: 0x000F1D1D
        public bool Equals(string x, string y)
        {
            return String.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
