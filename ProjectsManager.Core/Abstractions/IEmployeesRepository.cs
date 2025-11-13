using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IEmployeesRepository
{
    Task<ICollection<Employee>> GetAll();
    Task<Employee?> GetById(Guid id);
    Task<ICollection<Employee>> GetAllByText(string searchText);
    Task Add(Employee employee);
    Task<bool> Delete(Employee employee);
}