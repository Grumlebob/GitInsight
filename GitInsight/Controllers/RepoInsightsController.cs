using GitInsight.Core;
using GitInsight.Data;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GitInsight.Controllers;

[ApiController]
[Route("[controller]")]
public class RepoInsightsController : ControllerBase

{
    private readonly IRepoInsightRepository _repoInsightRepository;
    private readonly InsightContext _context;

    public RepoInsightsController(InsightContext context)
    {
        _context = context;
        _repoInsightRepository = new RepoInsightRepository(context);
    }

    [HttpGet]
    [Route("{user}/{repoName}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommitInsight>))]
    public async Task<IActionResult> AddOrUpdateLocalRepoData(string user, string repoName)
    {
        var url = $"https://github.com/{user}/{repoName}";
        var repoPath = Path.Combine(GetSavedRepositoriesFolder(), user, repoName);
        var dm = new DataManager(_context);

        if (Directory.Exists(repoPath))
        {
            //https://stackoverflow.com/questions/68673942/how-to-git-fetch-remote-using-libgit2sharp
            using var repo = new Repository(Path.Combine(repoPath, ".git"));
            var remote = repo.Network.Remotes["origin"];
            var refSpecs = new[] { $"+refs/heads/*:refs/remotes/{remote.Name}/*" };

            repo.Network.Fetch(remote.Name, refSpecs);
            await dm.Analyze(Path.Combine(repoPath, ".git"), user + "/" + repoName);
            return Ok();
        }

        try
        {
            Repository.Clone(url, repoPath);
        }
        catch (LibGit2SharpException e)
        {
            return BadRequest(e.Message);
        }

        if (!Directory.Exists(repoPath))
        {
        }

        return Ok();
    }
}