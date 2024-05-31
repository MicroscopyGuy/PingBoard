namespace PingBoard.DatabaseUtilities;

using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Microsoft.Data.Sqlite;

public class DatabaseHelper
{
    public DatabaseStatementsGenerator _statements;
    public DatabaseConstants _constants;
    
    public DatabaseHelper(DatabaseStatementsGenerator statements, DatabaseConstants constants) {
        _statements = statements;
        _constants = constants;
    }

    public void InitializeDatabase() {
        // connect to DB
        using var connection = new SqliteConnection($"Data Source = {_constants.DatabaseName}");
        connection.Open();
        
        // Make the table that houses the PingGroupSummary objects generated from SendPingGroupAsync
        var tableCommand = connection.CreateCommand();
        tableCommand.CommandText = _statements.SummariesTableDefinition();
        tableCommand.ExecuteNonQuery();
        
        // Drop the index that allows efficient lookup of anomalous pings, this way it always reflects
        // the up-to-date thresholds configured by the user. Updating the index would require dropping anyways.
        var dropIndexCommand = connection.CreateCommand();
        dropIndexCommand.CommandText = _statements.DropAnomaliesIndex();
        dropIndexCommand.ExecuteNonQuery();
        
        // Make the index that allows efficient lookup of anomalous pings stored in the table above
        var indexCommand = connection.CreateCommand();
        indexCommand.CommandText = _statements.AnomaliesIndexDefinition();
        indexCommand.ExecuteNonQuery();
    }

    public void InsertPingGroupSummary(PingGroupSummary summary)
    {
        using var connection = new SqliteConnection($"Data Source = {_constants.DatabaseName}");
        connection.Open();

        var insertSummaryCommand = connection.CreateCommand();
        insertSummaryCommand.CommandText = _statements.InsertPingGroupSummaryStmt(summary);
        insertSummaryCommand.Parameters.AddWithValue("$start_time", summary.Start.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.Parameters.AddWithValue("$end_time", summary.End.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.ExecuteNonQuery();
    }
    
}