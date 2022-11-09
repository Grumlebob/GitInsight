namespace GitInsight.Core;

public interface IRepoInsightRepository
{
    public Task<(RepositoryDto repo, Response response)> CreateAsync(RepositoryCreateDto repositoryCreateDto);
    public Task<Response> DeleteAsync(int id);
    public Task<(List<RepositoryDto>?, Response)> FindAllAsync();
    public Task<(RepositoryDto?, Response)> FindAsync(int id);
    public Task<Response> UpdateAsync(RepositoryDto repositoryDto);
}
