namespace GitInsight.Core;

public interface IAuthorRepository
{
    Task<(AuthorDto?, Response)> FindAsync(int id);
    Task<(List<AuthorDto>?, Response response)> FindAllAsync();
    Task<(AuthorDto?, Response)> CreateAsync(AuthorCreateDto authorCreateDto);
    Task<Response> DeleteAsync(int id);
    Task<Response> UpdateAsync(AuthorDto authorDto);
    Task<(List<AuthorDto>?, Response)> FindByNameAsync(string name);
    Task<(List<AuthorDto>?, Response)> FindByEmailAsync(string email);
    Task<(List<AuthorDto>?, Response)> FindByRepoIdAsync(int repositoryId);
    Task<(List<AuthorDto>?, Response)> FindByCommitIdAsync(int commitId);
}