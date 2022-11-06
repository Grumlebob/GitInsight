using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Commit = GitInsight.Entities.Commit;
using Repository = GitInsight.Entities.Repository;

namespace GitInsightTest;

public class AuthorRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly AuthorRepository _authorRepository;

    public AuthorRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _authorRepository = new AuthorRepository(_context);

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
    public async Task FindAuthorByIdReturnsOk()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorAsync(1);
        authorDto.Should().BeEquivalentTo(new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 },
            new List<int>() { 1 }));
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorByIdReturnsFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorAsync(3);
        authorDto.Should().BeNull();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAllAuthors()
    {
        var (authorDtos, response) = await _authorRepository.FindAllAuthorsAsync();
        authorDtos.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 }, new List<int>() { 1 }),
            new AuthorDto(2, "Second Author", "Second Email", new List<int>() { 2 }, new List<int>() { 1 }),
        });
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAllAuthorsFalse()
    {
        _context.RemoveRange(_context.Authors);
        await _context.SaveChangesAsync();
        var (authorDtos, response) = await _authorRepository.FindAllAuthorsAsync();
        authorDtos.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }


    [Fact]
    public async Task FindAuthorsByName()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByNameAsync("First Author");
        authorDto.Should().BeEquivalentTo(new[]
            { new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 }, new List<int>() { 1 }) });
        response.Should().Be(Response.Ok);
    }


    [Fact]
    public async Task FindAuthorsByNameFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByNameAsync("Non existing name");
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }


    [Fact]
    public async Task FindAuthorsByEmail()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByEmailAsync("First Email");
        authorDto.Should().BeEquivalentTo(new[]
            { new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 }, new List<int>() { 1 }) });
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorsByEmailFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByEmailAsync("Non existing email");
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAuthorsByRepositoryId()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByRepositoryIdAsync(1);
        authorDto.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 }, new List<int>() { 1 }),
            new AuthorDto(2, "Second Author", "Second Email", new List<int>() { 2 }, new List<int>() { 1 }),
        });

        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorsByRepositoryIdFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByRepositoryIdAsync(2);
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAuthorsByCommitId()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByCommitIdAsync(1);
        authorDto.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 }, new List<int>() { 1 }),
        });
        response.Should().Be(Response.Ok);
    }

    //find authors response not found
    [Fact]
    public async Task FindAuthorsByCommitIdFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorsByCommitIdAsync(10);
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task CreateAuthorReturnsCreated()
    {
        var (authorDto, response) = await _authorRepository.CreateAuthorAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int>() {}, new List<int>() { 1 }));
        authorDto.Should().BeEquivalentTo(new AuthorDto(3, "Third Author", "Third Email", new List<int>() { },
            new List<int>() { 1 }));
        response.Should().Be(Response.Created);
    }

    [Fact]
    public async Task CreateAuthorReturnsBadRequest()
    {
        var (authorDto, response) = await _authorRepository.CreateAuthorAsync(new AuthorCreateDto("Wrong repo",
            "Wrong repo", new List<int>() { }, new List<int>() { 10 }));
        response.Should().Be(Response.BadRequest);
        authorDto.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAuthorReturnsDeleted()
    {
        var response = await _authorRepository.DeleteAuthorAsync(1);
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async Task DeleteAuthorReturnsNotFound()
    {
        var response = await _authorRepository.DeleteAuthorAsync(10);
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task UpdateAuthorReturnsOk()
    {
        var (authorDto, response) = await _authorRepository.FindAuthorAsync(1);
        authorDto.Should().BeEquivalentTo(new AuthorDto(1, "First Author", "First Email", new List<int>() { 1 },
            new List<int>() { 1 }));
        response.Should().Be(Response.Ok);

        var updateDto = new AuthorDto(1, "New Name", "New Email", new List<int>() { }, new List<int>() { 1 });
        var updatedAuthorDto = await _authorRepository.UpdateAuthorAsync(updateDto);

        updatedAuthorDto.Should().Be(Response.Ok);

        var (updatedAuthor, updatedResponse) = await _authorRepository.FindAuthorAsync(1);

        updatedAuthor.Should().BeEquivalentTo(updateDto);
        updatedResponse.Should().Be(updatedAuthorDto);
    }

    [Fact]
    public async Task UpdateAuthorReturnsNotFound()
    {
        var updateDto = new AuthorDto(10, "Does not exist", "Does not exist", new List<int>() { },
            new List<int>() { 1 });
        var updatedAuthorDto = await _authorRepository.UpdateAuthorAsync(updateDto);
        updatedAuthorDto.Should().Be(Response.NotFound);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
