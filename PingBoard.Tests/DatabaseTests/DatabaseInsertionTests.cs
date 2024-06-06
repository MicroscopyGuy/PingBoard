namespace PingBoard.Tests.DatabaseTests;
using PingBoard.DatabaseUtilities;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Tests.PingingTests;

public class DatabaseInsertionTests{

    private DatabaseConstants databaseConstants = new DatabaseConstants();

    // Specifically want to use the same thresholds used by SendPingGroupAsyncTesting, just in case
    private DatabaseStatementsGenerator dbStatementsGenerator = new DatabaseStatementsGenerator(
        SendPingGroupAsyncTestingConfig.PingThresholdsOptions,
        new DatabaseConstants()
    );

    
    [Fact]
    public async Task TestRecordInsertion(){
        databaseConstants.DatabaseName = ":memory:";
        DatabaseHelper dbHelper = new DatabaseHelper(dbStatementsGenerator, databaseConstants);

        // Want to keep parity with the tests for SendPingGroupAsync, since the same behavior is needed
        var result = await PingGroupSummaryStub.GenerateSuccessSuccess(
            SendPingGroupAsyncTestingConfig.PingBehaviorOptions,
            SendPingGroupAsyncTestingConfig.PingThresholdsOptions
        );
        
        dbHelper.InsertPingGroupSummary(result);

        
    }


}