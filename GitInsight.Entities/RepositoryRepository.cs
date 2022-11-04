using static GitInsight.Entities.ExistingCollectionHelper;

namespace GitInsight.Entities;

public class RepositoryRepository : IRepositoryRepository
{
    private readonly InsightContext _context;

    public RepositoryRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(RepositoryDto repo, Response response)> CreateRepositoryAsync(
        RepositoryCreateDto repositoryCreateDto)
    {
        var commits = await _context.Commits.Where(c => repositoryCreateDto.CommitIds.Contains(c.Id)).ToListAsync();
        var branches = await _context.Branches.Where(c => repositoryCreateDto.BranchIds.Contains(c.Id)).ToListAsync();
        var authors = await _context.Authors.Where(c => repositoryCreateDto.AuthorIds.Contains(c.Id)).ToListAsync();

        var repository = new Repository
        {
            Name = repositoryCreateDto.Name,
            Path = repositoryCreateDto.Path,
            Commits = commits,
            Branches = branches,
            Authors = authors
        };

        var repoDto = RepositoryToRepositoryDto(repository);

        if (_context.Repositories.Contains(repository))
        {
            return (repoDto, Response.Conflict);
        }

        _context.Repositories.Add(repository);

        await _context.SaveChangesAsync();

        return (repoDto, Response.Created);
    }

    public async Task<Response> DeleteRepositoryAsync(int id)
    {
        var entity = await _context.Repositories.FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null) return Response.NotFound;

        _context.Repositories.Remove(entity);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }

    public async Task<(List<RepositoryDto>?, Response)> FindAllRepositoriesAsync()
    {
        var result = await _context.Repositories.Select(entity => RepositoryToRepositoryDto(entity)).ToListAsync();
        var count = result.Count;
        var more = count > 0;
        return more ? (result, Response.Ok) : (null, Response.NotFound);
    }

    public async Task<(RepositoryDto?, Response)> FindRepositoryAsync(int id)
    {
        var entity = await _context.Repositories.FirstOrDefaultAsync(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) : (RepositoryToRepositoryDto(entity), Response.Ok);
    }

    public async Task<Response> UpdateRepositoryAsync(RepositoryDto repositoryDto)
    {
        Response response;

        var repository = await _context.Repositories
            .FirstOrDefaultAsync(a => a.Id == repositoryDto.Id);

        if (repository is null)
        {
            response = Response.NotFound;
        }
        else
        {
            repository.Name = repositoryDto.Name;
            repository.Path = repositoryDto.Path;

            repository.Commits = await UpdateCommitsIfExist(_context, repositoryDto.CommitIds);
            repository.Branches = await UpdateBranchesIfExist(_context, repositoryDto.BranchIds);
            repository.Authors = await UpdateAuthorsIfExist(_context, repositoryDto.AuthorIds);

            _context.Repositories.Update(repository);
            await _context.SaveChangesAsync();

            response = Response.Ok;
        }

        return response;
    }


    public static RepositoryDto RepositoryToRepositoryDto(Repository repository)
    {
        return new RepositoryDto(repository.Id,
            repository.Path,
            repository.Name,
            repository.Branches.Select(b => b.Id),
            repository.Commits.Select(c => c.Id),
            repository.Authors.Select(a => a.Id));
    }
}
