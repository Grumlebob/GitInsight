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
        public static void GitLog()
        {
            using (var repo = new Repository("C:/Users/jgrum/Documents/Programming/Csharp/GitInsight/.git"))
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
                    Console.WriteLine();
                }
            }
        }

        public static void GitCommitFrequency()
        {
            Console.WriteLine("hello");
        }

        public static void GitCommitAuthor()
        {

        }
    }
}
