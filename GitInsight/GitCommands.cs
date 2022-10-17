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

        
        public static void GitLogByDateAuthor(string author)
        {
            var dateformat = "dd-MM-yyyy";

            Console.WriteLine(author);
            
            using (var repo = new Repository(GitJacobUrl))
            {
                repo.Commits.QueryBy(new CommitFilter
                        { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time })
                    .Where(c => c.Author.Name == author)
                    .ToList();
                
                Dictionary<string, int> commitsByDate = new Dictionary<string, int>();
                
                foreach (Commit c in repo.Commits)
                {
                    var whenFormatted = c.Author.When.ToString(dateformat, CultureInfo.InvariantCulture);
                    //Console.WriteLine(whenFormatted);
                   
                    if (c.Author.Name == author)
                    {
                        if (commitsByDate.ContainsKey(whenFormatted))
                        {
                            commitsByDate[whenFormatted]++;
                        }
                        else
                        {
                            commitsByDate.Add(whenFormatted, 1);
                        }
                    }
                }
                foreach (var item in commitsByDate)
                {
                    Console.WriteLine(item.Value + " " + item.Key);
                }
            }
        }
        
        public static void GitLogByAllAuthorsByDate()
        {
            var dateformat = "dd-MM-yyyy";
            
            using (var repo = new Repository(GitJacobUrl))
            {
                repo.Commits.QueryBy(new CommitFilter
                        { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time })
                    .Where(c => c.Author.Name == c.Author.Name)
                    .ToList();
                
                Dictionary<string, List<Commit>> commitsByAuthor = new Dictionary<string, List<Commit>>();
                
                foreach (Commit commit in repo.Commits)
                {
                    if (commitsByAuthor.ContainsKey(commit.Author.Name))
                    {
                        commitsByAuthor[commit.Author.Name].Add(commit);
                    }
                    else
                    {
                        commitsByAuthor.Add(commit.Author.Name, new List<Commit> {commit});
                    }
                    
                }
                
                foreach (var item in commitsByAuthor)
                {
                    //Key = Author
                    //Value = List of commits
                    Console.WriteLine(item.Key);
                    
                    Dictionary<string, int> commitsByDate = new Dictionary<string, int>();
                    
                    foreach (var commit in item.Value)
                    {
                        var whenFormatted = commit.Author.When.ToString(dateformat, CultureInfo.InvariantCulture);
                        if (commitsByDate.ContainsKey(whenFormatted))
                        {
                            commitsByDate[whenFormatted]++;
                        }
                        else
                        {
                            commitsByDate.Add(whenFormatted, 1);
                        }
                    }
                    foreach (var date in commitsByDate)
                    {
                        Console.WriteLine(date.Value + " " + date.Key);
                    }
                }
            }
        }
        
        //Base example from website:
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
        
        public static void GitCommitFrequency()
        {
            throw new NotImplementedException();
        }
    }
}
