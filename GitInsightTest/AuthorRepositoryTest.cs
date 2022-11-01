using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class AuthorRepositoryTest
{
    
    private readonly InsightContext _context;
    
    public AuthorRepositoryTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<InsightContext>();
        builder.UseSqlite(connection);
        var context = new InsightContext(builder.Options);
        context.Database.EnsureCreated();
        context.SaveChanges();

        _context = context;
        
    }
    [Fact]
    public void HowDoesItHandle()
    {
        var allAuthors = _context.Authors;

        allAuthors.Should().BeEmpty();
    }
    [Fact]
    public void HowDoesItHandleIt()
    {
        Author a = new Author();
        a.Email = "a"; 
        a.Id = 1; 
        a.Name = "Joe"; 
    }
}