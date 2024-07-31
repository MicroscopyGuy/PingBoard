using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.WebUtilities;
using PingBoard.Database.Models;

namespace PingBoard.Database.Utilities;

using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Microsoft.Data.Sqlite;

public class DatabaseHelper : IDisposable
{
    private readonly DatabaseStatementsGenerator _statements;
    private readonly DatabaseConstants _constants;
    private readonly SqliteConnection _databaseConnection;
    private readonly PingQualification _pingQualifier;
    
    public DatabaseHelper(DatabaseStatementsGenerator statements, DatabaseConstants constants,
                          PingQualification pingQualifier) {
        _statements = statements;
        _constants = constants;
        _databaseConnection = new SqliteConnection($"Data Source = {_constants.DatabaseName}");
        _databaseConnection.Open();
        _pingQualifier = pingQualifier;
    }

    public void InitializeDatabase() {
        // connect to DB
        using var connection = new SqliteConnection($"Data Source = {_constants.DatabaseName}");
        connection.Open();
        
        // Make the table that houses the PingGroupSummary objects generated from SendPingGroupAsync
        var tableCommand = _databaseConnection.CreateCommand();
        tableCommand.CommandText = _statements.SummariesTableDefinition();
        tableCommand.ExecuteNonQuery();
        
        // Drop the index that allows efficient lookup of anomalous pings, this way it always reflects
        // the up-to-date thresholds configured by the user. Updating the index would require dropping anyways.
        var dropIndexCommand = _databaseConnection.CreateCommand();
        dropIndexCommand.CommandText = _statements.DropAnomaliesIndex();
        dropIndexCommand.ExecuteNonQuery();
        
        // Make the index that allows efficient lookup of anomalous pings stored in the table above
        var indexCommand = _databaseConnection.CreateCommand();
        indexCommand.CommandText = _statements.AnomaliesIndexDefinition();
        indexCommand.ExecuteNonQuery();
    }

    public void InsertPingGroupSummary(PingGroupSummary summary)
    {
        var insertSummaryCommand = _databaseConnection.CreateCommand();
        insertSummaryCommand.CommandText = _statements.InsertPingGroupSummaryStmt(summary);
        insertSummaryCommand.Parameters.AddWithValue("@start_time", summary.Start.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.Parameters.AddWithValue("@end_time", summary.End.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.Parameters.AddWithValue("@target", summary.Target.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@min_ping", summary.MinimumPing.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@avg_ping", summary.AveragePing.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@max_ping", summary.MaximumPing.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@jitter", summary.Jitter.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@packet_loss", summary.PacketLoss.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@terminating_ipstatus",
            (object?) summary.TerminatingIPStatus?.ToString() ?? DBNull.Value);
        insertSummaryCommand.Parameters.AddWithValue("@last_abnormal_status",
            (object?) summary.LastAbnormalStatus?.ToString() ?? DBNull.Value);
        insertSummaryCommand.Parameters.AddWithValue("@consecutive_timeouts", summary.ConsecutiveTimeouts.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@packets_sent", summary.PacketsSent.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@packets_lost", summary.PacketsLost.ToString());
        insertSummaryCommand.Parameters.AddWithValue("@excluded_pings", summary.ExcludedPings.ToString());
        insertSummaryCommand.ExecuteNonQuery();
    }
    
    
    public PingGroupSummary RetrievePingGroupSummaryById(int id)
    {
        var selectSummaryCommand = _databaseConnection.CreateCommand();
        selectSummaryCommand.CommandText = _statements.SelectPingGroupSummaryByIdStmt(id);
        SqliteDataReader reader = selectSummaryCommand.ExecuteReader();
        var summaries  = ReadPingGroupSummariesFromReader(reader);
        if (summaries.Count > 1)
        {
            throw new InvalidOperationException("More than one matching PingGroupSummary found.");
        }

        return summaries[id];
    }

    public Dictionary<int, PingGroupSummary> ReadPingGroupSummariesFromReader(SqliteDataReader reader)
    {
        var summaries = new Dictionary<int, PingGroupSummary>();
        while (reader.Read())
        {
            var summary = new PingGroupSummary();
            summary.Start       = DateTime.ParseExact(reader[nameof(PingGroupSummary.Start)].ToString()!, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            summary.End         = DateTime.ParseExact(reader[nameof(PingGroupSummary.End)].ToString()!, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            summary.Target      = reader[nameof(PingGroupSummary.Target)].ToString()!;
            summary.MinimumPing = short.Parse(reader[nameof(PingGroupSummary.MinimumPing)].ToString()!);
            summary.AveragePing = float.Parse(reader[nameof(PingGroupSummary.AveragePing)].ToString()!);
            summary.MaximumPing = short.Parse(reader[nameof(PingGroupSummary.MaximumPing)].ToString()!);
            summary.Jitter      = float.Parse(reader[nameof(PingGroupSummary.Jitter)].ToString()!);
            summary.PacketLoss  = float.Parse(reader[nameof(PingGroupSummary.PacketLoss)].ToString()!);

            var terminatingIpStatus = reader[nameof(PingGroupSummary.TerminatingIPStatus)];
            summary.TerminatingIPStatus = Convert.IsDBNull(terminatingIpStatus)
                    ? null 
                    : Enum.Parse<IPStatus>(terminatingIpStatus.ToString()!);

            var abnormalStatus = reader[nameof(PingGroupSummary.LastAbnormalStatus)];
            summary.LastAbnormalStatus  = Convert.IsDBNull(abnormalStatus)
                    ? null
                    : Enum.Parse<IPStatus>(abnormalStatus.ToString()!);
            
            summary.ConsecutiveTimeouts = byte.Parse(reader[nameof(PingGroupSummary.ConsecutiveTimeouts)].ToString()!);
            summary.PacketsSent         = byte.Parse(reader[nameof(PingGroupSummary.PacketsSent)].ToString()!);
            summary.PacketsLost         = byte.Parse(reader[nameof(PingGroupSummary.PacketsLost)].ToString()!);
            summary.ExcludedPings       = byte.Parse(reader[nameof(PingGroupSummary.ExcludedPings)].ToString()!);
            summary.PingQualityFlags    = _pingQualifier.CalculatePingQualityFlags(summary);
            summaries.Add(int.Parse(reader["id"].ToString()!), summary);
        }

        return summaries;
    }
    
    
    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        _databaseConnection.Dispose();
    }
    
}