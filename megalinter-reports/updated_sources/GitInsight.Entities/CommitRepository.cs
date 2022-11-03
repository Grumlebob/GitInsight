namespace GitInsight.Entities;

public class CommitRepository : ICommitRepository
{
    private readonly InsightContext _context;

    public CommitRepository(InsightContext context)
    {
        _context = context;
    }

    public (CommitDTO? commit, Response response) Find(int id)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) :
            (CommitToCommitDto(entity), Response.Ok);
    }
    public (IReadOnlyCollection<CommitDTO> commits, Response response) FindAll()
    {
        return (_context.Commits.Select(entity => CommitToCommitDto(entity)).ToList()
            , Response.Ok);
    }

    public (Response response, CommitDTO? commit) Create(CommitCreateDTO DTO)
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
        if (_context.Commits.FirstOrDefault(c => c.Sha == commit.Sha) != null)
        {
            return (Response.Conflict, null);
        }

        //Check for no-existing branch, author, or repository
        if (_context.Authors.FirstOrDefault(c => c.Id == commit.AuthorId) is null
                   || _context.Branches.FirstOrDefault(c => c.Id == commit.BranchId) is null
                   || _context.Repositories.FirstOrDefault(c => c.Id == commit.RepositoryId) is null)
        {
            return (Response.BadRequest, null);
        }

        _context.Commits.Add(commit);
        _context.SaveChanges();

        return (Response.Created, CommitToCommitDto(commit));
    }

    public (Response response, CommitDTO? commit) Update(CommitDTO commit)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == commit.Id);

        //Check if commit exits
        if (entity == null)
        {
            return (Response.NotFound, null);
        }

        //Check for no-existing branch, author, or repository
        if (!RelationsExists(commit))
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
        _context.SaveChanges();

        return (Response.Ok, CommitToCommitDto(entity));
    }

    public Response Delete(int id)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == id);

        if (entity == null) return Response.NotFound;

        _context.Commits.Remove(entity);
        _context.SaveChanges();

        return Response.Deleted;
    }

    public static CommitDTO CommitToCommitDto(Commit commit)
    {
        return new CommitDTO(commit.Id, commit.Sha, commit.Date, commit.Tag, commit.AuthorId, commit.BranchId,
            commit.RepositoryId);
    }

    private bool RelationsExists(CommitDTO commit)
    {
        return !(_context.Authors.FirstOrDefault(c => c.Id == commit.AuthorId) is null
                 || _context.Branches.FirstOrDefault(c => c.Id == commit.BranchId) is null
                 || _context.Repositories.FirstOrDefault(c => c.Id == commit.RepositoryId) is null);

    }

}