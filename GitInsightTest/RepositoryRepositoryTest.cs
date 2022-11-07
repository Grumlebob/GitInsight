using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;


namespace GitInsightTest;

public class RepositoryRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly RepositoryRepository _repositoryRepository;

    public RepositoryRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _repositoryRepository = new RepositoryRepository(_context);

        var testRepo = new GitInsight.Entities.Repository()
        {
            Id = 1,
            Name = "First Repo",
            Path = "First RepoPath",
        };
        _context.Repositories.Add(testRepo);
        _context.SaveChanges();

        //Base testing branch
        var testBranch = new Branch()
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
    public async Task FindRepository_Test()
    {
        var a = await _repositoryRepository.FindRepositoryAsync(1);
        Assert.Equal("First Repo", a.Item1!.Name);
        Assert.Equal("First RepoPath", a.Item1.Path);
    }

    [Fact]
    public async Task FindRepository_DoesntExist_Test()
    {
        var a = await _repositoryRepository.FindRepositoryAsync(2);
        Assert.Equal(a, (null, Response.NotFound));
    }

    [Fact]
    public async Task FindAllRepositories_test()
    {
        var (dtoList, response) = await _repositoryRepository.FindAllRepositoriesAsync();
        dtoList.Count.Should().Be(1);
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAllRepositories_Empty_Test()
    {
        _context.Repositories.RemoveRange(_context.Repositories);
        _context.SaveChanges();
        var (dtoList, response) = await _repositoryRepository.FindAllRepositoriesAsync();
        dtoList.Should().BeNull();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task UpdateRepository_Test()
    {
        var (dto, _) = await _repositoryRepository.FindRepositoryAsync(1);
        var branches = dto.BranchIds;
        var commits = dto.CommitIds;
        var authors = dto.AuthorIds;

        var expect = new RepositoryDto(
            1,
            "Updated Path",
            "Updated Name",
            branches,
            commits,
            authors
        );

        var updatedResponse = await _repositoryRepository.UpdateRepositoryAsync(expect);
        var (resultRepo, _) = await _repositoryRepository.FindRepositoryAsync(1);

        updatedResponse.Should().Be(Response.Ok);
        resultRepo.Name.Should().Be("Updated Name");
        resultRepo.Path.Should().Be("Updated Path");
    }

    [Fact]
    public async Task UpdateRepository_DoesntExist_Test()
    {
        var expect = new RepositoryDto(
            2,
            "Updated Repo",
            "Updated RepoPath",
            new List<int>(),
            new List<int>(),
            new List<int>()
        );

        var updatedResponse = await _repositoryRepository.UpdateRepositoryAsync(expect);
        updatedResponse.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task DeleteRepository_Test()
    {
        var response = await _repositoryRepository.DeleteRepositoryAsync(1);
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async Task DeleteRepository_DoesntExist_Test()
    {
        var response = await _repositoryRepository.DeleteRepositoryAsync(2);
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task CreateRepository_should_return_created()
    {
        var create = new RepositoryCreateDto(
            "Second Repo",
            "Second RepoPath",
            new List<int>(),
            new List<int>(),
            new List<int>()
        );

        var (_, response) = await _repositoryRepository.CreateRepositoryAsync(create);

        response.Should().Be(Response.Created);
    }

    [Fact]
    public async Task CreateRepository_Duplicate_should_return_conflict()
    {
        var create = new RepositoryCreateDto(
            "First RepoPath",
            "First Repo",
            new List<int>(),
            new List<int>(),
            new List<int>()
        );

        var (_, response) = await _repositoryRepository.CreateRepositoryAsync(create);

        response.Should().Be(Response.Conflict);
    }


    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
