using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Rules.Item;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.RulesEvaluate;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.RulesValidate;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.InformedDesign.Managers;

/// <summary>
/// Manager for Rules operations
/// </summary>
public class RulesManager
{
    private readonly BaseInformedDesignClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="RulesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public RulesManager(BaseInformedDesignClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Evaluates inputs against the rules converted and parsed from the codeblocks in the provided rulesKey
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/rules:evaluate
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-evaluaterules-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="RulesEvaluatePostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// RulesEvaluatePostRequestBody body = new();
    /// RulesEvaluatePostResponse? result = await client.RulesManager.EvaluateRulesAsync(body);
    /// </code>
    /// </example>
    public async Task<RulesEvaluatePostResponse?> EvaluateRulesAsync(
        RulesEvaluatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.RulesEvaluate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Validates if codeblocks exist in the provided rulesKey and can be parsed successfully
    /// </summary>
    /// <remarks>
    /// Wraps: POST /industrialized-construction/informed-design/v1/rules:validate
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-validaterules-POST    /// </remarks>
    /// <param name="body">The request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="RulesValidatePostResponse"/> from the API, or <c>null</c> if the response was empty.</returns>
    /// <example>
    /// <code>
    /// RulesValidatePostRequestBody body = new();
    /// RulesValidatePostResponse? result = await client.RulesManager.ValidateRulesAsync(body);
    /// </code>
    /// </example>
    public async Task<RulesValidatePostResponse?> ValidateRulesAsync(
        RulesValidatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.RulesValidate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves server-side generated code of the codeblocks contained in the provided rulesKey
    /// </summary>
    /// <remarks>
    /// Wraps: GET /industrialized-construction/informed-design/v1/rules/{rulesKey}
    /// APS docs: https://aps.autodesk.com/en/docs/informed-design/v1/reference/quick_reference/informed-design-api-getrules-GET    /// </remarks>
    /// <param name="rulesKey">The rules key that identifies the rules document.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request such as headers, query parameters, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token to use when cancelling requests.</param>
    /// <returns>The <see cref="WithRulesKeyGetResponse"/> for the rules key, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// string rulesKey = "my-rules-key";
    /// WithRulesKeyGetResponse? rules = await client.RulesManager.GetRulesAsync(rulesKey);
    /// </code>
    /// </example>
    public async Task<WithRulesKeyGetResponse?> GetRulesAsync(
        string rulesKey,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.IndustrializedConstruction.InformedDesign.V1.Rules[rulesKey]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
