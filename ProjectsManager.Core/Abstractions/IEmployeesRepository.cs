using CSharpFunctionalExtensions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IEmployeesRepository
{
    Task<Employee?> GetById(Guid id);
    Task<Employee?> GetByEmail(string email);
    Task<ICollection<Employee>> GetAll(string? searchText = null);
    Task Add(Employee employee);
    Task Update(Employee employee);
    Task Delete(Guid id);
    Task<ICollection<Project>> GetProjectsByEmployee(Guid employeeId);
    Task<ICollection<Project>> GetLeadingProjectsByEmployee(Guid employeeId);
}
