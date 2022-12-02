namespace GitInsight.Core;

public interface ICommitInsightRepository
{
    Task<(CommitInsightDto? commit, Response response)> FindAsync(int id);
    Task<(IReadOnlyCollection<CommitInsightDto> commits, Response response)> FindAllAsync();
    Task<(Response response, CommitInsightDto? commit)> CreateAsync(CommitInsightCreateDto commitCreateDto);
    Task<(Response response, CommitInsightDto? commit)> UpdateAsync(CommitInsightDto commit);
    Task<Response> DeleteAsync(int id);
    Task<(CommitInsightDto? commit, Response response)> FindByShaAsync(string sha);
    Task<(List<CommitInsightDto?> commit, Response response)> FindByRepoIdAsync(int id);
}
