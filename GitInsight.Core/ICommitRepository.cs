namespace GitInsight.Core;

public interface ICommitRepository
{
    Task<(CommitDto? commit, Response response)> FindAsync(int id);
    Task<(IReadOnlyCollection<CommitDto> commits, Response response)> FindAllAsync();
    Task<(Response response, CommitDto? commit)> CreateAsync(CommitCreateDto commitCreateDto);
    Task<(Response response, CommitDto? commit)> UpdateAsync(CommitDto commit);
    Task<Response> DeleteAsync(int id);
}
