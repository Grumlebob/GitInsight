using System.Reflection.Metadata.Ecma335;
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
        var repoPath = $"{GetSavedRepositoriesFolder()}/{user}/{repoName}";
        var dm = new DataManager(_context);

        if (Directory.Exists(repoPath))
        {
            DeleteDirectory($"{GetSavedRepositoriesFolder()}/{user}");
        }
        
        try
        {
            Repository.Clone(url, repoPath);
        }
        catch (LibGit2SharpException e)
        {
            return BadRequest(e.Message);
        }

        DeleteDirectory(repoPath, foldersToSpare: new []{".git", repoPath}); //spare repoPath itself but delete all its content (but .git)

        await dm.Analyze(repoPath + "/.git", $"{GetRelativeSavedRepositoriesFolder()}/{user}/{repoName}");
        
        var (commits, _) = await new CommitInsightRepository(_context).FindAllAsync();
        return Ok(commits);
    }

    //https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
    private static void DeleteDirectory(string targetDir, string[]? foldersToSpare = null)
    {
        foldersToSpare ??= Array.Empty<string>();
        var files = Directory.GetFiles(targetDir);
        var dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            System.IO.File.SetAttributes(file, FileAttributes.Normal);
            System.IO.File.Delete(file);
        }

        foreach (var dir in dirs)
        {
            if (foldersToSpare.Any(dir.EndsWith)) continue;
            DeleteDirectory(dir, foldersToSpare);
        }
        
        if (foldersToSpare.Any(targetDir.EndsWith)) return;
        Directory.Delete(targetDir);
    }
}