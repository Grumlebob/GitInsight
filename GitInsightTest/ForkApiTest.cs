namespace GitInsightTest;

public class ForkApiTest
{
    [Fact]
    public async Task GetForksSuccess()
    {
        var res = (await new ForkApi().GetForks("Grumlebob/GitInsight")).ToList();
        var instance = res.First(a => a.owner.login == "AGmarsen" && a.name == "TestFork"); 
        instance.name.Should().Be("TestFork");
        instance.html_url.Should().Be("https://github.com/AGmarsen/TestFork");
        instance.created_at.Second.Should().Be(28);
        instance.created_at.Hour.Should().Be(14);
        instance.owner.login.Should().Be("AGmarsen");
        instance.owner.html_url.Should().Be("https://github.com/AGmarsen");
        instance.owner.avatar_url.Should().Be("https://avatars.githubusercontent.com/u/74052156?v=4");
    }

    [Fact]
    public async Task GetForksHasNoForks()
    {
        (await new ForkApi().GetForks("AGmarsen/Handin-3")).Should().BeEmpty();
    }

    [Fact]
    public async Task GetForksInvalidRepo()
    {
        var sut = new ForkApi();
        await Assert.ThrowsAsync<HttpRequestException>(async () => await sut.GetForks("AGmarsen/invalid/"));
    }
}
