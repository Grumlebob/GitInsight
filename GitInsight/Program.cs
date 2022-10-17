using GitInsight;

Console.WriteLine("Hello, World!");

if (args.Length > 0)
{
    if (args[0] == "commitfrequency")
    {
        GitCommands.GitCommitFrequency();
        Console.WriteLine(args[0]); //dotnet run --args
    }
    if (args[0] == "commitauthor") 
    {
        GitCommands.GitCommitFrequency();
        Console.WriteLine(args[0]); //dotnet run --args
    }
}

GitCommands.GitLog();
