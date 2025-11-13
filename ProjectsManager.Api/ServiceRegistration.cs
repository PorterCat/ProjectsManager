using Microsoft.EntityFrameworkCore;
using ProjectsManager.Business;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.DataAccess;
using ProjectsManager.DataAccess.Repository;

namespace ProjectsManager.Api;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectsManagerDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString(nameof(ProjectsManagerDbContext)));
        });

        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IProjectsService, ProjectsService>();

        services.AddScoped<IEmployeesRepository, EmployeesRepository>();
        services.AddScoped<IEmployeesServices, EmployeesService>();

        return services;
    }
}