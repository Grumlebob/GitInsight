namespace GitInsight.Core;

public record AuthorDto(int Id, string Name, [StringLength(int.MaxValue, MinimumLength = 1)]string Email, List<int> RepositoryIds);

public record AuthorCreateDto(string Name, [StringLength(int.MaxValue, MinimumLength = 1)] string Email, List<int> RepositoryIds);

public record AuthorUpdateDto(int Id, string Name, [StringLength(int.MaxValue, MinimumLength = 1)] string Email, List<int> RepositoryIds);