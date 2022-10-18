namespace GitInsight;

public static class GitCommands
{
    private const string DateFormatNoTime = "dd-MM-yyyy";
    private const string DateRfc2822Format = "ddd dd MMM HH:mm:ss yyyy K";
    private const string DateFormatWithTime = "dd-MM-yyyy HH:mm:ss";

    public static Dictionary<string, List<Commit>> GitLogByAllAuthorsByDate(string dateformat = DateFormatNoTime)
    {
        using var repo = new Repository(GetGitTestFolder());
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

    public static Dictionary<string, int> GitCommitFrequency(string dateformat = DateFormatNoTime)
    {
        using var repo = new Repository(GetGitTestFolder());

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