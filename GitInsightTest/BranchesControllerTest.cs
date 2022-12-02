namespace GitInsightTest;

public class BranchesControllerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly BranchesController _branchesController;

    public BranchesControllerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _branchesController = new BranchesController(_context);
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
        _context.SaveChanges();
        //Base testing branch 1
        var sampleBranchOne = new Branch
        {
            Id = 1,
            Name = "First Branch",
            Repository = testRepo,
            RepositoryId = 1,

        };
        _context.Add(sampleBranchOne);
        _context.SaveChanges();
        //Base testing branch 2
        var sampleBranchTwo = new Branch
        {
            Id = 2,
            Name = "Second Branch",
            Repository = testRepo,
            RepositoryId = 1,
        };
        _context.Add(sampleBranchTwo);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllBranchesReturnsOk()
    {
        var branches = await _branchesController.GetAllBranches();
        branches.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public async Task GetAllBranchesReturnsNotFound()
    {
        await _context.Branches.ExecuteDeleteAsync();
        var branches = await _branchesController.GetAllBranches();
        branches.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task GetBranchByIdReturnsOk()
    {
        var branches = await _branchesController.GetBranchById(1);
        branches.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public async Task GetBranchByIdReturnsNotFound()
    {
        await _context.Branches.ExecuteDeleteAsync();
        var branches = await _branchesController.GetBranchById(10);
        branches.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task CreateBranchReturnsCreated()
    {
        var branch = new Branch
        {
            Id = 3,
            Name = "Third Branch",
            RepositoryId = 1,
            Path = "Third Path",
        };
        var branches =
            await _branchesController.CreateBranch(new BranchCreateDto(branch.Name, branch.RepositoryId, branch.Path));
        branches.Should().BeAssignableTo<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateBranchReturnsBadRequest()
    {
        var branches = await _branchesController.CreateBranch(
            new BranchCreateDto("trying-to-fix-things", 10, "Wrong path"));
        branches.Should().BeAssignableTo<BadRequestResult>();
    }

    [Fact]
    public async Task CreateBranchReturnsConflict()
    {
        var branch = new Branch
        {
            Id = 3,
            Name = "Third-Branch",
            RepositoryId = 1,
            Path = "Third-Path",
        };
        var firstCreate =
            await _branchesController.CreateBranch(
                new BranchCreateDto(branch.Name, branch.RepositoryId, branch.Path));
        firstCreate.Should().BeAssignableTo<CreatedAtActionResult>();

        var sameBranch = new Branch
        {
            Id = 3,
            Name = "Third-Branch",
            RepositoryId = 1,
            Path = "Third-Path",
        };
        var secondCreate =
            await _branchesController.CreateBranch(
                new BranchCreateDto(sameBranch.Name, sameBranch.RepositoryId, sameBranch.Path));
        secondCreate.Should().BeAssignableTo<ConflictObjectResult>();
    }

    [Fact]
    public async Task UpdateBranchReturnsOk()
    {
        var branch = new Branch
        {
            Id = 3,
            Name = "Third Branch",
            RepositoryId = 1,
            Path = "Third Path",
        };
        var branches =
            await _branchesController.CreateBranch(
                new BranchCreateDto(branch.Name, branch.RepositoryId, branch.Path));
        branches.Should().BeAssignableTo<CreatedAtActionResult>();

        var updatedBranch = new Branch
        {
            Id = 3,
            Name = "I am updated",
            RepositoryId = 1,
            Path = "Third Path",
        };
        var updatedBranches = await _branchesController.UpdateBranch(
            new BranchDto(updatedBranch.Id, updatedBranch.Name, updatedBranch.RepositoryId, updatedBranch.Path));
        updatedBranches.Should().BeAssignableTo<OkResult>();
    }

    [Fact]
    public async Task UpdateBranchReturnsNotFound()
    {
        var branch = new Branch
        {
            Id = 999,
            Name = "Id does not exist",
            RepositoryId = 1,
            Path = "Unknown Path",
        };
        var updatedBranches =
            await _branchesController.UpdateBranch(
                new BranchDto(branch.Id, branch.Name, branch.RepositoryId, branch.Path));
        updatedBranches.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteBranchByIdReturnsNoContent()
    {
        var updatedBranches = await _branchesController.DeleteBranch(1);
        updatedBranches.Should().BeAssignableTo<NoContentResult>();
    }

    [Fact]
    public async Task DeleteBranchByIdReturnsNotFound()
    {
        var updatedBranches = await _branchesController.DeleteBranch(10);
        updatedBranches.Should().BeAssignableTo<NotFoundResult>();
    }


    public void Dispose()
    {
        _connection.Dispose();
    }
}
