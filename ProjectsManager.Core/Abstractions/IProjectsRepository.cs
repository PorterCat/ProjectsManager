using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsRepository
{
    Task<ICollection<Project>> GetAll();
    Task<Project?> GetById(Guid id);

    Task<int> GetCount();
    Task Add(Project project);
    Task<bool> Delete(Guid id);
    Task<bool> Save(Project project);
    Task<ICollection<Project>> GetByFilter(PageQuery? page = null, ProjectQuery? projectQuery = null);
}