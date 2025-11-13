namespace ProjectsManager.Core.Contracts;

public record PatchProjectRequest(
    string? Title, string? CustomerCompanyName,
    string? ContractorCompanyName, int? Priority,
    DateOnly? StartDate, DateOnly? EndDate,
    bool? RemoveLeader, Guid? LeaderId);