using System.Net.Http.Json;
using GitInsight.Core;

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
}