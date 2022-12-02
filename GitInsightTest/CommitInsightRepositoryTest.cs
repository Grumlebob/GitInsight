namespace GitInsightTest;

public class CommitInsightRepositoryTest : IDisposable
{
    private readonly InsightContext _context;
    private readonly CommitInsightRepository _repository;

    public CommitInsightRepositoryTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<InsightContext>();
        builder.UseSqlite(connection);
        var context = new InsightContext(builder.Options);

        context.Database.EnsureCreated();

        _context = context;
        _repository = new CommitInsightRepository(_context);

        _context.Authors.Add(new Author { Name = "Søren", Email = "søren@gmail.dk" });
        _context.Authors.Add(new Author { Name = "Per", Email = "per@gmail.dk" });
        _context.SaveChanges();

        _context.Repositories.Add(new RepoInsight { Name = "repo1", Path = "idk/idk" });
        _context.Repositories.Add(new RepoInsight { Name = "repo2", Path = "idc/idc" });
        _context.SaveChanges();

        _context.Branches.Add(new Branch { Name = "branch1", Path = "origin/idk", RepositoryId = 1 });
        _context.Branches.Add(new Branch { Name = "branch2", Path = "origin/idc", RepositoryId = 2 });
        _context.SaveChanges();

        _context.Commits.Add(new CommitInsight { Id = 1, Sha = "treg", AuthorId = 1, BranchId = 1, RepositoryId = 1, Date = DateTime.Now, Repository = null!, Author = null!, Branch = null! });
        _context.Commits.Add(new CommitInsight { Id = 2, Sha = "heck", AuthorId = 2, BranchId = 2, RepositoryId = 2, Date = DateTime.Now });
        _context.Commits.Add(new CommitInsight { Id = 3, Sha = "tger", AuthorId = 1, BranchId = 2, RepositoryId = 2, Date = DateTime.Now });

        _context.SaveChanges();
    }

    [Fact]
    public async Task Find_id_1_return_ok()
    {
        var (commit, response) = await _repository.FindAsync(1);

        response.Should().Be(Response.Ok);

        commit!.Sha.Should().Be("treg");

    }

    [Fact]
    public async Task Find_id_4_return_not_found()
    {
        var (commit, response) = await _repository.FindAsync(4);

        response.Should().Be(Response.NotFound);
        commit.Should().BeNull();
    }
    
    [Fact]
    public async Task Find_by_sha_return_ok()
    {
        var (commit, response) = await _repository.FindByShaAsync("heck");

        response.Should().Be(Response.Ok);

        commit!.Id.Should().Be(2);

    }
    
    [Fact]
    public async Task Find_by_sha_return_notFound()
    {
        var (commit, response) = await _repository.FindByShaAsync("ebeb");

        response.Should().Be(Response.NotFound);

        commit.Should().BeNull();
    }

    [Fact]
    public async Task Find_by_repoId_return_ok()
    {
        var (commits, response) = await _repository.FindByRepoIdAsync(2);

        response.Should().Be(Response.Ok);

        commits.Count.Should().Be(2);
        commits[0]!.Id.Should().Be(2);
        commits[1]!.Id.Should().Be(3);

    }
    
    [Fact]
    public async Task Find_by_repoId_return_notFound()
    {
        var (commits, response) = await _repository.FindByRepoIdAsync(5);

        response.Should().Be(Response.NotFound);

        commits.Should().BeNull();
    }

    [Fact]
    public async Task Find_all_return_ok()
    {
        var (commits, response) = await _repository.FindAllAsync();

        response.Should().Be(Response.Ok);
        commits.Count.Should().Be(3);
    }
    
    [Fact]
    public async Task Find_all_return_notFound()
    {
        await _context.Commits.ExecuteDeleteAsync();
        var (commits, response) = await _repository.FindAllAsync();
        response.Should().Be(Response.NotFound);
        commits.Should().BeNull();
    }

    [Fact]
    public async Task Create_return_created()
    {
        var expectedCommitDto = new CommitInsightDto(4, "heyuo", DateTimeOffset.Now, 2, 1, 1);

        var result = await _repository.CreateAsync(new CommitInsightCreateDto(expectedCommitDto.Sha, expectedCommitDto.Date, expectedCommitDto.AuthorId, expectedCommitDto.BranchId, expectedCommitDto.RepositoryId));

        result.response.Should().Be(Response.Created);

        _repository.FindAsync(4).Result.commit.Should().BeEquivalentTo(expectedCommitDto);
        result.commit.Should().BeEquivalentTo(expectedCommitDto);
    }

    [Fact]
    public async Task Create_return_conflict_because_duplicate_sha()
    {
        var result = await _repository.CreateAsync(new CommitInsightCreateDto("treg", DateTimeOffset.Now, 2, 1, 1));

        result.response.Should().Be(Response.Conflict);
        result.commit.Should().BeNull();
        _repository.FindAllAsync().Result.commits.Count.Should().Be(3);
    }

    [Fact]
    public async Task Create_return_badRequest_because_nonExisting_author()
    {
        var result = await _repository.CreateAsync(new CommitInsightCreateDto("heyuo", DateTimeOffset.Now, 3, 1, 1));

        result.response.Should().Be(Response.BadRequest);
        result.commit.Should().BeNull();
        _repository.FindAllAsync().Result.commits.Count.Should().Be(3);
    }

    [Fact]
    public async Task Update_id_1_return_ok()
    {
        var commitDto = new CommitInsightDto(1, "treg", DateTimeOffset.Now, 1, 1, 1);
        var result = await _repository.UpdateAsync(commitDto);

        result.response.Should().Be(Response.Ok);

        result.commit.Should().BeEquivalentTo(commitDto);
        _repository.FindAsync(1).Result.commit.Should().BeEquivalentTo(commitDto);

    }

    [Fact]
    public async Task Update_id_4_return_notfound()
    {
        var result = await _repository.UpdateAsync(new CommitInsightDto(4, "hjgk", DateTimeOffset.Now, 2, 1, 1));

        result.response.Should().Be(Response.NotFound);
        result.commit.Should().BeNull();
    }

    [Fact]
    public async Task Update_return_badRequest_because_nonExisting_repo()
    {
        var result = await _repository.UpdateAsync(new CommitInsightDto(2, "heck", DateTimeOffset.Now, 2, 2, 3));

        result.response.Should().Be(Response.BadRequest);
        _repository.FindAsync(2).Result.commit!.RepositoryId.Should().Be(2);
    }

    [Fact]
    public async Task Update_return_badRequest_because_nothing_is_changed()
    {
        var result = await _repository.UpdateAsync(new CommitInsightDto(3, "tger", _repository.FindAsync(3).Result.commit!.Date, 1, 2, 2));

        result.response.Should().Be(Response.BadRequest);
    }

    [Fact]
    public async Task Deleted_id_1_return_deleted()
    {
        var response = await _repository.DeleteAsync(1);

        response.Should().Be(Response.Deleted);
        _repository.FindAsync(1).Result.response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task Deleted_id_5_return_notFound()
    {
        var response = await _repository.DeleteAsync(4);

        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async Task Test_Relations()
    {
        (await _context.Commits.FirstOrDefaultAsync(c => c.Id == 1))!.Author.Name.Should().Be("Søren");
        (await _context.Commits.FirstOrDefaultAsync(c => c.Id == 1))!.Branch.Id.Should().Be(1);
        (await _context.Commits.FirstOrDefaultAsync(c => c.Id == 3))!.Repository.Name.Should().Be("repo2");
    }
    public void Dispose()
    {
        _context.Dispose();
    }

}