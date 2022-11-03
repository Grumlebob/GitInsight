namespace GitInsight.Core;

public record AuthorDto(int Id, string Name, string Email, List<int> CommitIds, List<int> RepositoryIds);

public record AuthorCreateDto(string Name, string Email, List<int> CommitIds, List<int> RepositoryIds);
