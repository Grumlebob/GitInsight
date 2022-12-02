namespace GitInsight.Core;

public record BranchDto(int Id, [StringLength(50, MinimumLength = 1)] string? Name,
    int RepositoryId,
    [StringLength(int.MaxValue, MinimumLength = 1)] string Path);

public record BranchCreateDto([StringLength(50, MinimumLength = 1)] string? Name,
    int RepositoryId,
    [StringLength(int.MaxValue, MinimumLength = 1)] string Path);