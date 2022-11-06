namespace GitInsight.Core;

public interface IRepositoryRepository
{
    public Task<(RepositoryDto repo, Response response)> CreateRepositoryAsync(RepositoryCreateDto repositoryCreateDto);
    public Task<Response> DeleteRepositoryAsync(int id);
    public Task<(List<RepositoryDto>?, Response)> FindAllRepositoriesAsync();
    public Task<(RepositoryDto?, Response)> FindRepositoryAsync(int id);
    public Task<Response> UpdateRepositoryAsync(RepositoryDto repositoryDto);
}
