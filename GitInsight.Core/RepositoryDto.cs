namespace GitInsight.Core;

public record RepositoryCreateDto(string Path, string Name, IEnumerable<int> BranchIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);
public record RepositoryDto(int Id, string Path, string Name, IEnumerable<int> BranchIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);
public record RepositoryLatestCommitUpdate(int Id, int LatestCommitId);
