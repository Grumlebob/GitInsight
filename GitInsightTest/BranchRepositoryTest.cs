using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class BranchRepositoryTest
{
    private readonly InsightContext _context;
    private readonly BranchRepository _repo;

    public BranchRepositoryTest()
    {
        _context = SetupTests.Setup();
        _repo = new BranchRepository(_context);
    }

    [Fact]
    public void Create_Branch_Unknown_Repository()
    {
        var newBranch = new BranchCreateDto("cool", "1234567890098765432112345678900987654321", 3, "head/cool");
        var result = _repo.Create(newBranch);
        result.Should().Be((Response.Created))
    }
}