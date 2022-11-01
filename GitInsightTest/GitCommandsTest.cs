using GitInsight;
using static GitInsight.DateFormats;
using static GitInsight.GitCommands.Pathing;

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
        public void LibGit2SharpRepositoryCommandTest()
        {
            _repo.Should().NotBeNull();

            // Object lookup
            var obj = _repo.Lookup("master^");
            var commit = _repo.Lookup<Commit>("8496071c1b46c854b31185ea977");
            var tree = _repo.Lookup<Tree>("master^{tree}");
            var blob = _repo.Lookup<Blob>("master:new.txt");

            obj.Should().NotBeNull();
            commit.Should().NotBeNull();
            tree.Should().NotBeNull();
            blob.Should().NotBeNull();

            // Refs
            var reference = _repo.Refs["refs/heads/master"];
            var allRefs = _repo.Refs.ToList();
            var headCommit = _repo.Head.Commits.First();

            reference.Should().NotBeNull();
            allRefs.Should().NotBeNull();
            headCommit.Should().NotBeNull();

            // Branches
            // special kind of reference
            var allBranches = _repo.Branches.ToList();
            var branch = _repo.Branches["master"];
            //No remote branches in our test git repo
            foreach (var b in allBranches)
            {
                b.IsRemote.Should().BeFalse();
            }

            allBranches.Should().NotBeNull();
            branch.Should().NotBeNull();

            // Tags
            var aTag = _repo.Tags["refs/tags/tag_without_tagger"];
            var allTags = _repo.Tags.ToList();
            allTags.Should().NotBeEmpty();
            aTag.Should().NotBeNull();
        }

        [Fact]
        public void GitLogByAllAuthorsByDateTest()
        {
            var dictionary = GitCommands.GitLogByAllAuthorsByDate(pathing: TestRepository);
            dictionary.Should().NotBeEmpty();

            var testAuthor = dictionary["Scott Chacon"];
            var testAuthor2 = dictionary["gor"];
            
            _repo.Commits.Count().Should().Be(testAuthor.Count + testAuthor2.Count);

            testAuthor.Count.Should().BeGreaterThan(2);
            testAuthor[0].Author.Name.Should().Be("Scott Chacon");
            testAuthor[0].Sha.Should().Contain("be3563ae3f795b2b");

            testAuthor2.Count.Should().Be(1);
            testAuthor2[0].Author.Name.Should().Be("gor");
            testAuthor2[0].Sha.Should().Contain("4c062a6361ae6959e");
        }

        [Fact]
        public void GitCommitFrequencyDateFormatNoTimeTest()
        {
            var dictionary = GitCommands.GitCommitFrequency(dateformat: DateFormatNoTime, pathing: TestRepository);
            dictionary.Should().NotBeEmpty();

            var commitsOnTestDate = dictionary["14-04-2011"];
            var commitsOnTestDate2 = dictionary["25-05-2010"];

            commitsOnTestDate.Should().Be(1);
            commitsOnTestDate2.Should().Be(2);
        }
        
        [Fact]
        public void GitCommitFrequencyDateRfc2822FormatTest()
        {
            var dateRfc2822Format = GitCommands.GitCommitFrequency(dateformat: DateFormatRfc2822, pathing: TestRepository);
            dateRfc2822Format.Should().NotBeEmpty();
            

            var commitsOnTestDate = dateRfc2822Format["Thu 14 Apr 18:44:16 2011 +03:00"];
            var commitsOnTestDate2 = dateRfc2822Format["Tue 25 May 11:58:27 2010 -07:00"];

            commitsOnTestDate.Should().Be(1);
            commitsOnTestDate2.Should().Be(1);
        }
        
        [Fact]
        public void GitCommitFrequencyDateFormatWithTimeTest()
        {
            var dateRfc2822Format = GitCommands.GitCommitFrequency(dateformat: DateFormatWithTime, pathing: TestRepository);
            dateRfc2822Format.Should().NotBeEmpty();
            

            var commitsOnTestDate = dateRfc2822Format["14-04-2011 18:44:16"];
            var commitsOnTestDate2 = dateRfc2822Format["14-04-2011 18:44:16"];

            commitsOnTestDate.Should().Be(1);
            commitsOnTestDate2.Should().Be(1);
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}