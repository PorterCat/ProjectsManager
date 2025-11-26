using System.ComponentModel.DataAnnotations;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Contracts;

public record ProjectWithEmployees(
    Project Project, 
    IEnumerable<Employee> Employees);

public record ProjectResponse(
    Guid Id,
    string Title,
    DateOnly StartDate,
    DateOnly? EndDate,
    int Priority,
    string CustomerCompanyName,
    string ContractorCompanyName,
    Guid? LeaderId);

public record CreateProjectRequest(
    [Required] string Title,
    DateOnly StartDate,
    DateOnly? EndDate = null,
    int Priority = 0,
    string CustomerCompanyName = "Unknown",
    string ContractorCompanyName = "Unknown",
    Guid? LeaderId = null,
    ICollection<Guid>? EmployeeIds = null);

public record UpdateProjectRequest(
    string? Title, string? CustomerCompanyName,
    string? ContractorCompanyName, int? Priority,
    DateOnly? StartDate, DateOnly? EndDate,
    bool? RemoveLeader, Guid? LeaderId);
    
public record AssignLeaderRequest(
    Guid? LeaderId = null);