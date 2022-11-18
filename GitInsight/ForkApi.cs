namespace GitInsight;

public class ForkApi //move me when blazor is added
{

    private HttpClient _client;
    public ForkApi()
    {

        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.github.com/");
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var token = config["GitCredentials:Token"]; //dotnet user-secrets set "GitCredentials:Token" "tokenString"
        
        _client.DefaultRequestHeaders.Add("User-Agent",".Net 6.0 windows");
        _client.DefaultRequestHeaders.Add("Authorization", token);
    }
    
    /// <param name="repo">"owner/repoName"</param>
    /// <returns></returns>
    
    public async Task<int> GetForks(string repo)
    {
        //If we want more info from the forks we can make our own struct that saves relevant values instead of "Object"
        var res = await _client.GetFromJsonAsync<IEnumerable<Object>>($"repos/{repo}/forks");
        return res.Count();
    }
}