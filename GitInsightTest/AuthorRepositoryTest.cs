using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class AuthorRepositoryTest
{
    
    private readonly InsightContext _context;
    
    public AuthorRepositoryTest()
    {
        _context = SetupTests.Setup();
    }
    
    [Fact]
    public void ContextShouldBeEmpty()
    {
        var allAuthors = _context.Authors;

        allAuthors.Should().BeEmpty();
    }
    [Fact]
    public void SimpleAuthorObject()
    {
        Author a = new Author();
        a.Email = "a"; 
        a.Id = 1; 
        a.Name = "Joe"; 
    }
}