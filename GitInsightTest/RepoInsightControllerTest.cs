using GitInsight;
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
    }

    [Fact]
    public async Task Get_Repo_From_Url_Success()
    {
        var result = await _repoInsightController.AddOrUpdateLocalRepoData("Grumlebob", "GitInsight");
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as List<CommitsByDateByAuthor>;
        content!.Count.Should().BePositive();
    }

    [Fact]
    public async Task Get_Repo_From_Url_Invalid_Url()
    {
        var result = await _repoInsightController.AddOrUpdateLocalRepoData("NonExisting", "notExist");
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Get_Repo_From_Url_Existing_User_NonExisting_Repo()
    {
        var control = await _repoInsightController.AddOrUpdateLocalRepoData("Grumlebob", "GitInsight");
        control.Should().BeAssignableTo<OkObjectResult>();
        var result = await _repoInsightController.AddOrUpdateLocalRepoData("Grumlebob", "notExist");
        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Get_Repo_From_Url_Updates_Existing()
    {
        var control = await _repoInsightController.AddOrUpdateLocalRepoData("Grumlebob", "GitInsight");
        control.Should().BeAssignableTo<OkObjectResult>();
        var result = await _repoInsightController.AddOrUpdateLocalRepoData("Grumlebob", "GitInsight");
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as List<CommitsByDateByAuthor>;
        content!.Count.Should().BePositive();
    }

    [Fact]
    public async Task Get_Repo_By_Id_Success()
    {
        var testRepo = new RepoInsightCreateDto("pathy", "namy", null!, null!, null!);
        await new RepoInsightRepository(_context).CreateAsync(testRepo);
        
        var result = await _repoInsightController.GetRepoById(1);
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as RepoInsightDto;
        content!.Name.Should().Be("namy");
    }

    [Fact]
    public async Task Get_Repo_By_Id_Not_Found()
    {
        var result = await _repoInsightController.GetRepoById(1);
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Get_All_Repo_Success()
    {
        var testRepo = new RepoInsightCreateDto("pathy", "namy", null!, null!, null!);
        await new RepoInsightRepository(_context).CreateAsync(testRepo);
        
        var result = await _repoInsightController.GetAllRepositories();
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as List<RepoInsightDto>;
        content!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Get_Repo_None_Found()
    {
        var result = await _repoInsightController.GetAllRepositories();
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}