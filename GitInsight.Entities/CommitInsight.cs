using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class CommitInsight
{
    public int Id { get; set; }
    public string Sha { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public Author Author { get; set; } = null!;
    public int AuthorId { get; set; }

    public Branch Branch { get; set; } = null!;
    public int BranchId { get; set; }

    public RepoInsight Repository { get; set; } = null!;
    public int RepositoryId { get; set; }

}

public class CommitConfigurations : IEntityTypeConfiguration<CommitInsight>
{
    public void Configure(EntityTypeBuilder<CommitInsight> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Repository)
            .WithMany(a => a.Commits!);

        builder.HasOne(a => a.Author)
            .WithMany(a => a.Commits);

        builder.HasOne(a => a.Branch)
            .WithMany(a => a.Commits);
    }
}