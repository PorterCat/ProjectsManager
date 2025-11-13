namespace ProjectsManager.Core.Contracts;

public record ProjectResponse(
    Guid Id,
    string Title,
    DateOnly StartDate,
    int Priority,
    string CustomerCompanyName,
    string ContractorCompanyName,
    DateOnly? EndDate);