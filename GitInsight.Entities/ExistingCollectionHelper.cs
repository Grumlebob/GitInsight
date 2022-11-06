namespace GitInsight.Entities;

public static class ExistingCollectionHelper
{
    public static async Task<List<Repository>> UpdateRepositoriesIfExist(InsightContext context, IEnumerable<int> repoIds) =>
        await context.Repositories.Where(r => repoIds.Contains(r.Id)).ToListAsync();

    public static async Task<List<Commit>> UpdateCommitsIfExist(InsightContext context,IEnumerable<int> commitIds) =>
        await context.Commits.Where(r => commitIds.Contains(r.Id)).ToListAsync();

    public static async Task<List<Branch>> UpdateBranchesIfExist(InsightContext context,IEnumerable<int> branchIds) =>
        await context.Branches.Where(r => branchIds.Contains(r.Id)).ToListAsync();

    public static async Task<List<Author>> UpdateAuthorsIfExist(InsightContext context,IEnumerable<int> authorIds) =>
        await context.Authors.Where(r => authorIds.Contains(r.Id)).ToListAsync();
}
