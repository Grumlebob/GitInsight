namespace GitInsight.Entities;

public static class ExistingCollectionHelper
{
    public static async Task<List<RepoInsight>> UpdateRepositoriesIfExist(InsightContext context, IEnumerable<int> repoIds) =>
        repoIds is null
        ? new List<RepoInsight>()
        : await context.Repositories.Where(r => repoIds.Contains(r.Id)).ToListAsync();


    public static async Task<List<CommitInsight>> UpdateCommitsIfExist(InsightContext context, IEnumerable<int> commitIds) =>
        commitIds is null
            ? new List<CommitInsight>()
            : await context.Commits.Where(r => commitIds.Contains(r.Id)).ToListAsync();

    public static async Task<List<Branch>> UpdateBranchesIfExist(InsightContext context, IEnumerable<int> branchIds) =>
        branchIds is null
            ? new List<Branch>()
            : await context.Branches.Where(r => branchIds.Contains(r.Id)).ToListAsync();

    public static async Task<List<Author>> UpdateAuthorsIfExist(InsightContext context, IEnumerable<int> authorIds) =>
        authorIds is null
            ? new List<Author>()
            : await context.Authors.Where(r => authorIds.Contains(r.Id)).ToListAsync();
}