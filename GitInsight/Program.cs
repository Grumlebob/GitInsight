using GitInsight;

if (args.Length > 0)
{
    if (args[0] == "commitfrequency")
    {
        GitCommands.GitCommitFrequency();
        Console.WriteLine(args[0]); //dotnet run --args
    }
    if (args[0] == "commitauthor") 
    {
        GitCommands.GitLogByDateAuthor("Jacob Grum");
        Console.WriteLine(args[0]); //dotnet run --args
    }
}

GitCommands.GitLogByAllAuthorsByDate();
//GitCommands.GetGitTestFolder();



