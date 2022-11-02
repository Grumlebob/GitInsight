using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Sha { get; set; }
    public string Path { get; set; }
    public string? Name { get; set; }

    public int RepositoryId { get; set; }
    public Repository Repository { get; set; }

    public List<Commit> Commits { get; set; }


    public Branch(BranchCreateDto b)
    {
        Sha = b.Sha;
        Path = b.Path;
        Name = b.Name;
        RepositoryId = b.RepositoryId;
        Commits = new List<Commit>();
    }
}

public class BranchConfigurations : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(a => a.Id);
    }
}