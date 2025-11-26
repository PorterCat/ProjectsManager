using CSharpFunctionalExtensions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsService
{
    Task<Project?> GetProjectById(Guid id);
    Task<ICollection<Project>> GetAllProjects(PageQuery? pageQuery, ProjectFilterQuery? query);
    Task<int> GetCount();
    Task<Result> CreateProject(Project project);
    Task<Result<PatchResponse<Project>>> UpdateProject(Project project, UpdateProjectRequest request);
    Task<Result> DeleteProject(Project project);
}