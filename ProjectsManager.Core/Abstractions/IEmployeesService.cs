using CSharpFunctionalExtensions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Abstractions;

public interface IEmployeesService
{
    Task<Employee?> GetEmployeeById(Guid id);
    Task<Employee?> GetEmployeeByEmail(string email);
    Task<ICollection<Employee>> GetAllEmployees(string? searchText);
    Task<Result> CreateEmployee(Employee employee);
    Task<Result<PatchResponse<Employee>>> UpdateEmployee(Employee employee, UpdateEmployeeRequest request);
    Task<Result> DeleteEmployee(Employee employee);
}