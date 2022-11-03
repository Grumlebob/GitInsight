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

var insightContextFactory = new InsightContextFactory();
AuthorRepository ap = new AuthorRepository(insightContextFactory.CreateDbContext(args));


var (authorDto, response) = await ap.FindAuthorAsync(5);

Console.WriteLine("res: "+ authorDto +"response:" + response);