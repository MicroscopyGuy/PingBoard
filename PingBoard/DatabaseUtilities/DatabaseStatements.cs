using System.Text;

namespace PingBoard.DatabaseUtilities;
using PingBoard.Pinging;
using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;

/// <summary>
/// Contains templated SQLite statements that establishes the tables and insert statements required to
/// store and efficiently use the pinging information generated from this application.
///
/// Note that the string interpolation required by the Anomalies Index definition must take place
/// before it is invoked in DatabaseHelper.Apparently, interpolation is prohibited in "WHERE" clauses
/// in partial index definitions using SQLite. 
/// </summary>
public class DatabaseStatementsGenerator
{
    /// <summary>
    /// The SQL statement needed to create the table that stores the PingGroupSummary objects' data.
    /// Note that the longest IPStatus is 30 characters (DestinationProtocolUnreachable)
    /// </summary>
    private readonly string _groupSummariesTable = """
                                                   CREATE TABLE IF NOT EXISTS @summaries_table_name(
                                                   Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                   Start TEXT UNIQUE NOT NULL,
                                                   End TEXT UNIQUE NOT NULL,
                                                   Target TEXT,
                                                   MinimumPing INTEGER NOT NULL,
                                                   AveragePing FLOAT NOT NULL,
                                                   MaximumPing INTEGER NOT NULL,
                                                   Jitter FLOAT NOT NULL,
                                                   PacketLoss FLOAT NOT NULL,
                                                   TerminatingIPStatus VARCHAR(30),
                                                   LastAbnormalStatus  VARCHAR(30),
                                                   ConsecutiveTimeouts INTEGER NOT NULL,
                                                   PacketsSent FLOAT NOT NULL,
                                                   PacketsLost INTEGER NOT NULL,
                                                   ExcludedPings INTEGER NOT NULL);
                                                   """;
    
    
    private readonly string _anomaliesIndex = """
                                               CREATE INDEX IF NOT EXISTS @anomaly_index_name ON @summaries_table_name(
                                               MinimumPing, AveragePing, PacketLoss, Jitter, LastAbnormalStatus)
                                                   WHERE
                                                       MinimumPing   > @min_ping_threshold OR
                                                       AveragePing   > @avg_ping_threshold OR
                                                       MaximumPing   > @max_ping_threshold OR
                                                       PacketLoss    > @packet_loss_threshold OR
                                                       Jitter        > @jitter_threshold OR
                                                       LastAbnormalStatus NOT NULL;
                                              """;

    private readonly string _dropAnomaliesIndex = """
                                                  DROP INDEX IF EXISTS @anomaly_index_name;
                                                  """;
    
    private readonly string _insertPingGroupSummary = """
                                                         INSERT INTO @summaries_table_name VALUES
                                                         (   
                                                             NULL,
                                                             @start_time,
                                                             @end_time,
                                                             @target,
                                                             @min_ping,
                                                             @avg_ping,
                                                             @max_ping,
                                                             @jitter,
                                                             @packet_loss,
                                                             @terminating_ipstatus,
                                                             @last_abnormal_status,
                                                             @consecutive_timeouts,
                                                             @packets_sent,
                                                             @packets_lost,
                                                             @excluded_pings
                                                         );
                                                      """;

    private readonly string _selectPingGroupSummaryById = """
                                                          SELECT *
                                                          FROM @summaries_table_name
                                                          WHERE id = @id
                                                          """;

    private readonly PingingThresholdsConfig _pingingThresholds;
    private readonly DatabaseConstants _databaseConstants;

    public DatabaseStatementsGenerator(IOptions<PingingThresholdsConfig> pingingThresholds, 
        DatabaseConstants constants) {
        _pingingThresholds = pingingThresholds.Value;
        _databaseConstants = constants;
    }

    public string SummariesTableDefinition() {
        return _groupSummariesTable
            .Replace("@summaries_table_name", _databaseConstants.SummariesTableName);

    }
    
    
    /// <summary>
    /// Uses string interpolation to replace the definition placeholders in _anomaliesIndex with their
    /// counterparts in either the PingingThresholdsConfig object, or the DatabaseConstants object.
    /// </summary>
    /// <returns>
    /// The constructed SQL statement that defines the index on the main table that allows efficient lookup
    /// of anomalous pings.
    /// </returns>
    public string AnomaliesIndexDefinition(){
        return _anomaliesIndex
            .Replace("@anomaly_index_name", _databaseConstants.AnomaliesIndexName)
            .Replace("@summaries_table_name", _databaseConstants.SummariesTableName)
            .Replace("@min_ping_threshold", _pingingThresholds.MinimumPingMs.ToString())
            .Replace("@avg_ping_threshold", _pingingThresholds.AveragePingMs.ToString()!)
            .Replace("@max_ping_threshold", _pingingThresholds.MaximumPingMs.ToString())
            .Replace("@packet_loss_threshold", _pingingThresholds.PacketLossPercentage.ToString())
            .Replace("@jitter_threshold", _pingingThresholds.JitterMs.ToString());

    }

    /// <summary>
    /// Constructs and returns a sql statement to drop the anomalies index from the database
    /// </summary>
    /// <returns>A completed SQL statement</returns>
    public string DropAnomaliesIndex() {
        return _dropAnomaliesIndex
            .Replace("@anomaly_index_name", _databaseConstants.AnomaliesIndexName);
    }
    
    /// <summary>
    /// Constructs and returns a mostly completed sql statement to insert a PingGroupSummary into the database
    /// </summary>
    /// <param name="summary"></param>
    /// <returns> the completed sql statement</returns>
    public string InsertPingGroupSummaryStmt(PingGroupSummary summary)
    {
        return _insertPingGroupSummary 
            .Replace("@summaries_table_name", _databaseConstants.SummariesTableName);/*
            .Replace("@target", summary.Target ?? "NULL")
            .Replace("@min_ping", summary.MinimumPing.ToString())
            .Replace("@avg_ping", summary.AveragePing.ToString())
            .Replace("@max_ping", summary.MaximumPing.ToString())
            .Replace("@jitter", summary.Jitter.ToString())
            .Replace("@packet_loss", summary.PacketLoss.ToString())
            .Replace("@terminating_ipstatus", summary.TerminatingIPStatus.ToString() ?? "NULL")
            .Replace("@last_abnormal_status", summary.LastAbnormalStatus.ToString() ?? "NULL")
            .Replace("@consecutive_timeouts", summary.ConsecutiveTimeouts.ToString())
            .Replace("@packets_sent", summary.PacketsSent.ToString())
            .Replace("@packets_lost", summary.PacketsLost.ToString())
            .Replace("@excluded_pings", summary.ExcludedPings.ToString());*/
    }

    public string SelectPingGroupSummaryByIdStmt(int id)
    {
        return _selectPingGroupSummaryById
            .Replace("@summaries_table_name", _databaseConstants.SummariesTableName)
            .Replace("@id", id.ToString());
    }
    
}
