﻿using System.Net;
using GitInsight.Core;

namespace GitInsight;

public class ForkApi
{

    private readonly HttpClient _client;
    public ForkApi()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.github.com/");
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        //If user secrets is null, use env variable.
        var token = config["GitCredentials:Token"] ?? Environment.GetEnvironmentVariable("api-key"); 
        //dotnet user-secrets set "GitCredentials:Token" "tokenString" //env var defined in buildAndTest.yml
        
        _client.DefaultRequestHeaders.Add("User-Agent", "net" + Environment.Version.ToString());
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    }
    
    /// <param name="repo">"owner/repoName"</param>
    /// <returns></returns>
    
    public async Task<IEnumerable<ForkDto>> GetForks(string repo)
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<ForkDto>>($"repos/{repo}/forks");
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode != HttpStatusCode.NotFound) throw;
            return new List<ForkDto>();
        }
    }
}