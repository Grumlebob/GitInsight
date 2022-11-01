using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GitInsight.Entities;

internal class InsightContextFactory : IDesignTimeDbContextFactory<InsightContext>
{
    public InsightContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var connectionString = configuration.GetConnectionString("ConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<InsightContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new InsightContext(optionsBuilder.Options);
    }
}