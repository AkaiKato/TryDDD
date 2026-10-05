using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth.Services;
using System.Text;


namespace Shared.Auth.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddSharedAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
