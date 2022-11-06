using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class Commit
{
    public int Id { get; set; }
    public string Sha { get; set; }
    public DateTimeOffset Date { get; set; }

    public Author Author { get; set; }
    public int AuthorId { get; set; }

    public Branch Branch { get; set; }
    public int BranchId { get; set; }

    public Repository Repository { get; set; }
    public int RepositoryId { get; set; }

}

public class CommitConfigurations : IEntityTypeConfiguration<Commit>
{
    public void Configure(EntityTypeBuilder<Commit> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.HasOne(a => a.Repository)
            .WithMany(a => a.Commits);
        
        builder.HasOne(a => a.Author)
            .WithMany(a => a.Commits);
        
        builder.HasOne(a => a.Branch)
            .WithMany(a => a.Commits);
    }
}