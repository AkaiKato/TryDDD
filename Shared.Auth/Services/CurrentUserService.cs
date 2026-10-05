using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Shared.Auth.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid? UserId
        {
            get
            {
                var user = httpContextAccessor.HttpContext?.User;
                
                if(user == null || !user.Identity?.IsAuthenticated == true)
                    return null;

                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return null;
                
                return userId;
            }
        }
    }
}
