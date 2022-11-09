using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GitInsight.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase

{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(InsightContext context)
    {
        _authorRepository = new AuthorRepository(context);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Author>))]
    public async Task<IActionResult> GetAllAuthors()
    {
        var (list, response) = await _authorRepository.FindAllAsync();
        if (response == Core.Response.Ok) return Ok(list);
        return NotFound();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        var (authorDto, response) = await _authorRepository.FindAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok(authorDto);
    }

}