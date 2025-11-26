using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IProjectsRepository
{
    Task<Project?> GetById(Guid id);
    Task<ICollection<Project>> GetAll(PageQuery? page = null, ProjectFilterQuery? projectQuery = null);
    Task<int> GetCount();
    Task Add(Project project);
    Task Update(Project project);
    Task Delete(Guid id);
    
    Task<ICollection<Employee>> GetEmployeesByProject(Guid projectId);
    Task<int> UpdateProjectEmployees(Guid projectId, IEnumerable<Guid> employeeIds);
    Task UpdateProjectLeader(Guid projectId, Guid? leaderId);
}