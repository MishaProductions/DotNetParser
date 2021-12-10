namespace System
{
    public struct Int16
    {
        public override string ToString()
        {
            return Internal.NumberFormatUtils.Internal__System_Int16_ToString(this); //this will magicly return our value
        }
    }
}
