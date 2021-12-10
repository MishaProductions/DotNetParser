using LibDotNetParser.CILApi;

namespace libDotNetClr
{
    public class CallStackItem
    {
        public DotNetMethod method;

        public override string ToString()
        {
            return method.ToString();
            var s = "a";
            var b = s[0];
        }
    }
}