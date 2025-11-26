using CSharpFunctionalExtensions;

namespace ProjectsManager.Core.Models;

public record Project
{
    public Guid Id { get; }
    public string Title { get; init; }
    public string CustomerCompanyName { get; init; }
    public string ContractorCompanyName { get; init; }
    public int Priority { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    private Project(Guid id, string title, string customerCompanyName, 
        string contractorCompanyName, int priority, DateOnly startDate, DateOnly? endDate)
    {
        Id = id;
        Title = title;
        CustomerCompanyName = customerCompanyName;
        ContractorCompanyName = contractorCompanyName;
        Priority = priority;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<Project> Create(
        Guid id,
        string title,
        string customerCompanyName,
        string contractorCompanyName,
        int priority,
        DateOnly startDate,
        DateOnly? endDate = null)
    {
        var validationResult = Validate(title,  customerCompanyName, contractorCompanyName, startDate, endDate);
        return validationResult.IsFailure 
            ? Result.Failure<Project>(validationResult.Error) 
            : new Project(id, title,  customerCompanyName, contractorCompanyName, priority, startDate, endDate);
    }
    
    public static Result Validate(string title, 
        string customerCompanyName, string contractorCompanyName,
        DateOnly startDate, DateOnly? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure($"{nameof(title)} cannot be empty");
            
        if (string.IsNullOrWhiteSpace(customerCompanyName))
            return Result.Failure($"{nameof(customerCompanyName)} cannot be empty");

        if (string.IsNullOrWhiteSpace(contractorCompanyName))
            return Result.Failure($"{nameof(contractorCompanyName)} cannot be empty");
        
        if (endDate.HasValue && startDate > endDate.Value)
            return Result.Failure<Project>($"{nameof(startDate)} cannot be before {nameof(endDate)}");

        return Result.Success();
    }
    
    public static Result Validate(Project project) =>
        Validate(project.Title, project.CustomerCompanyName, 
            project.ContractorCompanyName, project.StartDate, project.EndDate);
}