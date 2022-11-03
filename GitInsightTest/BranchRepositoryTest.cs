using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
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
        _context.Repositories.Add(repo);
        _context.SaveChanges();
    }

    [Fact]
    public void Create_Branch_Unknown_Repository()
    {
        var newBranch = new BranchCreateDto("cool", "1234567890098curtains345678900987654321!", 3, "head/cool");
        var (response, result) = _repo.Create(newBranch);
        response.Should().Be(Response.BadRequest);
        result.Path.Should().Be("No repository found with id: 3");
        _context.Branches.Find(1).Should().BeNull();
    }

    [Fact]
    public void Create_Branch_Success()
    {
        var newBranch = new BranchCreateDto("cooler", "09987654321098selfReplicas765432109887654321", 1, "main/");
        var (response, result) = _repo.Create(newBranch);
        response.Should().Be(Response.Created);
        result.Id.Should().Be(1);
        result.Name.Should().Be("cooler");
        _context.Branches.Find(1)!.Name.Should().Be("cooler");
    }

    [Fact]
    public void Create_Branch_Sha_Already_Exists()
    {
        var conflictBranch = new BranchCreateDto("I came first", "694206942069420TOOMuch?420694206942069420", 1, "no");
        _repo.Create(conflictBranch);
        var newBranch = new BranchCreateDto(null, "694206942069420TOOMuch?420694206942069420", 1, "yes");
        var (response, result) = _repo.Create(newBranch);
        response.Should().Be(Response.Conflict);
        result.Name.Should().Be("I came first");
        _context.Branches.Find(2).Should().BeNull();
    }

    [Fact]
    public void Create_Branch_Path_Already_Exists()
    {
        var conflictBranch =
            new BranchCreateDto("I came before", "69420694206942CannotStop69420694206942069420", 1, "main/no");
        _repo.Create(conflictBranch);
        var newBranch = new BranchCreateDto("aww man", "123456789012345678MeHeeHee2345678901234567890", 1, "main/no");
        var (response, result) = _repo.Create(newBranch);
        response.Should().Be(Response.Conflict);
        result.Name.Should().Be("I came before");
        _context.Branches.Find(2).Should().BeNull();
    }

    [Fact]
    public void Find_Branch_Not_Exist()
    {
        _repo.Find(1).Should().BeNull();
    }

    [Fact]
    public void Find_Branch_Success()
    {
        var newBranch = new BranchCreateDto("me", "ABC4EFGHIJKLMNOPQrstuvwxyzABCDEFGHIJKLMNOPQrstuvwxyz", 1, "69");
        _repo.Create(newBranch);
        _repo.Find(1).Name.Should().Be("me");
    }

    [Fact]
    public void FindAll_Branches_By_Repo()
    {
        _repo.Create(new BranchCreateDto("1", "LongDefaultStringInHereTillWeHit40Characters", 1, "ok/then"));
        _repo.Create(new BranchCreateDto("2", "LongDefaultStringInHereUntilWeHit40Characters", 1, "yeah/then"));
        _repo.Create(new BranchCreateDto("3", "VeryLongDefaultStringInHereUntilWeHit40Characters", 2, "yeah/then"));
        _repo.FindAll(1).Count.Should().Be(2);
    }

    [Fact]
    public void FindAll_Branches_In_Database()
    {
        _repo.Create(new BranchCreateDto("1st", "LongDefaultStringInHereTillWeHit40Characters", 1, "ok/then"));
        _repo.Create(new BranchCreateDto("2nd", "LongDefaultStringInHereUntilWeHit40Characters", 1, "yeah/then"));
        _context.Repositories.Add(new Repository() { Path = "69" });
        _context.SaveChanges();
        _repo.Create(new BranchCreateDto("3rd", "VeryLongDefaultStringInHereUntilWeHit40Characters", 2, "yeah/then"));
        _repo.FindAll().Count.Should().Be(3);
    }

    [Fact]
    public void Delete_Branch_Success()
    {
        _repo.Create(new BranchCreateDto("heh", "aWholeLotOfCharactersWithCapsSoIt'sEasyToRead", 1, "nah"));
        _repo.Find(1).Should().NotBeNull();
        _repo.Delete(1).Should().Be(Response.Deleted);
        _repo.Find(1).Should().BeNull();
    }

    [Fact]
    public void Delete_Not_Found()
    {
        _repo.Delete(1).Should().Be(Response.NotFound);
    }

    [Fact]
    public void Update_Branch_Repository_Id_Unknown_Repo()
    {
        _repo.Create(new BranchCreateDto("pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 1, "ok"));
        var response = _repo.Update(new BranchDto(1, "pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 2, "ok"));
        response.Should().Be(Response.BadRequest);
    }

    [Fact]
    public void Update_Branch_Success()
    {
        _context.Repositories.Add(new Repository() { Path = "silly" });
        _context.SaveChanges();
        _repo.Create(new BranchCreateDto("pff", "%q%w%er#symbols_weird_stuff_idk-moving0on", 1, "notOk"));
        var response = _repo.Update(new BranchDto(1, "Better", "%q%w%er#symbols_better_stuff_idk-moving0on", 2, "ok"));
        var updated = _repo.Find(1);
        response.Should().Be(Response.Ok);
        updated.Name.Should().Be("Better");
        updated.Sha.Should().Be("%q%w%er#symbols_better_stuff_idk-moving0on");
        updated.RepositoryId.Should().Be(2);
        updated.Path.Should().Be("ok");
    }

    [Fact]
    public void Update_Branch_Sha_Conflict()
    {
        _repo.Create(new BranchCreateDto(null, "ICameBeforeYouHaHaHaHaHaHaHaHaHaHaHaHaHa", 1, "Rect"));
        _repo.Create(new BranchCreateDto(null, "IWannaGoFirstManSobSobSobSobSobSobSobSob", 1, "Rect_d"));
        var response = _repo.Update(new BranchDto(2, null, "ICameBeforeYouHaHaHaHaHaHaHaHaHaHaHaHaHa", 1, "Rect_d"));
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Update_Branch_Path_Conflict()
    {
        _context.Repositories.Add(new Repository() { Path = "Interview" });
        _repo.Create(new BranchCreateDto(null, "ImJustMindingMyOwnBusinessThenSuddenlyBoom", 1, "Personal/Space"));
        _repo.Create(new BranchCreateDto("Twin", "ImAboutToInvadeYourPersonalSpaceHaHaHaHa", 2, "Personal/Space"));
        var response =
            _repo.Update(new BranchDto(2, "Twin", "ImAboutToInvadeYourPersonalSpaceHaHaHaHa", 1, "Personal/Space"));
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Update_Branch_Not_Found()
    {
        _repo.Update(new BranchDto(1, null, "whatEverICanThinkOfThatMakesAStringLonger", 1, "uhm")).Should()
            .Be(Response.NotFound);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}