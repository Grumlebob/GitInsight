namespace GitInsightTest;

public class GitPathHelperTest
{
    [Fact]
    public void TestGetRelativeTestFolder()
    {
        var result = GetRelativeTestFolder();
        var expected = @"/GitInsightTest/TestResources/Unzipped/Testrepo.git";
        result.Should().Be(expected);
    }

    [Fact]
    public void TestGetRelativeLocalFolder()
    {
        var result = GetRelativeLocalFolder();
        var expected = @"/.git";
        result.Should().Be(expected);
    }

    [Fact]
    public void TestGetRelativeSavedRepositoriesFolder()
    {
        var result = GetRelativeSavedRepositoriesFolder();
        var expected = @"/GitInsight.Data/SavedRepositories";
        result.Should().Be(expected);
    }

    [Fact]
    public void TestGetGitLocalFolder()
    {
        var result = GetGitLocalFolder();
        var expected = @"/.git";
        result.Should().Be(expected);
    }

    [Fact]
    public void TestGetGitTestFolder()
    {
        var result = GetGitTestFolder();
        result.Should().Contain(GetRelativeTestFolder());
    }

    [Fact]
    public void TestGetFullPathTestGit()
    {
        var result = GetFullPathTestGit();
        result.Should().Contain(GetRelativeTestFolder());
    }

    [Fact]
    public void TestGetSavedRepositoriesFolder()
    {
        var result = GetSavedRepositoriesFolder();
        result.Should().Contain(GetRelativeSavedRepositoriesFolder());
    }

    [Fact]
    public void TestGetFullPathWhenCalledFromProgram()
    {
        var result = GetFullPathWhenCalledFromProgram("");
        result.Should().BeOfType<string>();
    }
}