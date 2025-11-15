using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsRepository
{
    Task<Project?> GetById(Guid id);
    Task<ProjectWithEmployees?> GetWithEmployees(Guid id);
    Task<ICollection<Project>> GetByFilter(PageQuery? page = null, ProjectFilterQuery? projectQuery = null);
    Task<ICollection<Project>> GetAll();
    Task<int> GetCount();
    Task Add(Project project);
    Task Update(Project project);
    Task<bool> Delete(Guid id);
}