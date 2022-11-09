using GitInsight.Core;
using GitInsight.Data;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class DataManagerTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;

    
    public DataManagerTest()
    {
        (_connection, _context) = SetupTests.Setup();
        _context.Database.EnsureDeletedAsync();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task AnalyzeShouldBeCompletedReturnsTrue()
    {
        DataManager dataManager = new DataManager(_context);
        var res = await dataManager.Analyze( GetGitTestFolder(),GetRelativeTestFolder());
        res.Should().BeTrue();
    }
    
    [Fact]
    public async Task NoReAnalyzeNeededReturnsFalse()
    {
        DataManager dataManager = new DataManager(_context);
        //Run twice:
        await dataManager.Analyze( GetGitTestFolder(),GetRelativeTestFolder());
        var res = await dataManager.Analyze( GetGitTestFolder(),GetRelativeTestFolder());
        res.Should().BeFalse();
    }
    
    [Fact]
    public async Task NewChangesReanalyzeNeededReturnsTrue()
    {
        DataManager dataManager = new DataManager(_context);
        
        var firstScan = await dataManager.Analyze( GetGitTestFolder(),GetRelativeTestFolder());
        firstScan.Should().BeTrue();

        await _context.Repositories.ExecuteDeleteAsync();
        await _context.Commits.ExecuteDeleteAsync();
        
        var secondScanAfterAddedCommit = await dataManager.Analyze( GetGitTestFolder(),GetRelativeTestFolder());
        secondScanAfterAddedCommit.Should().BeTrue();
    }
    
    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}