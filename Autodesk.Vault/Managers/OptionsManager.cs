using System.Runtime.CompilerServices;
using Autodesk.Vault.Models;
using Autodesk.Vault.SystemOptions;
using Autodesk.Vault.SystemOptions.Item;
using Autodesk.Vault.Vaults.Item.VaultOptions;
using Autodesk.Vault.Vaults.Item.VaultOptions.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Vault.SystemOptions.SystemOptionsRequestBuilder;
using static Autodesk.Vault.Vaults.Item.VaultOptions.VaultOptionsRequestBuilder;

namespace Autodesk.Vault.Managers;

/// <summary>
/// Manager for Options operations (System Options and Vault Options)
/// </summary>
public class OptionsManager
{
    private readonly BaseVaultClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public OptionsManager(BaseVaultClient api)
    {
        _api = api;
    }

    #region System Options

    /// <summary>
    /// Lists options which apply to the entire system with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /system-options
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[name], limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="VaultOption"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (VaultOption option in client.Options.ListSystemOptionsAsync())
    /// {
    ///     Console.WriteLine($"{option.Name}: {option.Value}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<VaultOption> ListSystemOptionsAsync(
        RequestConfiguration<SystemOptionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            VaultOptionCollection? response = await _api.SystemOptions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (VaultOption item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Creates a system-wide option with input name and value.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /system-options
    /// </remarks>
    /// <param name="optionData">System option data containing name and value</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the created system option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.CreateSystemOptionAsync(
    ///     new SystemOptionsPostRequestBody { Name = "MyOption", Value = "MyValue" });
    /// </code>
    /// </example>
    public async Task<VaultOption?> CreateSystemOptionAsync(
        SystemOptionsPostRequestBody optionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.SystemOptions
            .PostAsync(optionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a system option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /system-options/{id}
    /// </remarks>
    /// <param name="optionId">The unique identifier of a system option</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the system option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.GetSystemOptionByIdAsync("42");
    /// </code>
    /// </example>
    public async Task<VaultOption?> GetSystemOptionByIdAsync(
        string optionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.SystemOptions[optionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Update a system option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /system-options/{id}
    /// </remarks>
    /// <param name="optionId">The unique identifier of a system option</param>
    /// <param name="optionData">Updated system option data containing the new value</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the updated system option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.UpdateSystemOptionByIdAsync("42",
    ///     new SystemOptionsPatchRequestBody { Value = "NewValue" });
    /// </code>
    /// </example>
    public async Task<VaultOption?> UpdateSystemOptionByIdAsync(
        string optionId,
        SystemOptionsPatchRequestBody optionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.SystemOptions[optionId]
            .PatchAsync(optionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Delete a system option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /system-options/{id}
    /// </remarks>
    /// <param name="optionId">The unique identifier of a system option</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.Options.DeleteSystemOptionByIdAsync("42");
    /// </code>
    /// </example>
    public async Task DeleteSystemOptionByIdAsync(
        string optionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.SystemOptions[optionId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Vault Options

    /// <summary>
    /// Lists vault options for a specific vault with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/vault-options
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[name], limit, cursorState)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="VaultOption"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (VaultOption option in client.Options.ListVaultOptionsAsync("1"))
    /// {
    ///     Console.WriteLine($"{option.Name}: {option.Value}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<VaultOption> ListVaultOptionsAsync(
        string vaultId,
        RequestConfiguration<VaultOptionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            VaultOptionCollection? response = await _api.Vaults[vaultId].VaultOptions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (VaultOption item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            cursor = PaginationHelper.ExtractBookmarkFromNextUrl(response.Pagination.NextUrl);
        }
    }

    /// <summary>
    /// Create a vault option.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /vaults/{vaultId}/vault-options
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="optionData">Vault option data containing name and value</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the created vault option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.CreateVaultOptionAsync("1",
    ///     new VaultOptionsPostRequestBody { Name = "MyOption", Value = "MyValue" });
    /// </code>
    /// </example>
    public async Task<VaultOption?> CreateVaultOptionAsync(
        string vaultId,
        VaultOptionsPostRequestBody optionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].VaultOptions
            .PostAsync(optionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Get a vault option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /vaults/{vaultId}/vault-options/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="optionId">The unique identifier of a vault option</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the vault option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.GetVaultOptionByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task<VaultOption?> GetVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].VaultOptions[optionId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Update a vault option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /vaults/{vaultId}/vault-options/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="optionId">The unique identifier of a vault option</param>
    /// <param name="optionData">Updated vault option data containing the new value</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VaultOption"/> containing the updated vault option</returns>
    /// <example>
    /// <code>
    /// VaultOption? option = await client.Options.UpdateVaultOptionByIdAsync("1", "42",
    ///     new VaultOptionsPatchRequestBody { Value = "NewValue" });
    /// </code>
    /// </example>
    public async Task<VaultOption?> UpdateVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        VaultOptionsPatchRequestBody optionData,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Vaults[vaultId].VaultOptions[optionId]
            .PatchAsync(optionData, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Delete a vault option by its ID.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /vaults/{vaultId}/vault-options/{id}
    /// </remarks>
    /// <param name="vaultId">The unique identifier of a vault</param>
    /// <param name="optionId">The unique identifier of a vault option</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.Options.DeleteVaultOptionByIdAsync("1", "42");
    /// </code>
    /// </example>
    public async Task DeleteVaultOptionByIdAsync(
        string vaultId,
        string optionId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Vaults[vaultId].VaultOptions[optionId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion
}
