using NSwag;
using NSwag.Generation.Processors.Security;

namespace ProjectsManager.Api;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddOpenApiDocument(options =>
        {
            options.AddSecurity("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Bearer token authorization header",
                Type = OpenApiSecuritySchemeType.Http,
                In = OpenApiSecurityApiKeyLocation.Header,
                Name = "Authorization",
                Scheme = "Bearer"
            });

            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });
        services.AddEndpointsApiExplorer();
        return services;
    }
}