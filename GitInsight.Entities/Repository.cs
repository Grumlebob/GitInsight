using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Repository
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string? Name { get; set; }

    public List<Branch> Branches { get; set; }
    public List<Author> Authors { get; set; }
    public List<Commit> Commits { get; set; }
}

public class RepositoryConfigurations : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        builder.HasKey(a => a.Id);
    }
}