using CSharpFunctionalExtensions;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business;

public class EmployeesService(IEmployeesRepository employeesRepository) : IEmployeesServices
{
    public async Task<Result> UpdateEmployee(Guid id, UpdateEmployeeRequest request)
    {
        var employee = await employeesRepository.GetById(id);
        if (employee == null)
            return Result.Failure($"Employee {id} not found");

        var updatedEmployee = Employee.Reconstruct(
            employee.Id,
            request.Firstname ?? employee.Firstname,
            request.Lastname ?? employee.Lastname,
            request.Patronymic ?? employee.Patronymic,
            request.Email ?? employee.Email,
            employee.ProjectIds);

        await employeesRepository.Update(updatedEmployee);
        return Result.Success();
    }
}