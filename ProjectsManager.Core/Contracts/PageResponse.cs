namespace ProjectsManager.Core.Contracts;

public record PageResponse<T>(
    IEnumerable<T> Items,
    int Total,
    int TotalPages);