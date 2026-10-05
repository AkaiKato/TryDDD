namespace Shared.Auth.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
