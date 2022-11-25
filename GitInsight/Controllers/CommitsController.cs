namespace GitInsight.Controllers;

[ApiController]
//[Authorize]
[Route("[controller]")]
public class CommitsController : ControllerBase

{
    private readonly ICommitInsightRepository _commitRepository;

    public CommitsController(InsightContext context)
    {
        _commitRepository = new CommitInsightRepository(context);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommitInsight>))]
    public async Task<IActionResult> GetAllCommits()
    {
        var (list, response) = await _commitRepository.FindAllAsync();
        if (response == Core.Response.Ok) return Ok(list);
        return NotFound();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Commit))]
    public async Task<IActionResult> GetCommitById(int id)
    {
        var (commitDto, response) = await _commitRepository.FindAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok(commitDto);
    }

}