namespace System
{
    public class Exception
    {
        internal string _message;
        public virtual string Message
        {
            get
            {
                return _message;
            }
        }

        public Exception(String message)
        {
            _message = message;
        }
        public Exception()
        {
            _message = "<no message>";
        }
    }
}
