using System.ComponentModel.DataAnnotations;

namespace ProjectsManager.Core.Contracts;

public record CreateEmployeeRequest(
    [Required] string FirstName,
    [Required] string LastName,
    string? Patronymic,
    [EmailAddress] string Email);

public record UpdateEmployeeRequest(
    string? Firstname,
    string? Lastname,
    string? Patronymic);