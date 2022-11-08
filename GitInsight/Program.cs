using GitInsight;
using GitInsight.Core;
using GitInsight.Data;
using GitInsight.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

builder.Services.AddDbContext<InsightContext>(o =>
    o.UseNpgsql(configuration.GetConnectionString("ConnectionString")));
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IRepositoryRepository, RepositoryRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICommitRepository, CommitRepository>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

InsightContext context = new InsightContextFactory().CreateDbContext(args);
DataManager dm = new DataManager(context);
await dm.Analyze( GetGitTestFolder(),GetRelativeGitFolder(@"GitInsightTest/Testrepo.git"));
await dm.Analyze(GetGitLocalFolder(), GetRelativeGitFolder(".git"));
