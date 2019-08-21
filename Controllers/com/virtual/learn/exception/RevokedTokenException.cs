namespace garda.Exceptions
{
    internal class RevokedTokenException : System.Exception
    {
        public RevokedTokenException() {}
        public RevokedTokenException(string message) : base(message) {}
        public RevokedTokenException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}