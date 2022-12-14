namespace GitInsight.Entities;

public class AuthorRepository : IAuthorRepository
{
    private readonly InsightContext _context;

    public AuthorRepository(InsightContext context)
    {
        _context = context;
    }

    private static AuthorDto AuthorToAuthorDto(Author? author)
    {
        return author is null
            ? null!
            : new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email,
                RepositoryIds: author.Repositories.Select(r => r.Id).ToList());
    }
        

    public async Task<(AuthorDto?, Response)> FindAsync(int id)
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


    public async Task<(List<AuthorDto>?, Response response)> FindAllAsync()
    {
        var authors = await _context.Authors
            .Include(a => a.Commits)
            .Include(a => a.Repositories)
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(AuthorToAuthorDto).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }

    public async Task<(AuthorDto?, Response)> CreateAsync(AuthorCreateDto authorCreateDto)
    {
        Response response;

        if (authorCreateDto.RepositoryIds.Any(repo => EnsureRepositoryExists(repo).Result == false))
        {
            response = Response.BadRequest;
            return (null, response);
        }

        var (existing, authorResponse) = await FindByEmailAsync(authorCreateDto.Email);

        if (authorResponse == Response.NotFound)
        {
            var author = new Author
            {
                Name = authorCreateDto.Name,
                Email = authorCreateDto.Email,
                Repositories = await UpdateRepositoriesIfExist(_context, authorCreateDto.RepositoryIds),
            };

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            response = Response.Created;
            return (
                new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email,
                    RepositoryIds: author.Repositories.Select(r => r.Id).ToList()), response);
        }

        //If author already exists
        var newRepos = NewRepos(authorCreateDto, existing!);
        if (newRepos.Count<1)
        {
            response = Response.Conflict;
            return (existing, response);
        }

        existing!.RepositoryIds.AddRange(newRepos);
        var updated = new AuthorUpdateDto(existing.Id, existing.Name, existing.Email, existing.RepositoryIds);
        await UpdateAsync(updated);
        return (existing, Response.Ok);
    }

    public async Task<Response> DeleteAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author is null) return Response.NotFound;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }

    public async Task<Response> UpdateAsync(AuthorUpdateDto authorDto)
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
            
            if (authorDto.RepositoryIds.Any()) author.Repositories =
                authorDto.RepositoryIds.Select(id => _context.Repositories.First(r => r.Id==id)).ToList();
            
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();

            response = Response.Ok;
        }

        return response;
    }

    public async Task<(List<AuthorDto>?, Response)> FindByNameAsync(string name)
    {
        var authors = await _context.Authors
            .Where(a => a.Name.Contains(name))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(AuthorToAuthorDto).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }

    public async Task<(AuthorDto?, Response)> FindByEmailAsync(string email)
    {
        var author = await _context.Authors
            .Where(a => a.Email == email).Include( a => a.Repositories ).FirstOrDefaultAsync();
        var response = author is null ? Response.NotFound : Response.Ok;
        return (AuthorToAuthorDto(author), response);
    }

    public async Task<(List<AuthorDto>?, Response)> FindByRepoIdAsync(int repositoryId)
    {
        
        var authors = await _context.Authors
            .Where(a => a.Repositories.Any(r => r.Id == repositoryId))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(AuthorToAuthorDto).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }


    public async Task<(List<AuthorDto>?, Response)> FindByCommitIdAsync(int commitId)
    {
        var authors = await _context.Authors
            .Where(a => a.Commits.Any(c => c.Id == commitId))
            .ToListAsync();

        return authors.Count > 0
            ? (authors.Select(AuthorToAuthorDto).ToList(), Response.Ok)
            : (null, Response.NotFound);
    }

    private static List<int> NewRepos(AuthorCreateDto newAuth, AuthorDto oldAuth)
    {
        return newAuth.RepositoryIds.Where(repoId => !oldAuth.RepositoryIds.Contains(repoId)).ToList();
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