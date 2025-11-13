using System.ComponentModel.DataAnnotations;

namespace ProjectsManager.Core.Contracts;

public record CreateProjectRequest(
    [Required] string Title,
    DateOnly StartDate,
    [DataFuture(ErrorMessage = "EndDate cannot be in the Past")]
    DateOnly? EndDate = null,
    int Priority = 0,
    string CustomerCompanyName = "Unknown",
    string ContractorCompanyName = "Unknown",
    Guid? LeaderId = null);

public class DataFutureAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is DateOnly date)
            return date >= DateOnly.FromDateTime(DateTime.Now);
        return false;
    }
}