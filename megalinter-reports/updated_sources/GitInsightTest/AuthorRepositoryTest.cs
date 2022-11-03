using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class AuthorRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;

    public AuthorRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();

        var sampleAuthorOne = new Author
        {
            Name = "First Author",
            Email = "First Email",
        };

        var sampleAuthorTwo = new Author
        {
            Name = "Second Author",
            Email = "Second Email",
        };

        _context.AddRange(sampleAuthorOne, sampleAuthorTwo);
        _context.SaveChanges();
    }

    [Fact]
    public void ContextShouldNotBeEmpty()
    {
        var allAuthors = _context.Authors;
        allAuthors.Should().NotBeEmpty();
    }

    [Fact]
    public void SimpleAuthorObject()
    {
        Author a = new Author();
        a.Email = "a";
        a.Id = 1;
        a.Name = "Joe";
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}