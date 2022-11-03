namespace GitInsight.Core;

public interface IAuthorRepository
{
    Task<(AuthorDto?, Response)> FindAuthorAsync(int id);
    Task<(List<AuthorDto>?, Response response)> FindAllAuthorsAsync();
    Task<(AuthorDto?, Response)> CreateAuthorAsync(AuthorCreateDto authorCreateDto);
    Task<Response> DeleteAuthorAsync(int id);
    Task<Response> UpdateAuthorAsync(AuthorDto authorDto);
    Task<(List<AuthorDto>?, Response)> FindAuthorsByNameAsync(string name);
    Task<(List<AuthorDto>?, Response)> FindAuthorsByEmailAsync(string email);
    Task<(List<AuthorDto>?, Response)> FindAuthorsByRepositoryIdAsync(int repositoryId);
    Task<(List<AuthorDto>?, Response)> FindAuthorsByCommitIdAsync(int commitId);
}