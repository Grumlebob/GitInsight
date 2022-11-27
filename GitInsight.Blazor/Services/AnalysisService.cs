using System.Net.Http.Json;
using GitInsight.Core;
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
    
    
    public async Task<GitAwardWinner> EarlyBird(string repoPath)
    {
        return await _httpClient.GetFromJsonAsync<GitAwardWinner>("repoinsights/" + repoPath + "/EarlyBird");
    }
}