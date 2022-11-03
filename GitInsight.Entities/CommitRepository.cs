using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;



public class CommitRepository : ICommitRepository
{
    private readonly InsightContext _context;

    public CommitRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(CommitDTO? commit, Response response)> FindAsync(int id)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }
    public async Task<(IReadOnlyCollection<CommitDTO> commits, Response response)> FindAllAsync()
    {
        return  ( await _context.Commits.Select(entity => CommitToCommitDto(entity)).ToListAsync()
            , Response.Ok);
    }

    public async Task<(Response response, CommitDTO? commit)> CreateAsync(CommitCreateDTO DTO)
    {
        var commit = new Commit
        {
            Sha = DTO.Sha,
            Date = DTO.Date,
            Tag = DTO.Tag,
            AuthorId = DTO.AuthorId,
            BranchId = DTO.BranchId,
            RepositoryId = DTO.RepositoryId
        };


        //Check if commit with that Sha already exists
        if (await _context.Commits.FirstOrDefaultAsync(c => c.Sha == commit.Sha) != null)
        {
            return (Response.Conflict, null);
        }

        //Check for no-existing branch, author, or repository
        if (await _context.Authors.FirstOrDefaultAsync(c => c.Id == commit.AuthorId) is null
                   || await _context.Branches.FirstOrDefaultAsync(c => c.Id == commit.BranchId) is null
                   || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == commit.RepositoryId) is null)
        {
            return (Response.BadRequest, null);
        }

        _context.Commits.Add(commit);
        await _context.SaveChangesAsync();

        return (Response.Created, CommitToCommitDto(commit));
    }

    public async Task<(Response response, CommitDTO? commit)> UpdateAsync(CommitDTO commit)
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
            && entity.Date == commit.Date
            && entity.Tag == commit.Tag)
        {
            return (Response.BadRequest, CommitToCommitDto(entity));
        }

        //Update commit
        entity.AuthorId = commit.AuthorId;
        entity.BranchId = commit.BranchId;
        entity.RepositoryId = commit.RepositoryId;
        entity.Sha = commit.Sha;
        entity.Date = commit.Date;
        entity.Tag = commit.Tag;
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

    public static CommitDTO CommitToCommitDto(Commit commit)
    {
        return new CommitDTO(commit.Id, commit.Sha, commit.Date, commit.Tag, commit.AuthorId, commit.BranchId,
            commit.RepositoryId);
    }

    private async Task<bool> RelationsExists(CommitDTO commit)
    {
        return !(await _context.Authors.FirstOrDefaultAsync(c => c.Id == commit.AuthorId) is null
                 || await _context.Branches.FirstOrDefaultAsync(c => c.Id == commit.BranchId) is null
                 || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == commit.RepositoryId) is null);

    }

}