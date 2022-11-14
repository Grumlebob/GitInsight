// using GitInsight.Core;
// using GitInsight.Entities;
// using Microsoft.AspNetCore.Mvc;

// namespace GitInsight.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class BranchController : ControllerBase

// {
//     private readonly IBranchRepository _branchRepository;

//     public BranchController(InsightContext context)
//     {
//         _branchRepository = new BranchRepository(context);
//     }

//     [HttpGet]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Entities.Branch>))]
//     public async Task<IActionResult> GetAllBranches()
//     {
//         var list = await _branchRepository.FindAllAsync();
//         if (response == Core.Response.Ok) return Ok(list);
//         return NotFound();
//     }

//     [HttpGet]
//     [Route("{id}")]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
//     public async Task<IActionResult> GetAuthorById(int id)
//     {
//         var (authorDto, response) = await _branchRepository.FindAsync(id);
//         if (response == Core.Response.NotFound) return NotFound();
//         return Ok(authorDto);
//     }

//     [HttpPost]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(StatusCodes.Status409Conflict)]
//     [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
//     public async Task<IActionResult> CreateAuthor(AuthorCreateDto author)
//     {
//         var (authorDto, response) = await _branchRepository.CreateAsync(author);
//         if (response == Core.Response.BadRequest) return BadRequest();
//         if (response == Core.Response.Conflict) return Conflict(authorDto);
//         if (response == Core.Response.Created) return CreatedAtAction(nameof(GetAuthorById), new { id = authorDto.Id }, authorDto);
//         return Ok(authorDto);
//     }

//     [HttpPut]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     public async Task<IActionResult> UpdateAuthor(AuthorDto author)
//     {
//         var response = await _branchRepository.UpdateAsync(author);
//         if (response == Core.Response.NotFound) return NotFound();
//         return Ok();
//     }

//     [HttpDelete]
//     [Route("{id}")]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     public async Task<IActionResult> DeleteAuthor(int id)
//     {
//         var response = await _branchRepository.DeleteAsync(id);
//         if (response == Core.Response.NotFound) return NotFound();
//         return NoContent();
//     }


// }