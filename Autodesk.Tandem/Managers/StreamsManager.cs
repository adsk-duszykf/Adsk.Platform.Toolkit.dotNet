using Autodesk.Tandem.Tandem.V1.ModelsRequests.Item.Getstreamssecrets;
using Autodesk.Tandem.Tandem.V1.ModelsRequests.Item.Resetstreamssecrets;
using Autodesk.Tandem.Tandem.V1.Timeseries.ModelsRequests.Item.Deletestreamsdata;
using Autodesk.Tandem.Tandem.V1.Timeseries.ModelsRequests.Item.Streams;
using Autodesk.Tandem.Tandem.V1.Timeseries.ModelsRequests.Item.Streams.Item;
using Autodesk.Tandem.Tandem.V1.Timeseries.ModelsRequests.Item.Webhooks.Generic;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Streams operations (time series data ingestion and retrieval)
/// </summary>
public class StreamsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public StreamsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves ingestion secret(s), associated with specific data stream(s).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/models/{modelID}/getstreamssecrets
    /// </remarks>
    /// <param name="modelId">ID of the model owning the stream</param>
    /// <param name="body">The request body with stream keys</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="GetstreamssecretsPostResponse"/> containing the stream secrets</returns>
    /// <example>
    /// <code>
    /// GetstreamssecretsPostResponse? secrets = await client.StreamsManager.GetStreamsSecretsAsync("urn:adsk.dtm:...", new GetstreamssecretsPostRequestBody { Keys = ["key1"] });
    /// </code>
    /// </example>
    public async Task<GetstreamssecretsPostResponse?> GetStreamsSecretsAsync(
        string modelId,
        GetstreamssecretsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Models[modelId].Getstreamssecrets
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Resets ingestion secret(s), associated with specific data stream(s).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/models/{modelID}/resetstreamssecrets
    /// </remarks>
    /// <param name="modelId">ID of the model owning the stream</param>
    /// <param name="body">The request body with stream keys</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.StreamsManager.ResetStreamsSecretsAsync("urn:adsk.dtm:...", new ResetstreamssecretsPostRequestBody { Keys = ["key1"] });
    /// </code>
    /// </example>
    public async Task ResetStreamsSecretsAsync(
        string modelId,
        ResetstreamssecretsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Tandem.V1.Models[modelId].Resetstreamssecrets
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Delete datapoints of multiple streams.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/timeseries/models/{modelID}/deletestreamsdata
    /// </remarks>
    /// <param name="modelId">Model URN of the model owning the stream</param>
    /// <param name="body">The request body with stream keys</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports substreams, from, to, allSubstreams query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.StreamsManager.DeleteStreamsDataAsync("urn:adsk.dtm:...", new DeletestreamsdataPostRequestBody { Keys = ["key1"] });
    /// </code>
    /// </example>
    public async Task DeleteStreamsDataAsync(
        string modelId,
        DeletestreamsdataPostRequestBody body,
        RequestConfiguration<DeletestreamsdataRequestBuilder.DeletestreamsdataRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Tandem.V1.Timeseries.Models[modelId].Deletestreamsdata
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the latest time series data for each provided element.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/timeseries/models/{modelID}/streams
    /// </remarks>
    /// <param name="modelId">ID of the model owning the stream</param>
    /// <param name="body">The request body with stream element keys</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StreamsPostResponse"/> containing the latest time series data</returns>
    /// <example>
    /// <code>
    /// StreamsPostResponse? data = await client.StreamsManager.GetLatestStreamsDataAsync("urn:adsk.dtm:...", new StreamsPostRequestBody { Keys = ["key1"] });
    /// </code>
    /// </example>
    public async Task<StreamsPostResponse?> GetLatestStreamsDataAsync(
        string modelId,
        StreamsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Timeseries.Models[modelId].Streams
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves time series data for a specific stream element.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/timeseries/models/{modelID}/streams/{elementID}
    /// </remarks>
    /// <param name="modelId">ID of the model owning the stream</param>
    /// <param name="elementId">ID of the stream element</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports from, to, limit, sort query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithElementGetResponse"/> containing the time series data</returns>
    /// <example>
    /// <code>
    /// WithElementGetResponse? data = await client.StreamsManager.GetStreamDataAsync("urn:adsk.dtm:...", "elementId");
    /// </code>
    /// </example>
    public async Task<WithElementGetResponse?> GetStreamDataAsync(
        string modelId,
        string elementId,
        RequestConfiguration<WithElementItemRequestBuilder.WithElementItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Timeseries.Models[modelId].Streams[elementId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Stores time series data for a specific stream element.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/timeseries/models/{modelID}/streams/{elementID}
    /// </remarks>
    /// <param name="modelId">ID of the model owning the stream</param>
    /// <param name="elementId">ID of the stream element</param>
    /// <param name="body">The time series data to store</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.StreamsManager.StoreStreamDataAsync("urn:adsk.dtm:...", "elementId", new WithElementPostRequestBody());
    /// </code>
    /// </example>
    public async Task StoreStreamDataAsync(
        string modelId,
        string elementId,
        WithElementPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Tandem.V1.Timeseries.Models[modelId].Streams[elementId]
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Generic endpoint for posting stream data via webhook. It expects either a single JSON object or an array of objects, where each object is treated as an individual event.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/timeseries/models/{modelID}/webhooks/generic
    /// </remarks>
    /// <param name="modelId">Model ID</param>
    /// <param name="body">Array of objects with stream data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response data</returns>
    /// <example>
    /// <code>
    /// Stream? result = await client.StreamsManager.PostGenericWebhookDataAsync("urn:adsk.dtm:...", new List&lt;Generic&gt; { new Generic() });
    /// </code>
    /// </example>
    public async Task<Stream?> PostGenericWebhookDataAsync(
        string modelId,
        List<Generic> body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Timeseries.Models[modelId].Webhooks.Generic
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
