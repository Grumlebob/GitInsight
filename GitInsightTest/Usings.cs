global using Xunit;
global using FluentAssertions;
global using static GitInsight.Core.GitPathHelper;
global using GitInsight;
global using GitInsight.Entities;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using GitInsight.Core;
global using GitInsight.Controllers;
global using Microsoft.AspNetCore.Mvc;
global using GitInsight.Data;
global using System.IO.Compression;

global using Commit = GitInsight.Entities.CommitInsight;
global using Repository = GitInsight.Entities.RepoInsight;
global using Branch = GitInsight.Entities.Branch;

