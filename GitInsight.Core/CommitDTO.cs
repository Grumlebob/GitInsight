namespace GitInsight.Core;

public record CommitDto(int Id, string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);

public record CommitCreateDto(string Sha, DateTimeOffset Date, int AuthorId, int BranchId, int RepositoryId);