using Microsoft.EntityFrameworkCore;
using ProjectsManager.DataAccess;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.Api;

public class DefaultAdminInitializer(IServiceProvider serviceProvider)
{
    public async Task InitializeAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectsManagerDbContext>();
        
        var existingAdmin = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Email == "admin@admin.com");
            
        if (existingAdmin is null)
        {
            var admin = new EmployeeEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@admin.com"
                // Password: password
                // Role = EmployeeRole.Director
            };
            
            dbContext.Employees.Add(admin);
            await dbContext.SaveChangesAsync();
        }
    }
}