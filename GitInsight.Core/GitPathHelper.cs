using System.IO.Compression;

namespace GitInsight.Core;

public static class GitPathHelper
{
    
    public static string GetRelativeTestFolder()  => @"/GitInsightTest/TestResources/Unzipped/Testrepo.git";
    public static string GetRelativeLocalFolder()  => @"/.git"; //måske tilføj @"GitInsight/";
    
    public static string GetGitLocalFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, GetRelativeLocalFolder());
    }

    public static string GetGitTestFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath! + GetRelativeTestFolder());
    }

    public static string GetFullPathWhenCalledFromProgram(string relativePath)
    {
        var projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        return Path.Combine(projectPath! + relativePath);
    }
    
}