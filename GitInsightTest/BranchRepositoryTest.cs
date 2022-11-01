using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class BranchRepositoryTest
{
    
    private readonly InsightContext _context;
    
    public BranchRepositoryTest()
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
        Branch a = new Branch();
        a.Id = 1; 
        a.Name = "Joe"; 
    }
}