namespace GitInsight.Core;

public record CommitsByDate(DateTime Date, int CommitAmount);
public class CommitsByDateFormatted
{
    public string Date { get; set; } = null!;
    public int CommitAmount { get; set; }
}

// public record CommitsByAuthor(string Author, int CommitAmount);
public record CommitsByDateByAuthor(string AuthorName, List<CommitsByDate> CommitsByDates);

public record GitAwardWinner(string WinnerName, float Value);
