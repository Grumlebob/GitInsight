if (args.Length > 0)
{
    //If user specifies a custom path regardless of order
    if (args.Length > 1)
    {
        if (args[0].StartsWith("commit"))
        {
            SpecifiedPath = args[1];
        }
        else
        {
            SpecifiedPath = args[0];
        }
    }
    if (args.Contains("commitfrequency"))
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        PrintCommitFrequency();
    }
    else if (args.Contains("commitauthor") )
    {
        Console.WriteLine($"{args[0]} mode:"); //dotnet run --args
        PrintAuthorCommitsByDate();
    }



}
