namespace GitInsight;

public static class GitCommands
{
    
    public enum TestingMode
    {
        None,
        Testing,
    }

    private static string GetPath(TestingMode testingMode = None)
    {
        return testingMode == Testing ? GetGitTestFolder() : GetGitLocalFolder();
    } 
    
    /*
Marie Beaumin
      1 2017-12-08
      6 2017-12-26
     12 2018-01-01
     13 2018-01-02
     10 2018-01-14
      7 2018-01-17
      5 2018-01-18 

Maxime Kauta
      5 2017-12-06
      3 2017-12-07
      1 2018-01-01
     */
    public static Dictionary<string, List<Commit>> GitLogByAllAuthorsByDate(string dateformat = DateFormatNoTime, TestingMode testingMode = None)
    {
        
        using var repo = new Repository(GetPath(testingMode));
        
        repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time });

        var commitsByAuthor = new Dictionary<string, List<Commit>>();
        var commitsByDate = new Dictionary<string, int>();

        foreach (Commit commit in repo.Commits)
        {
            if (commitsByAuthor.ContainsKey(commit.Author.Name))
            {
                commitsByAuthor[commit.Author.Name].Add(commit);
            }
            else
            {
                commitsByAuthor.Add(commit.Author.Name, new List<Commit> { commit });
            }
        }

        foreach (var item in commitsByAuthor)
        {
            //CommitsByAuthor:
            //Key = Author
            //Value = List of commits
            Console.WriteLine("Number of Commits | Date");
            Console.WriteLine("Author: " + item.Key);

            foreach (var commit in item.Value)
            {
                //value = Commit
                var dateFormatted = commit.Author.When.ToString(dateformat, CultureInfo.InvariantCulture);
                if (commitsByDate.ContainsKey(dateFormatted))
                {
                    commitsByDate[dateFormatted]++;
                }
                else
                {
                    commitsByDate.Add(dateFormatted, 1);
                }
            }

            foreach (var date in commitsByDate)
            {
                Console.WriteLine(date.Value + " | " + date.Key);
            }
        }

        return commitsByAuthor;
    }
    
    /*
      1 2017-12-08
      6 2017-12-26
     12 2018-01-01
     13 2018-01-02
     10 2018-01-14
      7 2018-01-17
      5 2018-01-18 
     */

    public static Dictionary<string, int> GitCommitFrequency(string dateformat = DateFormatNoTime,TestingMode testingMode = None)
    {
        using var repo = new Repository(GetPath(testingMode));

        repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time });

        var commitsByDate = new Dictionary<string, int>();

        foreach (Commit c in repo.Commits)
        {
            var whenFormatted = c.Author.When.ToString(dateformat, CultureInfo.InvariantCulture);
            if (commitsByDate.ContainsKey(whenFormatted))
            {
                commitsByDate[whenFormatted]++;
            }
            else
            {
                commitsByDate.Add(whenFormatted, 1);
            }
        }

        foreach (var item in commitsByDate)
        {
            Console.WriteLine(item.Value + " " + item.Key);
        }

        return commitsByDate;
    }
}