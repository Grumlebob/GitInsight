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

using var repo = new Repository(GetGitTestFolder());

var aTag = repo.Tags["refs/tags/e90810b"];
var allTags = repo.Tags.ToList();

foreach (var t in allTags)
{
    Console.WriteLine(t.CanonicalName);
}