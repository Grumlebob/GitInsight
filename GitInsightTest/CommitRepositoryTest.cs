using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class CommitRepositoryTest
{
    private readonly InsightContext _context;
    private readonly CommitRepository _repository;

    public CommitRepositoryTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<InsightContext>();
        builder.UseSqlite(connection);
        var context = new InsightContext(builder.Options);

        context.Database.EnsureCreated();

        _context = context;
        _repository = new CommitRepository(_context);

        _context.Authors.Add(new Author { Name = "Søren", Email = "søren@gmail.dk" });
        _context.Authors.Add(new Author { Name = "Per", Email = "per@gmail.dk" });
        _context.SaveChanges();

        _context.Repositories.Add(new GitInsight.Entities.Repository { Name = "repo1", Path = "idk/idk" });
        _context.Repositories.Add(new GitInsight.Entities.Repository { Name = "repo2", Path = "idc/idc" });
        _context.SaveChanges();

        _context.Branches.Add(new Branch { Name = "branch1", Sha = "huhu", Path = "origin/idk", RepositoryId = 1 });
        _context.Branches.Add(new Branch { Name = "branch2", Sha = "hihi", Path = "origin/idc", RepositoryId = 2 });
        _context.SaveChanges();

        _context.Commits.Add(new GitInsight.Entities.Commit { Id = 1, Sha = "treg", Tag = "1.2.3", AuthorId = 1, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now, Repository = null, Author = null, Branch = null });
        _context.Commits.Add(new GitInsight.Entities.Commit { Id = 2, Sha = "heck", Tag = "1.1.2", AuthorId = 2, BranchId = 2, RepositoryId = 2, Date = DateTimeOffset.Now });
        _context.Commits.Add(new GitInsight.Entities.Commit { Id = 3, Sha = "tger", Tag = "1.1.4", AuthorId = 1, BranchId = 2, RepositoryId = 2, Date = DateTimeOffset.Now });

        _context.SaveChanges();
    }

    [Fact]
    public async Task Find_id_1_return_ok()
    {
        var (commit, response) = await _repository.FindAsync(1);

        response.Should().Be(Response.Ok);

        commit.Sha.Should().Be("treg");


    }

    [Fact]
    public async Task Find_id_4_return_not_found()
    {
        var (commit, response) = await _repository.FindAsync(4);

        response.Should().Be(Response.NotFound);
        commit.Should().BeNull();
    }

    [Fact]
    public async Task Find_id_all_return_ok()
    {
        var (commits, response) = await _repository.FindAllAsync();

        response.Should().Be(Response.Ok);
        commits.Count.Should().Be(3);
    }

    [Fact]
    public async Task Create_return_created()
    {
        var expectedCommitDTO = new CommitDTO(4, "heyuo", DateTimeOffset.Now, "3.2.5", 2, 1, 1);

        var result = await _repository.CreateAsync(new CommitCreateDTO(expectedCommitDTO.Sha, expectedCommitDTO.Date, expectedCommitDTO.Tag, expectedCommitDTO.AuthorId, expectedCommitDTO.BranchId, expectedCommitDTO.RepositoryId));

        result.response.Should().Be(Response.Created);

        _repository.FindAsync(4).Result.commit.Should().BeEquivalentTo(expectedCommitDTO);
        result.commit.Should().BeEquivalentTo(expectedCommitDTO);
    }

    [Fact]
    public async Task Create_return_conflict_because_duplicate_sha()
    {
        var result = await _repository.CreateAsync(new CommitCreateDTO("treg", DateTimeOffset.Now, "3.2.5", 2, 1, 1));

        result.response.Should().Be(Response.Conflict);
        result.commit.Should().BeNull();
        _repository.FindAllAsync().Result.commits.Count.Should().Be(3);
    }

    [Fact]
    public async Task Create_return_badRequest_because_nonExisting_author()
    {
        var result = await _repository.CreateAsync(new CommitCreateDTO("heyuo", DateTimeOffset.Now, "3.2.5", 3, 1, 1));

        result.response.Should().Be(Response.BadRequest);
        result.commit.Should().BeNull();
        _repository.FindAllAsync().Result.commits.Count.Should().Be(3);
    }

    [Fact]
    public async Task Update_id_1_return_ok()
    {
        var commitDTO = new CommitDTO(1, "treg", DateTimeOffset.Now, "1.2.5", 1, 1, 1);
        var result = await _repository.UpdateAsync(commitDTO);

        result.response.Should().Be(Response.Ok);

        result.commit.Should().BeEquivalentTo(commitDTO);
        _repository.FindAsync(1).Result.commit.Should().BeEquivalentTo(commitDTO);

    }

    [Fact]
    public async Task Update_id_4_return_notfound()
    {
        var result = await _repository.UpdateAsync(new CommitDTO(4, "hjgk", DateTimeOffset.Now, "1.2.5", 2, 1, 1));

        result.response.Should().Be(Response.NotFound);
        result.commit.Should().BeNull();
    }

    [Fact]
    public async Task Update_return_badRequest_because_nonExisting_repo()
    {
        var result = await _repository.UpdateAsync(new CommitDTO(2, "heck", DateTimeOffset.Now, "1.1.2", 2, 2, 3));

        result.response.Should().Be(Response.BadRequest);
        _repository.FindAsync(2).Result.commit.RepositoryId.Should().Be(2);
    }

    [Fact]
    public async Task Update_return_badRequest_because_nothing_is_changed()
    {
        var result = await _repository.UpdateAsync(new CommitDTO(3, "tger", _repository.FindAsync(3).Result.commit.Date, "1.1.4", 1, 2, 2));

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
        _context.Commits.FirstOrDefaultAsync(c => c.Id == 1).Result.Author.Name.Should().Be("Søren");
        _context.Commits.FirstOrDefaultAsync(c => c.Id == 2).Result.Branch.Sha.Should().Be("hihi");
        _context.Commits.FirstOrDefaultAsync(c => c.Id == 3).Result.Repository.Name.Should().Be("repo2");
    }

}