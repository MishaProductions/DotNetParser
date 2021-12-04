namespace System
{
    public struct UInt16
    {
        public override string ToString()
        {
            return NumberFormatUtils.Internal__System_UInt16_ToString(this); //this will magicly return our value
        }
    }
}
