namespace ProjectsManager.Core.Contracts;

public record AssignEmployeesRequest(
    IEnumerable<Guid> EmployeesToAdd,
    IEnumerable<Guid> EmployeesToRemove);