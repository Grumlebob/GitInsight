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
        _context = SetupTests.Setup();
    }
    
    [Fact]
    public void SimpleBranchObject()
    {
        Branch a = new Branch();
        a.Id = 1; 
        a.Name = "Joe"; 
    }
}