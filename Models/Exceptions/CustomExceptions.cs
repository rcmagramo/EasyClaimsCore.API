namespace EasyClaimsCore.API.Models.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message) : base(message) { }
        public ExternalApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MethodAccessException : Exception
    {
        public MethodAccessException(string message) : base(message) { }
        public MethodAccessException(string message, Exception innerException) : base(message, innerException) { }
    }
}