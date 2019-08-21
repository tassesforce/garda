namespace garda.Exceptions
{
    internal class UnknownClientAppException : System.Exception
    {
        public UnknownClientAppException() {}
        public UnknownClientAppException(string message) : base(message) {}
        public UnknownClientAppException(string message, System.Exception innerException) : base(message, innerException) {}
    }
}