using System.Runtime.CompilerServices;
using Autodesk.ACC.Cost.V1.Containers.Item.Attachments;
using Autodesk.ACC.Cost.V1.Containers.Item.AttachmentsBatchCreate;
using Autodesk.ACC.Cost.V1.Containers.Item.AttachmentFolders;
using Autodesk.ACC.Cost.V1.Containers.Item.Budgets;
using Autodesk.ACC.Cost.V1.Containers.Item.BudgetsContractsLink;
using Autodesk.ACC.Cost.V1.Containers.Item.BudgetsImport;
using Autodesk.ACC.Cost.V1.Containers.Item.ChangeOrders;
using Autodesk.ACC.Cost.V1.Containers.Item.Contracts;
using Autodesk.ACC.Cost.V1.Containers.Item.CostItems;
using Autodesk.ACC.Cost.V1.Containers.Item.CostItemsAttach;
using Autodesk.ACC.Cost.V1.Containers.Item.CostItemsBatchCreate;
using Autodesk.ACC.Cost.V1.Containers.Item.CostItemsDetach;
using Autodesk.ACC.Cost.V1.Containers.Item.Documents;
using Autodesk.ACC.Cost.V1.Containers.Item.Expenses;
using Autodesk.ACC.Cost.V1.Containers.Item.MainContracts;
using Autodesk.ACC.Cost.V1.Containers.Item.PaymentItems;
using Autodesk.ACC.Cost.V1.Containers.Item.Payments;
using Autodesk.ACC.Cost.V1.Containers.Item.PerformanceTrackingItems;
using Autodesk.ACC.Cost.V1.Containers.Item.Properties;
using Autodesk.ACC.Cost.V1.Containers.Item.PropertyValuesBatchUpdate;
using Autodesk.ACC.Cost.V1.Containers.Item.ScheduleOfValues;
using Autodesk.ACC.Cost.V1.Containers.Item.SegmentValues;
using Autodesk.ACC.Cost.V1.Containers.Item.Taxes;
using Autodesk.ACC.Cost.V1.Containers.Item.Templates;
using Autodesk.ACC.Cost.V1.Containers.Item.TimeSheets;
using CostCont = Autodesk.ACC.Cost.V1.Containers.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.Budgets.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.ChangeOrders.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.ChangeOrders.Item.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.Contracts.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.CostItems.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.Segments.Item.Values;
using Autodesk.ACC.Cost.V1.Containers.Item.Segments.Item.Values.Item;
using Autodesk.ACC.Cost.V1.Containers.Item.Segments.Item.ValuesImport;
using Autodesk.ACC.Cost.V1.Containers.Item.Templates.Item.Segments;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Cost.V1.Containers.Item.Budgets.BudgetsRequestBuilder;
using static Autodesk.ACC.Cost.V1.Containers.Item.Contracts.ContractsRequestBuilder;
using static Autodesk.ACC.Cost.V1.Containers.Item.CostItems.CostItemsRequestBuilder;
using static Autodesk.ACC.Cost.V1.Containers.Item.Expenses.ExpensesRequestBuilder;
using static Autodesk.ACC.Cost.V1.Containers.Item.Attachments.AttachmentsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Cost Management operations — manages budgets, contracts, change orders, expenses, and cost settings.
/// </summary>
public class CostManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="CostManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public CostManager(BaseACCclient api)
    {
        _api = api;
    }

    #region Budget Operations

    /// <summary>
    /// Lists all budgets in a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/budgets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="BudgetsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var budget in client.CostManager.ListBudgetsAsync(projectId))
    /// {
    ///     Console.WriteLine(budget.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<BudgetsGetResponse_results> ListBudgetsAsync(
        Guid containerId,
        RequestConfiguration<BudgetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].Budgets
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Gets a single page of budgets.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/budgets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BudgetsGetResponse"/> containing a single page of budgets</returns>
    /// <example>
    /// <code>
    /// BudgetsGetResponse? budgets = await client.CostManager.GetBudgetsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<BudgetsGetResponse?> GetBudgetsAsync(
        Guid containerId,
        RequestConfiguration<BudgetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Budgets.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a budget in the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/budgets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The budget creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BudgetsPostResponse"/> containing the created budget</returns>
    /// <example>
    /// <code>
    /// BudgetsPostResponse? budget = await client.CostManager.CreateBudgetAsync(projectId, new BudgetsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<BudgetsPostResponse?> CreateBudgetAsync(
        Guid containerId,
        BudgetsPostRequestBody body,
        RequestConfiguration<BudgetsRequestBuilder.BudgetsRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Budgets.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a budget by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/budgets/{budgetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-budgetId-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="budgetId">The budget ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithBudgetGetResponse"/> containing the budget</returns>
    /// <example>
    /// <code>
    /// WithBudgetGetResponse? budget = await client.CostManager.GetBudgetAsync(projectId, budgetId);
    /// </code>
    /// </example>
    public async Task<WithBudgetGetResponse?> GetBudgetAsync(
        Guid containerId,
        string budgetId,
        RequestConfiguration<WithBudgetItemRequestBuilder.WithBudgetItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Budgets[budgetId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a budget.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/budgets/{budgetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-budgetId-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="budgetId">The budget ID</param>
    /// <param name="body">The budget update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithBudgetPatchResponse"/> containing the updated budget</returns>
    /// <example>
    /// <code>
    /// WithBudgetPatchResponse? updated = await client.CostManager.UpdateBudgetAsync(projectId, budgetId, new WithBudgetPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithBudgetPatchResponse?> UpdateBudgetAsync(
        Guid containerId,
        string budgetId,
        WithBudgetPatchRequestBody body,
        RequestConfiguration<WithBudgetItemRequestBuilder.WithBudgetItemRequestBuilderPatchQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Budgets[budgetId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a budget.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/budgets/{budgetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-budgetId-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="budgetId">The budget ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteBudgetAsync(projectId, budgetId);
    /// </code>
    /// </example>
    public async Task DeleteBudgetAsync(
        Guid containerId,
        string budgetId,
        RequestConfiguration<WithBudgetItemRequestBuilder.WithBudgetItemRequestBuilderDeleteQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].Budgets[budgetId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Imports budgets into the project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/budgets:import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgetsimport-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The import request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.ImportBudgetsAsync(projectId, new BudgetsImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task ImportBudgetsAsync(
        Guid containerId,
        BudgetsImportPostRequestBody body,
        RequestConfiguration<CostCont.BudgetsImport.BudgetsImportRequestBuilder.BudgetsImportRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].BudgetsImport.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Links budgets to contracts.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/budgets-contracts:link
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-budgets-contractslink-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The link request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response data</returns>
    /// <example>
    /// <code>
    /// System.IO.Stream? stream = await client.CostManager.LinkBudgetsContractsAsync(projectId, new BudgetsContractsLinkPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Stream?> LinkBudgetsContractsAsync(
        Guid containerId,
        BudgetsContractsLinkPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].BudgetsContractsLink.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Change Order Operations

    /// <summary>
    /// Lists all change orders (all types) in the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/change-orders
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ChangeOrdersGetResponse"/> containing the change orders</returns>
    /// <example>
    /// <code>
    /// ChangeOrdersGetResponse? orders = await client.CostManager.GetChangeOrdersAsync(projectId);
    /// </code>
    /// </example>
    public async Task<ChangeOrdersGetResponse?> GetChangeOrdersAsync(
        Guid containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].ChangeOrders.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists change orders of a specific type (pco, rfq, rco, oco, sco) with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/change-orders/{changeOrder}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-changeOrder-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="changeOrderType">The change order type (e.g. pco, rfq, rco)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="WithChangeOrderGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var order in client.CostManager.ListChangeOrdersByTypeAsync(projectId, "pco"))
    /// {
    ///     Console.WriteLine(order.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<WithChangeOrderGetResponse_results> ListChangeOrdersByTypeAsync(
        Guid containerId,
        string changeOrderType,
        RequestConfiguration<WithChangeOrderItemRequestBuilder.WithChangeOrderItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].ChangeOrders[changeOrderType]
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new change order (e.g. PCO).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/change-orders/{changeOrder}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-changeOrder-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="changeOrderType">The change order type</param>
    /// <param name="body">The change order creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithChangeOrderPostResponse"/> containing the created change order</returns>
    /// <example>
    /// <code>
    /// WithChangeOrderPostResponse? order = await client.CostManager.CreateChangeOrderAsync(projectId, "pco", new WithChangeOrderPostRequestBody());
    /// </code>
    /// </example>
    public async Task<WithChangeOrderPostResponse?> CreateChangeOrderAsync(
        Guid containerId,
        string changeOrderType,
        WithChangeOrderPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].ChangeOrders[changeOrderType].PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a change order by type and ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/change-orders/{changeOrder}/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-changeOrder-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="changeOrderType">The change order type</param>
    /// <param name="changeOrderId">The change order ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.ChangeOrders.Item.Item.ChangeOrderGetResponse"/> containing the change order</returns>
    /// <example>
    /// <code>
    /// CostCont.ChangeOrders.Item.Item.ChangeOrderGetResponse? order = await client.CostManager.GetChangeOrderAsync(projectId, "pco", changeOrderId);
    /// </code>
    /// </example>
    public async Task<CostCont.ChangeOrders.Item.Item.ChangeOrderGetResponse?> GetChangeOrderAsync(
        Guid containerId,
        string changeOrderType,
        Guid changeOrderId,
        RequestConfiguration<ChangeOrderItemRequestBuilder.ChangeOrderItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].ChangeOrders[changeOrderType][changeOrderId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a change order.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/change-orders/{changeOrder}/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-changeOrder-id-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="changeOrderType">The change order type</param>
    /// <param name="changeOrderId">The change order ID</param>
    /// <param name="body">The change order update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ChangeOrderPatchResponse"/> containing the updated change order</returns>
    /// <example>
    /// <code>
    /// ChangeOrderPatchResponse? updated = await client.CostManager.UpdateChangeOrderAsync(projectId, "pco", changeOrderId, new ChangeOrderPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<ChangeOrderPatchResponse?> UpdateChangeOrderAsync(
        Guid containerId,
        string changeOrderType,
        Guid changeOrderId,
        ChangeOrderPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].ChangeOrders[changeOrderType][changeOrderId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a change order.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/change-orders/{changeOrder}/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-change-orders-changeOrder-id-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="changeOrderType">The change order type</param>
    /// <param name="changeOrderId">The change order ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteChangeOrderAsync(projectId, "pco", changeOrderId);
    /// </code>
    /// </example>
    public async Task DeleteChangeOrderAsync(
        Guid containerId,
        string changeOrderType,
        Guid changeOrderId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].ChangeOrders[changeOrderType][changeOrderId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Contract Operations

    /// <summary>
    /// Lists all contracts with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/contracts
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-contracts-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ContractsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var contract in client.CostManager.ListContractsAsync(projectId))
    /// {
    ///     Console.WriteLine(contract.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ContractsGetResponse_results> ListContractsAsync(
        Guid containerId,
        RequestConfiguration<ContractsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].Contracts
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a contract in the project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/contracts
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-contracts-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The contract creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ContractsPostResponse"/> containing the created contract</returns>
    /// <example>
    /// <code>
    /// ContractsPostResponse? contract = await client.CostManager.CreateContractAsync(projectId, new ContractsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ContractsPostResponse?> CreateContractAsync(
        Guid containerId,
        ContractsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Contracts.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a contract by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/contracts/{contractId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-contracts-contractId-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="contractId">The contract ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithContractGetResponse"/> containing the contract</returns>
    /// <example>
    /// <code>
    /// WithContractGetResponse? contract = await client.CostManager.GetContractAsync(projectId, contractId);
    /// </code>
    /// </example>
    public async Task<WithContractGetResponse?> GetContractAsync(
        Guid containerId,
        string contractId,
        RequestConfiguration<WithContractItemRequestBuilder.WithContractItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Contracts[contractId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a contract.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/contracts/{contractId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-contracts-contractId-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="contractId">The contract ID</param>
    /// <param name="body">The contract update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithContractPatchResponse"/> containing the updated contract</returns>
    /// <example>
    /// <code>
    /// WithContractPatchResponse? updated = await client.CostManager.UpdateContractAsync(projectId, contractId, new WithContractPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithContractPatchResponse?> UpdateContractAsync(
        Guid containerId,
        string contractId,
        WithContractPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Contracts[contractId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a contract.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/contracts/{contractId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-contracts-contractId-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="contractId">The contract ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteContractAsync(projectId, contractId);
    /// </code>
    /// </example>
    public async Task DeleteContractAsync(
        Guid containerId,
        string contractId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].Contracts[contractId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Cost Item Operations

    /// <summary>
    /// Lists all cost items with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/cost-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-items-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CostItemsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var item in client.CostManager.ListCostItemsAsync(projectId))
    /// {
    ///     Console.WriteLine(item.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CostItemsGetResponse_results> ListCostItemsAsync(
        Guid containerId,
        RequestConfiguration<CostItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].CostItems
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a cost item.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/cost-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-items-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The cost item creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostItemsPostResponse"/> containing the created cost item</returns>
    /// <example>
    /// <code>
    /// CostItemsPostResponse? item = await client.CostManager.CreateCostItemAsync(projectId, new CostItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostItemsPostResponse?> CreateCostItemAsync(
        Guid containerId,
        CostItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItems.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Batch creates cost items.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/cost-items:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-itemsbatch-create-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The batch create request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostItemsBatchCreatePostResponse"/> containing the batch create result</returns>
    /// <example>
    /// <code>
    /// CostItemsBatchCreatePostResponse? result = await client.CostManager.BatchCreateCostItemsAsync(projectId, new CostItemsBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostItemsBatchCreatePostResponse?> BatchCreateCostItemsAsync(
        Guid containerId,
        CostItemsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItemsBatchCreate.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Attaches cost items to budgets or contracts.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/cost-items:attach
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-itemsattach-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The attach request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostItemsAttachPostResponse"/> containing the attach result</returns>
    /// <example>
    /// <code>
    /// CostItemsAttachPostResponse? result = await client.CostManager.AttachCostItemsAsync(projectId, new CostItemsAttachPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostItemsAttachPostResponse?> AttachCostItemsAsync(
        Guid containerId,
        CostItemsAttachPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItemsAttach.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Detaches cost items from budgets or contracts.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/cost-items:detach
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-itemsdetach-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The detach request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostItemsDetachPostResponse"/> containing the detach result</returns>
    /// <example>
    /// <code>
    /// CostItemsDetachPostResponse? result = await client.CostManager.DetachCostItemsAsync(projectId, new CostItemsDetachPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostItemsDetachPostResponse?> DetachCostItemsAsync(
        Guid containerId,
        CostItemsDetachPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItemsDetach.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a cost item by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/cost-items/{costItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-items-costItemId-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="costItemId">The cost item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCostItemGetResponse"/> containing the cost item</returns>
    /// <example>
    /// <code>
    /// WithCostItemGetResponse? item = await client.CostManager.GetCostItemAsync(projectId, costItemId);
    /// </code>
    /// </example>
    public async Task<WithCostItemGetResponse?> GetCostItemAsync(
        Guid containerId,
        string costItemId,
        RequestConfiguration<WithCostItemItemRequestBuilder.WithCostItemItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItems[costItemId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a cost item.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/cost-items/{costItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-items-costItemId-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="costItemId">The cost item ID</param>
    /// <param name="body">The cost item update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithCostItemPatchResponse"/> containing the updated cost item</returns>
    /// <example>
    /// <code>
    /// WithCostItemPatchResponse? updated = await client.CostManager.UpdateCostItemAsync(projectId, costItemId, new WithCostItemPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithCostItemPatchResponse?> UpdateCostItemAsync(
        Guid containerId,
        string costItemId,
        WithCostItemPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].CostItems[costItemId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a cost item.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/cost-items/{costItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-cost-items-costItemId-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="costItemId">The cost item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteCostItemAsync(projectId, costItemId);
    /// </code>
    /// </example>
    public async Task DeleteCostItemAsync(
        Guid containerId,
        string costItemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].CostItems[costItemId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Expense Operations

    /// <summary>
    /// Lists all expenses with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/expenses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ExpensesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var expense in client.CostManager.ListExpensesAsync(projectId))
    /// {
    ///     Console.WriteLine(expense.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ExpensesGetResponse_results> ListExpensesAsync(
        Guid containerId,
        RequestConfiguration<ExpensesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].Expenses
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates an expense.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/expenses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The expense creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ExpensesPostResponse"/> containing the created expense</returns>
    /// <example>
    /// <code>
    /// ExpensesPostResponse? expense = await client.CostManager.CreateExpenseAsync(projectId, new ExpensesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ExpensesPostResponse?> CreateExpenseAsync(
        Guid containerId,
        ExpensesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets an expense by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/expenses/{expenseId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.ExpenseGetResponse"/> containing the expense</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.ExpenseGetResponse? expense = await client.CostManager.GetExpenseAsync(projectId, expenseId);
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.ExpenseGetResponse?> GetExpenseAsync(
        Guid containerId,
        string expenseId,
        RequestConfiguration<CostCont.Expenses.Item.ExpenseItemRequestBuilder.ExpenseItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an expense.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/expenses/{expenseId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-id-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="body">The expense update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.ExpensePatchResponse"/> containing the updated expense</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.ExpensePatchResponse? updated = await client.CostManager.UpdateExpenseAsync(projectId, expenseId, new CostCont.Expenses.Item.ExpensePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.ExpensePatchResponse?> UpdateExpenseAsync(
        Guid containerId,
        string expenseId,
        CostCont.Expenses.Item.ExpensePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/expenses/{expenseId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-id-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteExpenseAsync(projectId, expenseId);
    /// </code>
    /// </example>
    public async Task DeleteExpenseAsync(
        Guid containerId,
        string expenseId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].Expenses[expenseId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists expense items for an expense.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/expenses/{expenseId}/items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-expenseId-items-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.Items.ItemsGetResponse"/> containing the expense items</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.Items.ItemsGetResponse? items = await client.CostManager.GetExpenseItemsAsync(projectId, expenseId);
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.Items.ItemsGetResponse?> GetExpenseItemsAsync(
        Guid containerId,
        string expenseId,
        RequestConfiguration<CostCont.Expenses.Item.Items.ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].Items.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates an expense item.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/expenses/{expenseId}/items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-expenseId-items-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="body">The expense item creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.Items.ItemsPostResponse"/> containing the created expense item</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.Items.ItemsPostResponse? item = await client.CostManager.CreateExpenseItemAsync(projectId, expenseId, new CostCont.Expenses.Item.Items.ItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.Items.ItemsPostResponse?> CreateExpenseItemAsync(
        Guid containerId,
        string expenseId,
        CostCont.Expenses.Item.Items.ItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].Items.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets an expense item by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/expenses/{expenseId}/items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-expenseId-items-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="itemId">The expense line item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.Items.Item.ItemsGetResponse"/> containing the expense item</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.Items.Item.ItemsGetResponse? item = await client.CostManager.GetExpenseItemAsync(projectId, expenseId, itemId);
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.Items.Item.ItemsGetResponse?> GetExpenseItemAsync(
        Guid containerId,
        string expenseId,
        string itemId,
        RequestConfiguration<CostCont.Expenses.Item.Items.Item.ItemsItemRequestBuilder.ItemsItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].Items[itemId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an expense item.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /cost/v1/containers/{containerId}/expenses/{expenseId}/items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-expenseId-items-id-PATCH
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="itemId">The expense line item ID</param>
    /// <param name="body">The expense item update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Expenses.Item.Items.Item.ItemsPatchResponse"/> containing the updated expense item</returns>
    /// <example>
    /// <code>
    /// CostCont.Expenses.Item.Items.Item.ItemsPatchResponse? updated = await client.CostManager.UpdateExpenseItemAsync(projectId, expenseId, itemId, new CostCont.Expenses.Item.Items.Item.ItemsPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<CostCont.Expenses.Item.Items.Item.ItemsPatchResponse?> UpdateExpenseItemAsync(
        Guid containerId,
        string expenseId,
        string itemId,
        CostCont.Expenses.Item.Items.Item.ItemsPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Expenses[expenseId].Items[itemId].PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an expense item.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/expenses/{expenseId}/items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-expenses-expenseId-items-id-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="expenseId">The expense ID</param>
    /// <param name="itemId">The expense line item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteExpenseItemAsync(projectId, expenseId, itemId);
    /// </code>
    /// </example>
    public async Task DeleteExpenseItemAsync(
        Guid containerId,
        string expenseId,
        string itemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].Expenses[expenseId].Items[itemId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Main Contract Operations

    /// <summary>
    /// Lists main contracts with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/main-contracts
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-main-contracts-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MainContractsGetResponse"/> containing the main contracts</returns>
    /// <example>
    /// <code>
    /// MainContractsGetResponse? contracts = await client.CostManager.GetMainContractsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MainContractsGetResponse?> GetMainContractsAsync(
        Guid containerId,
        RequestConfiguration<MainContractsRequestBuilder.MainContractsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].MainContracts.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a main contract by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/main-contracts/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-main-contracts-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="mainContractId">The main contract ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.MainContracts.Item.GetResponse"/> containing the main contract</returns>
    /// <example>
    /// <code>
    /// CostCont.MainContracts.Item.GetResponse? contract = await client.CostManager.GetMainContractAsync(projectId, mainContractId);
    /// </code>
    /// </example>
    public async Task<CostCont.MainContracts.Item.GetResponse?> GetMainContractAsync(
        Guid containerId,
        Guid mainContractId,
        RequestConfiguration<CostCont.MainContracts.Item.ItemRequestBuilder.ItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].MainContracts[mainContractId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Payment Operations

    /// <summary>
    /// Lists payment items with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/payment-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-payment-items-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PaymentItemsGetResponse"/> containing the payment items</returns>
    /// <example>
    /// <code>
    /// PaymentItemsGetResponse? items = await client.CostManager.GetPaymentItemsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<PaymentItemsGetResponse?> GetPaymentItemsAsync(
        Guid containerId,
        RequestConfiguration<PaymentItemsRequestBuilder.PaymentItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].PaymentItems.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists payments with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/payments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-payments-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PaymentsGetResponse"/> containing the payments</returns>
    /// <example>
    /// <code>
    /// PaymentsGetResponse? payments = await client.CostManager.GetPaymentsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<PaymentsGetResponse?> GetPaymentsAsync(
        Guid containerId,
        RequestConfiguration<PaymentsRequestBuilder.PaymentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Payments.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Attachment Operations

    /// <summary>
    /// Lists attachments with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-attachments-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="AttachmentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var attachment in client.CostManager.ListAttachmentsAsync(projectId))
    /// {
    ///     Console.WriteLine(attachment.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AttachmentsGetResponse_results> ListAttachmentsAsync(
        Guid containerId,
        RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        while (true)
        {
            var response = await _api.Cost.V1.Containers[containerId].Attachments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Batch creates attachments.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/attachments:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-attachmentsbatch-create-POST
    /// </remarks>
    public async Task<AttachmentsBatchCreatePostResponse?> BatchCreateAttachmentsAsync(
        Guid containerId,
        AttachmentsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].AttachmentsBatchCreate.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates an attachment.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-attachments-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The attachment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> containing the created attachment</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? attachment = await client.CostManager.CreateAttachmentAsync(projectId, new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> CreateAttachmentAsync(
        Guid containerId,
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Attachments.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an attachment.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /cost/v1/containers/{containerId}/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-attachments-attachmentId-DELETE
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="attachmentId">The attachment ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.CostManager.DeleteAttachmentAsync(projectId, attachmentId);
    /// </code>
    /// </example>
    public async Task DeleteAttachmentAsync(
        Guid containerId,
        string attachmentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Cost.V1.Containers[containerId].Attachments[attachmentId].DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Finds or creates an attachment folder in BIM 360 Docs for a given item.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/attachment-folders
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-attachment-folders-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The attachment folder request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentFoldersPostResponse"/> containing the attachment folder result</returns>
    /// <example>
    /// <code>
    /// AttachmentFoldersPostResponse? folder = await client.CostManager.CreateAttachmentFolderAsync(projectId, new AttachmentFoldersPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentFoldersPostResponse?> CreateAttachmentFolderAsync(
        Guid containerId,
        AttachmentFoldersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].AttachmentFolders.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Settings: Properties, Segments, Templates, Taxes

    /// <summary>
    /// Gets cost management properties for the container.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/properties
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-properties-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PropertiesGetResponse"/> containing the cost properties</returns>
    /// <example>
    /// <code>
    /// PropertiesGetResponse? properties = await client.CostManager.GetPropertiesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<PropertiesGetResponse?> GetPropertiesAsync(
        Guid containerId,
        RequestConfiguration<Autodesk.ACC.Cost.V1.Containers.Item.Properties.PropertiesRequestBuilder.PropertiesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Properties.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Batch updates property values.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/property-values:batch-update
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-property-valuesbatch-update-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The batch update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PropertyValuesBatchUpdatePostResponse"/> containing the batch update result</returns>
    /// <example>
    /// <code>
    /// PropertyValuesBatchUpdatePostResponse? result = await client.CostManager.BatchUpdatePropertyValuesAsync(projectId, new PropertyValuesBatchUpdatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<PropertyValuesBatchUpdatePostResponse?> BatchUpdatePropertyValuesAsync(
        Guid containerId,
        PropertyValuesBatchUpdatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].PropertyValuesBatchUpdate.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets segment values for the container.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/segment-values
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-segment-values-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SegmentValuesGetResponse"/> containing the segment values</returns>
    /// <example>
    /// <code>
    /// SegmentValuesGetResponse? values = await client.CostManager.GetSegmentValuesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<SegmentValuesGetResponse?> GetSegmentValuesAsync(
        Guid containerId,
        RequestConfiguration<SegmentValuesRequestBuilder.SegmentValuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].SegmentValues.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets segment values for a specific segment.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/segments/{segmentId}/values
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-values-GET
    /// </remarks>
    public async Task<ValuesGetResponse?> GetSegmentValuesBySegmentAsync(
        Guid containerId,
        Guid segmentId,
        RequestConfiguration<ValuesRequestBuilder.ValuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Segments[segmentId].Values.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Imports segment values.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/segments/{segmentId}/values:import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-valuesimport-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="segmentId">The segment ID</param>
    /// <param name="body">The import request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ValuesImportPostResponse"/> containing the import result</returns>
    /// <example>
    /// <code>
    /// ValuesImportPostResponse? result = await client.CostManager.ImportSegmentValuesAsync(projectId, segmentId, new ValuesImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ValuesImportPostResponse?> ImportSegmentValuesAsync(
        Guid containerId,
        Guid segmentId,
        ValuesImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Segments[segmentId].ValuesImport.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a segment value by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/segments/{segmentId}/values/{valueId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-values-valueId-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="segmentId">The segment ID</param>
    /// <param name="valueId">The segment value ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithValueGetResponse"/> containing the segment value</returns>
    /// <example>
    /// <code>
    /// WithValueGetResponse? value = await client.CostManager.GetSegmentValueAsync(projectId, segmentId, valueId);
    /// </code>
    /// </example>
    public async Task<WithValueGetResponse?> GetSegmentValueAsync(
        Guid containerId,
        Guid segmentId,
        string valueId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Segments[segmentId].Values[valueId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists templates.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/templates
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-templates-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TemplatesGetResponse"/> containing the templates</returns>
    /// <example>
    /// <code>
    /// TemplatesGetResponse? templates = await client.CostManager.GetTemplatesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<TemplatesGetResponse?> GetTemplatesAsync(
        Guid containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Templates.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets template segments.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/templates/{templateId}/segments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-templates-templateId-segments-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="templateId">The template ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SegmentsGetResponse"/> containing the template segments</returns>
    /// <example>
    /// <code>
    /// SegmentsGetResponse? segments = await client.CostManager.GetTemplateSegmentsAsync(projectId, templateId);
    /// </code>
    /// </example>
    public async Task<SegmentsGetResponse?> GetTemplateSegmentsAsync(
        Guid containerId,
        Guid templateId,
        RequestConfiguration<CostCont.Templates.Item.Segments.SegmentsRequestBuilder.SegmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Templates[templateId].Segments.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets taxes.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/taxes
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-taxes-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="TaxesGetResponse"/> containing the taxes</returns>
    /// <example>
    /// <code>
    /// TaxesGetResponse? taxes = await client.CostManager.GetTaxesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<TaxesGetResponse?> GetTaxesAsync(
        Guid containerId,
        RequestConfiguration<TaxesRequestBuilder.TaxesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Taxes.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Documents, Schedule of Values, Time Sheets

    /// <summary>
    /// Gets documents.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/documents
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-documents-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DocumentsGetResponse"/> containing the documents</returns>
    /// <example>
    /// <code>
    /// DocumentsGetResponse? documents = await client.CostManager.GetDocumentsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<DocumentsGetResponse?> GetDocumentsAsync(
        Guid containerId,
        RequestConfiguration<DocumentsRequestBuilder.DocumentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Documents.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets schedule of values.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/schedule-of-values
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-schedule-of-values-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ScheduleOfValuesGetResponse"/> containing the schedule of values</returns>
    /// <example>
    /// <code>
    /// ScheduleOfValuesGetResponse? sov = await client.CostManager.GetScheduleOfValuesAsync(projectId);
    /// </code>
    /// </example>
    public async Task<ScheduleOfValuesGetResponse?> GetScheduleOfValuesAsync(
        Guid containerId,
        RequestConfiguration<ScheduleOfValuesRequestBuilder.ScheduleOfValuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].ScheduleOfValues.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists time sheets with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/time-sheets
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-time-sheets-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.TimeSheets.TimeSheetsGetResponse"/> containing the time sheets</returns>
    /// <example>
    /// <code>
    /// CostCont.TimeSheets.TimeSheetsGetResponse? sheets = await client.CostManager.GetTimeSheetsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<CostCont.TimeSheets.TimeSheetsGetResponse?> GetTimeSheetsAsync(
        Guid containerId,
        RequestConfiguration<TimeSheetsRequestBuilder.TimeSheetsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].TimeSheets.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a time sheet by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/time-sheets/{timeSheetId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-time-sheets-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="timeSheetId">The time sheet ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.TimeSheets.Item.TimeSheetsGetResponse"/> containing the time sheet</returns>
    /// <example>
    /// <code>
    /// CostCont.TimeSheets.Item.TimeSheetsGetResponse? sheet = await client.CostManager.GetTimeSheetAsync(projectId, timeSheetId);
    /// </code>
    /// </example>
    public async Task<CostCont.TimeSheets.Item.TimeSheetsGetResponse?> GetTimeSheetAsync(
        Guid containerId,
        string timeSheetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].TimeSheets[timeSheetId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    #endregion

    #region Performance Tracking, Workflows

    /// <summary>
    /// Lists performance tracking items with pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/performance-tracking-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-performance-tracking-items-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.PerformanceTrackingItems.PerformanceTrackingItemsGetResponse"/> containing the performance tracking items</returns>
    /// <example>
    /// <code>
    /// CostCont.PerformanceTrackingItems.PerformanceTrackingItemsGetResponse? items = await client.CostManager.GetPerformanceTrackingItemsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<CostCont.PerformanceTrackingItems.PerformanceTrackingItemsGetResponse?> GetPerformanceTrackingItemsAsync(
        Guid containerId,
        RequestConfiguration<PerformanceTrackingItemsRequestBuilder.PerformanceTrackingItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].PerformanceTrackingItems.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a performance tracking item.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /cost/v1/containers/{containerId}/performance-tracking-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-performance-tracking-items-POST
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="body">The performance tracking item creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.PerformanceTrackingItems.PerformanceTrackingItemsPostResponse"/> containing the created item</returns>
    /// <example>
    /// <code>
    /// CostCont.PerformanceTrackingItems.PerformanceTrackingItemsPostResponse? item = await client.CostManager.CreatePerformanceTrackingItemAsync(projectId, new CostCont.PerformanceTrackingItems.PerformanceTrackingItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CostCont.PerformanceTrackingItems.PerformanceTrackingItemsPostResponse?> CreatePerformanceTrackingItemAsync(
        Guid containerId,
        CostCont.PerformanceTrackingItems.PerformanceTrackingItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].PerformanceTrackingItems.PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a performance tracking item by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/performance-tracking-items/{itemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-performance-tracking-items-id-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="itemId">The performance tracking item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Cost.V1.Containers.Item.PerformanceTrackingItems.Item.PerformanceTrackingItemsGetResponse"/> containing the performance tracking item</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Cost.V1.Containers.Item.PerformanceTrackingItems.Item.PerformanceTrackingItemsGetResponse? item = await client.CostManager.GetPerformanceTrackingItemAsync(projectId, itemId);
    /// </code>
    /// </example>
    public async Task<Autodesk.ACC.Cost.V1.Containers.Item.PerformanceTrackingItems.Item.PerformanceTrackingItemsGetResponse?> GetPerformanceTrackingItemAsync(
        Guid containerId,
        Guid itemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].PerformanceTrackingItems[itemId].GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets workflow actions for an association.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /cost/v1/containers/{containerId}/workflows/{associationType}/{associationId}/actions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/cost-actions-GET
    /// </remarks>
    /// <param name="containerId">The project/container ID</param>
    /// <param name="associationType">The workflow association type</param>
    /// <param name="associationId">The association ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CostCont.Workflows.Item.Item.Actions.ActionsGetResponse"/> containing the workflow actions</returns>
    /// <example>
    /// <code>
    /// CostCont.Workflows.Item.Item.Actions.ActionsGetResponse? actions = await client.CostManager.GetWorkflowActionsForAssociationAsync(projectId, associationType, associationId);
    /// </code>
    /// </example>
    public async Task<CostCont.Workflows.Item.Item.Actions.ActionsGetResponse?> GetWorkflowActionsForAssociationAsync(
        Guid containerId,
        string associationType,
        Guid associationId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Cost.V1.Containers[containerId].Workflows[associationType][associationId].Actions
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion
}
