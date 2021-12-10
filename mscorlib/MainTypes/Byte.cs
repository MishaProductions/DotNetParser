namespace System
{
    public struct Byte
    {
        public override string ToString()
        {
            return Internal.NumberFormatUtils.Internal__System_Byte_ToString(this); //this will magicly return our value
        }
    }
}
