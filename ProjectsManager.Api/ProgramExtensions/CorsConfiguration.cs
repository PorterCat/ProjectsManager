namespace ProjectsManager.Api;

public static class CorsConfiguration
{
    public static IServiceCollection AddDevCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Dev",
                policy =>
                {
                    policy.WithOrigins(configuration.GetConnectionString("NextJsClient") ?? throw new InvalidOperationException())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }
}