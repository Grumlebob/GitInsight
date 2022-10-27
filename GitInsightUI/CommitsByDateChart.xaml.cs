using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GitInsight;

namespace GitInsightUI;

public record NumberOfCommitsOnDate(
    string Date,
    int NumberOfCommits);

public partial class CommitsByDateChart : UserControl
{
    
    public CommitsByDateChart()
    {
        DataContext = this;
        InitCommitCountPerDateList();
        InitializeComponent();
    }
    
    public List<NumberOfCommitsOnDate> CommitCountPerDate { get; } = new();
        
    public static int MaxCommitsPerDate { get; set; } = 0;
    
    public void InitCommitCountPerDateList()
    {
        var dc = GitCommands.GitCommitFrequency();
        foreach (var date in dc)
        {
            CommitCountPerDate.Add(new NumberOfCommitsOnDate(Date: date.Key, NumberOfCommits: date.Value));
        }
        MaxCommitsPerDate = dc.Values.Max();
        Console.WriteLine("maxCommits: " + MaxCommitsPerDate);
    }
    
}