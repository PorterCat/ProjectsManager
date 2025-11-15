namespace ProjectsManager.Core.Contracts;

public record ProjectFilterQuery(
    DateOnly? StartDateFrom,
    DateOnly? StartDateTo,
    int? PriorityFrom,
    int? PriorityTo,
    string? SearchText,
    string? SortBy, // enum with json serialization?
    bool SortDescending = false
);