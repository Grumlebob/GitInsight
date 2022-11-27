using GitInsight.Core;
using GitInsight.Entities;

namespace GitInsight;


public class Analysis
{
    private InsightContext _context;

    public Analysis(InsightContext context)
    {
        _context = context;
    }

    public async Task<List<CommitsByDate>> GetCommitsByDate(int repoId, int? authorId = null)
    {
        var commitRepo = new CommitInsightRepository(_context);
        var (commits, response) = await commitRepo.FindByRepoIdAsync(repoId);

        if (authorId != null)
        {
            commits = commits.Where(c => c.AuthorId == authorId).ToList();
        }

        commits = commits.OrderBy(c => c.Date).ToList();

        List<CommitsByDate> commitsByDates = new List<CommitsByDate>();
        DateTime lastDate = commits.First().Date.Date;
        int currentCommits = 0;
        foreach (var commit in commits)
        {
            if (commit.Date.Date == lastDate)
            {
                currentCommits++;
            }
            else
            {
                commitsByDates.Add(new CommitsByDate(lastDate, currentCommits));
                lastDate = commit.Date.Date;
                currentCommits = 1;
            }
        }
        commitsByDates.Add(new CommitsByDate(lastDate, currentCommits));

        return commitsByDates;

    }

    public async Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(int repoId)
    {
        var authorRepo = new AuthorRepository(_context);
        var (authors, _) = await authorRepo.FindByRepoIdAsync(repoId);

        var commitsByauthor = new List<CommitsByDateByAuthor>();
        foreach (var author in authors!)
        {
            commitsByauthor.Add(new CommitsByDateByAuthor(author.Name, await GetCommitsByDate(repoId, author.Id)));
        }

        return commitsByauthor;

    }

    public async Task<GitAwardWinner> HighestCommitterWithinTimeframe(int repoId, DateTime start, DateTime end)
    {
        var commitRepo = new CommitInsightRepository(_context);
        var authorRepo = new AuthorRepository(_context);
        var (commits, response) = await commitRepo.FindByRepoIdAsync(repoId);

        var dictionary = new Dictionary<string, int>();

        var startTime = start - start.Date;
        var endTime = end - end.Date;
        
        foreach (var commit in commits)
        {
            var time = commit.Date - commit.Date.Date;
            if (time >= startTime && time <= endTime)
            {
                var (author,_) = await authorRepo.FindAsync(commit.AuthorId);
                if (!dictionary.ContainsKey(author.Name)) dictionary.Add(author.Name,0);
                dictionary[author.Name] += 1;
            }
        }
        
        KeyValuePair<string,int> currentWinner = new KeyValuePair<string, int>("",0);
        foreach (var keyValue in dictionary)
        {
            if (keyValue.Value > currentWinner.Value) currentWinner = keyValue;
        }

        return new GitAwardWinner(currentWinner.Key, currentWinner.Value);
    }
}

