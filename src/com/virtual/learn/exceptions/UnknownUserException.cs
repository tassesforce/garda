namespace garda.Exceptions
{
    internal class UnknownUserException : System.Exception
    {
        public UnknownUserException() {}
        public UnknownUserException(string message) : base(message) {}
        public UnknownUserException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}