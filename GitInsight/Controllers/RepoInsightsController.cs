using GitInsight.Data;
using Microsoft.AspNetCore.Cors;

namespace GitInsight.Controllers;

[ApiController]
[Authorize]
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

    [Authorize]
    [HttpGet]
    [Route("{user}/{repoName}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommitsByDateByAuthor>))]
    public async Task<IActionResult> AddOrUpdateLocalRepoData(string user, string repoName)
    {
        var url = $"https://github.com/{user}/{repoName}";
        var repoPath = $"{GetSavedRepositoriesFolder()}/{user}/{repoName}";
        var userPath = $"{GetSavedRepositoriesFolder()}/{user}";
        var dm = new DataManager(_context);

        if (Directory.Exists(repoPath))
        {
            DeleteDirectory(repoPath);
        }

        try
        {
            Repository.Clone(url, repoPath);
        }
        catch (LibGit2SharpException)
        {
            if (Directory.Exists(userPath) && !Directory.EnumerateFileSystemEntries(userPath).Any())
            {
                DeleteDirectory(userPath); //clone makes a new folder even when failed. Delete if empty
            }

            return BadRequest("Something went wrong in trying to reach the repository. " +
                              "Please check that your spelling is correct and that it is a public repository");
        }

        DeleteDirectory(repoPath,
            foldersToSpare: new[] { ".git", repoPath }); //spare repoPath itself but delete all its content (but .git)

        var relPath = $"{GetRelativeSavedRepositoriesFolder()}/{user}/{repoName}";

        await dm.Analyze(repoPath + "/.git", relPath);
        var analysis = new Analysis(_context);

        var (repo, _) = await new RepoInsightRepository(_context).FindRepositoryByPathAsync(relPath);

        Console.WriteLine("repoId: " + repo.Id);

        return Ok(await analysis.GetCommitsByAuthor(repo!.Id));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RepoInsightRepository>))]
    public async Task<IActionResult> GetAllRepositories()
    {
        var (list, response) = await _repoInsightRepository.FindAllAsync();
        if (response == Core.Response.Ok) return Ok(list);
        return NotFound();
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RepoInsightRepository))]
    public async Task<IActionResult> GetRepoById(int id)
    {
        var (repoDto, response) = await _repoInsightRepository.FindAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok(repoDto);
    }

    //https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
    private static void DeleteDirectory(string targetDir, string[]? foldersToSpare = null)
    {
        foldersToSpare ??= Array.Empty<string>();
        var files = Directory.GetFiles(targetDir);
        var dirs = Directory.GetDirectories(targetDir);

        foreach (var file in files)
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

    [Authorize]
    [HttpGet]
    [Route("{user}/{repoName}/EarlyBird")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GitAwardWinner))]
    public async Task<IActionResult> EarlyBird(string user, string repoName)
    {
        var response = await AddOrUpdateLocalRepoData(user, repoName);
        if (response is BadRequestObjectResult)
        {
            return BadRequest();
        }
        
        var relPath = $"{GetRelativeSavedRepositoriesFolder()}/{user}/{repoName}";
        var analysis = new Analysis(_context);
        var (repo, _) = await _repoInsightRepository.FindRepositoryByPathAsync(relPath);

        return Ok(await analysis.HighestCommitterWithinTimeframe(repo.Id, DateTime.Parse("1/1/1111 06:00:00 AM"),
            DateTime.Parse("1/1/1111 10:00:00 AM")));
    }

    [Authorize]
    [HttpGet]
    [Route("{user}/{repoName}/NightOwl")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GitAwardWinner))]
    public async Task<IActionResult> NightOwl(string user, string repoName)
    {
        var response = await AddOrUpdateLocalRepoData(user, repoName);
    
        if (response is BadRequestObjectResult)
        {
            return BadRequest();
        }

        var relPath = $"{GetRelativeSavedRepositoriesFolder()}/{user}/{repoName}";
        var analysis = new Analysis(_context);
        var (repo, _) = await _repoInsightRepository.FindRepositoryByPathAsync(relPath);

        return Ok(await analysis.HighestCommitterWithinTimeframe(repo.Id, DateTime.Parse("1/1/1111 00:00:00 "),
            DateTime.Parse("1/1/1111 06:00:00 AM")));
    }
    
    
    [Authorize]
    [HttpGet]
    [Route("{user}/{repoName}/CommitsByDate")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommitsByDate))]
    public async Task<IActionResult> CommitsByDate(string user, string repoName)
    {
        var response = await AddOrUpdateLocalRepoData(user, repoName);
        if (response is BadRequestObjectResult)
        {
            return BadRequest();
        }
        
        var relPath = $"{GetRelativeSavedRepositoriesFolder()}/{user}/{repoName}";
        var analysis = new Analysis(_context);
        var (repo, _) = await _repoInsightRepository.FindRepositoryByPathAsync(relPath);

        return Ok(await analysis.GetCommitsByDate(repo!.Id));
    }


    [HttpGet]
    [Route("{user}/{repoName}/forks")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ForkDto>))]
    public async Task<IActionResult> GetForks(string user, string repoName)
    {
        var forkApi = new ForkApi();
        var forks = await forkApi.GetForks($"{user}/{repoName}");
        return Ok(forks);
    }

}
