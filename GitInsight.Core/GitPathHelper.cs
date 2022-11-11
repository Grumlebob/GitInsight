using System.IO.Compression;

namespace GitInsight.Core;

public static class GitPathHelper
{
    
    public static string GetRelativeTestFolder()  => @"/GitInsightTest/TestResources/Unzipped/Testrepo.git";
    public static string GetRelativeLocalFolder()  => @"/.git";
    
    
    public static string GetFullPathFromRelativePath(string relativePath)
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, relativePath);
    }
    
    public static string GetFullPathSourceCodeGit()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath!, GetRelativeLocalFolder());
    }

    public static string GetFullPathTestGit()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath! + GetRelativeTestFolder());
    }

    public static string GetFullPathWhenCalledFromProgram(string relativePath)
    {
        var projectPath = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
        return Path.Combine(projectPath! + relativePath);
    }
    
}