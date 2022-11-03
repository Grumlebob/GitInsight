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

        _context.Authors.Add(new Author{ Name = "Søren", Email = "søren@gmail.dk" });
        _context.Authors.Add(new Author{ Name = "Per", Email = "per@gmail.dk" });
        _context.SaveChanges();

        
        _context.Repositories.Add(new GitInsight.Entities.Repository{ Name = "repo1", Path = "idk/idk"});
        _context.Repositories.Add(new GitInsight.Entities.Repository{ Name = "repo2", Path = "idc/idc"});
        _context.SaveChanges();

        _context.Branches.Add(new Branch{ Name = "branch1", Sha= "huhu", Path = "origin/idk", RepositoryId = 1});
        _context.Branches.Add(new Branch{ Name = "branch2", Sha= "hihi", Path = "origin/idc", RepositoryId = 2});
        _context.SaveChanges();

        _context.Commits.Add(new GitInsight.Entities.Commit{Sha= "treg", Tag = "1.2.3", AuthorId = 1, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});
        _context.Commits.Add(new GitInsight.Entities.Commit{Sha= "heck", Tag = "1.1.2", AuthorId = 2, BranchId = 2, RepositoryId = 2, Date = DateTimeOffset.Now});
        _context.Commits.Add(new GitInsight.Entities.Commit{Sha= "tger", Tag = "1.1.4", AuthorId = 1, BranchId = 2, RepositoryId = 2, Date = DateTimeOffset.Now});

        
        _context.SaveChanges();

    }
 
    [Fact]
    public void Find_id_1_return_ok()
    {
        var (commit,response) = _repository.Find(1);

        response.Should().Be(Response.Ok);

        commit.Sha.Should().Be("treg");
    }
    
    [Fact]
    public void Find_id_4_return_not_found()
    {
        var (commit,response) = _repository.Find(4);

        response.Should().Be(Response.NotFound);
        commit.Should().BeNull();
    }
    
    
    [Fact]
    public void Find_id_all_return_ok()
    {
        var (commits,response) = _repository.FindAll();

        response.Should().Be(Response.Ok);
        commits.Count.Should().Be(3);
    }
    
    
    [Fact]
    public void Create_return_created()
    {
        var result = _repository.Create(new CommitCreateDTO("heyuo",DateTimeOffset.Now, "3.2.5", 2,  1, 1));

        result.response.Should().Be(Response.Created);
        _repository.Find(4).commit.Sha.Should().Be("heyuo");
    }
    
    
    [Fact]
    public void Create_return_conflict_because_duplicate_sha()
    {
        var result = _repository.Create(new CommitCreateDTO("treg", DateTimeOffset.Now , "3.2.5",  2, 1, 1));

        result.response.Should().Be(Response.Conflict);
        _repository.FindAll().Item1.Count.Should().Be(3);
    }
    
    [Fact]
    public void Create_return_badRequest_because_nonExisting_author()
    {
        var result = _repository.Create(new CommitCreateDTO("heyuo",DateTimeOffset.Now, "3.2.5", 3, 1, 1));

        result.response.Should().Be(Response.BadRequest);
        _repository.FindAll().Item1.Count.Should().Be(3);
    }
    
    
    [Fact]
    public void Update_id_1_return_ok()
    {
        var result = _repository.Update(new CommitDTO(1, "treg",DateTimeOffset.Now, "1.2.5", 1, 1, 1));

        result.response.Should().Be(Response.Ok);
        _repository.Find(1).commit.Tag.Should().Be("1.2.5");
    }
    
    
    [Fact]
    public void Update_id_4_return_notfound()
    {
        var result = _repository.Update(new CommitDTO(4, "hjgk",DateTimeOffset.Now, "1.2.5",  2, 1, 1));

        result.response.Should().Be(Response.NotFound);
    }
    
    [Fact]
    public void Update_return_badRequest_because_nonExisting_repo()
    {
        var result = _repository.Update(new CommitDTO(2,  "heck",DateTimeOffset.Now, "1.1.2",  2,  2,  3));

        result.response.Should().Be(Response.BadRequest);
        _repository.Find(2).commit.RepositoryId.Should().Be(2);
    }
    
    [Fact]
    public void Update_return_badRequest_because_nothing_is_changed()
    {
        var result = _repository.Update(new CommitDTO(3 ,"tger", _repository.Find(3).commit.Date, "1.1.4", 1, 2, 2 ));

            result.response.Should().Be(Response.BadRequest);
    }
    
    [Fact]
    public void Deleted_id_1_return_deleted()
    {
        var response = _repository.Delete(1);

        response.Should().Be(Response.Deleted);
        _repository.Find(1).response.Should().Be(Response.NotFound);
    }
    
    [Fact]
    public void Deleted_id_5_return_notFound()
    {
        var response = _repository.Delete(4);

        response.Should().Be(Response.NotFound);
    }
    
}