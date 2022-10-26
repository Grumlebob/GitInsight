namespace GitInsight;

public static class GitCommands
{

    public static string SpecifiedPath = string.Empty;
    public static readonly Dictionary<string, List<Commit>> AuthorLog = GitLogByAllAuthorsByDate();
    public static readonly Dictionary<DateTimeOffset, int> CommitFrequency = GitCommitFrequency(Testing);

    public enum TestingMode
    {
        None,
        Testing,
    }

    private static string GetPath(TestingMode testingMode = None)
    {
        if (!string.IsNullOrEmpty(SpecifiedPath))
        {
            return SpecifiedPath;
        }

        return testingMode == Testing ? GetGitTestFolder() : GetGitLocalFolder();
    }

    public static Dictionary<string, List<Commit>> GitLogByAllAuthorsByDate(TestingMode testingMode = None)
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

        return commitsByAuthor;
    }


    public static Dictionary<DateTimeOffset, int> GitCommitFrequency(TestingMode testingMode = None)
    {
        using var repo = new Repository(GetPath(testingMode));
        var commitsByDate = new Dictionary<DateTimeOffset, int>();

        foreach (var commit in repo.Commits)
        {

            var date = commit.Author.When.Date;
            if (commitsByDate.ContainsKey(date))
            {
                // Console.WriteLine(when+" Added one");
                commitsByDate[date]++;
            }
            else
            {
                // Console.WriteLine(when+" created! linked to "+commit.Author.Name );
                commitsByDate.Add(date, 1);
            }
        }

        return commitsByDate;
    }

    public static void PrintCommitFrequency(string dateformat = DateFormatNoTime)
    {
        foreach (var item in CommitFrequency)
        {
            Console.WriteLine(item.Value + " " + item.Key.ToString(dateformat));
        }
    }

    public static void PrintAuthorCommitsByDate(string dateformat = DateFormatNoTime)
    {
        foreach (var item in AuthorLog)
        {
            var commitsByDate = new Dictionary<DateTime, int>();
            //they might already be sorted, but just in case
            item.Value.Sort((q,p)=>q.Author.When.CompareTo(p.Author.When));
            foreach (var commit in item.Value)
            {
                var date = commit.Author.When.Date;
                if (commitsByDate.ContainsKey(date))
                {
                    commitsByDate[date]++;
                }
                else
                {
                    commitsByDate.Add(date, 1);
                }
            }


            Console.WriteLine(item.Key);
            foreach (var res in commitsByDate)
            {
                Console.WriteLine($"\t{res.Value} | {res.Key.ToString(dateformat)}");
            }
        }
    }
}
