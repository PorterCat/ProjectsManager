using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectsManager.Business.Auth;

namespace ProjectsManager.Api;

public static class AuthRegistration
{
    public static void AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(
            configuration.GetSection(nameof(AuthSettings)));
        
        services.AddScoped<JwtService>();
        services.AddScoped<DefaultAdminInitializer>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection(nameof(AuthSettings))[nameof(AuthSettings.SecretKey)] ?? string.Empty)),
                };
            });
        
        services.AddAuthorization();
    }
}