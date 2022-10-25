using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public record AuthorWithCommits(
        string Author,
        ObservableCollection<Commit> commits);
    
    public partial class MainWindow : Window
    {
        public List<AuthorWithCommits> Authors { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitAuthorList();
        }

        public void InitAuthorList()
        {
            var dc = GitCommands.GitLogByAllAuthorsByDate();
            foreach (var author in dc)
            {
                var authorWithCommits = new AuthorWithCommits(author.Key, new(author.Value));
                Authors.Add(authorWithCommits);
            }
            
        }
    }
}