using GitInsight.Core;

namespace GitInsight.Blazor.Services;

public interface IAnalysisService
{
    Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(string repoPath);
    Task<GitAwardWinner> EarlyBird(string repoPath);
    Task<List<CommitsByDate>> GetCommitsByDate(string repoPath);
    public Task<GitAwardWinner> NightOwl(string repoPath);
    Task<List<RepoInsightDto>> AllRepos();

    Task<IEnumerable<ForkDto>> GetForksFromApi(string repoPath);
}
