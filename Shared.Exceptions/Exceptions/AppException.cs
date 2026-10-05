namespace Shared.Exceptions.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
