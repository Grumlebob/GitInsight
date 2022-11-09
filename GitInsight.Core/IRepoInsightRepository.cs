namespace GitInsight.Core;

public interface IRepoInsightRepository
{
    public Task<(RepoInsightDto repo, Response response)> CreateAsync(RepoInsightCreateDto repositoryCreateDto);
    public Task<Response> DeleteAsync(int id);
    public Task<(List<RepoInsightDto>?, Response)> FindAllAsync();
    public Task<(RepoInsightDto?, Response)> FindAsync(int id);
    public Task<Response> UpdateAsync(RepoInsightDto repositoryDto);
}
