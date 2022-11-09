using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.EntityFrameworkCore;

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

    //Returns true if analyze completed, false if analyse was not needed.
    public async Task<bool> Analyze(string fullPath, string relPath)
    {
        await CheckIfReanalyzeNeeded(fullPath);
        if (!_shouldReanalyze)
        {
            Console.WriteLine("No analyze was needed");
            return false;
        }

        using var repo = new Repository(fullPath);
        var repos = new RepositoryRepository(_context);
        var branches = new BranchRepository(_context);
        var authors = new AuthorRepository(_context);
        var commits = new CommitInsightRepository(_context);

        //repo
        var dto = new RepositoryCreateDto(relPath, relPath, null!, null!, null!);
        var (result, _) = await repos.CreateAsync(dto);
        //branches
        foreach (var b in repo.Branches)
        {
            var branchDto = new BranchCreateDto(b.FriendlyName, result.Id, b.CanonicalName);
            var (_, branchResult) = await branches.CreateAsync(branchDto);
            //Authors and commits
            foreach (var c in b.Commits)
            {
                var authorDto = new AuthorCreateDto(c.Author.Name, c.Author.Email, null, new List<int> { result.Id });
                var (authResult, _) = await authors.CreateAsync(authorDto);
                var commitDto = new CommitInsightCreateDto(c.Sha, c.Author.When, authResult!.Id, branchResult!.Id, result.Id);
                await commits.CreateAsync(commitDto);
            }
        }
        await UpdateLatestCommit(result.Id, fullPath);
        Console.WriteLine("Analyze finished");
        return true;
    }

    private async Task CheckIfReanalyzeNeeded(string fullPath)
    {
        using var repo = new Repository(fullPath);
        var commits = new CommitInsightRepository(_context);

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        var (_, response) = await commits.FindByShaAsync(actualLastCommit!.Sha);
        if (response == Response.NotFound)
        {
            _shouldReanalyze = true;
        }
        else if (response == Response.Ok)
        {
            _shouldReanalyze = false;
        }
    }

    private async Task UpdateLatestCommit(int id, string fullPath)
    {
        using var repo = new Repository(fullPath);
        var commits = new CommitInsightRepository(_context);
        var repository = new RepositoryRepository(_context);

        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        var (latestDbCommit, _) = await commits.FindByShaAsync(actualLastCommit!.Sha);
        var withLatest = new RepositoryLatestCommitUpdate(id, latestDbCommit!.Id);
        await repository.UpdateLatestCommitAsync(withLatest);
    }
}