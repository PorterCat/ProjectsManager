using System.ComponentModel.DataAnnotations;

namespace ProjectsManager.Core.Contracts;

public record PageQuery(
    int PageNum = 1,
    [Range(0, byte.MaxValue)] int PageSize = 20);