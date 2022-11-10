using GitInsight.Controllers;
using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Commit = GitInsight.Entities.CommitInsight;
using Repository = GitInsight.Entities.RepoInsight;

namespace GitInsightTest;

public class AuthorsControllerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly AuthorsController _authorsController;

    public AuthorsControllerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _authorsController = new AuthorsController(_context);
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

    [Fact]
    public async Task GetAllAuthorsReturnsOk()
    {
        var authors = await _authorsController.GetAllAuthors();
        authors.Should().BeAssignableTo<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAllAuthorsReturnsNotFound()
    {
        await _context.Authors.ExecuteDeleteAsync();
        var authors = await _authorsController.GetAllAuthors();
        authors.Should().BeAssignableTo<NotFoundResult>();
    }
    
    [Fact]
    public async Task GetAuthorByIdReturnsOk()
    {
        var authors = await _authorsController.GetAuthorById(1);
        authors.Should().BeAssignableTo<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetAuthorByIdReturnsNotFound()
    {
        await _context.Authors.ExecuteDeleteAsync();
        var authors = await _authorsController.GetAuthorById(10);
        authors.Should().BeAssignableTo<NotFoundResult>();
    }
    
    [Fact]
    public async Task CreateAuthorReturnsCreated()
    {
        var author = new Author
        {
            Id = 3,
            Name = "Third Author",
            Email = "Third Email",
        };
        var authors = await _authorsController.CreateAuthor(new AuthorCreateDto(author.Name, author.Email, new List<int>(), new List<int>(){1}));
        authors.Should().BeAssignableTo<CreatedAtActionResult>();
    }
    
    [Fact]
    public async Task CreateAuthorReturnsBadRequest()
    {
        var authors = await _authorsController.CreateAuthor(new AuthorCreateDto("Wrong repo",
            "Wrong repo", new List<int>() { }, new List<int>() { 10 }));
        authors.Should().BeAssignableTo<BadRequestResult>();
    }
    
    [Fact]
    public async Task CreateAuthorReturnsConflict()
    {
        var author = new Author
        {
            Id = 3,
            Name = "Third Author",
            Email = "Third Email",
        };
        var firstCreate = await _authorsController.CreateAuthor(new AuthorCreateDto(author.Name, author.Email, new List<int>(), new List<int>(){1}));
        firstCreate.Should().BeAssignableTo<CreatedAtActionResult>();
        
        var sameAuthor = new Author
        {
            Id = 3,
            Name = "Third Author",
            Email = "Third Email",
        };
        var secondCreate = await _authorsController.CreateAuthor(new AuthorCreateDto(sameAuthor.Name, sameAuthor.Email, new List<int>(), new List<int>(){1}));
        secondCreate.Should().BeAssignableTo<ConflictObjectResult>();
    }
    
    [Fact]
    public async Task UpdateAuthorReturnsOk()
    {
        var author = new Author
        {
            Id = 3,
            Name = "Third Author",
            Email = "Third Email",
        };
        var authors = await _authorsController.CreateAuthor(new AuthorCreateDto(author.Name, author.Email, new List<int>(), new List<int>(){1}));
        authors.Should().BeAssignableTo<CreatedAtActionResult>();
        
        var updatedAuthor = new Author
        {
            Id = 3,
            Name = "I am updated",
            Email = "I am updated",
        };
        var updatedAuthors = await _authorsController.UpdateAuthor(new AuthorDto(updatedAuthor.Id, updatedAuthor.Name, updatedAuthor.Email, new List<int>(){}, new List<int>()));
        updatedAuthors.Should().BeAssignableTo<OkResult>();
    }
    
    [Fact]
    public async Task UpdateAuthorReturnsNotFound()
    {
        var author = new Author
        {
            Id = 999,
            Name = "Id does not exist",
            Email = "Id does not exist",
        };
        var updatedAuthors = await _authorsController.UpdateAuthor(new AuthorDto(author.Id, author.Name, author.Email, new List<int>(){}, new List<int>()));
        updatedAuthors.Should().BeAssignableTo<NotFoundResult>();
    }
    
    [Fact]
    public async Task DeleteAuthorByIdReturnsNoContent()
    {
        var updatedAuthors = await _authorsController.DeleteAuthor(1);
        updatedAuthors.Should().BeAssignableTo<NoContentResult>();
    }
    
    [Fact]
    public async Task DeleteAuthorByIdReturnsNotFound()
    {
        var updatedAuthors = await _authorsController.DeleteAuthor(10);
        updatedAuthors.Should().BeAssignableTo<NotFoundResult>();
    }
    
    
    public void Dispose()
    {
        _connection.Dispose();
    }
    
}