using System.Text.RegularExpressions;
using GitInsight.Core;
using GitInsight.Entities;

namespace GitInsight.Data;

using LibGit2Sharp;

public class DataManager
{
    private readonly IRepoInsightRepository _repoInsightRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitInsightRepository _commitInsightRepository;
    private readonly IAuthorRepository _authorRepository;

    private bool _shouldReanalyze;

    public DataManager(InsightContext context)
    {
        var ctx = context;
        _repoInsightRepository = new RepoInsightRepository(ctx);
        _branchRepository = new BranchRepository(ctx);
        _authorRepository = new AuthorRepository(ctx);
        _commitInsightRepository = new CommitInsightRepository(ctx);
    }

    //Returns true if analyze completed, false if analyse was not needed.
    public async Task<bool> Analyze(string fullPath, string relPath)
    {
        _shouldReanalyze = await CheckIfReanalyzeNeeded(fullPath);
        if (!_shouldReanalyze)
        {
            return false;
        }
        
        using var repo = new Repository(fullPath);
        //repo
        var dto = new RepoInsightCreateDto(relPath, relPath, null!, null!, null!);
        var (repoResult, _) = await _repoInsightRepository.CreateAsync(dto);
        
        //branches
        foreach (var b in repo.Branches)
        {
            var (branchResult, _) =
                await _branchRepository.CreateAsync(new BranchCreateDto(b.FriendlyName, repoResult.Id,
                    b.CanonicalName));
            //Authors and commits
            foreach (var c in b.Commits)
            {
                if (c.Author.Email=="codeMerge") continue;
                var authorDto = new AuthorCreateDto(c.Author.Name, c.Author.Email, new List<int> { repoResult.Id });
                var (authResult, _) = await _authorRepository.CreateAsync(authorDto);
                var commitDto = new CommitInsightCreateDto(c.Sha, c.Author.When, authResult!.Id, branchResult!.Id,
                    repoResult.Id);
                await _commitInsightRepository.CreateAsync(commitDto);
            }
        }

        await UpdateLatestCommit(repoResult.Id, fullPath);
        return true;
    }

    private async Task<bool> CheckIfReanalyzeNeeded(string fullPath)
    {
        using var repo = new Repository(fullPath);
        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time, 
        });
        var actualLastCommit = queryFilter.FirstOrDefault();
        if (actualLastCommit.Author.Email == "codeMerge") actualLastCommit = queryFilter.ToList()[1];
        var (_, response) = await _commitInsightRepository.FindByShaAsync(actualLastCommit!.Sha);
        if (response == Response.NotFound) return true;
        return false;
    }

    private async Task UpdateLatestCommit(int id, string fullPath)
    {
        using var repo = new Repository(fullPath);
        var queryFilter = repo.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
        });
        
        var actualLastCommit = queryFilter.FirstOrDefault();
        if (actualLastCommit.Author.Email == "codeMerge") actualLastCommit = queryFilter.ToList()[1];
        var (latestDbCommit, _) = await _commitInsightRepository.FindByShaAsync(actualLastCommit!.Sha);
        var withLatest = new RepoInsightLatestCommitUpdate(id, latestDbCommit!.Id);
        await _repoInsightRepository.UpdateLatestCommitAsync(withLatest);
    }
}