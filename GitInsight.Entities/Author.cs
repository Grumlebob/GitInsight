using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    
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
        
        builder.HasMany(a => a.Commits)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId);

        builder.HasMany(a => a.Repositories)
            .WithMany(a => a.Authors);
       
    }
}