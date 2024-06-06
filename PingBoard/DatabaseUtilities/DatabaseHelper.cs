namespace PingBoard.DatabaseUtilities;

using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Microsoft.Data.Sqlite;

public class DatabaseHelper : IDisposable
{
    private readonly DatabaseStatementsGenerator _statements;
    private readonly DatabaseConstants _constants;
    private readonly SqliteConnection _databaseConnection;
    
    public DatabaseHelper(DatabaseStatementsGenerator statements, DatabaseConstants constants) {
        _statements = statements;
        _constants = constants;
        _databaseConnection = new SqliteConnection($"Data Source = {_constants.DatabaseName}");
        _databaseConnection.Open();
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
        insertSummaryCommand.Parameters.AddWithValue("$start_time", summary.Start.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.Parameters.AddWithValue("$end_time", summary.End.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        insertSummaryCommand.ExecuteNonQuery();
    }
    /*
    public PingGroupSummary RetrievePingGroupSummaryById(int id){
        using var connection = new SqliteConnec
    }*/
    
    public void Dispose()
    {
        _databaseConnection.Dispose();
    }
    
}