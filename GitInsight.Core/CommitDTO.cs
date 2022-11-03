using System.Data.Common;

namespace GitInsight.Core;

public record CommitDTO(int Id, string Sha, DateTimeOffset Date, string? Tag, int AuthorId, int BranchId, int RepositoryId);

public record CommitCreateDTO(string Sha, DateTimeOffset Date, string? Tag, int AuthorId, int BranchId, int RepositoryId);