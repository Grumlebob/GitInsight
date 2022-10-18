if (args.Length > 0)
{
    if (args[0].ToLower() == "commitfrequency")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitCommitFrequency(dateformat: DateFormatWithTime, testingMode: Testing);
    }
    else if (args[0].ToLower() == "commitauthor")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitLogByAllAuthorsByDate(dateformat: DateFormatWithTime, testingMode: Testing);
    }
}





