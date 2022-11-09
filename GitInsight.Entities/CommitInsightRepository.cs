namespace GitInsight.Entities;

public class CommitInsightRepository : ICommitInsightRepository
{
    private readonly InsightContext _context;

    public CommitInsightRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(CommitInsightDto? commit, Response response)> FindAsync(int id)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }

    public async Task<(CommitInsightDto? commit, Response response)> FindByShaAsync(string sha)
    {
        var entity = await _context.Commits.FirstOrDefaultAsync(c => c.Sha == sha);
        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }

    public async Task<(IReadOnlyCollection<CommitInsightDto> commits, Response response)> FindAllAsync()
    {
        return (await _context.Commits.Select(entity => CommitToCommitDto(entity)).ToListAsync()
            , Response.Ok);
    }

    public async Task<(Response response, CommitInsightDto? commit)> CreateAsync(CommitInsightCreateDto commitCreateDto)
    {

        //Check if commit with that Sha already exists
        if (await _context.Commits.FirstOrDefaultAsync(c => c.Sha == commitCreateDto.Sha) != null)
        {
            return (Response.Conflict, null);
        }

        //Check for no-existing branch, author, or repository
        if (await _context.Authors.FirstOrDefaultAsync(c => c.Id == commitCreateDto.AuthorId) is null
                   || await _context.Branches.FirstOrDefaultAsync(c => c.Id == commitCreateDto.BranchId) is null
                   || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == commitCreateDto.RepositoryId) is null)
        {
            return (Response.BadRequest, null);
        }

        var commit = new CommitInsight
        {
            Sha = commitCreateDto.Sha,
            Date = commitCreateDto.Date.UtcDateTime,
            AuthorId = commitCreateDto.AuthorId,
            BranchId = commitCreateDto.BranchId,
            RepositoryId = commitCreateDto.RepositoryId
        };

        _context.Commits.Add(commit);
        await _context.SaveChangesAsync();

        return (Response.Created, CommitToCommitDto(commit));
    }

    public async Task<(Response response, CommitInsightDto? commit)> UpdateAsync(CommitInsightDto commit)
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

    public static CommitInsightDto CommitToCommitDto(CommitInsight commit)
    {
        return new CommitInsightDto(commit.Id, commit.Sha, commit.Date, commit.AuthorId, commit.BranchId,
            commit.RepositoryId);
    }

    private async Task<bool> RelationsExists(CommitInsightDto commit)
    {
        return !(await _context.Authors.FirstOrDefaultAsync(c => c.Id == commit.AuthorId) is null
                 || await _context.Branches.FirstOrDefaultAsync(c => c.Id == commit.BranchId) is null
                 || await _context.Repositories.FirstOrDefaultAsync(c => c.Id == commit.RepositoryId) is null);

    }

}