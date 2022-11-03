using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class AuthorRepository : IAuthorRepository
{
    private readonly InsightContext _context;

    public AuthorRepository(InsightContext context)
    {
        _context = context;
    }

    public async Task<(AuthorDto?, Response)> FindAuthorAsync(int id)
    {
        var response = new Response();
        
        var author = await _context.Authors
            .Include(a => a.Commits)
            .FirstOrDefaultAsync(a =>  a.Id == id);

        if (author is null)
        {
            response = Response.NotFound;
            return (null, response);
        }
        else
        {
            response = Response.Ok;
            return (new AuthorDto(Id: author.Id, Name: author.Name, Email: author.Email), response);
        }
    }

    public record AuthorDto(int Id, string Name, string Email);
}