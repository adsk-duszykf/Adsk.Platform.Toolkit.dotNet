using System.Runtime.CompilerServices;
using Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Instances;
using Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Templates;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Checklists (Field Management) — checklist instances and templates under a project container.
/// </summary>
/// <remarks>
/// The Kiota model for this SDK currently exposes GET operations for instances and templates only.
/// When the OpenAPI bundle adds POST/PATCH for these resources, regenerate the client and extend this manager.
/// </remarks>
public class ChecklistsManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChecklistsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ChecklistsManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all checklist instances in a container, following JSON:API <c>links.next</c> until exhausted.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/instances
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-instances-GET
    /// </remarks>
    /// <param name="containerId">Checklists container ID for the project (see APS tutorial &quot;Retrieve a Container ID&quot;)</param>
    /// <param name="requestConfiguration">(Optional) Configuration (filters, limit, offset, sort, sparse fieldsets)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{InstancesGetResponse_data}"/> of checklist instance resources</returns>
    /// <example>
    /// <code>
    /// await foreach (InstancesGetResponse_data row in client.ChecklistsManager.ListInstancesAsync("container-guid"))
    /// {
    ///     Console.WriteLine(row.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<InstancesGetResponse_data> ListInstancesAsync(
        string containerId,
        RequestConfiguration<InstancesRequestBuilder.InstancesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? nextUrl = null;
        InstancesRequestBuilder root = _api.Bim360.Checklists.V1.Containers[containerId].Instances;

        while (true)
        {
            InstancesGetResponse? response;
            if (nextUrl is not null)
            {
                response = await root.WithUrl(nextUrl).GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
            }
            else
            {
                response = await root.GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
            }

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (InstancesGetResponse_data item in response.Data)
            {
                yield return item;
            }

            nextUrl = response.Links?.Next;
            if (string.IsNullOrEmpty(nextUrl))
                yield break;
        }
    }

    /// <summary>
    /// Returns a single page of checklist instances (no automatic pagination).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/instances
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-instances-GET
    /// </remarks>
    /// <param name="containerId">Checklists container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The JSON:API payload for one request</returns>
    /// <example>
    /// <code>
    /// InstancesGetResponse? page = await client.ChecklistsManager.GetInstancesPageAsync("container-guid");
    /// </code>
    /// </example>
    public async Task<InstancesGetResponse?> GetInstancesPageAsync(
        string containerId,
        RequestConfiguration<InstancesRequestBuilder.InstancesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Checklists.V1.Containers[containerId].Instances.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves one checklist instance by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/instances/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-instances-id-GET
    /// </remarks>
    /// <param name="containerId">Checklists container ID</param>
    /// <param name="instanceId">Checklist instance ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (fields, include, signatures)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The instance resource payload</returns>
    /// <example>
    /// <code>
    /// InstancesGetResponse? one = await client.ChecklistsManager.GetInstanceAsync("container-guid", "instance-id");
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Instances.Item.InstancesGetResponse?> GetInstanceAsync(
        string containerId,
        string instanceId,
        RequestConfiguration<Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Instances.Item.InstancesItemRequestBuilder.InstancesItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Checklists.V1.Containers[containerId].Instances[instanceId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all checklist templates in a container, following JSON:API <c>links.next</c> until exhausted.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/templates
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-templates-GET
    /// </remarks>
    /// <param name="containerId">Checklists container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (filters, limit, page offset, sort)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{TemplatesGetResponse_data}"/> of template resources</returns>
    /// <example>
    /// <code>
    /// await foreach (TemplatesGetResponse_data template in client.ChecklistsManager.ListTemplatesAsync("container-guid"))
    /// {
    ///     Console.WriteLine(template.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TemplatesGetResponse_data> ListTemplatesAsync(
        string containerId,
        RequestConfiguration<TemplatesRequestBuilder.TemplatesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? nextUrl = null;
        TemplatesRequestBuilder root = _api.Bim360.Checklists.V1.Containers[containerId].Templates;

        while (true)
        {
            TemplatesGetResponse? response;
            if (nextUrl is not null)
            {
                response = await root.WithUrl(nextUrl).GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
            }
            else
            {
                response = await root.GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
            }

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (TemplatesGetResponse_data item in response.Data)
            {
                yield return item;
            }

            nextUrl = response.Links?.Next;
            if (string.IsNullOrEmpty(nextUrl))
                yield break;
        }
    }

    /// <summary>
    /// Returns a single page of checklist templates (no automatic pagination).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/templates
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-templates-GET
    /// </remarks>
    public async Task<TemplatesGetResponse?> GetTemplatesPageAsync(
        string containerId,
        RequestConfiguration<TemplatesRequestBuilder.TemplatesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Checklists.V1.Containers[containerId].Templates.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves one checklist template by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/checklists/v1/containers/{containerId}/templates/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/checklists-templates-id-GET
    /// </remarks>
    /// <param name="containerId">Checklists container ID</param>
    /// <param name="templateId">Template ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration (sparse fieldsets)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The template resource payload</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Templates.Item.TemplatesGetResponse? t =
    ///     await client.ChecklistsManager.GetTemplateAsync("container-guid", "template-id");
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Templates.Item.TemplatesGetResponse?> GetTemplateAsync(
        string containerId,
        string templateId,
        RequestConfiguration<Autodesk.BIM360.Bim360.Checklists.V1.Containers.Item.Templates.Item.TemplatesItemRequestBuilder.TemplatesItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Checklists.V1.Containers[containerId].Templates[templateId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
