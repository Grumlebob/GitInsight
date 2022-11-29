using GitInsight;

namespace GitInsightTest;

public class ForkApiTest
{
    [Fact]
    public async Task GetForksSuccess()
    {
        var res = (await new ForkApi().GetForks("OliFryser/GitInsightTestRepo_1Fork")).ToList();
        res.Count().Should().Be(1);
        var instance = res.First();
        instance.name.Should().Be("ForkedRepo");
        instance.html_url.Should().Be("https://github.com/A-Guldborg/ForkedRepo");
        instance.created_at.Second.Should().Be(18);
        instance.owner.login.Should().Be("A-Guldborg");
        instance.owner.html_url.Should().Be("https://github.com/A-Guldborg");
        instance.owner.avatar_url.Should().Be("https://avatars.githubusercontent.com/u/95026056?v=4");
    }

    [Fact]
    public async Task GetForksHasNoForks()
    {
        (await new ForkApi().GetForks("AGmarsen/Handin-3")).Should().BeEmpty();
    }

    [Fact]
    public async Task GetForksInvalidRepo()
    {
        (await new ForkApi().GetForks("AGmarsen/NotValid")).Should().BeEmpty();
    }
}
