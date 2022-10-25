using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitInsight;
using LibGit2Sharp;

namespace GitInsightUI
{
    public record AuthorWithNumberOfCommits(
        string AuthorName,
        int NumberOfCommits);
    
    public record NumberOfCommitsOnDate(
        string Date,
        int NumberOfCommits);
    

    public partial class MainWindow : Window
    {

        public List<AuthorWithNumberOfCommits> AllCommits { get; } = new();

        public List<NumberOfCommitsOnDate> CommitCountPerDate { get; } = new();

        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitAuthorList();
            InitCommitCountPerDateList();
        }

        public void InitAuthorList()
        {
            var dc = GitCommands.GitLogByAllAuthorsByDate();
            foreach (var author in dc)
            {
                AllCommits.Add(new AuthorWithNumberOfCommits(AuthorName: author.Key, NumberOfCommits:author.Value.Count));
            }
            
        }

        public void InitCommitCountPerDateList()
        {
            var dc = GitCommands.GitCommitFrequency();
            foreach (var date in dc)
            {
                CommitCountPerDate.Add(new NumberOfCommitsOnDate(Date: date.Key, NumberOfCommits: date.Value));
            }
        }
    }
}