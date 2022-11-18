namespace GitInsight.Core;

public record CommitsByDate(DateTime Date, int Commits);

public record CommitsByDateByAuthor(string AuthorName, List<CommitsByDate> CommitsByDates);