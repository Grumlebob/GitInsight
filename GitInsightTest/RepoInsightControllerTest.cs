using GitInsight.Controllers;
using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Commit = GitInsight.Entities.CommitInsight;
using Repository = GitInsight.Entities.RepoInsight;

namespace GitInsightTest;

public class RepoInsightControllerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly RepoInsightsController _repoInsightController;

    public RepoInsightControllerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _repoInsightController = new RepoInsightsController(_context);
        //Base testing repository
        var testRepo = new Repository()
        {
            Id = 1,
            Name = "First Repo",
            Path = "First RepoPath",
        };
        _context.Repositories.Add(testRepo);
        _context.SaveChanges();
        //Base testing branch
        var testBranch = new GitInsight.Entities.Branch()
        {
            Id = 1,
            Name = "First Branch",
            Path = "First BranchPath",
            Repository = testRepo,
            RepositoryId = 1,
        };
        _context.Branches.Add(testBranch);
        _context.SaveChanges();
        //Base testing author 1
        var sampleAuthorOne = new Author
        {
            Id = 1,
            Name = "First Author",
            Email = "First Email",
            Repositories = { testRepo },
            Commits =
            {
                new Commit
                {
                    Sha = "First Commit",
                    Date = DateTime.Now,
                    Branch = testBranch,
                    Repository = testRepo,
                    RepositoryId = 1,
                },
            },
        };
        _context.Add(sampleAuthorOne);
        _context.SaveChanges();
        //Base testing author 2
        var sampleAuthorTwo = new Author
        {
            Id = 2,
            Name = "Second Author",
            Email = "Second Email",
            Repositories = { testRepo },
            Commits =
            {
                new Commit
                {
                    Sha = "Second Commit",
                    Date = DateTime.Now,
                    Branch = testBranch,
                    Repository = testRepo,
                    RepositoryId = 1,
                },
            },
        };
        _context.Add(sampleAuthorTwo);
        _context.SaveChanges();
        
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
    
}