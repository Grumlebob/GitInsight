using GitInsight.Core;
using GitInsight.Entities;

namespace GitInsight.Data;

using LibGit2Sharp;

public class DataManager
{
    private InsightContext _context;
    private string _relPath;
    private bool _shouldReanalyze = false;
    

    public DataManager(InsightContext context, string relPath)
    {
        _context = context;
        _relPath = relPath;
    }

    public async Task AnalyzeRepo()
    {
        using var repo = new Repository(GitPathHelper.GetGitLocalFolder());

        //repo
        var repos = new RepositoryRepository(_context);
        var repoName = repo.Info.WorkingDirectory.Split('\\').Last();
        var dto = new RepositoryCreateDto(_relPath, repoName, null, null, null);
        var (result, response) = await repos.CreateRepositoryAsync(dto);
    }

    public async Task Analyze()
    {
        await _context.Database.EnsureCreatedAsync();
        await _context.SaveChangesAsync();
        
        await CheckIfReanalyzeNeeded();
        if (!_shouldReanalyze)
        {
            Console.WriteLine("No analyze was needed");
            return;
        }

        
        
        using var repo = new Repository(GitPathHelper.GetGitLocalFolder());

        //repo
        var repos = new RepositoryRepository(_context);
        var repoName = repo.Info.WorkingDirectory.Split('\\').Last();
        var dto = new RepositoryCreateDto(_relPath, repoName, null, null, null);
        var (result, response) = await repos.CreateRepositoryAsync(dto);

        var createdRepo = await repos.FindRepositoryAsync(result.Id);

        
        
        
        //branches
        var branches = new BranchRepository(_context);
        var authors = new AuthorRepository(_context);
        var commits = new CommitRepository(_context);
        foreach (var b in repo.Branches)
        {
            var branchDto = new BranchCreateDto(b.FriendlyName, result.Id, b.UpstreamBranchCanonicalName);
            var (branchResponse, branchResult) = await branches.CreateAsync(branchDto);
            foreach (var c in b.Commits)
            {
                var authorDto = new AuthorCreateDto(c.Author.Name, c.Author.Email, null, new List<int> { result.Id });
                var (authResult, authResponse) = await authors.CreateAuthorAsync(authorDto);
                Console.WriteLine(authResult);
                Console.WriteLine(branchResult);
                var commitDto = new CommitCreateDTO(c.Sha, c.Author.When, authResult.Id, branchResult.Id, result.Id);
                await commits.CreateAsync(commitDto);
            }
        }

        await UpdateLatestCommit(result.Id);

    }

    private async Task CheckIfReanalyzeNeeded()
    {
        using var repo = new Repository(GitPathHelper.GetGitLocalFolder());
        var commits = new CommitRepository(_context);

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
            //IncludeReachableFrom = repo.Branches["master"].Tip
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        var (res, response) = await commits.FindByShaAsync(actualLastCommit.Sha);
        Console.WriteLine(res.Sha);
        Console.WriteLine(actualLastCommit.Sha);
        if (response == Response.NotFound)
        {
            _shouldReanalyze = true;
        } else if (response == Response.Ok)
        {
            _shouldReanalyze = false;
        }
    }

    private async Task UpdateLatestCommit(int id)
    {
        using var repo = new Repository(GitPathHelper.GetGitLocalFolder());

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
            //IncludeReachableFrom = repo.Branches["master"].Tip
        });
        var lastCommit = queryFilter.FirstOrDefault();
        var commits = new CommitRepository(_context);
        var (c, x) = await commits.FindByShaAsync(lastCommit.Sha);
        var repository = new RepositoryRepository(_context);
        var (found, r) = await repository.FindRepositoryAsync(id);
        var withlatest = new RepositoryLatestCommitUpdate(id, c.Id);
        await repository.UpdateLatestCommitAsync(withlatest);
    }
}