namespace garda.Exceptions
{
    internal class UnauthorizedUserException : System.Exception
    {
        public UnauthorizedUserException() {}
        public UnauthorizedUserException(string message) : base(message) {}
        public UnauthorizedUserException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}