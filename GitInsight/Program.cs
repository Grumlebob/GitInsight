using static GitInsight.GitCommands;

if (args.Length > 0)
{
    if (args[0].ToLower() == "commitfrequency")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitCommitFrequency();
    }
    else if (args[0].ToLower() == "commitauthor")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitLogByAllAuthorsByDate();
    }
}


//Uncomment to check your git folder is located correctly:
//Console.WriteLine(GitCommands.GetGitLocalFolder());
//Console.WriteLine(GitCommands.GetGitTestFolder());




