using GitInsight;
using GitInsight.Core;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class AnalysisTest
{
    private readonly InsightContext _context;
    private readonly Analysis _analysis;

    public AnalysisTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<InsightContext>();
        builder.UseSqlite(connection);
        var context = new InsightContext(builder.Options);

        context.Database.EnsureCreated();

        _context = context;
        _analysis=new Analysis(_context);


        var repo = new RepoInsight { Name = "repo1", Path = "idk/idk" };
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
            Date = new DateTime(2022,10,13,0,0,0)
            
        });
        
        _context.Commits.Add(new GitInsight.Entities.CommitInsight
        {
            Id = 2,
            Sha = "heck", 
            AuthorId = 2,
            BranchId = 1, 
            RepositoryId = 1, 
            Date = new DateTime(2022,10,13,11,45,0)
        });
        
        _context.Commits.Add(new GitInsight.Entities.CommitInsight
        {
            Id = 3, Sha = "tger", 
            AuthorId = 1, 
            BranchId = 1, 
            RepositoryId = 1, 
            Date = new DateTime(2022,11,15,0,0,0)
        });

        _context.SaveChanges();
    }
    
    [Fact]
    public async Task Get_Commits_by_Date_Returns_correct_result()
    {
        var commitsByDate = await _analysis.GetCommitsByDate(1);

        commitsByDate.Count.Should().Be(2);

        commitsByDate[0].Date.Should().Be(new DateTime(2022, 10, 13, 0, 0, 0));
        commitsByDate[0].CommitAmount.Should().Be(2);
        
        commitsByDate[1].Date.Should().Be(new DateTime(2022, 11, 15, 0, 0, 0));
        commitsByDate[1].CommitAmount.Should().Be(1);
    }
    
    
    [Fact]
    public async Task Get_Commits_by_Author_Returns_correct_result()
    {
        var commitsByAuthor = await _analysis.GetCommitsByAuthor(1);

        commitsByAuthor.Count.Should().Be(2);

        commitsByAuthor[0].AuthorName.Should().Be("Søren");
        commitsByAuthor[0].CommitsByDates.Count.Should().Be(2);
        commitsByAuthor[0].CommitsByDates[0].Date.Should().Be(new DateTime(2022, 10, 13, 0, 0, 0));
        commitsByAuthor[0].CommitsByDates[0].CommitAmount.Should().Be(1);
        commitsByAuthor[0].CommitsByDates[1].Date.Should().Be(new DateTime(2022, 11, 15, 0, 0, 0));
        commitsByAuthor[0].CommitsByDates[1].CommitAmount.Should().Be(1);
        
        commitsByAuthor[1].AuthorName.Should().Be("Per");
        commitsByAuthor[1].CommitsByDates.Count.Should().Be(1);
        commitsByAuthor[1].CommitsByDates[0].Date.Should().Be(new DateTime(2022, 10, 13, 0, 0, 0));
        commitsByAuthor[1].CommitsByDates[0].CommitAmount.Should().Be(1);
        
    }
    
    [Fact]
    public async Task Highest_Committer_Within_Timeframe_Return_correct_result()
    {
        var winner = await _analysis.HighestCommitterWithinTimeframe(1, 
            DateTime.Parse("1/11/1111 00:00:00 AM"),
            DateTime.Parse("2/12/1111 10:00:00 AM"));

        winner.WinnerName.Should().Be("Søren");
        winner.Value.Should().Be(2);
        
        var winner2 = await _analysis.HighestCommitterWithinTimeframe(1, 
            DateTime.Parse("1/11/1111 10:00:00 AM"),
            DateTime.Parse("2/12/1111 12:00:00 PM"));

        winner2.WinnerName.Should().Be("Per");
        winner2.Value.Should().Be(1);
    }



}