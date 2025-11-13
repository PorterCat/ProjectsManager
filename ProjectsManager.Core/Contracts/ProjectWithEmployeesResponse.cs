using ProjectsManager.Core.Models;

namespace ProjectsManager.Core.Contracts;

public record ProjectWithEmployeesResponse(
    Guid Id,
    string Title,
    IEnumerable<(Guid Id, string Name)> Employees);