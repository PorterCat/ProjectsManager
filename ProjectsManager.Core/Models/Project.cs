using CSharpFunctionalExtensions;

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
    public Guid? LeaderId { get; set; }
    public ICollection<Guid> EmployeeIds { get; init; } = [];

    private Project(Guid id, string title, string customerCompanyName,
        string contractorCompanyName, int priority, DateOnly startDate,
        DateOnly? endDate = null, Guid? leaderId = null,
        IEnumerable<Guid>? employeeIds = null)
    {
        Id = id;
        Title = title;
        CustomerCompanyName = customerCompanyName;
        ContractorCompanyName = contractorCompanyName;
        Priority = priority;
        StartDate = startDate;
        EndDate = endDate;
        LeaderId = leaderId;

        if (employeeIds != null)
            EmployeeIds = employeeIds.ToHashSet();
    }

    public static Result<Project> Create(string title, string customerCompanyName,
        string contractorCompanyName, int priority, DateOnly startDate,
        DateOnly? endDate = null)
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
            priority, startDate, endDate));
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
        if (EmployeeIds.Contains(employeeId))
            return Result.Failure($"Employee {employeeId} is already assigned to project");
        EmployeeIds.Add(employeeId);
        return Result.Success();
    }

    public Result RemoveEmployee(Guid employeeId)
    {
        if (!EmployeeIds.Contains(employeeId))
            return Result.Failure($"Employee {employeeId} is not assigned to project");

        EmployeeIds.Remove(employeeId);

        if (LeaderId == employeeId)
            LeaderId = null;

        return Result.Success();
    }

    public Result AssignLeader(Guid leaderId)
    {
        if (LeaderId == leaderId)
            return Result.Failure($"Employee {leaderId} is already the project leader");

        if (!EmployeeIds.Contains(leaderId))
        {
            var assignResult = AssignEmployee(leaderId);
            if (assignResult.IsFailure)
                return Result.Failure(assignResult.Error);
        }

        LeaderId = leaderId;
        return Result.Success();
    }

    public Result UpdateBasicInfo(string title, string customerCompanyName,
        string contractorCompanyName, int priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(customerCompanyName))
            return Result.Failure("CustomerCompanyName cannot be empty");
        if (string.IsNullOrWhiteSpace(contractorCompanyName))
            return Result.Failure("ContractorCompanyName cannot be empty");

        Title = title.Trim();
        CustomerCompanyName = customerCompanyName.Trim();
        ContractorCompanyName = contractorCompanyName.Trim();
        Priority = priority;

        return Result.Success();
    }

    public Result UpdateDates(DateOnly startDate, DateOnly? endDate)
    {
        if (endDate.HasValue && startDate > endDate.Value)
            return Result.Failure("StartDate must be before EndDate");

        StartDate = startDate;
        EndDate = endDate;

        return Result.Success();
    }
}