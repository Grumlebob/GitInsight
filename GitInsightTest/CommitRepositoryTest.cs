using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class CommitRepositoryTest
{
    
    private readonly InsightContext _context;
    
    public CommitRepositoryTest()
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
    public void HowDoesItHandleIt()
    {
        GitInsight.Entities.Commit a = new GitInsight.Entities.Commit();
        a.Id = 1;
        a.Sha = "wafwa";
    }
}