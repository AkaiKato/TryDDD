using System.Security.Claims;

namespace Shared.Auth.Constants
{
    public class AuthConstants
    {
        public const string UserIdClaimType = ClaimTypes.NameIdentifier;
        public const string RoleClaimType = ClaimTypes.Role; 
        public const string BrearerScheme = "Bearer";
    }
}
