using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    public List<Commit> Commits  { get; set; }
    public List<Repository> Repositories { get; set; }

    public Author()
    {
        Commits = new List<Commit>();
        Repositories = new List<Repository>();
    }
}

public class AuthorConfigurations : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);
    }
}