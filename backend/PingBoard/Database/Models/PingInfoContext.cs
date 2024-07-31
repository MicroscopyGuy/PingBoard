namespace PingBoard.Database.Models;
using PingBoard.Database.Utilities;
using Microsoft.EntityFrameworkCore;

public class PingInfoContext : DbContext
{
    public DbSet<PingGroupSummary> Summaries { get; set; }
    public string DbPath;
    private readonly DatabaseConstants _dbConstants;

    public PingInfoContext(DatabaseConstants dbConstants)
    {
        _dbConstants = dbConstants;
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, _dbConstants.SummariesTableName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder.Entity<PingGroupSummary>()
            .HasKey(p => p._id);
    }
    
}