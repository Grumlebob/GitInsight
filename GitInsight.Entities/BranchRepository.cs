using Npgsql.Replication.PgOutput.Messages;

namespace GitInsight.Entities;

public class BranchRepository : IBranchRepository
{
    private readonly InsightContext _context;

    public BranchRepository(InsightContext context)
    {
        _context = context;
    }

    public (Response, BranchDto) Create(BranchCreateDto newBranch)
    {
        var conflict = from b in _context.Branches
            where b.Sha == newBranch.Sha ||
                  (b.RepositoryId == newBranch.RepositoryId && b.Path == newBranch.Path)
            select new BranchDto(b.Id, b.Name, b.Sha, b.RepositoryId, b.Path);

        var repo = _context.Repositories.FirstOrDefault(r => r.Id == newBranch.RepositoryId);

        if (conflict.Any())
        {
            var match = conflict.FirstOrDefault()!;
            return (Response.Conflict, new BranchDto(match.Id, match.Name, match.Sha, match.RepositoryId, match.Path));
        }

        var created = new Branch(newBranch);
        _context.Branches.Add(created);
        _context.SaveChanges();

        return (Response.Created,
            new BranchDto(created.Id, created.Name, created.Sha, created.RepositoryId, created.Path));
    }

    public BranchDto Find(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BranchDto> FindAll(int repositoryId)
    {
        throw new NotImplementedException();
    }

    public Response Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Response Update(BranchDto b)
    {
        throw new NotImplementedException();
    }
}