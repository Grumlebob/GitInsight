namespace GitInsight;

public static class GitPathHelper
{
    public static string GetGitLocalFolder()
    {
        var projectPath =
            Path.Combine(Directory.GetParent(typeof(GitCommands).GetTypeInfo().Assembly.Location)?.FullName!);
        var rootFolder = projectPath.Substring(0, projectPath.IndexOf("\\GitInsight\\", StringComparison.Ordinal) + 12);

        return Path.Combine(rootFolder, ".git");
    }

    public static string GetGitTestFolder()
    {
        /* Virker ikke med github linter.
        var projectPath =
            Path.Combine(Directory.GetParent(typeof(GitCommands).GetTypeInfo().Assembly.Location)?.FullName!);
        var rootFolder = projectPath.Substring(0, projectPath.IndexOf("\\GitInsight\\", StringComparison.Ordinal) + 12);

        return Path.Combine(rootFolder, "GitInsightTest/Testrepo.git");
        */
        
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
        return Path.Combine(projectPath, "GitInsightTest/Testrepo.git");
    }
}