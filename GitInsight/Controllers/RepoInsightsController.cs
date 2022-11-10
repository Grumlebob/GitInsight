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
        var repoPath = Path.Combine(GetSavedRepositoriesFolder(), user, repoName);
        try
        {
            Console.WriteLine("Attempt clone");
            var repoGitPath = Repository.Clone($"https://github.com/{user}/{repoName}", repoPath);
        }
        catch (LibGit2SharpException e)
        {
            Console.WriteLine("Clone failed");
            return BadRequest(e.Message);
        }

        var dm = new DataManager(_context);
        if (!Directory.Exists(repoPath))
        {
            
        }

        return Ok();
    }
}