using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Attributes;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Attributes.Item;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.RfiTypes;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.CustomIdentifier;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Attachments;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Comments;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Responses;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Responses.Item;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.SearchRfis;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Users.Me;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Workflow;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Attributes.AttributesRequestBuilder;
using static Autodesk.ACC.Construction.Rfis.V3.Projects.Item.RfiTypes.RfiTypesRequestBuilder;
using static Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Attachments.AttachmentsRequestBuilder;
using static Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Comments.CommentsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for ACC RFIs operations — manages RFIs, custom attributes, responses, comments,
/// attachments, RFI types, workflow, and current user permissions.
/// </summary>
public class RFIsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="RFIsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public RFIsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Searches RFIs in a project with filters and pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/rfis/v3/projects/{projectId}/search:rfis
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfi-search-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The search criteria and pagination options</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SearchRfisPostResponse"/> containing the search results, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// SearchRfisPostResponse? results = await client.RFIsManager.SearchRfisAsync(projectId, new SearchRfisPostRequestBody());
    /// </code>
    /// </example>
    public async Task<SearchRfisPostResponse?> SearchRfisAsync(
        string projectId,
        SearchRfisPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .SearchRfis
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a specific RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRfiGetResponse"/> containing the RFI details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithRfiGetResponse? rfi = await client.RFIsManager.GetRfiAsync(projectId, rfiId);
    /// </code>
    /// </example>
    public async Task<WithRfiGetResponse?> GetRfiAsync(
        string projectId,
        string rfiId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a new RFI in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/rfis/v3/projects/{projectId}/rfis
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The RFI creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RfisPostResponse"/> containing the created RFI, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// RfisPostResponse? rfi = await client.RFIsManager.CreateRfiAsync(projectId, new RfisPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RfisPostResponse?> CreateRfiAsync(
        string projectId,
        RfisPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates an existing RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-id-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="body">The RFI update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRfiPatchResponse"/> containing the updated RFI, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithRfiPatchResponse? updated = await client.RFIsManager.UpdateRfiAsync(projectId, rfiId, new WithRfiPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithRfiPatchResponse?> UpdateRfiAsync(
        string projectId,
        string rfiId,
        WithRfiPatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the current and next available RFI custom identifier for the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/rfis/custom-identifier
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-custom-identifier-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CustomIdentifierGetResponse"/> containing the custom identifier info, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CustomIdentifierGetResponse? customId = await client.RFIsManager.GetCustomIdentifierAsync(projectId);
    /// </code>
    /// </example>
    public async Task<CustomIdentifierGetResponse?> GetCustomIdentifierAsync(
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis
            .CustomIdentifier
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the list of RFI types configured for the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/rfi-types
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-RFI-types-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter[status])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RfiTypesGetResponse"/> containing the RFI types, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// RfiTypesGetResponse? types = await client.RFIsManager.GetRfiTypesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<RfiTypesGetResponse?> GetRfiTypesAsync(
        string projectId,
        Action<RequestConfiguration<RfiTypesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .RfiTypes
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves all custom attribute definitions for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/attributes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-attributes-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter[status])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttributesGetResponse"/> containing the custom attributes, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttributesGetResponse? attributes = await client.RFIsManager.GetAttributesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<AttributesGetResponse?> GetAttributesAsync(
        string projectId,
        Action<RequestConfiguration<AttributesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Attributes
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a custom attribute definition for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/rfis/v3/projects/{projectId}/attributes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-attributes-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The attribute creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttributesPostResponse"/> containing the created attribute, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttributesPostResponse? attribute = await client.RFIsManager.CreateAttributeAsync(projectId, new AttributesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttributesPostResponse?> CreateAttributeAsync(
        string projectId,
        AttributesPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Attributes
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates an existing custom attribute definition for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/rfis/v3/projects/{projectId}/attributes/{attributeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-custom-attributes-attributeId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="attributeId">The ID of the attribute</param>
    /// <param name="body">The attribute update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAttributePatchResponse"/> containing the updated attribute, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithAttributePatchResponse? updated = await client.RFIsManager.UpdateAttributeAsync(projectId, attributeId, new WithAttributePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithAttributePatchResponse?> UpdateAttributeAsync(
        string projectId,
        string attributeId,
        WithAttributePatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Attributes[attributeId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of attachments for a specific RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-id-attachments-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter[attachmentTypes])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsGetResponse"/> containing the attachments, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttachmentsGetResponse? attachments = await client.RFIsManager.GetRfiAttachmentsAsync(projectId, rfiId);
    /// </code>
    /// </example>
    public async Task<AttachmentsGetResponse?> GetRfiAttachmentsAsync(
        string projectId,
        string rfiId,
        Action<RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .Attachments
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a response to the specified RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}/responses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-id-responses-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="body">The response creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ResponsesPostResponse"/> containing the created RFI response, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// ResponsesPostResponse? rfiResponse = await client.RFIsManager.CreateRfiResponseAsync(projectId, rfiId, new ResponsesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ResponsesPostResponse?> CreateRfiResponseAsync(
        string projectId,
        string rfiId,
        ResponsesPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .Responses
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates an existing RFI response.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}/responses/{responseId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-id-responses-responseId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="responseId">The ID of the response</param>
    /// <param name="body">The response update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithResponsePatchResponse"/> containing the updated response, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithResponsePatchResponse? updated = await client.RFIsManager.UpdateRfiResponseAsync(projectId, rfiId, responseId, new WithResponsePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithResponsePatchResponse?> UpdateRfiResponseAsync(
        string projectId,
        string rfiId,
        string responseId,
        WithResponsePatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .Responses[responseId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of comments associated with a specific RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-rfiId-comments-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, sort, fields, filter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CommentsGetResponse"/> containing the comments, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CommentsGetResponse? comments = await client.RFIsManager.GetRfiCommentsAsync(projectId, rfiId);
    /// </code>
    /// </example>
    public async Task<CommentsGetResponse?> GetRfiCommentsAsync(
        string projectId,
        string rfiId,
        Action<RequestConfiguration<CommentsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .Comments
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Adds a comment to an RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/rfis/v3/projects/{projectId}/rfis/{rfiId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-rfis-rfiId-comments-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="rfiId">The ID of the RFI</param>
    /// <param name="body">The comment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CommentsPostResponse"/> containing the created comment, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CommentsPostResponse? comment = await client.RFIsManager.CreateRfiCommentAsync(projectId, rfiId, new CommentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CommentsPostResponse?> CreateRfiCommentAsync(
        string projectId,
        string rfiId,
        CommentsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Rfis[rfiId]
            .Comments
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns the current user permissions for RFIs in the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-users-me-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MeGetResponse"/> containing the current user profile and permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.RFIsManager.GetCurrentUserAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Users
            .Me
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the workflow configuration for the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/rfis/v3/projects/{projectId}/workflow
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/rfis-workflow-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkflowGetResponse"/> containing the workflow configuration, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WorkflowGetResponse? workflow = await client.RFIsManager.GetWorkflowAsync(projectId);
    /// </code>
    /// </example>
    public async Task<WorkflowGetResponse?> GetWorkflowAsync(
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Rfis.V3.Projects[projectId]
            .Workflow
            .GetAsync(requestConfiguration, cancellationToken);
    }
}
