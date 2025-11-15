using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.DataAccess.Entities;

public static class Mapping
{
    public static Project ToDomain(this ProjectEntity entity)
    {
        var employeeIds = entity.Employees.Select(e => e.Id);

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

    public static ProjectEntity ToEntity(this Project domain)
    {
        return new ProjectEntity
        {
            Id = domain.Id,
            Title = domain.Title,
            CustomerCompanyName = domain.CustomerCompanyName,
            ContractorCompanyName = domain.ContractorCompanyName,
            Priority = domain.Priority,
            StartDate = domain.StartDate,
            EndDate = domain.EndDate,
            LeaderId = domain.LeaderId,
            Employees = new List<EmployeeEntity>()
        };
    }

    public static void ApplyDomainChanges(this ProjectEntity entity, Project domain)
    {
        entity.Title = domain.Title;
        entity.CustomerCompanyName = domain.CustomerCompanyName;
        entity.ContractorCompanyName = domain.ContractorCompanyName;
        entity.Priority = domain.Priority;
        entity.StartDate = domain.StartDate;
        entity.EndDate = domain.EndDate;
        entity.LeaderId = domain.LeaderId;
    }

    public static Employee ToDomain(this EmployeeEntity entity)
    {
        var projectIds = entity.Projects.Select(p => p.Id);

        return Employee.Reconstruct(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Patronymic,
            entity.Email,
            projectIds);
    }

    public static EmployeeEntity ToEntity(this Employee domain)
    {
        return new EmployeeEntity
        {
            Id = domain.Id,
            FirstName = domain.Firstname,
            LastName = domain.Lastname,
            Patronymic = domain.Patronymic,
            Email = domain.Email,
            Projects = []
        };
    }

    public static void ApplyDomainChanges(this EmployeeEntity entity, Employee domain)
    {
        entity.FirstName = domain.Firstname;
        entity.LastName = domain.Lastname;
        entity.Patronymic = domain.Patronymic;
        entity.Email = domain.Email;
    }
}