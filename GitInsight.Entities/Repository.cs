using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Repository
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    
    
    public int LatestCommitId { get; set; }


    public List<Branch?> Branches { get; set; }
    public List<Author?> Authors { get; set; }
    public List<Commit?> Commits { get; set; }

    public Repository()
    {
        Branches = new List<Branch?>();
        Authors = new List<Author?>();
        Commits = new List<Commit?>();
    }
}

public class RepositoryConfigurations : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasMany(a => a.Branches)
            .WithOne(b => b!.Repository)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasMany(a => a.Commits)
            .WithOne(a => a.Repository);
        builder.HasIndex(a => a.LatestCommitId)
            .IsUnique(); //cant make foreign key (workaround). The use case is if foreign key rule broken: reanalyze.
    }
}
