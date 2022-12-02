namespace GitInsightTest;

public class CommitsControllerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly CommitsController _commitsController;

    
    public CommitsControllerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _commitsController = new CommitsController(_context);
        //Base testing repository
        var testRepo = new Repository
        {
            Id = 1,
            Name = "First Repo",
            Path = "First RepoPath",
        };
        _context.Repositories.Add(testRepo);
        _context.SaveChanges();
        //Base testing branch
        var testBranch = new Branch
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
        };
    
        _context.Add(sampleAuthorOne);
        _context.SaveChanges();



        var sampleCommit1 = new Commit
        {
            Sha = "first Commit",
            Date = DateTime.Now,
            Branch = testBranch,
            Repository = testRepo,
            RepositoryId = 1,
            AuthorId = 1
        };
        
        var sampleCommit2 = new Commit
        {
            Sha = "Second Commit",
            Date = DateTime.Now,
            Branch = testBranch,
            Repository = testRepo,
            RepositoryId = 1,
            AuthorId = 1
        };
        
        
        var sampleCommit3 = new Commit
        {
            Sha = "Third Commit",
            Date = DateTime.Now,
            Branch = testBranch,
            Repository = testRepo,
            RepositoryId = 1,
            AuthorId = 1
        };
        
        _context.Add(sampleCommit1);
        _context.Add(sampleCommit2);
        _context.Add(sampleCommit3);
        _context.SaveChanges();

    }

    [Fact]
    public async Task GetAllCommitsReturnsOk()
    {
        var commits = await _commitsController.GetAllCommits();
        commits.Should().BeAssignableTo<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAllCommitsReturnsNotFound()
    {
        await _context.Commits.ExecuteDeleteAsync();
        var commits = await _commitsController.GetAllCommits();
        commits.Should().BeAssignableTo<NotFoundResult>();
    }
    
    
    
    [Fact]
    public async Task GetCommitByIdReturnsOk()
    {
        var commits = await _commitsController.GetCommitById(1);
        commits.Should().BeAssignableTo<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAuthorByIdReturnsNotFound()
    {
        await _context.Commits.ExecuteDeleteAsync();
        var commits = await _commitsController.GetCommitById(10);
        commits.Should().BeAssignableTo<NotFoundResult>();
    }


    public void Dispose()
    {
        _connection.Dispose();
    }
    
}