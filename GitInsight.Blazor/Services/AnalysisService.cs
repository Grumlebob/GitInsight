using System.Net.Http.Json;
using GitInsight.Core;
using LibGit2Sharp;

namespace GitInsight.Blazor.Services;

public class AnalysisService : IAnalysisService
{
    private readonly HttpClient _httpClient;

    public AnalysisService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(string repoPath)
    {
        var result= await _httpClient.GetFromJsonAsync<List<CommitsByDateByAuthor>>(repoPath);
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }

    
    public async Task<List<CommitsByDate>> GetCommitsByDate(string repoPath)
    {
        var result = await _httpClient.GetFromJsonAsync<List<CommitsByDate>>(repoPath + "/CommitsByDate");
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }
    
    public async Task<List<RepoInsightDto>> AllRepos()
    {
        var result = await _httpClient.GetFromJsonAsync<List<RepoInsightDto>>("/repoinsights");
        if(result == null) throw new NotFoundException($"No Repositories");
        return result;
    }
    
    public async Task<GitAwardWinner> EarlyBird(string repoPath)
    {
        var result = await _httpClient.GetFromJsonAsync<GitAwardWinner>("repoinsights/" + repoPath + "/EarlyBird");
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }

    public async Task<GitAwardWinner> NightOwl(string repoPath)
    {
        
        var result = await _httpClient.GetFromJsonAsync<GitAwardWinner>("repoinsights/" + repoPath + "/NightOwl");
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }
    public async Task<IEnumerable<ForkDto>> GetForksFromApi(string repoPath)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ForkDto>>($"repoinsights/{repoPath}/forks");
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }
}
