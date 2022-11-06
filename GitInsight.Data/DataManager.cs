using GitInsight.Core;
using GitInsight.Entities;

namespace GitInsight.Data;

using LibGit2Sharp;

public class DataManager
{
    private readonly InsightContext _context;
    private bool _shouldReanalyze;

    public DataManager(InsightContext context)
    {
        _context = context;
    }

    public async Task Analyze(string fullPath, string relPath)
    {
        await _context.Database.EnsureCreatedAsync();
        await _context.SaveChangesAsync();
        
        await CheckIfReanalyzeNeeded(fullPath);
        if (!_shouldReanalyze)
        {
            Console.WriteLine("No analyze was needed");
            return;
        }

        using var repo = new Repository(fullPath);
        var repos = new RepositoryRepository(_context);
        var branches = new BranchRepository(_context);
        var authors = new AuthorRepository(_context);
        var commits = new CommitRepository(_context);

        //repo
        var dto = new RepositoryCreateDto(relPath, relPath, null!, null!, null!);
        var (result, _) = await repos.CreateRepositoryAsync(dto);
        
        //branches
        foreach (var b in repo.Branches)
        {
            var branchDto = new BranchCreateDto(b.FriendlyName, result.Id, b.CanonicalName);
            var (_, branchResult) = await branches.CreateAsync(branchDto);
            foreach (var c in b.Commits)
            {
                var authorDto = new AuthorCreateDto(c.Author.Name, c.Author.Email, null, new List<int> { result.Id });
                var (authResult, _) = await authors.CreateAuthorAsync(authorDto);
                var commitDto = new CommitCreateDTO(c.Sha, c.Author.When, authResult!.Id, branchResult!.Id, result.Id);
                await commits.CreateAsync(commitDto);
            }
        }

        await UpdateLatestCommit(result.Id, fullPath);

    }

    private async Task CheckIfReanalyzeNeeded(string fullPath)
    {
        using var repo = new Repository(fullPath);
        var commits = new CommitRepository(_context);

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
            //IncludeReachableFrom = repo.Branches["master"].Tip
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        var (_, response) = await commits.FindByShaAsync(actualLastCommit!.Sha);
        if (response == Response.NotFound)
        {
            _shouldReanalyze = true;
        } else if (response == Response.Ok)
        {
            _shouldReanalyze = false;
        }
    }

    private async Task UpdateLatestCommit(int id, string fullPath)
    {
        using var repo = new Repository(fullPath);
        var commits = new CommitRepository(_context);
        var repository = new RepositoryRepository(_context);

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
            //IncludeReachableFrom = repo.Branches["master"].Tip
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        var (latestDbCommit, _) = await commits.FindByShaAsync(actualLastCommit!.Sha);
        var withLatest = new RepositoryLatestCommitUpdate(id, latestDbCommit!.Id);
        await repository.UpdateLatestCommitAsync(withLatest);
    }
}