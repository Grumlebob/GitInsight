using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GitInsight.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase

{
    private readonly AuthorRepository _authorRepository;

    public AuthorsController(InsightContext context)
    {
        _authorRepository = new AuthorRepository(context);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuthors()
    {
        var (list, response) = await _authorRepository.FindAllAuthorsAsync();
        if (response == Core.Response.Ok) return Ok(list);
        return NotFound();
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        var (authorDto, response) = await _authorRepository.FindAuthorAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok(authorDto);
    }
    
}