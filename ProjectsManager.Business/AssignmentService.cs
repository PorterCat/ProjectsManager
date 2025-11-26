using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class AssignmentService(
    IProjectsRepository projectsRepository,
    IEmployeesRepository employeesRepository) : IAssignmentService
{
    public async Task<Result<int>> AssignEmployeesToProject(Guid projectId, IEnumerable<Guid> employeeIds)
    {
        var project = await projectsRepository.GetById(projectId);
        if (project is null)
            return Result.Failure<int>($"Project with id {projectId} not found.");
        
        if (project.EndDate.HasValue && project.EndDate < DateOnly.FromDateTime(DateTime.Now))
            return Result.Failure<int>("Cannot assign employees to completed project.");
        
        return Result.Success(await projectsRepository.UpdateProjectEmployees(projectId, employeeIds));
    }

    public async Task<Result> AssignProjectLeader(Guid projectId, Guid? leaderId)
    {
        var project = await projectsRepository.GetById(projectId);
        if (project is null)
            return Result.Failure($"Project with id {projectId} not found.");
        
        if (leaderId is null)
        {
            await projectsRepository.UpdateProjectLeader(projectId, null);
            return Result.Success();
        }
        
        var leader = await employeesRepository.GetById(leaderId.Value);
        if (leader is null)
            return Result.Failure($"Employee with id {leaderId} not found.");
        
        await projectsRepository.UpdateProjectLeader(projectId, leaderId.Value);
        return Result.Success();
    }

    public async Task<ICollection<Employee>> GetProjectEmployees(Guid projectId) =>
        await projectsRepository.GetEmployeesByProject(projectId);

    public async Task<ICollection<Project>> GetEmployeeProjects(Guid employeeId) =>
        await employeesRepository.GetProjectsByEmployee(employeeId);

    public async Task<ICollection<Project>> GetEmployeeLeadingProjects(Guid employeeId) =>
        await employeesRepository.GetLeadingProjectsByEmployee(employeeId);
}