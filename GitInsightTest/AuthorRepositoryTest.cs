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

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}