namespace System
{
    public struct UInt32
    {
        public override string ToString()
        {
            return Internal.NumberFormatUtils.UInt32ToString(this);
        }
    }
}
