using Microsoft.EntityFrameworkCore;

namespace PingBoard.Services;
using PingBoard.Database.Models;

public class DatabaseInitializer : BackgroundService
{
    private ILogger<DatabaseInitializer> _logger;
    private PingInfoContext _pingInfoContext;
    private IDbContextFactory<PingInfoContext> _pingInfoContextFactory;

    public DatabaseInitializer(IDbContextFactory<PingInfoContext> pingInfoContextFactory,
        ILogger<DatabaseInitializer> logger)
    {
        _pingInfoContextFactory = pingInfoContextFactory;
        _pingInfoContext = _pingInfoContextFactory.CreateDbContext();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            //Sqlite doesn't support async, so ensure DB creation synchronously
            var alreadyCreated = _pingInfoContext.Database.EnsureCreated();
            string resultMsg = alreadyCreated ? "already exists" : "has now been created";
            _logger.LogDebug("Database {resultMsg}", resultMsg);
            
            //See if the tables have been created
            var tables = _pingInfoContext.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .Order()
                .ToList();
            
            if (!tables.Contains("Summaries"))
            {
                throw new Exception("Missing the Summaries table");
            }
        }
        
        catch (Exception e)
        {
            var exceptionText = e.InnerException?.ToString() ?? e.ToString();
            _logger.LogError("{exceptionText}", exceptionText);
            
            // if can't create the database, this *should* terminate the application
            throw;
        }
        
    }

}