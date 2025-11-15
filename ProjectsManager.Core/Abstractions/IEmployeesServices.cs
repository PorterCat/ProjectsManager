using CSharpFunctionalExtensions;
using ProjectsManager.Core.Contracts;

namespace ProjectsManager.Core.Abstractions;

public interface IEmployeesServices
{
    Task<Result> UpdateEmployee(Guid id, UpdateEmployeeRequest request);
}