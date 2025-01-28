namespace PingBoard.Database.Models;

using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Utilities;
using PingBoard.Probes.NetworkProbes;
using Probes.NetworkProbes.Common;

public class ProbeResultsContext : DbContext
{
    public DbSet<ProbeResult> ProbeResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite($"Data Source={ServiceExtensions.DatabasePath}");

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder
            .Entity<ProbeResult>()
            .ToTable(DatabaseConstants.ProbeResultsTableName)
            .HasKey(probeResult => probeResult.Id);
    }
}
