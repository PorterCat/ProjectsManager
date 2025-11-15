using System.ComponentModel.DataAnnotations;

namespace ProjectsManager.Core.Contracts;

public record EmployeeResponse(
    Guid Id,
    string Firstname,
    string Lastname,
    string? Patronymic,
    string Email);

public record CreateEmployeeRequest(
    [Required] string Firstname,
    [Required] string Lastname,
    string? Patronymic,
    [EmailAddress] string Email);

public record UpdateEmployeeRequest(
    string? Firstname,
    string? Lastname,
    string? Patronymic,
    string? Email);