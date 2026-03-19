using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Forms.V1.Projects.Item.Forms;
using Autodesk.ACC.Construction.Forms.V1.Projects.Item.Forms.Item.ValuesBatchUpdate;
using Autodesk.ACC.Construction.Forms.V1.Projects.Item.FormTemplates;
using Autodesk.ACC.Construction.Forms.V1.Projects.Item.FormTemplates.Item.Forms;
using Autodesk.ACC.Construction.Forms.V1.Projects.Item.FormTemplates.Item.Forms.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Forms.V1.Projects.Item.Forms.FormsRequestBuilder;
using static Autodesk.ACC.Construction.Forms.V1.Projects.Item.FormTemplates.FormTemplatesRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Forms operations — manages form templates, forms, and form values
/// in ACC projects.
/// </summary>
public class FormsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public FormsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns all project form templates the user has access to.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/forms/v1/projects/{projectId}/form-templates
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/forms-form-templates-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, SortOrder, UpdatedAfter, UpdatedBefore)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FormTemplatesGetResponse"/> containing the form templates</returns>
    /// <example>
    /// <code>
    /// FormTemplatesGetResponse? templates = await client.FormsManager.GetFormTemplatesAsync("projectId");
    /// </code>
    /// </example>
    public async Task<FormTemplatesGetResponse?> GetFormTemplatesAsync(
        string projectId,
        Action<RequestConfiguration<FormTemplatesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Forms.V1.Projects[projectId]
            .FormTemplates
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of forms in a project with automatic pagination.
    /// Forms are sorted by updatedAt, most recent first.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/forms/v1/projects/{projectId}/forms
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/forms-forms-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering by status, template, dates, locations, etc.)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{FormsGetResponse_data}"/> of <see cref="FormsGetResponse_data"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var form in client.FormsManager.ListFormsAsync("projectId"))
    /// {
    ///     Console.WriteLine(form.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<FormsGetResponse_data> ListFormsAsync(
        string projectId,
        Action<RequestConfiguration<FormsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = 0;

        while (true)
        {
            var capturedOffset = offset;
            var response = await _api.Construction.Forms.V1.Projects[projectId]
                .Forms
                .GetAsync(config =>
                {
                    requestConfiguration?.Invoke(config);
                    config.QueryParameters.Offset = capturedOffset;
                }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (var item in response.Data)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Data.Count;
        }
    }

    /// <summary>
    /// Adds a new form to a project based on a form template.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/forms/v1/projects/{projectId}/form-templates/{templateId}/forms
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/forms-forms-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="templateId">The ID of the form template</param>
    /// <param name="body">The form creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FormsPostResponse"/> containing the created form</returns>
    /// <example>
    /// <code>
    /// FormsPostResponse? form = await client.FormsManager.CreateFormAsync("projectId", "templateId", new FormsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<FormsPostResponse?> CreateFormAsync(
        string projectId,
        string templateId,
        FormsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Forms.V1.Projects[projectId]
            .FormTemplates[templateId]
            .Forms
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates a form's details. Note: PDF forms are not currently supported for updates.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/forms/v1/projects/{projectId}/form-templates/{templateId}/forms/{formId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/forms-forms-formId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="templateId">The ID of the form template</param>
    /// <param name="formId">The ID of the form to update</param>
    /// <param name="body">The form update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithFormPatchResponse"/> containing the updated form</returns>
    /// <example>
    /// <code>
    /// WithFormPatchResponse? updated = await client.FormsManager.UpdateFormAsync("projectId", "templateId", "formId", new WithFormPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithFormPatchResponse?> UpdateFormAsync(
        string projectId,
        string templateId,
        string formId,
        WithFormPatchRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Forms.V1.Projects[projectId]
            .FormTemplates[templateId]
            .Forms[formId]
            .PatchAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Updates a form's main form fields, both tabular and non-tabular.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /construction/forms/v1/projects/{projectId}/forms/{formId}/values:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/forms-valuesbatch-update-PUT
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="formId">The ID of the form</param>
    /// <param name="body">The batch update data for form field values</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ValuesBatchUpdatePutResponse"/> containing the batch update result</returns>
    /// <example>
    /// <code>
    /// ValuesBatchUpdatePutResponse? result = await client.FormsManager.BatchUpdateFormValuesAsync("projectId", "formId", new ValuesBatchUpdatePutRequestBody());
    /// </code>
    /// </example>
    public async Task<ValuesBatchUpdatePutResponse?> BatchUpdateFormValuesAsync(
        string projectId,
        string formId,
        ValuesBatchUpdatePutRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Forms.V1.Projects[projectId]
            .Forms[formId]
            .ValuesBatchUpdate
            .PutAsync(body, requestConfiguration, cancellationToken);
    }
}
