using CSharpFunctionalExtensions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IAssignmentService
{
    Task<Result<int>> AssignEmployeesToProject(Guid projectId, IEnumerable<Guid> employeeId);
    Task<Result> AssignProjectLeader(Guid projectId, Guid? leaderId);
    Task<ICollection<Employee>> GetEmployeesByProject(Guid projectId);
    Task<ICollection<Project>> GetProjectsByEmployee(Guid employeeId);
    Task<ICollection<Project>> GetLeadingProjectsByEmployee(Guid employeeId);
    Task<Employee?> GetProjectLeader(Guid projectId);
}