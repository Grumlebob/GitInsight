namespace GitInsight.Core;

public static class GitPathHelper
{
    public static string GetGitLocalFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, ".git");
    }

    public static string GetRelativeGitFolder(string s)
    {
        var prefix = @"GitInsight\";
        return prefix + s;
    }

    public static string GetGitTestFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, @"GitInsightTest\Testrepo.git");
    }
}