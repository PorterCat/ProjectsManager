using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class ProjectsService(
    IProjectsRepository projectsRepository) : IProjectsService
{
    public async Task<Project?> GetProjectById(Guid id) =>
        await projectsRepository.GetById(id);

    public async Task<ICollection<Project>> GetAllProjects(PageQuery? pageQuery, ProjectFilterQuery? query) =>
        await projectsRepository.GetAll(pageQuery, query);

    public async Task<int> GetCount() => 
        await projectsRepository.GetCount();

    public async Task<Result> CreateProject(Project project)
    {
        if(project.StartDate == DateOnly.MinValue)
            project = project with {StartDate = DateOnly.FromDateTime(DateTime.Now)};
        
        await projectsRepository.Add(project);
        return Result.Success();
    }

    public async Task<Result<PatchResponse<Project>>> UpdateProject(Project project, UpdateProjectRequest request)
    {
        var updatedProject = project with 
        {
            Title = request.Title ?? project.Title,
            ContractorCompanyName = request.ContractorCompanyName ?? project.ContractorCompanyName,
            CustomerCompanyName = request.CustomerCompanyName ?? project.CustomerCompanyName,
            Priority = request.Priority ?? project.Priority,
            StartDate = request.StartDate ?? project.StartDate,
            EndDate = request.EndDate ?? project.EndDate
        };
        
        var validationResult = Project.Validate(updatedProject);
        if (validationResult.IsFailure)
            return Result.Failure<PatchResponse<Project>>(validationResult.Error);
        
        await projectsRepository.Update(updatedProject);
        return Result.Success(project.CreatePatchResponse(updatedProject, updatedProject.Id));
    }

    public async Task<Result> DeleteProject(Project project)
    {
        await projectsRepository.Delete(project.Id);
        return Result.Success();
    }
}