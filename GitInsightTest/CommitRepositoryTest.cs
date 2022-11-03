using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class CommitRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;

    public CommitRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
    }

    [Fact]
    public void SimpleCommitObject()
    {
        GitInsight.Entities.Commit a = new GitInsight.Entities.Commit();
        a.Id = 1;
        a.Sha = "wafwa";
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}