using CSharpFunctionalExtensions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsService
{
    Task<Result<Guid>> AddProject(Project project, Guid? leaderId);
    Task<Result> AssignEmployees(Project project, IEnumerable<Guid> employeeIdsToAdd, IEnumerable<Guid> employeesIdsToRemove);
}