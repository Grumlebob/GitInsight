﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitInsight.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Sha { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;

    public int RepositoryId { get; set; }
    public Repository Repository { get; set; }

    public List<Commit> Commits { get; set; }


    public Branch()
    {
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
