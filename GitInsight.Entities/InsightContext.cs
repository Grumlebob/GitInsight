namespace GitInsight.Entities;

public class InsightContext : DbContext
{

    public InsightContext(DbContextOptions<InsightContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<CommitInsight> Commits { get; set; } = null!;
    public DbSet<RepoInsight> Repositories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }

}