namespace EasyClaimsCore.API.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetUserMessage(this Exception exception)
        {
            return exception.InnerException?.Message ?? exception.Message;
        }

        public static string GetInnerExceptionMessage(this Exception exception)
        {
            var innermost = exception;
            while (innermost.InnerException != null)
            {
                innermost = innermost.InnerException;
            }
            return innermost.Message;
        }
    }
}