namespace ProjectsManager.DataAccess.Entities;

public class EmployeeEntity
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? Patronymic { get; init; }
    public string Email { get; init; }

    public List<ProjectEntity> Projects { get; init; }
    public List<ProjectEntity> LeadingProjects { get; init; }
}