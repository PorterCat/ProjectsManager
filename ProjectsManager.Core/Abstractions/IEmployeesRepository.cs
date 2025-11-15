using CSharpFunctionalExtensions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IEmployeesRepository
{
    Task<Employee?> GetById(Guid id);
    Task<ICollection<Employee>> GetByFilter(string? searchText = null);
    Task<ICollection<Employee>> GetAll();
    Task<Result> Add(Employee employee);
    Task Update(Employee employee);
    Task<bool> Delete(Guid id);
}
