namespace GitInsight;

public static class GitPathHelper
{
    public static string GetGitLocalFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, ".git");
    }

    public static string GetGitTestFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, "GitInsightTest/Testrepo.git");
    }
}