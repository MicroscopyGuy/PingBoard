namespace PingBoard.Apis.Events;

public class ServerEventNotifications
{
    private ProbeOperationsCenter _probeOperationsCenter;
    private readonly IImmutableList<IChannelReaderAdapter> _serverEventChannelReaders;
    private readonly ILogger<PingBoardService> _logger;
    private readonly CrudOperations _crudOperations;

    public PingBoardService(
        ProbeOperationsCenter probeOperationsCenter,
        CrudOperations crudOperations,
        [FromKeyedServices("ServerEventChannelReaders")]
            IImmutableList<IChannelReaderAdapter> serverEventChannelReaders,
        ILogger<PingBoardService> logger
    )
    {
        _probeOperationsCenter = probeOperationsCenter;
        _crudOperations = crudOperations;
        _serverEventChannelReaders = serverEventChannelReaders;
        _logger = logger;
    }

    /* Consider renaming this to SendLatestServerEvents: pluralize it, and clarify direction of communication */
    /// <summary>
    /// Streams ServerEvents to the FrontEnd, one at a time. See the ServerEvent definition in protos/service.proto
    /// for a comprehensive list of which ServerEvents are supported.
    /// </summary>
    /// <param name="request">Empty, since no user parameters are needed./param>
    /// <param name="responseStream">The writer that writes ServerEvents to the stream.</param>
    /// <param name="context">Represents the context of a server-side call.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task GetLatestServerEvent(
        Empty request,
        IServerStreamWriter<IServerEvent> responseStream,
        ServerCallContext context
    )
    {
        _logger.LogDebug("GetLatestServerEvent: Starting API call");
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var readyTaskReader = await GetReadyChannelReaderAdapter(context.CancellationToken);

            IServerEvent? eventToSend;
            while ((eventToSend = readyTaskReader.ReadNextServerEvent()) != null)
            {
                await responseStream.WriteAsync(eventToSend, context.CancellationToken);
            }
        }
    }

    /// <summary>
    /// A helper method for GetLatestServerEvent which figures out which IChannelReaderAdapter has a message,
    /// and then returns it so GetLatestServerEvent can send the ServerEvent the IChannelReaderAdapter
    /// needs to tell the frontend about.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The IChannelReaderAdapter that has a new message</returns>
    /// <exception cref="TaskCanceledException"></exception>
    private async Task<IChannelReaderAdapter> GetReadyChannelReaderAdapter(
        CancellationToken cancellationToken
    )
    {
        List<Task<bool>> channelReaderTasks = new List<Task<bool>>();

        foreach (var reader in _serverEventChannelReaders)
        {
            channelReaderTasks.Add(reader.WaitToReadAsync(cancellationToken));
        }

        // find which task completed, and then find the reader for that task
        var readyTask = await Task.WhenAny(channelReaderTasks);

        // this would mean that all the tasks are also canceled, so it's safe to simply return
        if (readyTask.IsCanceled)
        {
            throw new TaskCanceledException("Cancellation was requested");
        }

        // unclear exactly how, when, or if this could even happen. Due to this, the application should be treated
        // as corrupted or unusable.
        if (readyTask.IsFaulted)
        {
            string failFastMsg = """
                One or more Backend event-processing-channels encountered a faulted state for an unknown reason.
                Application is now in an unusable state and must be restarted.
                """;

            _logger.LogCritical(failFastMsg + "\n" + readyTask.Exception);

            // kills the application immediately, logs to the OS crash logs
            Environment.FailFast(failFastMsg, readyTask.Exception);
        }

        var readyTaskReader = _serverEventChannelReaders[channelReaderTasks.IndexOf(readyTask)];
        return readyTaskReader;
    }
}
