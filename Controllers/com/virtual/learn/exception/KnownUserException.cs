namespace garda.Exceptions
{
    internal class KnownUserException : System.Exception
    {
        public KnownUserException() {}
        public KnownUserException(string message) : base(message) {}
        public KnownUserException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}