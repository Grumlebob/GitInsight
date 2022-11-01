using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class RepositoryRepositoryTest
{
    
    private readonly InsightContext _context;
    
    public RepositoryRepositoryTest()
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
        GitInsight.Entities.Repository a = new GitInsight.Entities.Repository();
        a.Id = 1;
        a.Name = "Repository,repo of the repos.";
    }
}