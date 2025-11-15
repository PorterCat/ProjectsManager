using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class ProjectsService(
    IProjectsRepository projectsRepository,
    IEmployeesRepository employeesRepository) : IProjectsService
{
    public async Task<Result<Guid>> CreateProject(CreateProjectRequest request)
    {
        var projectResult = Project.Create(
            request.Title,
            request.CustomerCompanyName,
            request.ContractorCompanyName,
            request.Priority,
            request.StartDate,
            request.EndDate);

        if (projectResult.IsFailure)
            return Result.Failure<Guid>(projectResult.Error);

        var project = projectResult.Value;

        if (request.LeaderId.HasValue)
        {
            var leader = await employeesRepository.GetById(request.LeaderId.Value);
            if (leader is null)
                return Result.Failure<Guid>($"Leader with id {request.LeaderId} doesn't exist");

            var leaderResult = project.AssignLeader(request.LeaderId.Value);
            if (leaderResult.IsFailure)
                return Result.Failure<Guid>(leaderResult.Error);
        }

        if (request.EmployeeIds != null && request.EmployeeIds.Any())
        {
            foreach (var employeeId in request.EmployeeIds)
            {
                if (request.LeaderId.HasValue && employeeId == request.LeaderId.Value)
                    continue;

                var employee = await employeesRepository.GetById(employeeId);
                if (employee is null)
                    return Result.Failure<Guid>($"Employee with id {employeeId} doesn't exist");

                var result = project.AssignEmployee(employeeId);
                if (result.IsFailure)
                    return Result.Failure<Guid>(result.Error);
            }
        }

        await projectsRepository.Add(project);
        return Result.Success(project.Id);
    }

    public async Task<Result> AssignEmployees(Guid projectId,
        IEnumerable<Guid> employeeIdsToAdd, IEnumerable<Guid> employeeIdsToRemove)
    {
        employeeIdsToAdd = employeeIdsToAdd.ToList();
        employeeIdsToRemove = employeeIdsToRemove.ToList();

        var project = await projectsRepository.GetById(projectId);
        if (project is null)
            return Result.Failure($"Project {projectId} not found");

        foreach (var employeeId in employeeIdsToAdd.Concat(employeeIdsToRemove))
        {
            var employee = await employeesRepository.GetById(employeeId);
            if (employee is null)
                return Result.Failure($"Employee {employeeId} doesn't exist");
        }

        foreach (var employeeId in employeeIdsToAdd)
        {
            var result = project.AssignEmployee(employeeId);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        foreach (var employeeId in employeeIdsToRemove)
        {
            var result = project.RemoveEmployee(employeeId);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        await projectsRepository.Update(project);
        return Result.Success();
    }

    public async Task<Result> UpdateProjectLeader(Guid projectId, Guid leaderId)
    {
        var project = await projectsRepository.GetById(projectId);
        if (project is null)
            return Result.Failure($"Project {projectId} not found");

        var employee = await employeesRepository.GetById(leaderId);
        if (employee is null)
            return Result.Failure($"Employee {leaderId} doesn't exist");

        if (!project.EmployeeIds.Contains(leaderId))
        {
            var assignResult = project.AssignEmployee(leaderId);
            if (assignResult.IsFailure)
                return Result.Failure(assignResult.Error);
        }

        var result = project.AssignLeader(leaderId);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await projectsRepository.Update(project);
        return Result.Success();
    }

    public async Task<Result> UpdateProject(Guid projectId, PatchProjectRequest request)
    {
        var project = await projectsRepository.GetById(projectId);
        if (project == null)
            return Result.Failure($"Project {projectId} not found");

        if (request.Title != null || request.CustomerCompanyName != null ||
            request.ContractorCompanyName != null || request.Priority.HasValue)
        {
            var title = request.Title ?? project.Title;
            var customer = request.CustomerCompanyName ?? project.CustomerCompanyName;
            var contractor = request.ContractorCompanyName ?? project.ContractorCompanyName;
            var priority = request.Priority ?? project.Priority;

            var result = project.UpdateBasicInfo(title, customer, contractor, priority);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? project.StartDate;
            var endDate = request.EndDate ?? project.EndDate;

            var result = project.UpdateDates(startDate, endDate);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        if (request.RemoveLeader == true)
            project.LeaderId = null;
        else if (request.LeaderId.HasValue)
        {
            var result = project.AssignLeader(request.LeaderId.Value);
            if (result.IsFailure)
                return Result.Failure(result.Error);
        }

        await projectsRepository.Update(project);
        return Result.Success();
    }
}