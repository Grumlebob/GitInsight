using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class AuthorRepository : IAuthorRepository
{
    private readonly InsightContext _context;

    public AuthorRepository(InsightContext context)
    {
        _context = context;
    }
    
    public static AuthorDto AuthorToAuthorDto(Author author) =>  
        new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email, 
            CommitIds: author.Commits.Select(c => c.Id).ToList(), 
            RepositoryIds: author.Repositories.Select(r => r.Id).ToList());

    public async Task<(AuthorDto?, Response)> FindAuthorAsync(int id)
    {
        var response = new Response();
        
        var author = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .FirstOrDefaultAsync(a =>  a.Id == id);

        if (author is null)
        {
            response = Response.NotFound;
            return (null, response);
        }
        else
        {
            response = Response.Ok;
            return (AuthorToAuthorDto(author), response);
        }
    }
    
    
    public async Task<(List<AuthorDto>?, Response response)> FindAllAuthorsAsync()
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .ToListAsync();
        
        return authors.Count > 0 
            ? (authors.Select(a => AuthorToAuthorDto(a)).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }
    
    public async Task<(AuthorDto?, Response)> CreateAuthorAsync(AuthorCreateDto authorCreateDto)
    {
        var response = new Response();

        if (authorCreateDto.RepositoryIds.Any(repo => EnsureRepoisitoryExists(repo).Result == false))
        {
            response = Response.Conflict;
            return (null, response);
        }
        
        var author = new Author
        {
            Name = authorCreateDto.Name,
            Email = authorCreateDto.Email,
            Commits = authorCreateDto.CommitIds.Select(c => new Commit {Id = c}).ToList(),
            Repositories = await CreateOrUpdateRepositories(authorCreateDto.RepositoryIds),
        };

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        response = Response.Created;
        return (new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email, CommitIds: author.Commits.Select(c => c.Id).ToList(), RepositoryIds: author.Repositories.Select(r => r.Id).ToList()), response);
    }
    
    private async Task<bool> EnsureRepoisitoryExists(int id)
    {
        var repository = await _context.Repositories.FindAsync(id);
        if (repository is null)
        {
            return false;
        }
        return true;
    }
    
    private async Task<List<Repository>> CreateOrUpdateRepositories(IEnumerable<int> repoIds)
    {
        var existing = _context.Repositories.Where(r => repoIds.Contains(r.Id));
        return new List<Repository>(existing);
        
    }
    
    /*
    public async Task<(AuthorDto?, Response)> UpdateAuthorAsync(AuthorDto authorDto)
    {
        var response = new Response();
        
        var author = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .FirstOrDefaultAsync(a => a.Id == authorDto.Id);

        if (author is null)
        {
            response = Response.NotFound;
            return (null, response);
        }
        else
        {
            author.Name = authorDto.Name;
            author.Email = authorDto.Email;
            author.Commits = authorDto.CommitIds.Select(c => new Commit {Id = c}).ToList();
            author.Repositories = authorDto.RepositoryIds.Select(r => new Repository {Id = r}).ToList();

            await _context.SaveChangesAsync();

            response = Response.Ok;
            return (new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email, CommitIds: author.Commits.Select(c => c.Id).ToList(), RepositoryIds: author.Repositories.Select(r => r.Id).ToList()), response);
        }
    }
    
    public async Task<Response> DeleteAuthorAsync(int id)
    {
        var response = new Response();
        
        var author = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author is null)
        {
            response = Response.NotFound;
            return response;
        }
        else
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            response = Response.Ok;
            return response;
        }
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByRepositoryIdAsync(int repositoryId)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByCommitIdAsync(int commitId)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Commits.Any(c => c.Id == commitId))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByNameAsync(string name)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Name.Contains(name))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByEmailAsync(string email)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Email.Contains(email))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByCommitIdAndRepositoryIdAsync(int commitId, int repositoryId)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Commits.Any(c => c.Id == commitId) && a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByNameAndEmailAsync(string name, string email)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Name.Contains(name) && a.Email.Contains(email))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByNameAndRepositoryIdAsync(string name, int repositoryId)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Name.Contains(name) && a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByEmailAndRepositoryIdAsync(string email, int repositoryId)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Email.Contains(email) && a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByCommitIdAndNameAndEmailAsync(int commitId, string name, string email)
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .Where(a => a.Commits.Any(c => c.Id == commitId) && a.Name.Contains(name) && a.Email.Contains(email))
            .ToListAsync();

        return authors.Count > 0 
            ? (authors.Select(a => new AuthorDto(Id: a.Id, Name: a.Name, Email: a.Email,
                CommitIds: a.Commits.Select(c=>c.Id).ToList(),
                RepositoryIds: a.Repositories.Select(r => r.Id).ToList())).ToList(), 
                Response.Ok) 
            : (null, Response.NotFound);
    }
    
    */

}