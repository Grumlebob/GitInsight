﻿namespace GitInsight.Controllers;

[ApiController]
//[Authorize]
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    public async Task<IActionResult> CreateAuthor(AuthorCreateDto author)
    {
        var (authorDto, response) = await _authorRepository.CreateAsync(author);
        if (response == Core.Response.BadRequest) return BadRequest();
        if (response == Core.Response.Conflict) return Conflict(authorDto);
        if (response == Core.Response.Created)
            return CreatedAtAction(nameof(GetAuthorById), new { id = authorDto.Id }, authorDto);
        return Ok(authorDto);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAuthor(AuthorUpdateDto author)
    {
        var response = await _authorRepository.UpdateAsync(author);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var response = await _authorRepository.DeleteAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return NoContent();
    }
}