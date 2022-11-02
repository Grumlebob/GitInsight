namespace GitInsight.Core;

public interface IBranchRepository
{
    public (Response, BranchDto) Create(BranchCreateDto newBranch);
    public BranchDto Find(int id);
    public IEnumerable<BranchDto> FindAll(int repositoryId);
    public Response Delete(int id);
    public Response Update(BranchDto b);
}