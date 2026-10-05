namespace Shared.Exceptions.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message, 403)
        {
        }
    }
}
