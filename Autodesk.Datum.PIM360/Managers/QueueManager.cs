using Autodesk.Datum.PIM360.Etl_queue;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for ETL queue status and control operations.</summary>
public sealed class QueueManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="QueueManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public QueueManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Gets the ETL activity queue status and list of jobs.</summary>
    /// <remarks>Wraps: GET /etl_queue</remarks>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated queue status response.</returns>
    public Task<Etl_queueGetResponse?> GetQueueAsync(CancellationToken cancellationToken = default) =>
        _api.Etl_queue.GetAsync(null, cancellationToken);

    /// <summary>Pauses or resumes the activity queue.</summary>
    /// <remarks>Wraps: POST /etl_queue</remarks>
    /// <param name="body">The queue control action.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated queue status response.</returns>
    public Task<Etl_queuePostResponse?> ControlQueueAsync(Etl_queuePostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.PostAsync(body, null, cancellationToken);

    /// <summary>Provides access to the generated ETL queue request builders.</summary>
    public Etl_queueRequestBuilder Api => _api.Etl_queue;
}
