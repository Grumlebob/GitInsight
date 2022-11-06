using Microsoft.EntityFrameworkCore;
using static GitInsight.Entities.ExistingCollectionHelper;

namespace GitInsight.Entities;

public class AuthorRepository : IAuthorRepository
{
    private readonly InsightContext _context;

    public AuthorRepository(InsightContext context)
    {
        _context = context;
    }

    public static AuthorDto AuthorToAuthorDto(Author author) =>
        new(Id: author.Id, Name: author.Name, Email: author.Email,
            CommitIds: author.Commits.Select(c => c.Id).ToList(),
            RepositoryIds: author.Repositories.Select(r => r.Id).ToList());

    public async Task<(AuthorDto?, Response)> FindAuthorAsync(int id)
    {
        Response response;

        var author = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .FirstOrDefaultAsync(a => a.Id == id);

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
        Response response;

        if (authorCreateDto.RepositoryIds.Any(repo => EnsureRepositoryExists(repo).Result == false))
        {
            response = Response.BadRequest;
            return (null, response);
        }

        var (existing, authorResponse) = await FindAuthorsByEmailAsync(authorCreateDto.Email);
        if (authorResponse != Response.NotFound)
        {
            response = Response.Conflict;
            return (existing!.FirstOrDefault(), response);
        }

        var author = new Author
        {
            Name = authorCreateDto.Name,
            Email = authorCreateDto.Email,
            Commits = await UpdateCommitsIfExist(_context, authorCreateDto.CommitIds),
            Repositories = await UpdateRepositoriesIfExist(_context, authorCreateDto.RepositoryIds),
        };

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        response = Response.Created;
        return (
            new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email,
                CommitIds: author.Commits.Select(c => c.Id).ToList(),
                RepositoryIds: author.Repositories.Select(r => r.Id).ToList()), response);
    }

    public async Task<Response> DeleteAuthorAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author is null) return Response.NotFound;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }

    public async Task<Response> UpdateAuthorAsync(AuthorDto authorDto)
    {
        Response response;

        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == authorDto.Id);

        if (author is null)
        {
            response = Response.NotFound;
        }
        else
        {
            author.Name = authorDto.Name;
            author.Email = authorDto.Email;
            author.Commits = await UpdateCommitsIfExist(_context,authorDto.CommitIds);
            author.Repositories = await UpdateRepositoriesIfExist(_context,authorDto.RepositoryIds);

            _context.Authors.Update(author);
            await _context.SaveChangesAsync();

            response = Response.Ok;
        }

        return response;
    }

    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByNameAsync(string name)
    {
        var authors = await _context.Authors
            .Where(a => a.Name.Contains(name))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(a => AuthorToAuthorDto(a)).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }

    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByEmailAsync(string email)
    {
        var authors = await _context.Authors
            .Where(a => a.Email.Contains(email))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(a => AuthorToAuthorDto(a)).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }

    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByRepositoryIdAsync(int repositoryId)
    {
        var authors = await _context.Authors
            .Where(a => a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(a => AuthorToAuthorDto(a)).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }


    public async Task<(List<AuthorDto>?, Response)> FindAuthorsByCommitIdAsync(int commitId)
    {
        var authors = await _context.Authors
            .Where(a => a.Commits.Any(c => c.Id == commitId))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(a => AuthorToAuthorDto(a)).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }


    private async Task<bool> EnsureRepositoryExists(int id)
    {
        var repository = await _context.Repositories.FindAsync(id);
        if (repository is null)
        {
            return false;
        }

        return true;
    }
}
