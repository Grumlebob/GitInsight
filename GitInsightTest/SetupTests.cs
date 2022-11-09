﻿using System.IO.Compression;
using GitInsight.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GitInsightTest;

public class SetupTests
{
    public static (SqliteConnection, InsightContext) Setup()
    {
        EnsureZipIsUnzippedTesting();
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<InsightContext>();
        builder.UseSqlite(connection);
        var context = new InsightContext(builder.Options);
        context.Database.EnsureCreated();
        context.SaveChanges();
        return (connection, context);
    }
    
    public static void EnsureZipIsUnzippedTesting() //Only works in testing directory
    {
        var projectPath =  Directory.GetParent(Directory.GetCurrentDirectory())?.Parent!.Parent!.Parent!.FullName;
        
        string unzippedFolder = Path.Combine(projectPath! + GetRelativeTestFolder());
        string zippedFolder = Path.Combine(projectPath! + @"/GitInsightTest/TestResources/Zipped/Testrepo.git.zip");
        string destinationFolder = Path.Combine(projectPath! + @"/GitInsightTest/TestResources/Unzipped/");
        
        if (!Directory.Exists(unzippedFolder))
        {
            ZipFile.ExtractToDirectory(
                zippedFolder, 
                destinationFolder,true);
            Console.WriteLine("Testrepo.git unzipped");
        }
        else
        {
            Console.WriteLine("Testrepo.git already unzipped");
        }
    }
}