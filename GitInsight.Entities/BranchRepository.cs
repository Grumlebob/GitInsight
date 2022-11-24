namespace GitInsight.Entities;

public class BranchRepository : IBranchRepository
{
    private readonly InsightContext _context;

    public BranchRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(BranchDto?, Response)> CreateAsync(BranchCreateDto newBranch)
    {
        var conflict = from b in _context.Branches
                       where newBranch.RepositoryId == b.RepositoryId && newBranch.Path == b.Path
                       select new BranchDto(b.Id, b.Name, b.RepositoryId, b.Path);

        var repo = await _context.Repositories.FirstOrDefaultAsync(r => r.Id == newBranch.RepositoryId);

        if (await conflict.AnyAsync())
        {
            var match = await conflict.FirstAsync();
            return (new BranchDto(match!.Id, match.Name, match.RepositoryId, match.Path), Response.Conflict);
        }

        if (repo is null)
        {
            return (null, Response.BadRequest);
        }

        var created = new Branch
        {
            Name = newBranch.Name,
            RepositoryId = newBranch.RepositoryId,
            Path = newBranch.Path
        };
        await _context.Branches.AddAsync(created);
        await _context.SaveChangesAsync();

        return (
            new BranchDto(created.Id, created.Name, created.RepositoryId, created.Path), Response.Created);
    }

    public async Task<(BranchDto?, Response)> FindAsync(int id)
    {
        var result = from b in _context.Branches
                     where b.Id == id
                     select new BranchDto(b.Id, b.Name, b.RepositoryId, b.Path);

        if (!await result.AnyAsync()) return (null, Response.NotFound);
        var match = await result.FirstAsync();
        return (match!, Response.Ok);

    }

    /// <summary>
    /// Find all branches in the repository with Id = repositoryId.
    /// </summary>
    /// <param name="repositoryId"></param>
    public async Task<(List<BranchDto>?, Response)> FindAllAsync(int repositoryId)
    {
        var result = from b in _context.Branches
                     where b.RepositoryId == repositoryId
                     select new BranchDto(b.Id, b.Name, b.RepositoryId, b.Path);

        if (!await result.AnyAsync()) return (null, Response.NotFound);
        var match = await result.ToListAsync();
        return (match, Response.Ok);

    }

    /// <returns>All branches in database.</returns>
    public async Task<(List<BranchDto>?, Response)> FindAllAsync()
    {
        var result = from b in _context.Branches
                     select new BranchDto(b.Id, b.Name, b.RepositoryId, b.Path);

        if (await result.AnyAsync())
        {
            var match = await result.ToListAsync();
            return (match, Response.Ok);
        }

        return (null, Response.NotFound);
        // return await result.ToListAsync().ContinueWith(x => x.Result as IReadOnlyCollection<BranchDto>);
    }

    public async Task<Response> DeleteAsync(int id)
    {
        var result = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);
        if (result is null)
        {
            return Response.NotFound;
        }

        _context.Branches.Remove(result);
        await _context.SaveChangesAsync();
        return Response.Deleted;
    }

    public async Task<Response> UpdateAsync(BranchDto updatedBranch)
    {
        var found = await _context.Branches.FirstOrDefaultAsync(b => b.Id == updatedBranch.Id);
        if (found is null) return Response.NotFound;

        var conflict = await _context.Branches.FirstOrDefaultAsync(b =>
            b.Id != updatedBranch.Id &&
            (updatedBranch.RepositoryId == b.RepositoryId && updatedBranch.Path == b.Path));
        var repo = await _context.Repositories.FirstOrDefaultAsync(r => r.Id == updatedBranch.RepositoryId);
        if (conflict is not null) return Response.Conflict;
        if (repo is null) return Response.BadRequest;

        found.Name = updatedBranch.Name;
        found.RepositoryId = updatedBranch.RepositoryId;
        found.Path = updatedBranch.Path;
        await _context.SaveChangesAsync();
        return Response.Ok;
    }
}