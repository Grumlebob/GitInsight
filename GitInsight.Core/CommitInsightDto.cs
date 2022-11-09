namespace GitInsight.Core;

public record CommitInsightDto(int Id, string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);

public record CommitInsightCreateDto(string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);