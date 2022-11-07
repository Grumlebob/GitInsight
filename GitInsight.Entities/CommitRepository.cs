using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;



public class CommitRepository : ICommitRepository
{
    private readonly InsightContext _context;

    public CommitRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(CommitDto? commit, Response response)> FindAsync(int id)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }
    
    public async Task<(CommitDto? commit, Response response)> FindByShaAsync(string sha)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Sha == sha);
        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }
    
    public async Task<(IReadOnlyCollection<CommitDto> commits, Response response)> FindAllAsync()
    {
        return (await _context.Commits.Select(entity => CommitToCommitDto(entity)).ToListAsync()
            , Response.Ok);
    }

    public async Task<(Response response, CommitDto? commit)> CreateAsync(CommitCreateDto DTO)
    {

        //Check if commit with that Sha already exists
        if (await _context.Commits.FirstOrDefaultAsync(c => c.Sha == DTO.Sha) != null)
        {
            return (Response.Conflict, null);
        }

        //Check for no-existing branch, author, or repository
        if (await _context.Authors.FirstOrDefaultAsync(c => c.Id == DTO.AuthorId) is null
                   || await _context.Branches.FirstOrDefaultAsync(c => c.Id == DTO.BranchId) is null
                   || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == DTO.RepositoryId) is null)
        {
            return (Response.BadRequest, null);
        }

        var commit = new Commit
        {
            Sha = DTO.Sha,
            Date = DTO.Date.UtcDateTime,
            AuthorId = DTO.AuthorId,
            BranchId = DTO.BranchId,
            RepositoryId = DTO.RepositoryId
        };
        
        _context.Commits.Add(commit);
        await _context.SaveChangesAsync();

        return (Response.Created, CommitToCommitDto(commit));
    }

    public async Task<(Response response, CommitDto? commit)> UpdateAsync(CommitDto commit)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Id == commit.Id);

        //Check if commit exits
        if (entity == null)
        {
            return (Response.NotFound, null);
        }

        //Check for no-existing branch, author, or repository
        if (!await RelationsExists(commit))
        {
            return (Response.BadRequest, CommitToCommitDto(entity));
        }

        //Check if any information actually have been updated
        if (entity.AuthorId == commit.AuthorId
            && entity.BranchId == commit.BranchId
            && entity.RepositoryId == commit.RepositoryId
            && entity.Sha == commit.Sha
            && entity.Date == commit.Date)
        {
            return (Response.BadRequest, CommitToCommitDto(entity));
        }

        //Update commit
        entity.AuthorId = commit.AuthorId;
        entity.BranchId = commit.BranchId;
        entity.RepositoryId = commit.RepositoryId;
        entity.Sha = commit.Sha;
        entity.Date = commit.Date.UtcDateTime;
        await _context.SaveChangesAsync();

        return (Response.Ok, CommitToCommitDto(entity));
    }

    public async Task<Response> DeleteAsync(int id)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null) return Response.NotFound;

        _context.Commits.Remove(entity);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }

    public static CommitDto CommitToCommitDto(Commit commit)
    {
        return new CommitDto(commit.Id, commit.Sha, commit.Date, commit.AuthorId, commit.BranchId,
            commit.RepositoryId);
    }

    private async Task<bool> RelationsExists(CommitDto commit)
    {
        return !(await _context.Authors.FirstOrDefaultAsync(c => c.Id == commit.AuthorId) is null
                 || await _context.Branches.FirstOrDefaultAsync(c => c.Id == commit.BranchId) is null
                 || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == commit.RepositoryId) is null);

    }

}