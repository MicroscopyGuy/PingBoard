using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;

namespace PingBoard.Tests.DatabaseTests;
using PingBoard.DatabaseUtilities;
using PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Tests.PingingTests;

public class DatabaseInsertionTests{

    private readonly DatabaseConstants databaseConstants = new DatabaseConstants();

    // Specifically want to use the same thresholds used by SendPingGroupAsyncTesting, just in case
    private readonly DatabaseStatementsGenerator dbStatementsGenerator = new DatabaseStatementsGenerator(
        SendPingGroupAsyncTestingConfig.PingThresholdsOptions,
        new DatabaseConstants()
    );
    
    
    
    [Theory]
    [MemberData(nameof(PingGroupSummaryTestGenerator.GetScenarioFunctions), MemberType = typeof(PingGroupSummaryTestGenerator))]
    public async Task TestRecordInsertion(string scenario){
        
        databaseConstants.DatabaseName = ":memory:";

        // make the qualifer that DatabaseHelper needs, using the same PingingThresholdsOptions from 
        // SendPingGroupAsyncTesting 
        PingQualification pingQualifier = new PingQualification(SendPingGroupAsyncTestingConfig.PingThresholdsOptions);
        
        DatabaseHelper dbHelper = new DatabaseHelper(
            dbStatementsGenerator, 
            databaseConstants,
            pingQualifier
            );
        
        // Want to keep parity with the tests for SendPingGroupAsync, since the same behavior is needed
        var result = await PingGroupSummaryStub.RunFunctionByName(
            scenario,
            SendPingGroupAsyncTestingConfig.PingBehaviorOptions,
            SendPingGroupAsyncTestingConfig.PingThresholdsOptions
        );
        
        dbHelper.InitializeDatabase();
        dbHelper.InsertPingGroupSummary(result);
        dbHelper.RetrievePingGroupSummaryById(1);
        PingGroupSummaryExpectedValues.AssertExpectedValues(
            PingGroupSummaryExpectedValues.ExpectedSummaries[scenario],
            result
            );
    }


}