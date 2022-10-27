using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GitInsight;

namespace GitInsightUI;

public record AuthorWithNumberOfCommits(
    string AuthorName,
    int NumberOfCommits);

public partial class CommitsByAuthorChart : UserControl
{
    public List<AuthorWithNumberOfCommits> CommitCountPerAuthor { get; } = new();
        
    public static int MaxCommitsPerAuthor { get; set; } = 0;

    public CommitsByAuthorChart()
    {
        DataContext = this;
        InitializeComponent();
        InitAuthorList();
    }
    
    public void InitAuthorList()
    {
        var dc = GitCommands.GitLogByAllAuthorsByDate();
            
        foreach (var author in dc)
        {
            CommitCountPerAuthor.Add(new AuthorWithNumberOfCommits(AuthorName: author.Key, NumberOfCommits:author.Value.Count));
        }
        MaxCommitsPerAuthor = CommitCountPerAuthor.Max(x => x.NumberOfCommits);
        Console.WriteLine("maxCommits: " + MaxCommitsPerAuthor);
    }
}