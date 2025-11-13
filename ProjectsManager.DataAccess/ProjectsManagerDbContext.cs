using Microsoft.EntityFrameworkCore;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess;

public class ProjectsManagerDbContext(DbContextOptions<ProjectsManagerDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
}