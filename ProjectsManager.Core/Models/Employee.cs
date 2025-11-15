using CSharpFunctionalExtensions;

namespace ProjectsManager.Core.Models;

public record Employee
{
    public Guid Id { get; }
    public string Firstname { get; }
    public string Lastname { get; }
    public string? Patronymic { get; }
    public string Email { get; }
    public ICollection<Guid> ProjectIds { get; init; } = [];

    private Employee(Guid id, string firstname, string lastname,
        string? patronymic, string email, IEnumerable<Guid>? projectIds)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        Patronymic = patronymic;
        Email = email;

        if (projectIds != null)
            ProjectIds = projectIds.ToHashSet();
    }

    public static Result<Employee> Create(Guid id,
        string firstname, string lastname, string email,
        string? patronymic = null)
    {
        if (string.IsNullOrEmpty(firstname))
            return Result.Failure<Employee>("Firstname cannot be empty");
        if (string.IsNullOrEmpty(lastname))
            return Result.Failure<Employee>("Lastname cannot be empty");
        if (string.IsNullOrEmpty(email))
            return Result.Failure<Employee>("Email cannot be empty");

        var employee = new Employee(id, firstname, lastname, patronymic, email, null);
        return Result.Success(employee);
    }

    public static Employee Reconstruct(Guid id, string firstname, string lastname,
        string? patronymic, string email, IEnumerable<Guid> projectIds)
    {
        return new Employee(id, firstname, lastname, patronymic, email, projectIds);
    }

    public void AddProject(Guid projectId) => ProjectIds.Add(projectId);
    public void RemoveProject(Guid projectId) => ProjectIds.Remove(projectId);
}