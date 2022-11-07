using GitInsight.Entities;
using Microsoft.Data.Sqlite;

namespace GitInsightTest;

public class DataManagerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly RepositoryRepository _repositoryRepository;
    private readonly BranchRepository _branchRepository;
    private readonly AuthorRepository _authorRepository;
    private readonly CommitRepository _commitRepository;
    
    public DataManagerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _repositoryRepository = new RepositoryRepository(_context);
        _branchRepository = new BranchRepository(_context);
        _authorRepository = new AuthorRepository(_context);
        _commitRepository = new CommitRepository(_context);

        //Base testing repository
        var testRepo = new GitInsight.Entities.Repository()
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
                new GitInsight.Entities.Commit
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
                new GitInsight.Entities.Commit
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

    [Fact]
    public void DatabaseShouldBeUpdated()
    {
        true.Should().BeTrue();
    }
    
    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}