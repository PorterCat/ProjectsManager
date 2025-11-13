using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class ProjectsService(
    IProjectsRepository projectsRepository,
    IEmployeesRepository employeesRepository) : IProjectsService
{
    public async Task<Result<Guid>> AddProject(Project project, Guid? leaderId)
    {
        if (leaderId.HasValue)
        {
            var leader = await employeesRepository.GetById(leaderId.Value);
            if (leader is null)
                return Result.Failure<Guid>($"Leader with id {leaderId} doesn't exist");

            var assignResult = project.AssignLeader(leaderId.Value);
            if (assignResult.IsFailure)
                return Result.Failure<Guid>(assignResult.Error);
        }

        await projectsRepository.Add(project);
        return Result.Success(project.Id);
    }

    public async Task<Result> AssignEmployees(Project project, IEnumerable<Guid> employeeIdsToAdd, IEnumerable<Guid> employeesIdsToRemove)
    {
        foreach (var id in employeeIdsToAdd)
        {
            var result = project.AssignEmployee(id);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        foreach (var id in employeesIdsToRemove)
        {
            var result = project.RemoveEmployee(id);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        await projectsRepository.Save(project);
        return Result.Success();
    }
}