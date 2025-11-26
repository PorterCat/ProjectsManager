namespace ProjectsManager.DataAccess.Entities;

public class EmployeeEntity
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Patronymic { get; init; }
    public string Email { get; init; } = string.Empty;

    public List<ProjectEntity> Projects { get; init; } = [];
    public List<ProjectEntity> LeadingProjects { get; init; } = [];
}