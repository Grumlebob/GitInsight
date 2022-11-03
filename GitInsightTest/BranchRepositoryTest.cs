using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Branch = GitInsight.Entities.Branch;
using Repository = GitInsight.Entities.Repository;

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
        var newBranch = new BranchCreateDto("cool", "1234567890098curtains345678900987654321!", 3, "head/cool");
        var (response, result) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.BadRequest);
        result.Path.Should().Be("No repository found with id: 3");
        (await _context.Branches.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task Create_Branch_Success()
    {
        var newBranch = new BranchCreateDto("cooler", "09987654321098selfReplicas765432109887654321", 1, "main/");
        var (response, result) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.Created);
        result.Id.Should().Be(1);
        result.Name.Should().Be("cooler");
        (await _context.Branches.FindAsync(1))!.Name.Should().Be("cooler");
    }

    [Fact]
    public async Task Create_Branch_Sha_Already_Exists()
    {
        var conflictBranch = new BranchCreateDto("I came first", "694206942069420TOOMuch?420694206942069420", 1, "no");
        await _repo.CreateAsync(conflictBranch);
        var newBranch = new BranchCreateDto(null, "694206942069420TOOMuch?420694206942069420", 1, "yes");
        var (response, result) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.Conflict);
        result.Name.Should().Be("I came first");
        (await _context.Branches.FindAsync(2)).Should().BeNull();
    }

    [Fact]
    public async Task Create_Branch_Path_Already_Exists()
    {
        var conflictBranch =
            new BranchCreateDto("I came before", "69420694206942CannotStop69420694206942069420", 1, "main/no");
        await _repo.CreateAsync(conflictBranch);
        var newBranch = new BranchCreateDto("aww man", "123456789012345678MeHeeHee2345678901234567890", 1, "main/no");
        var (response, result) = await _repo.CreateAsync(newBranch);
        response.Should().Be(Response.Conflict);
        result.Name.Should().Be("I came before");
        (await _context.Branches.FindAsync(2)).Should().BeNull();
    }

    [Fact]
    public async Task Find_Branch_Not_Exist()
    {
        (await _repo.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task Find_Branch_Success()
    {
        var newBranch = new BranchCreateDto("me", "ABC4EFGHIJKLMNOPQrstuvwxyzABCDEFGHIJKLMNOPQrstuvwxyz", 1, "69");
        await _repo.CreateAsync(newBranch);
        (await _repo.FindAsync(1)).Name.Should().Be("me");
    }

    [Fact]
    public async Task FindAll_Branches_By_Repo()
    {
        await _repo.CreateAsync(new BranchCreateDto("1", "LongDefaultStringInHereTillWeHit40Characters", 1, "ok/then"));
        await _repo.CreateAsync(new BranchCreateDto("2", "LongDefaultStringInHereUntilWeHit40Characters", 1,
            "yeah/then"));
        await _repo.CreateAsync(new BranchCreateDto("3", "VeryLongDefaultStringInHereUntilWeHit40Characters", 2,
            "yeah/then"));
        (await _repo.FindAllAsync(1)).Count.Should().Be(2);
    }

    [Fact]
    public async Task FindAll_Branches_In_Database()
    {
        await _repo.CreateAsync(
            new BranchCreateDto("1st", "LongDefaultStringInHereTillWeHit40Characters", 1, "ok/then"));
        await _repo.CreateAsync(new BranchCreateDto("2nd", "LongDefaultStringInHereUntilWeHit40Characters", 1,
            "yeah/then"));
        await _context.Repositories.AddAsync(new Repository() { Path = "69" });
        await _context.SaveChangesAsync();
        await _repo.CreateAsync(new BranchCreateDto("3rd", "VeryLongDefaultStringInHereUntilWeHit40Characters", 2,
            "yeah/then"));
        (await _repo.FindAllAsync()).Count.Should().Be(3);
    }

    [Fact]
    public async Task Delete_Branch_Success()
    {
        await _repo.CreateAsync(new BranchCreateDto("heh", "aWholeLotOfCharactersWithCapsSoIt'sEasyToRead", 1, "nah"));
        (await _repo.FindAsync(1)).Should().NotBeNull();
        (await _repo.DeleteAsync(1)).Should().Be(Response.Deleted);
        (await _repo.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task Delete_Not_Found()
    {
        (await _repo.DeleteAsync(1)).Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task Update_Branch_Repository_Id_Unknown_Repo()
    {
        await _repo.CreateAsync(new BranchCreateDto("pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 1, "ok"));
        var response =
            await _repo.UpdateAsync(new BranchDto(1, "pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 2, "ok"));
        response.Should().Be(Response.BadRequest);
    }

    [Fact]
    public async Task Update_Branch_Success()
    {
        await _context.Repositories.AddAsync(new Repository() { Path = "silly" });
        await _context.SaveChangesAsync();
        await _repo.CreateAsync(new BranchCreateDto("pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 1, "notOk"));
        var response =
            await _repo.UpdateAsync(new BranchDto(1, "Better", "%q%w%er#symbols_better_stuff_idk-moving0on", 2, "ok"));
        var updated = await _repo.FindAsync(1);
        response.Should().Be(Response.Ok);
        updated.Name.Should().Be("Better");
        updated.Sha.Should().Be("%q%w%er#symbols_better_stuff_idk-moving0on");
        updated.RepositoryId.Should().Be(2);
        updated.Path.Should().Be("ok");
    }

    [Fact]
    public async Task Update_Branch_Sha_Conflict()
    {
        await _repo.CreateAsync(new BranchCreateDto(null, "ICameBeforeYouHaHaHaHaHaHaHaHaHaHaHaHaHa", 1, "Rect"));
        await _repo.CreateAsync(new BranchCreateDto(null, "IWannaGoFirstManSobSobSobSobSobSobSobSob", 1, "Rect_d"));
        var response =
            await _repo.UpdateAsync(new BranchDto(2, null, "ICameBeforeYouHaHaHaHaHaHaHaHaHaHaHaHaHa", 1, "Rect_d"));
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public async Task Update_Branch_Path_Conflict()
    {
        await _context.Repositories.AddAsync(new Repository() { Path = "Interview" });
        await _repo.CreateAsync(new BranchCreateDto(null, "ImJustMindingMyOwnBusinessThenSuddenlyBoom", 1,
            "Personal/Space"));
        await _repo.CreateAsync(new BranchCreateDto("Twin", "ImAboutToInvadeYourPersonalSpaceHaHaHaHa", 2,
            "Personal/Space"));
        var response =
            await _repo.UpdateAsync(new BranchDto(2, "Twin", "ImAboutToInvadeYourPersonalSpaceHaHaHaHa", 1,
                "Personal/Space"));
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public async Task Update_Branch_Not_Found()
    {
        (await _repo.UpdateAsync(new BranchDto(1, null, "whatEverICanThinkOfThatMakesAStringLonger", 1, "uhm")))
            .Should()
            .Be(Response.NotFound);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}