using System.ComponentModel.DataAnnotations;

namespace ProjectsManager.Core.Contracts;

public record CreateEmployeeRequest(
    [Required] string Firstname,
    [Required] string Lastname,
    string? Patronymic,
    [EmailAddress] string Email);