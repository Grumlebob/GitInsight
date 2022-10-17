using FluentAssertions;
using LibGit2Sharp;


namespace GitInsightTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            Console.WriteLine("Commit Test");
            Assert.Equal(1, 1);

            
            using (var repo = new Repository("path\to\repo.git"))
            {
                /*
                // Object lookup
                var obj = repo.Lookup("sha");
                var commit = repo.Lookup<Commit>("sha");
                var tree = repo.Lookup<Tree>("sha");
                var tag = repo.Lookup<Tag>("sha");
                
                
                // Rev walking
                foreach (var c in repo.Commits.Walk("sha")) { }
                var commits = repo.Commits.StartingAt("sha").Where(c => c).ToList();
                var sortedCommits = repo.Commits.StartingAt("sha").SortBy(SortMode.Topo).ToList();

                // Refs
                var reference = repo.Refs["refs/heads/master"];
                var allRefs = repo.Refs.ToList();
                foreach (var c in repo.Refs["HEAD"].Commits) { }
                foreach (var c in repo.Head.Commits) { }
                var headCommit = repo.Head.Commits.First();
                var allCommits = repo.Refs["HEAD"].Commits.ToList();
                var newRef = repo.Refs.CreateFrom(reference);
                var anotherNewRef = repo.Refs.CreateFrom("sha");
                

                // Branches
                // special kind of reference
                var allBranches = repo.Branches.ToList();
                var branch = repo.Branches["master"];
                var remoteBranch = repo.Branches["origin/master"];
                

                allBranches.Should().NotBeNull();
                branch.Should().NotBeNull();
                remoteBranch.Should().NotBeNull();


                // Tags
                var aTag = repo.Tags["refs/tags/v1.0"];
                var allTags = repo.Tags.ToList();
                
                aTag.Should().NotBeNull();
                allTags.Should().NotBeEmpty();

*/
            }
        }
    }
}