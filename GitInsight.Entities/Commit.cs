using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class Commit
{
    public int Id { get; set; }
    public string Sha { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Tag { get; set; }
    
    public Author Author { get; set; }
    public int AuthorId { get; set; }
    
    public Branch Branch { get; set; }
    public int BranchId { get; set; }
    
    public Repository Repository { get; set; }
    public int RepositoryId { get; set; }
    
    public Commit() 
    {
       
    }
    
}

public class CommitConfigurations : IEntityTypeConfiguration<Commit>
{
    public void Configure(EntityTypeBuilder<Commit> builder)
    {
        builder.HasKey(a => a.Id);
    }
}