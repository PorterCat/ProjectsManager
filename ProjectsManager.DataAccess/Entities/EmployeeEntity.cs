namespace ProjectsManager.DataAccess.Entities;

public class EmployeeEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string Email { get; set; } = string.Empty;

    public List<ProjectEntity> Projects { get; set; } = [];
    public List<ProjectEntity> LeadingProjects { get; set; } = [];
}