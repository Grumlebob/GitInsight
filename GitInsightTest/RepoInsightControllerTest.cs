using GitInsight;
using GitInsight.Controllers;
using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;
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
        
        var repo = new RepoInsight { Name = "repo1", Path = $"{GetRelativeSavedRepositoriesFolder()}/idk/idk" };
        _context.Repositories.Add(repo);
        _context.SaveChanges();

        _context.Authors.Add(new Author { Name = "Søren", Email = "søren@gmail.dk", Repositories = {repo}});
        _context.Authors.Add(new Author { Name = "Per", Email = "per@gmail.dk", Repositories = {repo}});
        _context.SaveChanges();

        

        _context.Branches.Add(new Branch { Name = "branch1", Path = "origin/idk", RepositoryId = 1 });
        _context.SaveChanges();

        _context.Commits.Add(new GitInsight.Entities.CommitInsight { 
            Id = 1, 
            Sha = "treg", 
            AuthorId = 1, 
            BranchId = 1, 
            RepositoryId = 1, 
            Date = new DateTime(2022,10,13,0,33,0)
            
        });
        
        _context.Commits.Add(new GitInsight.Entities.CommitInsight
        {
            Id = 2,
            Sha = "heck", 
            AuthorId = 2,
            BranchId = 1, 
            RepositoryId = 1, 
            Date = new DateTime(2022,10,13,8,45,0)
        });
        
        _context.Commits.Add(new GitInsight.Entities.CommitInsight
        {
            Id = 3, Sha = "tger", 
            AuthorId = 1, 
            BranchId = 1, 
            RepositoryId = 1, 
            Date = new DateTime(2022,11,15,4,0,0)
        });
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
        var result = await _repoInsightController.GetRepoById(1);
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as RepoInsightDto;
        content!.Name.Should().Be("repo1");
    }

    [Fact]
    public async Task Get_Repo_By_Id_Not_Found()
    {
        var result = await _repoInsightController.GetRepoById(2);
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Get_All_Repo_Success()
    {
        var result = await _repoInsightController.GetAllRepositories();
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as List<RepoInsightDto>;
        content!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Get_Repo_None_Found()
    {
        await _context.Repositories.ExecuteDeleteAsync();
        var result = await _repoInsightController.GetAllRepositories();
        result.Should().BeAssignableTo<NotFoundResult>();
    }
    
    [Fact]
    public async Task NightOwl_Return_correct_result()
    {
        var result = await _repoInsightController.NightOwl("Grumlebob","GitInsight");
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as GitAwardWinner;
        content!.Should().BeEquivalentTo(await new Analysis(_context).HighestCommitterWithinTimeframe(2,
            DateTime.Parse("1/1/1111 0:00:00 "),
            DateTime.Parse("1/1/1111 06:00:00 AM")));
    }
    
    [Fact]
    public async Task NightOwl_Return_Not_Found()
    {
        var result = await _repoInsightController.NightOwl("Grumlebob","NotExists");
        result.Should().BeAssignableTo<BadRequestResult>();
    }
    
    [Fact]
    public async Task EarlyBird_Return_correct_result()
    {
        var result = await _repoInsightController.EarlyBird("Grumlebob","GitInsight");
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as GitAwardWinner;
        content!.Should().BeEquivalentTo(await new Analysis(_context).HighestCommitterWithinTimeframe(2,
            DateTime.Parse("1/1/1111 06:00:00 "),
            DateTime.Parse("1/1/1111 10:00:00 AM")));
    }
    
    [Fact]
    public async Task EarlyBird_Return_Not_Found()
    {
        var result = await _repoInsightController.EarlyBird("Grumlebob","NotExists");
        result.Should().BeAssignableTo<BadRequestResult>();
    }
    
    [Fact]
    public async Task CommitsByDate_Return_correct_result()
    {
        var result = await _repoInsightController.CommitsByDate("Grumlebob","GitInsight");
        result.Should().BeAssignableTo<OkObjectResult>();
        var content = (result as OkObjectResult)!.Value as List<CommitsByDate>;
        content!.Should().BeEquivalentTo(await new Analysis(_context).GetCommitsByDate(2));
    }
    
    [Fact]
    public async Task CommitsByDate_Return_Not_Found()
    {
        var result = await _repoInsightController.CommitsByDate("Grumlebob","NotExists");
        result.Should().BeAssignableTo<BadRequestResult>();
    }
    
    
    

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}