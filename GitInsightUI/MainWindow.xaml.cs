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
    /* complex stuff, trying to simplify before making work.
    public record AuthorWithCommits(
        string Author,
        ObservableCollection<Commit> commits);
        */
    
    public record AuthorWithNumberOfCommits(
        string AuthorName,
        int NumberOfCommits);
    

    public partial class MainWindow : Window
    {
        /* complex stuff, trying to simplify before making work.
        public List<AuthorWithCommits> Authors { get; } = new();
        */

        public List<AuthorWithNumberOfCommits> AllCommits { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitAuthorList();
            foreach (var VARIABLE in AllCommits)
            {
                Console.WriteLine(VARIABLE);
            }
        }

        public void InitAuthorList()
        {
            var dc = GitCommands.GitLogByAllAuthorsByDate();
            foreach (var author in dc)
            {
                /* complex stuff, trying to simplify before making work.
                var authorWithCommits = new AuthorWithCommits(author.Key, new(author.Value));
                Authors.Add(authorWithCommits);
                */

                AllCommits.Add(new AuthorWithNumberOfCommits(AuthorName: author.Key, NumberOfCommits:author.Value.Count));
            }
        }
    }
}