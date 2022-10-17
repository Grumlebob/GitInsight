using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    public class GitCommands
    {
        public static string GitJacobUrl { get; set; } = "C:/Users/jgrum/Documents/Programming/Csharp/ThirdSemesterProjectGitInsight/GitInsight/.git";
        
        public static void GitLog()
        {
            using (var repo = new Repository(GitJacobUrl))
            {
                var RFC2822Format = "ddd dd MMM HH:mm:ss yyyy K";

                
                
                foreach (Commit c in repo.Commits.Take(15))
                {
                
                    Console.WriteLine(string.Format("commit {0}", c.Id));

                    if (c.Parents.Count() > 1)
                    {
                        Console.WriteLine("Merge: {0}",
                            string.Join(" ", c.Parents.Select(p => p.Id.Sha.Substring(0, 7)).ToArray()));
                    }

                    Console.WriteLine(string.Format("Author: {0} <{1}>", c.Author.Name, c.Author.Email));
                    Console.WriteLine("Date:   {0}", c.Author.When.ToString(RFC2822Format, CultureInfo.InvariantCulture));
                    Console.WriteLine();
                    Console.WriteLine(c.Message);
                    Console.WriteLine("author below");
                    Console.WriteLine(c.Author.Name);
                    Console.WriteLine();
                }
            }
        }

        public static void GitLogByAuthor(string author)
        {
            using (var repo = new Repository(GitJacobUrl))
            {
                repo.Commits.QueryBy(new CommitFilter
                        { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time })
                    .Where(c => c.Author.Name == author)
                    .ToList()
                    .ForEach(c => Console.WriteLine(c.Message));

                Dictionary<DateTimeOffset, int> commitsByDate = new Dictionary<DateTimeOffset, int>();
                foreach (Commit c in repo.Commits)
                {
                    if (c.Author.Name == author)
                    {
                        if (commitsByDate.ContainsKey(c.Author.When))
                        {
                            commitsByDate[c.Author.When]++;
                        }
                        else
                        {
                            commitsByDate.Add(c.Author.When, 1);
                        }
                    }
                }

                foreach (var item in commitsByDate)
                {
                    Console.WriteLine(item.Key + " " + item.Value);
                }
            }
        }

        //https://stackoverflow.com/questions/35769003/git-commit-count-per-day

        public static void GitCommitFrequency()
        {
            Console.WriteLine("hello");
        }

        public static void GitCommitAuthor()
        {
            Console.WriteLine("hello");
        }
    }
}
