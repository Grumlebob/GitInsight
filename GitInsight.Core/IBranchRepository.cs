namespace GitInsight.Core;

public interface IBranchRepository
{
    public Task<(BranchDto?, Response)> CreateAsync(BranchCreateDto newBranch);
    public Task<(BranchDto?, Response)> FindAsync(int id);
    public Task<(List<BranchDto>?, Response)> FindAllAsync(int repositoryId);
    public Task<(List<BranchDto>?, Response)> FindAllAsync();
    public Task<Response> DeleteAsync(int id);
    public Task<Response> UpdateAsync(BranchDto b);
}