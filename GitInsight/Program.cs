using GitInsight;
using GitInsight.Data;
using GitInsight.Entities;

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
    if (args.Contains("commitfrequency"))
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitCommitFrequency(dateformat: DateFormatNoTime, pathing: SourceCode);
    }
    else if (args.Contains("commitauthor") )
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        GitLogByAllAuthorsByDate(dateformat: DateFormatNoTime, pathing: SourceCode);
    }
    
}

DataManager dm = new DataManager(new InsightContextFactory().CreateDbContext(args));
    await dm.Analyze( GetGitLocalFolder(),GetRelativeGitFolder(".git"));