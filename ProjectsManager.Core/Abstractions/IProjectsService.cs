using CSharpFunctionalExtensions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsService
{
    // Task<Result<Guid>> AddProject(Project project, Guid? leaderId);
    //Task<Result> AssignEmployees(Project project, IEnumerable<Guid> employeeIdsToAdd, IEnumerable<Guid> employeesIdsToRemove);

    Task<Result<Guid>> CreateProject(CreateProjectRequest request);
    Task<Result> AssignEmployees(Guid projectId,
        IEnumerable<Guid> employeeIdsToAdd, IEnumerable<Guid> employeeIdsToRemove);
    Task<Result> UpdateProject(Guid projectId, PatchProjectRequest request);
    Task<Result> UpdateProjectLeader(Guid projectId, Guid leaderId);
}