using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Utilities;
using PingBoard.Probes.NetworkProbes;
using PingBoard.Services;

namespace PingBoard.Database.Models;

public class ProbeResultsContext : DbContext
{

    public DbSet<ProbeResult> ProbeResults { get; set; }
    public string DbPath;
    private readonly DatabaseConstants _dbConstants;

    public ProbeResultsContext(DatabaseConstants dbConstants)
    {
        _dbConstants = dbConstants;
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, _dbConstants.ProbeResultsTableName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder.Entity<ProbeResult>()
            .HasKey(probeResult => probeResult.Id); // what is the primary key going to be?
    }
}