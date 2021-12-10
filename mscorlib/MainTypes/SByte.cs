namespace System
{
    public struct SByte
    {
        public override string ToString()
        {
            return Internal.NumberFormatUtils.Internal__System_SByte_ToString(this); //this will magicly return our value
        }
    }
}
