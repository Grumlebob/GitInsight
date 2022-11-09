namespace GitInsight.Core;

public record RepoInsightCreateDto(string Path, string Name, IEnumerable<int> BranchIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);
public record RepoInsightDto(int Id, string Path, string Name, IEnumerable<int> BranchIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);
public record RepoInsightLatestCommitUpdate(int Id, int LatestCommitId);
