namespace GitInsightUI;

public class ImagePathBuilder
{
    public static string prefix { get; set; } = "Images/";

    public static string Build(string relativePath) => $"{prefix}{relativePath}";

}