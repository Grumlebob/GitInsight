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
}

