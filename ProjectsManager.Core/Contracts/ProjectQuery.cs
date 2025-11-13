namespace ProjectsManager.Core.Contracts;

public record ProjectQuery
{
    public DateOnly? StartDateFrom { get; init; }
    public DateOnly? StartDateTo { get; init; }

    public int? PriorityFrom { get; init; }
    public int? PriorityTo { get; init; }

    public string? SearchText { get; init; }

    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = false;
}