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
        var repository = new Repository
        {
            Name = repositoryCreateDto.Name,
            Path = repositoryCreateDto.Path,
            Commits = await UpdateCommitsIfExist(_context, repositoryCreateDto.CommitIds),
            Branches = await UpdateBranchesIfExist(_context, repositoryCreateDto.BranchIds),
            Authors = await UpdateAuthorsIfExist(_context, repositoryCreateDto.AuthorIds)
        };
        
        //if the new repository has the same path return conflict
        var existing = _context.Repositories.FirstOrDefault(r => r.Path == repository.Path);
        if (existing is not null)
        {
            return (RepositoryToRepositoryDto(existing), Response.Conflict);
        }

        await _context.Repositories.AddAsync(repository);
        await _context.SaveChangesAsync();

        var repoDto = RepositoryToRepositoryDto(repository);
        
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
        // if (result == null || result.Count == 0)
        //     return (new List<RepositoryDto>(), Response.NotFound);
        var count = result.Count;
        var more = count > 0;
        return more ? (result, Response.Ok) : (null, Response.NotFound);
    }

    public async Task<(RepositoryDto?, Response)> FindRepositoryAsync(int id)
    {
        var entity = await _context.Repositories.FirstOrDefaultAsync(c => c.Id == id);

        return entity == null ? (null, Response.NotFound) : (RepositoryToRepositoryDto(entity), Response.Ok);
    }
    
    public async Task<(RepositoryDto?, Response)> FindRepositoryByPathAsync(string path)
    {
        var entity = await _context.Repositories.FirstOrDefaultAsync(c => c.Path == path);

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

    public async Task<Response> UpdateLatestCommitAsync(RepositoryLatestCommitUpdate dto)
    {
        var toUpdate = await _context.Repositories.FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (toUpdate is null) return Response.NotFound;
        toUpdate.LatestCommitId = dto.LatestCommitId;
        _context.Repositories.Update(toUpdate);
        await _context.SaveChangesAsync();

        return Response.Ok;
    }


    public static RepositoryDto RepositoryToRepositoryDto(Repository repository)
    {
        return new RepositoryDto(repository.Id,
            repository.Path,
            repository.Name!,
            repository.Branches.Select(b => b.Id),
            repository.Commits.Select(c => c.Id),
            repository.Authors.Select(a => a.Id));
    }
}
