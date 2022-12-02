namespace GitInsight.Core;

public record CommitInsightDto(int Id, [StringLength(int.MaxValue, MinimumLength = 1)]string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);

public record CommitInsightCreateDto([StringLength(int.MaxValue, MinimumLength = 1)]string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);