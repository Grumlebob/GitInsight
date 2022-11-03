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
                       where newBranch.Sha == b.Sha || (newBranch.RepositoryId == b.RepositoryId && newBranch.Path == b.Path)
                       select new BranchDto(b.Id, b.Name, b.Sha, b.RepositoryId, b.Path);

        var repo = _context.Repositories.FirstOrDefault(r => r.Id == newBranch.RepositoryId);

        if (conflict.Any())
        {
            var match = conflict.FirstOrDefault()!;
            return (Response.Conflict, new BranchDto(match.Id, match.Name, match.Sha, match.RepositoryId, match.Path));
        }

        if (repo is null)
        {
            return (Response.BadRequest,
                new BranchDto(-1, newBranch.Name, newBranch.Sha, newBranch.RepositoryId,
                    "No repository found with id: " + newBranch.RepositoryId));
        }

        var created = new Branch
        {
            Name = newBranch.Name,
            Sha = newBranch.Sha,
            RepositoryId = newBranch.RepositoryId,
            Path = newBranch.Path
        };
        _context.Branches.Add(created);
        _context.SaveChanges();

        return (Response.Created,
            new BranchDto(created.Id, created.Name, created.Sha, created.RepositoryId, created.Path));
    }

    public BranchDto Find(int id)
    {
        var result = from b in _context.Branches
                     where b.Id == id
                     select new BranchDto(b.Id, b.Name, b.Sha, b.RepositoryId, b.Path);
        return result.FirstOrDefault()!;
    }

    /// <summary>
    /// Find all branches in the repository with Id = repositoryId.
    /// </summary>
    /// <param name="repositoryId"></param>
    public IReadOnlyCollection<BranchDto> FindAll(int repositoryId)
    {
        var result = from b in _context.Branches
                     where b.RepositoryId == repositoryId
                     select new BranchDto(b.Id, b.Name, b.Sha, b.RepositoryId, b.Path);
        return result.ToList().AsReadOnly();
    }

    /// <returns>All branches in database.</returns>
    public IReadOnlyCollection<BranchDto> FindAll()
    {
        var result = from b in _context.Branches
                     select new BranchDto(b.Id, b.Name, b.Sha, b.RepositoryId, b.Path);
        return result.ToList().AsReadOnly();
    }

    public Response Delete(int id)
    {
        var result = _context.Branches.FirstOrDefault(b => b.Id == id);
        if (result is null)
        {
            return Response.NotFound;
        }

        _context.Branches.Remove(result);
        _context.SaveChanges();
        return Response.Deleted;
    }

    public Response Update(BranchDto updatedBranch)
    {
        var found = _context.Branches.FirstOrDefault(b => b.Id == updatedBranch.Id);
        if (found is null) return Response.NotFound;

        var conflict = _context.Branches.FirstOrDefault(b =>
            b.Id != updatedBranch.Id && updatedBranch.Sha == b.Sha ||
            (updatedBranch.RepositoryId == b.RepositoryId && updatedBranch.Path == b.Path));
        var repo = _context.Repositories.FirstOrDefault(r => r.Id == updatedBranch.RepositoryId);
        if (conflict is not null) return Response.Conflict;
        if (repo is null) return Response.BadRequest;

        found.Name = updatedBranch.Name;
        found.Sha = updatedBranch.Sha;
        found.RepositoryId = updatedBranch.RepositoryId;
        found.Path = updatedBranch.Path;
        _context.SaveChanges();
        return Response.Ok;
    }
}