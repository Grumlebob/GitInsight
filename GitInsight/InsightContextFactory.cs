using GitInsight.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GitInsight;

internal class InsightContextFactory : IDesignTimeDbContextFactory<InsightContext>
{
    public InsightContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var connectionString = configuration.GetConnectionString("ConnectionString");
        
        var optionsBuilder = new DbContextOptionsBuilder<InsightContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        var context = new InsightContext(optionsBuilder.Options);
        context.Database.Migrate();
        return new InsightContext(optionsBuilder.Options);
    }
}