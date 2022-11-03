using Microsoft.EntityFrameworkCore;

namespace GitInsight.Entities;

public class InsightContext : DbContext
{

    public InsightContext(DbContextOptions<InsightContext> options): base(options)
    {
        
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Commit> Commits { get; set; }
    public DbSet<Repository> Repositories { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
    
}