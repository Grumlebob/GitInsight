using System.ComponentModel.DataAnnotations;

namespace GitInsight.Core;

public record RepositoryDto(int Id, string Path, string Name, IEnumerable<int> BrancheIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);

public record RepositoryCreateDto(string Path, string Name, IEnumerable<int> BranchIds, IEnumerable<int> CommitIds, IEnumerable<int> AuthorIds);
