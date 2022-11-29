using System.Net.Http.Json;
using GitInsight.Core;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authorization;

namespace GitInsight.Web.Services;

public class AnalysisService : IAnalysisService
{
    private readonly HttpClient _httpClient;

    public AnalysisService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(string repoPath)
    {
        return await _httpClient.GetFromJsonAsync<List<CommitsByDateByAuthor>>(repoPath);
    }

    
    public async Task<List<CommitsByDate>> GetCommitsByDate(string repoPath)
    {
        return await _httpClient.GetFromJsonAsync<List<CommitsByDate>>(repoPath + "/CommitsByDate");
    }
    
    public async Task<GitAwardWinner> EarlyBird(string repoPath)
    {
        return await _httpClient.GetFromJsonAsync<GitAwardWinner>("repoinsights/" + repoPath + "/EarlyBird");
    }

    public async Task<GitAwardWinner> NightOwl(string repoPath)
    {
        return await _httpClient.GetFromJsonAsync<GitAwardWinner>("repoinsights/" + repoPath + "/NightOwl");
    }
    public async Task<IEnumerable<ForkDto>> GetForksFromApi(string repoPath)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ForkDto>>($"repoinsights/{repoPath}/forks");
        if(result == null) throw new NotFoundException($"No forks found for {repoPath}");
        return result;
    }
}
