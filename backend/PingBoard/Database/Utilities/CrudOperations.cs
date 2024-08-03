namespace PingBoard.Database.Utilities;
using PingBoard.Database.Models;
    
public static class CrudOperations
{
    
    /// <summary>
    /// Inserts a PingGroupSummary (private) into the database
    /// </summary>
    /// <param name="dbContext">Represents Unit of Work with the PingInfo database</param>
    /// <param name="summary">The PingGroupSummary objects whose values should be stored in the DB</param>
    public static void InsertPingGroupSummary(this PingInfoContext dbContext, PingGroupSummary summary)
    {
        dbContext.Add(summary);
        dbContext.SaveChanges();
    }
    
    /// <summary>
    /// Retrieves the next N number of records from the database from a certain point in time, returned
    /// as a list of PingGroupSummaryPublic objects
    /// </summary>
    /// <param name="dbContext">Represents a Unit of Work with the PingInfo database</param>
    /// <param name="startingTime">The initial (inclusive) time from which the offset N, should be applied</param>
    /// <param name="numToGet">The number of records to retrieve from the initial (inclusive) time</param>
    /// <returns></returns>
    public static List<PingGroupSummaryPublic> RetrieveNPingGroupSummariesFrom(this PingInfoContext dbContext,
        DateTime startingTime, int numToGet)
    {
        var summaries = dbContext
            .Summaries
            .Select(s => PingGroupSummaryPublic.Parse(s))
            .OrderByDescending(s => s.Start)
            .Where(s => s.Start >= startingTime)
            .Take(numToGet)
            .ToList();
        
        return summaries;
    }
}