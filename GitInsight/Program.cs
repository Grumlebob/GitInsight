if (args.Length > 0)
{
    //If user specifies a custom path regardless of order
    if (args.Length > 1)
    {
        if (args[0].StartsWith("commit"))
        {
            CommandLineSpecifiedPath = args[1];
        }
        else
        {
            CommandLineSpecifiedPath = args[0];
        }
    }
    if (args[0].ToLower() == "commitfrequency" || args[1].ToLower() == "commitfrequency")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitCommitFrequency(dateformat: DateFormatNoTime, testingMode: None);
    }
    else if (args[0].ToLower() == "commitauthor" || args[1].ToLower() == "commitauthor" )
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitLogByAllAuthorsByDate(dateformat: DateFormatNoTime, testingMode: None);
    }
}

