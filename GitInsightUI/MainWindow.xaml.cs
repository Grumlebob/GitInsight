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
    

    public partial class MainWindow : Window
    {

        public List<AuthorWithNumberOfCommits> CommitCountPerAuthor { get; } = new();

        public List<NumberOfCommitsOnDate> CommitCountPerDate { get; } = new();
        
        public static int MaxCommitsPerDate { get; set; } = 0;
        
        public static int MaxCommitsPerAuthor { get; set; } = 0;

        public string TabItem1Icon { get; }
        public string TabItem2Icon { get; }

        
        public MainWindow()
        {
            TabItem1Icon = ImagePathBuilder.Build("TabIcons/Sample_User_Icon.png");
            TabItem2Icon = ImagePathBuilder.Build("TabIcons/date.png");
            DataContext = this;
            InitAuthorList();
            InitializeComponent();
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
}