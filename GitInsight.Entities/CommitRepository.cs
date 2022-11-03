namespace GitInsight.Entities;

public class CommitRepository : ICommitRepository
{
    private readonly InsightContext _context;

    public CommitRepository(InsightContext context)
    {
        _context = context;
    }
    
    public (Commit? commit,Response response) Find(int id)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == id);
        
        return entity==null ? (null, Response.NotFound) : (entity, Response.Ok);
    }
    
    public (IReadOnlyCollection<Commit>,Response) FindAll()
    {
        return (_context.Commits.ToList(),Response.Ok);
    }
    
    public Response Create(Commit commit)
    {
        
        //Check if commit with that Sha already exists
        if (_context.Commits.FirstOrDefault(c => c.Sha == commit.Sha) != null)
        {
            return Response.Conflict;
        } 
        
        //Check for no-existing branch, author, or repository
        if (_context.Authors.FirstOrDefault(c => c.Id == commit.AuthorId) is null
                   || _context.Branches.FirstOrDefault(c => c.Id == commit.BranchId) is null
                   || _context.Repositories.FirstOrDefault(c => c.Id == commit.RepositoryId) is null)
        {
            return Response.BadRequest;
        }
        
        _context.Commits.Add(commit);
        _context.SaveChanges();
        
        return Response.Created;
    }
    
    public Response Update(Commit commit)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == commit.Id);
        
        //Check if commit exits
        if (entity == null)
        {
            return Response.NotFound;
        } 
        
        //Check for no-existing branch, author, or repository
        if (_context.Authors.FirstOrDefault(c => c.Id == commit.AuthorId) is null
            || _context.Branches.FirstOrDefault(c => c.Id == commit.BranchId) is null
            || _context.Repositories.FirstOrDefault(c => c.Id == commit.RepositoryId) is null)
        {
            return Response.BadRequest;
        }
        
        //Check if any information actually have been updated
        if (   entity.AuthorId==commit.AuthorId 
            && entity.BranchId==commit.BranchId
            && entity.RepositoryId==commit.RepositoryId
            && entity.Sha==commit.Sha
            && entity.Date==commit.Date
            && entity.Tag==commit.Tag)
        {
            return Response.BadRequest;
        }

        //Update commit
        entity.AuthorId = commit.AuthorId; 
        entity.BranchId=commit.BranchId;
        entity.RepositoryId=commit.RepositoryId;
        entity.Sha=commit.Sha;
        entity.Date=commit.Date;
        entity.Tag = commit.Tag;
        _context.SaveChanges();
        
        return Response.Ok;
    }
    
    public Response Delete(int id)
    {
        var entity = _context.Commits.FirstOrDefault(c => c.Id == id);
        
        if (entity == null) return Response.NotFound;

        _context.Commits.Remove(entity);
        _context.SaveChanges();

        return Response.Deleted;
    }
}