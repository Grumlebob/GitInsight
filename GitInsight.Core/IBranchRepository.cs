namespace GitInsight.Core;

public interface IBranchRepository
{
    public (Response, BranchDto) Create(BranchCreateDto newBranch);
    public BranchDto Find(int id);
    public IReadOnlyCollection<BranchDto> FindAll(int repositoryId);
    public IReadOnlyCollection<BranchDto> FindAll();
    public Response Delete(int id);
    public Response Update(BranchDto b);
}