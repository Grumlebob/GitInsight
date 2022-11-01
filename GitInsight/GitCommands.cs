namespace GitInsight;

public static class GitCommands
{
    public static string SpecifiedPath = string.Empty;
    public static Dictionary<string, List<Commit>> AuthorLog = GitLogByAllAuthorsByDate();
    public static Dictionary<DateTimeOffset, List<DateTimeOffset>> CommitFrequency = GitCommitFrequency();
    public static Dictionary<string, List<string>> FrequencyFormat = GitFrequencyFormat();

    public enum Pathing
    {
        SourceCode,
        TestRepository,
    }

    public static void SetMode(Pathing pathing = SourceCode)
    {
        AuthorLog = GitLogByAllAuthorsByDate(pathing);
        CommitFrequency = GitCommitFrequency(pathing);
        FrequencyFormat = GitFrequencyFormat(pathing: pathing);
    }

    private static string GetPath(Pathing pathing = SourceCode)
    {
        if (!string.IsNullOrEmpty(SpecifiedPath))
        {
            return SpecifiedPath;
        }

        return pathing == TestRepository ? GetGitTestFolder() : GetGitLocalFolder();
    }

    public static Dictionary<string, List<Commit>> GitLogByAllAuthorsByDate(Pathing pathing = SourceCode)
    {
        using var repo = new Repository(GetPath(pathing));

        repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = repo.Head, SortBy = CommitSortStrategies.Time });

        var commitsByAuthor = new Dictionary<string, List<Commit>>();

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


    public static Dictionary<DateTimeOffset, List<DateTimeOffset>> GitCommitFrequency(Pathing pathing = SourceCode)
    {
        using var repo = new Repository(GetPath(pathing));
        var commitsByDate = new Dictionary<DateTimeOffset, List<DateTimeOffset>>();

        foreach (var commit in repo.Commits)
        {
            var date = commit.Author.When.Date;
            var when = commit.Author.When;
            if (commitsByDate.ContainsKey(date))
            {
                commitsByDate[date].Add(when);
            }
            else
            {
                // Console.WriteLine(when+" created! linked to "+commit.Author.Name );
                commitsByDate.Add(date, new List<DateTimeOffset> { when });
            }
        }
        return commitsByDate;
    }

    public static Dictionary<string, List<string>> GitFrequencyFormat(string dateformat = DateFormatNoTime, Pathing pathing = SourceCode )
    {
        using var repo = new Repository(GetPath(pathing));

        var formatted = new Dictionary<string, List<string>>();
        foreach (var item in CommitFrequency)
        {
            var formattedDate = item.Key.ToString(DateFormatNoTime);
            var formattedCommits = item.Value.Select(c => c.ToString(dateformat, CultureInfo.InvariantCulture)).ToList();
            formatted.Add(formattedDate, formattedCommits);

        }
        return formatted;
    }

    public static void PrintCommitFrequency()
    {
        foreach (var item in FrequencyFormat)
        {
            Console.WriteLine(item.Value.Count + "\t" + item.Key);
        }
    }

    public static void PrintAuthorCommitsByDate(string dateformat = DateFormatNoTime)
    {
        foreach (var item in AuthorLog)
        {
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
