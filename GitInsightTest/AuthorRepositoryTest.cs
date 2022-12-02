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
        var testRepo = new Repository
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
        var (authorDto, response) = await _authorRepository.FindAsync(1);
        authorDto.Should().BeEquivalentTo(new AuthorDto(1, "First Author", "First Email",
            new List<int> { 1 }));
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorByIdReturnsFalse()
    {
        var (authorDto, response) = await _authorRepository.FindAsync(3);
        authorDto.Should().BeNull();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAllAuthors()
    {
        var (authorDtos, response) = await _authorRepository.FindAllAsync();
        authorDtos.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int> { 1 }),
            new AuthorDto(2, "Second Author", "Second Email", new List<int> { 1 }),
        });
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAllAuthorsFalse()
    {
        _context.RemoveRange(_context.Authors);
        await _context.SaveChangesAsync();
        var (authorDtos, response) = await _authorRepository.FindAllAsync();
        authorDtos.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }


    [Fact]
    public async Task FindAuthorsByName()
    {
        var (authorDto, response) = await _authorRepository.FindByNameAsync("First Author");
        authorDto.Should().BeEquivalentTo(new[]
            { new AuthorDto(1, "First Author", "First Email", new List<int> { 1 }) });
        response.Should().Be(Response.Ok);
    }


    [Fact]
    public async Task FindAuthorsByNameFalse()
    {
        var (authorDto, response) = await _authorRepository.FindByNameAsync("Non existing name");
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }


    [Fact]
    public async Task FindAuthorsByEmail()
    {
        var (authorDto, response) = await _authorRepository.FindByEmailAsync("First Email");
        authorDto.Should().BeEquivalentTo(
            new AuthorDto(1, "First Author", "First Email", new List<int> { 1 }));
        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorsByEmailFalse()
    {
        var (authorDto, response) = await _authorRepository.FindByEmailAsync("Non existing email");
        authorDto.Should().BeNull();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAuthorsByRepositoryId()
    {
        var (authorDto, response) = await _authorRepository.FindByRepoIdAsync(1);
        authorDto.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int> { 1 }),
            new AuthorDto(2, "Second Author", "Second Email", new List<int> { 1 }),
        });

        response.Should().Be(Response.Ok);
    }

    [Fact]
    public async Task FindAuthorsByRepositoryIdFalse()
    {
        var (authorDto, response) = await _authorRepository.FindByRepoIdAsync(2);
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task FindAuthorsByCommitId()
    {
        var (authorDto, response) = await _authorRepository.FindByCommitIdAsync(1);
        authorDto.Should().BeEquivalentTo(new List<AuthorDto>
        {
            new AuthorDto(1, "First Author", "First Email", new List<int> { 1 }),
        });
        response.Should().Be(Response.Ok);
    }

    //find authors response not found
    [Fact]
    public async Task FindAuthorsByCommitIdFalse()
    {
        var (authorDto, response) = await _authorRepository.FindByCommitIdAsync(10);
        authorDto.Should().BeNullOrEmpty();
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task CreateAuthorReturnsCreated()
    {
        var (authorDto, response) = await _authorRepository.CreateAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int> { 1 }));
        authorDto.Should().BeEquivalentTo(new AuthorDto(3, "Third Author", "Third Email",
            new List<int> { 1 }));
        response.Should().Be(Response.Created);
    }

    [Fact]
    public async Task CreateAuthorReturnsBadRequest()
    {
        var (authorDto, response) = await _authorRepository.CreateAsync(new AuthorCreateDto("Wrong repo",
            "Wrong repo", new List<int> { 10 }));
        response.Should().Be(Response.BadRequest);
        authorDto.Should().BeNull();
    }

    [Fact]
    public async Task CreateAuthorReturnsConflict()
    {
        var (authorDto, response) = await _authorRepository.CreateAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int> { 1 }));
        authorDto.Should().BeEquivalentTo(new AuthorDto(3, "Third Author", "Third Email",
            new List<int> { 1 }));
        response.Should().Be(Response.Created);

        var (_, responseSame) = await _authorRepository.CreateAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int> { 1 }));
        responseSame.Should().Be(Response.Conflict);
    }

    [Fact]
    public async Task CreateExistingAuthorWithNewReposReturnsOk()
    {
        var testRepo = new Repository
        {
            Id = 69,
            Name = "Another repo",
            Path = "Random Path",
        };
        await _context.Repositories.AddAsync(testRepo);
        await _context.SaveChangesAsync();
        var (authorDto, response) = await _authorRepository.CreateAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int> { 1 }));
        authorDto.Should().BeEquivalentTo(new AuthorDto(3, "Third Author", "Third Email",
            new List<int> { 1 }));
        response.Should().Be(Response.Created);

        var (updated, responseSame) = await _authorRepository.CreateAsync(new AuthorCreateDto("Third Author",
            "Third Email", new List<int> { 1, 69 }));
        responseSame.Should().Be(Response.Ok);
        updated!.RepositoryIds.Should().Contain(69);
    }

    [Fact]
    public async Task DeleteAuthorReturnsDeleted()
    {
        var response = await _authorRepository.DeleteAsync(1);
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async Task DeleteAuthorReturnsNotFound()
    {
        var response = await _authorRepository.DeleteAsync(10);
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task UpdateAuthorReturnsOk()
    {
        var (authorDto, response) = await _authorRepository.FindAsync(1);
        authorDto.Should().BeEquivalentTo(new AuthorDto(1, "First Author", "First Email",
            new List<int> { 1 }));
        response.Should().Be(Response.Ok);

        var updateDto = new AuthorUpdateDto(1, "New Name", "New Email", new List<int> { 1 });
        var updatedAuthorDto = await _authorRepository.UpdateAsync(updateDto);

        updatedAuthorDto.Should().Be(Response.Ok);

        var (updatedAuthor, updatedResponse) = await _authorRepository.FindAsync(1);

        updatedAuthor.Should().BeEquivalentTo(updateDto);
        updatedResponse.Should().Be(updatedAuthorDto);
    }

    [Fact]
    public async Task UpdateAuthorReturnsNotFound()
    {
        var updateDto = new AuthorUpdateDto(10, "Does not exist", "Does not exist",
            new List<int> { 1 });
        var updatedAuthorDto = await _authorRepository.UpdateAsync(updateDto);
        updatedAuthorDto.Should().Be(Response.NotFound);
    }

    [Fact]
    public void AuthorConfigurations()
    {
        AuthorConfigurations authorConfigurations = new AuthorConfigurations();
        authorConfigurations.Should().NotBeNull();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}