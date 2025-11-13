using CSharpFunctionalExtensions;

namespace ProjectsManager.Core.Models;

public record Employee
{
    public Guid Id { get; }
    public string Firstname { get; }
    public string Lastname { get; }
    public string? Patronymic { get; }
    public string Email { get; }

    private readonly HashSet<Guid> _projects = [];
    public IReadOnlyCollection<Guid> Projects => _projects;

    private Employee(Guid id, string firstname, string lastname,
        string? patronymic, string email, IEnumerable<Guid>? projects)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        Patronymic = patronymic;
        Email = email;

        if (projects != null)
            _projects = projects as HashSet<Guid> ?? [];
    }

    public static Result<Employee> Create(Guid id,
        string firstname, string lastname, string email, IEnumerable<Guid>? projects = null,
        string? patronymic = null)
    {
        if (string.IsNullOrEmpty(firstname))
            return Result.Failure<Employee>("Firstname cannot be empty");
        if (string.IsNullOrEmpty(lastname))
            return Result.Failure<Employee>("Lastname cannot be empty");
        if (string.IsNullOrEmpty(email))
            return Result.Failure<Employee>("Email cannot be empty");

        var employee = new Employee(id, firstname, lastname, patronymic, email, projects);
        return Result.Success(employee);
    }
}