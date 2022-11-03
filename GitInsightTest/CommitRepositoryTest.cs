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
        var response = _repository.Create(new GitInsight.Entities.Commit{Sha= "heyuo", Tag = "3.2.5", AuthorId = 2, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});

        response.Should().Be(Response.Created);
        _repository.Find(4).commit.Sha.Should().Be("heyuo");
    }
    
    
    [Fact]
    public void Create_return_conflict_because_duplicate_sha()
    {
        var response = _repository.Create(new GitInsight.Entities.Commit{Sha= "treg", Tag = "3.2.5", AuthorId = 2, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});

        response.Should().Be(Response.Conflict);
        _repository.FindAll().Item1.Count.Should().Be(3);
    }
    
    [Fact]
    public void Create_return_badRequest_because_nonExisting_author()
    {
        var response = _repository.Create(new GitInsight.Entities.Commit{Sha= "heyuo", Tag = "3.2.5", AuthorId = 3, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});

        response.Should().Be(Response.BadRequest);
        _repository.FindAll().Item1.Count.Should().Be(3);
    }
    
    
    [Fact]
    public void Update_id_1_return_ok()
    {
        var response = _repository.Update(new GitInsight.Entities.Commit{Id=1, Sha= "treg", Tag = "1.2.5", AuthorId = 1, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});

        response.Should().Be(Response.Ok);
        _repository.Find(1).commit.Tag.Should().Be("1.2.5");
    }
    
    
    [Fact]
    public void Update_id_4_return_notfound()
    {
        var response = _repository.Update(new GitInsight.Entities.Commit{Id=4, Sha= "hjgk", Tag = "1.2.5", AuthorId = 2, BranchId = 1, RepositoryId = 1, Date = DateTimeOffset.Now});

        response.Should().Be(Response.NotFound);
    }
    
    [Fact]
    public void Update_return_badRequest_because_nonExisting_repo()
    {
        var response = _repository.Update(new GitInsight.Entities.Commit{Id = 2, Sha= "heck", Tag = "1.1.2", AuthorId = 2, BranchId = 2, RepositoryId = 3, Date = DateTimeOffset.Now});

        response.Should().Be(Response.BadRequest);
        _repository.Find(2).commit.RepositoryId.Should().Be(2);
    }
    
    [Fact]
    public void Update_return_badRequest_because_nothing_is_changed()
    {
        var response = _repository.Update(new GitInsight.Entities.Commit{Id=3 ,Sha= "tger", Tag = "1.1.4", AuthorId = 1, BranchId = 2, RepositoryId = 2, Date = _repository.Find(3).commit.Date});

        response.Should().Be(Response.BadRequest);
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