namespace GitInsight.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<CommitInsight> Commits { get; set; }
    public ICollection<RepoInsight> Repositories { get; set; }

    public Author()
    {
        Commits = new List<CommitInsight>();
        Repositories = new List<RepoInsight>();
    }
}

public class AuthorConfigurations : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.Email).IsUnique();
        builder.Property(e => e.Email).IsRequired();

        builder.HasMany(a => a.Commits)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(a => a.Repositories)
            .WithMany(a => a.Authors);
    }
}