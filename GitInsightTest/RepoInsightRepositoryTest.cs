namespace GitInsightTest;

public class RepoInsightRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;
    private readonly RepoInsightRepository _repoInsightRepository;

    public RepoInsightRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _repoInsightRepository = new RepoInsightRepository(_context);

        var testRepo = new RepoInsight
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
            Commits =
            {
                new CommitInsight
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
                new CommitInsight
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
        var a = await _repoInsightRepository.FindAsync(1);
        Assert.Equal("First Repo", a.Item1!.Name);
        Assert.Equal("First RepoPath", a.Item1.Path);
        Assert.Equal(0, a.Item1.LatestCommitId);
    }

    [Fact]
    public async Task FindRepository_DoesntExist_Test()
    {
        var a = await _repoInsightRepository.FindAsync(2);
        Assert.Equal(a, (null, Response.NotFound));
    }

    [Fact]
    public async Task FindRepositoryByPath_Test()
    {
        var a = await _repoInsightRepository.FindRepositoryByPathAsync("First RepoPath");
        Assert.Equal("First Repo", a.Item1!.Name);
        Assert.Equal("First RepoPath", a.Item1.Path);
    }

    [Fact]
    public async Task FindRepositoryByPath_DoesntExist_Test()
    {
        var a = await _repoInsightRepository.FindRepositoryByPathAsync("Non-existing Path");
        Assert.Equal(a, (null, Response.NotFound));
    }

    [Fact]
    public async Task FindAllRepositories_test()
    {
        var (dtoList, response) = await _repoInsightRepository.FindAllAsync();
        dtoList!.Count.Should().Be(1);
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAllRepositories_Empty_Test()
    {
        _context.Repositories.RemoveRange(_context.Repositories);
        await _context.SaveChangesAsync();
        var (dtoList, response) = await _repoInsightRepository.FindAllAsync();
        dtoList.Should().BeNull();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task UpdateRepository_Test()
    {
        var (dto, _) = await _repoInsightRepository.FindAsync(1);
        var branches = dto!.BranchIds;
        var commits = dto.CommitIds;
        var authors = dto.AuthorIds;

        var expect = new RepoInsightDto(
            1,
            "Updated Path",
            "Updated Name",
            branches,
            commits,
            authors,
            0
        );

        var updatedResponse = await _repoInsightRepository.UpdateAsync(expect);
        var (resultRepo, _) = await _repoInsightRepository.FindAsync(1);

        updatedResponse.Should().Be(Response.Ok);
        resultRepo!.Name.Should().Be("Updated Name");
        resultRepo.Path.Should().Be("Updated Path");
        resultRepo.LatestCommitId.Should().Be(0);
    }

    [Fact]
    public async Task UpdateRepository_DoesntExist_Test()
    {
        var expect = new RepoInsightDto(
            2,
            "Updated Repo",
            "Updated RepoPath",
            new List<int>(),
            new List<int>(),
            new List<int>(),
            1
        );

        var updatedResponse = await _repoInsightRepository.UpdateAsync(expect);
        updatedResponse.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task DeleteRepository_Test()
    {
        var response = await _repoInsightRepository.DeleteAsync(1);
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async Task DeleteRepository_DoesntExist_Test()
    {
        var response = await _repoInsightRepository.DeleteAsync(2);
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task CreateRepository_should_return_created()
    {
        var create = new RepoInsightCreateDto(
            "Second Repo",
            "Second RepoPath",
            new List<int>(),
            new List<int>(),
            new List<int>()
        );


        var (result, response) = await _repoInsightRepository.CreateAsync(create);

        var expected = new RepoInsightDto(
            2,
            "Second Repo",
            "Second RepoPath",
            new List<int>(),
            new List<int>(),
            new List<int>(),
            0
        );
        result.Should().BeEquivalentTo(expected);
        response.Should().Be(Response.Created);
    }

    [Fact]
    public async Task CreateRepository_Duplicate_should_return_conflict()
    {
        var create = new RepoInsightCreateDto(
            "First RepoPath",
            "First Repo",
            new List<int>(),
            new List<int>(),
            new List<int>()
        );

        var (result, response) = await _repoInsightRepository.CreateAsync(create);

        var expected = new RepoInsightDto(
            Id: 1,
            Name: "First Repo",
            Path: "First RepoPath",
            BranchIds: new List<int> { 1 },
            CommitIds: new List<int> { 1, 2 },
            AuthorIds: new List<int> { 1, 2 },
            LatestCommitId: 0
        );

        response.Should().Be(Response.Conflict);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Update_Latest_Commit_Success()
    {
        var newCommit = new CommitInsightCreateDto("ab", DateTimeOffset.Now, 1, 1, 1);
        await new CommitInsightRepository(_context).CreateAsync(newCommit);
        var newLastCommit = new RepoInsightLatestCommitUpdate(1, 2);
        
        var (result, response) = await _repoInsightRepository.FindAsync(1);
        result!.LatestCommitId.Should().Be(0);
        response.Should().Be(Response.Ok);
        
        var response2 = await _repoInsightRepository.UpdateLatestCommitAsync(newLastCommit);
        var (result2, _) = await _repoInsightRepository.FindAsync(1);
        response2.Should().Be(Response.Ok);
        result2!.LatestCommitId.Should().Be(2);
    }
    
    [Fact]
    public async Task Update_Latest_Commit_Not_Exist()
    {
        var newLastCommit = new RepoInsightLatestCommitUpdate(1, 5);
        var response = await _repoInsightRepository.UpdateLatestCommitAsync(newLastCommit);
        var (result, _) = await _repoInsightRepository.FindAsync(1);
        response.Should().Be(Response.BadRequest);
        result!.LatestCommitId.Should().Be(0);
    }

    [Fact]
    public async Task Update_Latest_Commit_Repo_Not_Exist()
    {
        var response = await _repoInsightRepository.UpdateLatestCommitAsync(new RepoInsightLatestCommitUpdate(2, 2));
        response.Should().Be(Response.NotFound);
    }


    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}