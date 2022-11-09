using System.IO.Compression;

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
        var prefix = @"GitInsight/";
        return prefix + s;
    }

    public static string GetGitTestFolder()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectPath! + GetRelativeTestFolder());
    }
    
    public static string GetRelativeTestFolder()  => @"/GitInsightTest/TestResources/Unzipped/Testrepo.git";
    public static void EnsureZipIsUnzipped() //Works everywhere but in Testfolder
    {
        //Todo lav pathing også her
        if (!Directory.Exists("../../GitInsight/GitInsightTest/TestResources/Unzipped/Testrepo.git"))
        {
            ZipFile.ExtractToDirectory(
                "../../GitInsight/GitInsightTest/TestResources/Zipped/Testrepo.git.zip", 
                "../../GitInsight/GitInsightTest/TestResources/Unzipped/");
        }
        else
        {
            Console.WriteLine("Testrepo.git already unzipped");
        }
    }
    
    public static void EnsureZipIsUnzippedTesting()
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        
        string unzippedFolder = Path.Combine(projectPath! + GetRelativeTestFolder());
        string zippedFolder = Path.Combine(projectPath! + @"/GitInsightTest/TestResources/Zipped/Testrepo.git.zip");
        string destinationFolder = Path.Combine(projectPath! + @"/GitInsightTest/TestResources/Unzipped/");
        
        if (!Directory.Exists(unzippedFolder))
        {
            ZipFile.ExtractToDirectory(
                zippedFolder, 
                destinationFolder);
            Console.WriteLine("Testrepo.git unzipped");
        }
        else
        {
            Console.WriteLine("Testrepo.git already unzipped");
        }
    }
}