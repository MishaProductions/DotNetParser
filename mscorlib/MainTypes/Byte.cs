namespace System
{
    public struct Byte
    {
        private byte m_value;

        // The maximum value that a Byte may represent: 255.
        public const byte MaxValue = (byte)0xFF;

        // The minimum value that a Byte may represent: 0.
        public const byte MinValue = 0;

        public override string ToString()
        {
            return NumberFormatUtils.Internal__System_Byte_ToString(); //this will magicly return our value
        }
    }
}
