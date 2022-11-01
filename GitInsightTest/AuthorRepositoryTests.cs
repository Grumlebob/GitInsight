using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class AuthorRepositoryTests
{
    
    private readonly InsightContext _context;
    
    public AuthorRepositoryTests()
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
    
    
}