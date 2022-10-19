if (args.Length > 0)
{
    if (args[0].ToLower() == "commitfrequency")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitCommitFrequency(dateformat: DateFormatNoTime, testingMode: None);
    }
    else if (args[0].ToLower() == "commitauthor")
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitLogByAllAuthorsByDate(dateformat: DateFormatNoTime, testingMode: None);
    }
}

string RunningPath = AppDomain.CurrentDomain.BaseDirectory;

var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

Console.WriteLine(projectPath);

