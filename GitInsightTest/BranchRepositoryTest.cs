using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Branch = GitInsight.Entities.Branch;
using Repository = GitInsight.Entities.RepoInsight;

namespace GitInsightTest;

public class BranchRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly BranchRepository _repo;

    public BranchRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _repo = new BranchRepository(_context);
        var repo = new Repository()
        {
            Path = "www.gitgit/coolest/repo"
        };
        _context.Repositories.AddAsync(repo);
        _context.SaveChangesAsync();
        //100% coverage :/
        var gojleren1 = new Branch { Id = 1 };
        var gojleren2 = gojleren1.Commits;
        gojleren1.Repository = new Repository() { Path = "Mega Gargoyle" };
        var gojleren3 = gojleren1.Repository;
    }

    [Fact]
    public async Task Create_Branch_Unknown_Repository()
    {
        var newBranch = new BranchCreateDto("cool", 3, "head/cool");
        var (result, response) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.BadRequest);
        result.Should().BeNull();
        (await _context.Branches.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task Create_Branch_Success()
    {
        var newBranch = new BranchCreateDto("cooler", 1, "main/");
        var (result, response) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.Created);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("cooler");
        (await _context.Branches.FindAsync(1))!.Name.Should().Be("cooler");
    }

    [Fact]
    public async Task Create_Branch_Path_Already_Exists()
    {
        var conflictBranch =
            new BranchCreateDto("I came before", 1, "main/no");
        await _repo.CreateAsync(conflictBranch);
        var newBranch = new BranchCreateDto("aww man", 1, "main/no");
        var (result, response) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.Conflict);
        result.Should().NotBeNull();
        result!.Name.Should().Be("I came before");
        (await _context.Branches.FindAsync(2)).Should().BeNull();
    }

    [Fact]
    public async Task Find_Branch_Not_Exist()
    {
        var (result, response) = await _repo.FindAsync(1);
        response.Should().Be(Response.NotFound);
        result.Should().BeNull();

    }

    [Fact]
    public async Task Find_Branch_Success()
    {
        var newBranch = new BranchCreateDto("me", 1, "69");
        await _repo.CreateAsync(newBranch);
        var (result, response) = await _repo.FindAsync(1);
        response.Should().Be(Response.Ok);
        result.Should().NotBeNull();
        result!.Name.Should().Be("me");
    }

    [Fact]
    public async Task FindAll_Branches_By_Repo()
    {
        await _repo.CreateAsync(new BranchCreateDto("1", 1, "ok/then"));
        await _repo.CreateAsync(new BranchCreateDto("2", 1, "yeah/then"));
        await _repo.CreateAsync(new BranchCreateDto("3", 2, "yeah/then"));

        var (result, response) = await _repo.FindAllAsync(1);
        response.Should().Be(Response.Ok);
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);

    }

    [Fact]
    public async Task FindAll_Branches_In_Database()
    {
        await _repo.CreateAsync(
            new BranchCreateDto("1st", 1, "ok/then"));
        await _repo.CreateAsync(new BranchCreateDto("2nd", 1,
            "yeah/then"));
        await _context.Repositories.AddAsync(new Repository() { Path = "69" });
        await _context.SaveChangesAsync();
        await _repo.CreateAsync(new BranchCreateDto("3rd", 2,
            "yeah/then"));

        var (result, response) = await _repo.FindAllAsync();
        response.Should().Be(Response.Ok);
        result.Should().NotBeNull();
        result!.Should().HaveCount(3);

    }

    [Fact]
    public async Task Delete_Branch_Success()
    {
        await _repo.CreateAsync(new BranchCreateDto("heh", 1, "nah"));
        (await _repo.FindAsync(1)).Should().NotBeNull();
        (await _repo.DeleteAsync(1)).Should().Be(Response.Deleted);

        var (result, response) = await _repo.FindAsync(1);

        response.Should().Be(Response.NotFound);
        result.Should().BeNull();

    }

    [Fact]
    public async Task Delete_Not_Found()
    {
        (await _repo.DeleteAsync(1)).Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task Update_Branch_Repository_Id_Unknown_Repo()
    {
        await _repo.CreateAsync(new BranchCreateDto("pff", 1, "ok"));
        var response =
            await _repo.UpdateAsync(new BranchDto(1, "pff", 2, "ok"));
        response.Should().Be(Response.BadRequest);
    }

    [Fact]
    public async Task Update_Branch_Success()
    {
        await _context.Repositories.AddAsync(new Repository() { Path = "silly" });
        await _context.SaveChangesAsync();
        await _repo.CreateAsync(new BranchCreateDto("pff", 1, "notOk"));
        var response =
            await _repo.UpdateAsync(new BranchDto(1, "Better", 2, "ok"));

        var (updatedResult, findResponse) = await _repo.FindAsync(1);
        response.Should().Be(Response.Ok);
        findResponse.Should().Be(Response.Ok);
        updatedResult.Should().NotBeNull();
        updatedResult!.Name.Should().Be("Better");
        updatedResult.RepositoryId.Should().Be(2);
        updatedResult.Path.Should().Be("ok");
    }

    [Fact]
    public async Task Update_Branch_Path_Conflict()
    {
        await _context.Repositories.AddAsync(new Repository() { Path = "Interview" });
        await _repo.CreateAsync(new BranchCreateDto(null, 1, "Personal/Space"));
        await _repo.CreateAsync(new BranchCreateDto("Twin", 2, "Personal/Space"));
        var response =
            await _repo.UpdateAsync(new BranchDto(2, "Twin", 1, "Personal/Space"));
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public async Task Update_Branch_Not_Found()
    {
        (await _repo.UpdateAsync(new BranchDto(1, null, 1, "uhm")))
            .Should()
            .Be(Response.NotFound);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
