﻿using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Branch = GitInsight.Entities.Branch;

namespace GitInsightTest;

public class RepositoryRepositoryTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly InsightContext _context;

    public RepositoryRepositoryTest()
    {
        (_connection, _context) = SetupTests.Setup();
    }

    [Fact]
    public void SimpleRepoObject()
    {
        GitInsight.Entities.Repository a = new GitInsight.Entities.Repository();
        a.Id = 1;
        a.Name = "Repository,repo of the repos.";
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}