using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class EmployeesService(IEmployeesRepository employeesRepository) : IEmployeesService
{
    public async Task<Employee?> GetEmployeeById(Guid id) => 
        await employeesRepository.GetById(id);

    public async Task<Employee?> GetEmployeeByEmail(string email) =>
        await employeesRepository.GetByEmail(email);

    public async Task<ICollection<Employee>> GetAllEmployees(string? searchText) => 
        await employeesRepository.GetAll(searchText);

    public async Task<Result> CreateEmployee(Employee employee)
    {
        if(await employeesRepository.GetByEmail(employee.Email) is not null)
            return Result.Failure($"Employee with email [{employee.Email}] already exists.");
        
        await employeesRepository.Add(employee);
        return Result.Success();
    }

    public async Task<Result<PatchResponse<Employee>>> UpdateEmployee(Employee employee, UpdateEmployeeRequest request)
    {
        if(employee.Email == "admin@admin.com")
            return Result.Failure<PatchResponse<Employee>>("You cannot edit admin account.");
        
        var updatedEmployee = employee with 
        {
            FirstName = request.Firstname ?? employee.FirstName,
            LastName = request.Lastname ?? employee.LastName,
            Patronymic = request.Patronymic ?? employee.Patronymic
        };
        
        var validationResult = Employee.Validate(updatedEmployee);
        if (validationResult.IsFailure)
            return Result.Failure<PatchResponse<Employee>>(validationResult.Error);
        
        await employeesRepository.Update(updatedEmployee);
        return Result.Success(employee.CreatePatchResponse(updatedEmployee, updatedEmployee.Id));
    }

    public async Task<Result> DeleteEmployee(Employee employee)
    {
        if(employee.Email == "admin@admin.com")
            return Result.Failure("You cannot delete admin account.");
        
        await employeesRepository.Delete(employee.Id);
        return Result.Success();
    }
}