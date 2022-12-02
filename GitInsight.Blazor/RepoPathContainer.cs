namespace GitInsight.Blazor;

public static class RepoPathContainer
{
    public static List<string> RepositoryPaths { get; set; } = new();
    public static string? PickedRepositoryPath { get; set; }

}