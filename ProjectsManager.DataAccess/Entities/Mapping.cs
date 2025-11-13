using ProjectsManager.Core.Models;

namespace ProjectsManager.DataAccess.Entities;

public static class Mapping
{
    public static Project ToDomain(this ProjectEntity entity)
    {
        var employeeIds = entity.Employees?.Select(e => e.Id) ?? [];

        return Project.Reconstruct(
            entity.Id,
            entity.Title,
            entity.CustomerCompanyName,
            entity.ContractorCompanyName,
            entity.Priority,
            entity.StartDate,
            entity.EndDate,
            entity.LeaderId,
            employeeIds);
    }
    public static void Apply(this ProjectEntity e, Project p)
    {
        e.Title = p.Title;
        e.CustomerCompanyName = p.CustomerCompanyName;
        e.ContractorCompanyName = p.ContractorCompanyName;
        e.Priority = p.Priority;
        e.StartDate = p.StartDate;
        e.EndDate = p.EndDate;
        e.LeaderId = p.LeaderId;
    }

    public static ProjectEntity ToEntity(this Project p)
        => new()
        {
            Id = p.Id,
            Title = p.Title,
            CustomerCompanyName = p.CustomerCompanyName,
            ContractorCompanyName = p.ContractorCompanyName,
            Priority = p.Priority,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            LeaderId = p.LeaderId
        };

    public static Employee ToDomain(this EmployeeEntity e)
    {
        var projectsIds = e.Projects?.Select(p => p.Id) ?? [];
        return Employee.Create(e.Id, e.FirstName, e.LastName, e.Email, projectsIds, e.Patronymic).Value;
    }

    public static EmployeeEntity ToEntity(this Employee e)
        => new()
        {
            Id = e.Id,
            FirstName = e.Firstname,
            LastName = e.Lastname,
            Patronymic = e.Patronymic,
            Email = e.Email
        };
}