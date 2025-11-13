using CSharpFunctionalExtensions;
using ProjectsManager.Core.Contracts;

namespace ProjectsManager.Core.Models;

public record Project
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string CustomerCompanyName { get; private set; }
    public string ContractorCompanyName { get; private set; }
    public int Priority { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public Guid? LeaderId { get; private set; }

    public IReadOnlyCollection<Guid> Employees => _employees;

    [IgnorePatch]
    public IReadOnlyCollection<Guid> EmployeesToAdd => _employeesToAdd.AsReadOnly();

    [IgnorePatch]
    public IReadOnlyCollection<Guid> EmployeesToRemove => _employeesToRemove.AsReadOnly();

    private readonly HashSet<Guid> _employees = [];
    private readonly List<Guid> _employeesToAdd = [];
    private readonly List<Guid> _employeesToRemove = [];

    private Project(Guid id, string title, string customerCompanyName,
        string contractorCompanyName, int priority, DateOnly startDate,
        DateOnly? endDate = null, Guid? leaderId = null,
        IEnumerable<Guid>? employees = null)
    {
        Id = id;
        Title = title;
        CustomerCompanyName = customerCompanyName;
        ContractorCompanyName = contractorCompanyName;
        Priority = priority;
        StartDate = startDate;
        EndDate = endDate;
        LeaderId = leaderId;

        if (employees != null)
            _employees = employees.ToHashSet();
    }

    public static Result<Project> Create(string title, string customerCompanyName,
        string contractorCompanyName, int priority, DateOnly startDate,
        DateOnly? endDate = null, Guid? leaderId = null,
        IEnumerable<Guid>? employeeIds = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Project>("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(customerCompanyName))
            return Result.Failure<Project>("CustomerCompanyName cannot be empty");
        if (string.IsNullOrWhiteSpace(contractorCompanyName))
            return Result.Failure<Project>("ContractorCompanyName cannot be empty");
        if (endDate.HasValue && startDate > endDate.Value)
            return Result.Failure<Project>("StartDate must be before EndDate");

        return Result.Success(new Project(Guid.NewGuid(), title.Trim(),
            customerCompanyName.Trim(), contractorCompanyName.Trim(),
            priority, startDate, endDate, leaderId, employeeIds));
    }

    public static Project Reconstruct(Guid id, string title, string customerCompanyName,
        string contractorCompanyName, int priority, DateOnly startDate,
        DateOnly? endDate, Guid? leaderId, IEnumerable<Guid> employeeIds)
    {
        return new Project(id, title, customerCompanyName, contractorCompanyName,
            priority, startDate, endDate, leaderId, employeeIds);
    }

    public Result AssignEmployee(Guid employeeId)
    {
        if (!_employees.Add(employeeId))
            return Result.Failure($"Employee {employeeId} is already assigned to project");

        _employees.Add(employeeId);
        _employeesToAdd.Add(employeeId);
        _employeesToRemove.Remove(employeeId);

        return Result.Success();
    }

    public Result RemoveEmployee(Guid employeeId)
    {
        if (!_employees.Contains(employeeId))
            return Result.Failure($"Employee {employeeId} is not assigned to project");

        _employees.Remove(employeeId);
        _employeesToRemove.Add(employeeId);
        _employeesToAdd.Remove(employeeId);

        if (LeaderId == employeeId)
            LeaderId = null;

        return Result.Success();
    }

    public Result AssignLeader(Guid leaderId)
    {
        if (LeaderId == leaderId)
            return Result.Failure($"Employee {leaderId} is already the project leader");

        AssignEmployee(leaderId);

        LeaderId = leaderId;
        return Result.Success();
    }

    public void ClearEmployeeBuffers()
    {
        _employeesToAdd.Clear();
        _employeesToRemove.Clear();
    }

    public bool HasEmployeeChanges => _employeesToAdd.Count > 0 || _employeesToRemove.Count > 0;

    public Result ApplyPatch(PatchProjectRequest req)
    {
        if (req.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Result.Failure("Title cannot be empty");
            Title = req.Title.Trim();
        }

        if (req.CustomerCompanyName is not null)
        {
            if (string.IsNullOrWhiteSpace(req.CustomerCompanyName))
                return Result.Failure("CustomerCompanyName cannot be empty");
            CustomerCompanyName = req.CustomerCompanyName.Trim();
        }

        if (req.ContractorCompanyName is not null)
        {
            if (string.IsNullOrWhiteSpace(req.ContractorCompanyName))
                return Result.Failure("ContractorCompanyName cannot be empty");
            ContractorCompanyName = req.ContractorCompanyName.Trim();
        }

        if (req.Priority is not null) Priority = req.Priority.Value;

        if (req.StartDate is not null)
        {
            if (req.EndDate.HasValue && req.StartDate.Value > req.EndDate.Value)
                return Result.Failure("StartDate must be before EndDate");
            StartDate = req.StartDate.Value;
        }

        if (req.EndDate is not null)
        {
            if (req.EndDate.Value < StartDate)
                return Result.Failure("EndDate must be after StartDate");
            EndDate = req.EndDate;
        }

        if (req.RemoveLeader == true)
            LeaderId = null;
        else if (req.LeaderId is not null)
            return AssignLeader(req.LeaderId.Value);

        return Result.Success();
    }
}