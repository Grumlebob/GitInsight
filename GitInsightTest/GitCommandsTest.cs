using System.Reflection;
using FluentAssertions;
using GitInsight;
using LibGit2Sharp;
using Xunit.Abstractions;


namespace GitInsightTest
{
    public class UnitTest1
    {
        
        [Fact]
        public void GitCommandsTest()
        { 
            
            using (var repo = new Repository(GitCommands.GetGitLocalFolder()))
            {
                
                // Object lookup
                var obj = repo.Lookup("sha");
                var commit = repo.Lookup<Commit>("99d1be96");
                var tree = repo.Lookup<Tree>("sha");
                var blob = repo.Lookup<Blob>("sha");

                //obj.Should().NotBeNull();
                commit.Should().NotBeNull();
                //tree.Should().NotBeNull();
                //blob.Should().NotBeNull();
                

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
                var remoteBranch = repo.Branches["origin/master"];

                allBranches.Should().NotBeNull();
                branch.Should().NotBeNull();
                remoteBranch.Should().NotBeNull();
                
                // Tags
                var aTag = repo.Tags["refs/tags/tagForTesting"];
                var allTags = repo.Tags.ToList();
                
                aTag.Should().NotBeNull();
                allTags.Should().NotBeEmpty();
                
            }
        }
    }
}