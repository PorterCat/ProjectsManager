using CSharpFunctionalExtensions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IAssignmentService
{
    Task<Result<int>> AssignEmployeesToProject(Guid projectId, IEnumerable<Guid> employeeId);
    Task<Result> AssignProjectLeader(Guid projectId, Guid? leaderId);
    Task<ICollection<Employee>> GetProjectEmployees(Guid projectId);
    Task<ICollection<Project>> GetEmployeeProjects(Guid employeeId);
    Task<ICollection<Project>> GetEmployeeLeadingProjects(Guid employeeId);
}