namespace GitInsight.Core;

public interface ICommitRepository
{
    Task<(CommitDTO? commit, Response response)> FindAsync(int id);
    Task<(IReadOnlyCollection<CommitDTO> commits, Response response)> FindAllAsync();
    Task<(Response response, CommitDTO? commit)> CreateAsync(CommitCreateDTO DTO);
    Task<(Response response, CommitDTO? commit)> UpdateAsync(CommitDTO commit);
    Task<Response> DeleteAsync(int id);
}
