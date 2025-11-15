using System.ComponentModel.DataAnnotations;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Contracts;

public record ProjectResponse(
    Guid Id,
    string Title,
    DateOnly StartDate,
    int Priority,
    string CustomerCompanyName,
    string ContractorCompanyName,
    DateOnly? EndDate);

public record ProjectWithEmployeesResponse(
    Guid Id,
    string Title,
    Guid? LeaderId,
    IEnumerable<EmployeeResponse> Employees);

public record ProjectWithEmployees(
    Project Project, IEnumerable<Employee> Employees);

public record AssignLeaderRequest(Guid LeaderId);

public record AssignEmployeesRequest(
    IEnumerable<Guid> EmployeesToAdd,
    IEnumerable<Guid> EmployeesToRemove);

public record CreateProjectRequest(
    [Required] string Title,
    DateOnly StartDate,
    [DateFuture(ErrorMessage = "EndDate cannot be in the Past")]
    DateOnly? EndDate = null,
    int Priority = 0,
    string CustomerCompanyName = "Unknown",
    string ContractorCompanyName = "Unknown",
    Guid? LeaderId = null,
    IEnumerable<Guid>? EmployeeIds = null);

public record PatchProjectRequest(
    string? Title, string? CustomerCompanyName,
    string? ContractorCompanyName, int? Priority,
    DateOnly? StartDate, DateOnly? EndDate,
    bool? RemoveLeader, Guid? LeaderId);

public class DateFutureAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is DateOnly date)
            return date >= DateOnly.FromDateTime(DateTime.Now);
        return false;
    }
}