using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GitInsight.Controllers;

[ApiController]
[Route("[controller]")]
public class BranchController : ControllerBase

{
    private readonly IBranchRepository _branchRepository;

    public BranchController(InsightContext context)
    {
        _branchRepository = new BranchRepository(context);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Entities.Branch>))]
    public async Task<IActionResult> GetAllBranches()
    {
        var (list, response) = await _branchRepository.FindAllAsync();
        if (response == Core.Response.Ok) return Ok(list);
        return NotFound();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Entities.Branch))]
    public async Task<IActionResult> GetBranchById(int id)
    {
        var (branchDto, response) = await _branchRepository.FindAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok(branchDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Entities.Branch))]
    public async Task<IActionResult> CreateBranch(BranchCreateDto branch)
    {
        var (branchDto, response) = await _branchRepository.CreateAsync(branch);
        if (response == Core.Response.BadRequest) return BadRequest();
        if (response == Core.Response.Conflict) return Conflict(branchDto);
        if (response == Core.Response.Created) return CreatedAtAction(nameof(GetBranchById), new { id = branchDto.Id }, branchDto);
        return Ok(branchDto);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBranch(BranchDto branch)
    {
        var response = await _branchRepository.UpdateAsync(branch);
        if (response == Core.Response.NotFound) return NotFound();
        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBranch(int id)
    {
        var response = await _branchRepository.DeleteAsync(id);
        if (response == Core.Response.NotFound) return NotFound();
        return NoContent();
    }


}