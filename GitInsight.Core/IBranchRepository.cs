namespace GitInsight.Core;

public interface IBranchRepository
{
    public Task<(Response, BranchDto?)> CreateAsync(BranchCreateDto newBranch);
    public Task<BranchDto> FindAsync(int id);
    public Task<IReadOnlyCollection<BranchDto>> FindAllAsync(int repositoryId);
    public Task<IReadOnlyCollection<BranchDto>> FindAllAsync();
    public Task<Response> DeleteAsync(int id);
    public Task<Response> UpdateAsync(BranchDto b);
}