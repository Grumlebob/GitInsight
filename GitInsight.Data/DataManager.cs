using GitInsight.Core;
using GitInsight.Entities;

namespace GitInsight.Data;

using LibGit2Sharp;

public class DataManager
{
    private InsightContext _context;
    private string _relPath;

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
        using var repo = new Repository(GitPathHelper.GetGitLocalFolder());
        
        //repo
        var repos = new RepositoryRepository(_context);
        var repoName = repo.Info.WorkingDirectory.Split('\\').Last();
        var dto = new RepositoryCreateDto(_relPath, repoName, null, null, null);
        var (result, response) = await repos.CreateRepositoryAsync(dto);
        Console.WriteLine(result);
        var createdRepo = await repos.FindRepositoryAsync(result.Id);
        Console.WriteLine(createdRepo);
        
        
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
                Console.WriteLine(c.Sha + " " + c.Author.When +" " +" "+ branchResult +" "+ result.Id);
                var commitDto = new CommitCreateDTO(c.Sha, c.Author.When, authResult.Id, branchResult.Id, result.Id);
                await commits.CreateAsync(commitDto);
            }
        }
    }
}