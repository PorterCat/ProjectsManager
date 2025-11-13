using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
{
    public void Configure(EntityTypeBuilder<ProjectEntity> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title)
               .HasMaxLength(byte.MaxValue)
               .IsRequired();

        builder.Property(p => p.CustomerCompanyName)
               .HasMaxLength(byte.MaxValue);

        builder.Property(p => p.ContractorCompanyName)
               .HasMaxLength(byte.MaxValue);

        builder.HasOne(p => p.Leader)
               .WithMany(l => l.LeadingProjects)
               .HasForeignKey(p => p.LeaderId)
               .IsRequired(false);

        builder.Property(p => p.StartDate)
               .HasColumnType("date");

        builder.Property(p => p.EndDate)
               .HasColumnType("date")
               .IsRequired(false);

        builder.HasMany(p => p.Employees)
               .WithMany(e => e.Projects);
    }
}