namespace GitInsight.Entities;

public class RepoInsight
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;

    public int LatestCommitId { get; set; }

    public List<Branch> Branches { get; set; }
    public List<Author> Authors { get; set; }
    public List<CommitInsight> Commits { get; set; }

    public RepoInsight()
    {
        Branches = new List<Branch>();
        Authors = new List<Author>();
        Commits = new List<CommitInsight>();
    }
}

public class RepositoryConfigurations : IEntityTypeConfiguration<RepoInsight>
{
    public void Configure(EntityTypeBuilder<RepoInsight> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasMany(a => a.Branches)
            .WithOne(b => b.Repository)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasMany(a => a.Commits)
            .WithOne(a => a.Repository);
        builder.HasIndex(a => a.LatestCommitId)
            .IsUnique(); //cant make foreign key (workaround). The use case is if foreign key rule broken: reanalyze.
    }
}
