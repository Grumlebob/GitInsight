using GitInsight;

namespace GitInsightTest
{
    public class GitCommandsTest : IDisposable
    {
        private readonly Repository _repo;

        public GitCommandsTest()
        {
            _repo = new Repository(GetGitTestFolder());
        }

        [Fact]
        public void Libgit2SharpCommandsTest()
        {
            using var repo = new Repository(GetGitTestFolder());
            repo.Should().NotBeNull();

            // Object lookup
            var obj = repo.Lookup("master^");
            var commit = repo.Lookup<Commit>("8496071c1b46c854b31185ea97743be6a8774479");
            var tree = repo.Lookup<Tree>("master^{tree}");
            var blob = repo.Lookup<Blob>("master:new.txt");

            obj.Should().NotBeNull();
            commit.Should().NotBeNull();
            tree.Should().NotBeNull();
            blob.Should().NotBeNull();

            // Refs
            var reference = repo.Refs["refs/heads/master"];
            var allRefs = repo.Refs.ToList();
            var headCommit = repo.Head.Commits.First();

            reference.Should().NotBeNull();
            allRefs.Should().NotBeNull();
            headCommit.Should().NotBeNull();

            // Branches
            // special kind of reference
            var allBranches = repo.Branches.ToList();
            var branch = repo.Branches["master"];
            //No remote branches in our test git repo
            foreach (var b in allBranches)
            {
                b.IsRemote.Should().BeFalse();
            }

            allBranches.Should().NotBeNull();
            branch.Should().NotBeNull();

            // Tags
            var aTag = repo.Tags["refs/tags/e90810b"];
            var allTags = repo.Tags.ToList();
            aTag.Should().NotBeNull();
            allTags.Should().NotBeEmpty();
        }

        [Fact]
        public void GitLogByAllAuthorsByDateTest()
        {
            var dictionary = GitCommands.GitLogByAllAuthorsByDate();
            dictionary.Should().NotBeEmpty();

            var testAuthor = dictionary["Scott Chacon"];
            var testAuthor2 = dictionary["gor"];

            testAuthor.Count.Should().BeGreaterThan(2);
            testAuthor[0].Author.Name.Should().Be("Scott Chacon");
            testAuthor[0].Sha.Should().Be("be3563ae3f795b2b4353bcce3a527ad0a4f7f644");

            testAuthor2.Count.Should().Be(1);
            testAuthor2[0].Author.Name.Should().Be("gor");
            testAuthor2[0].Sha.Should().Be("4c062a6361ae6959e06292c1fa5e2822d9c96345");
        }

        [Fact]
        public void GitCommitFrequencyTest()
        {
            var dictionary = GitCommands.GitCommitFrequency();
            dictionary.Should().NotBeEmpty();

            var commitsOnTestDate = dictionary["14-04-2011"];
            var commitsOnTestDate2 = dictionary["25-05-2010"];

            commitsOnTestDate.Should().Be(1);
            commitsOnTestDate2.Should().Be(2);
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}