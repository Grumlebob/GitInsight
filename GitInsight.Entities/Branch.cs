namespace GitInsight.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;

    public int RepositoryId { get; set; }
    public RepoInsight Repository { get; set; } = null!;

    public List<CommitInsight> Commits { get; set; }


    public Branch()
    {
        Commits = new List<CommitInsight>();
    }
}

public class BranchConfigurations : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        
        builder.Property(e => e.Path).IsRequired();
        
        builder.HasKey(a => a.Id);
        builder.HasKey(a => new { a.RepositoryId, a.Path });
        builder.HasMany(a => a.Commits)
            .WithOne(b => b.Branch)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
