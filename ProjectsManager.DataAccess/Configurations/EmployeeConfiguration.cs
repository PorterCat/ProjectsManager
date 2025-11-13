using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("Employee");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
               .IsRequired();
        builder.Property(e => e.LastName)
               .IsRequired();
        builder.Property(e => e.Patronymic)
               .IsRequired(false);

        builder.HasIndex(e => e.Email)
               .IsUnique();

        builder.HasMany(e => e.Projects)
               .WithMany(p => p.Employees);
    }
}