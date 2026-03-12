using Autodesk.Vault;
using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Tests.Vault;

[TestClass]
public class VaultSearchTests
{
    private static WireMockServer _server = null!;
    private static VaultClient _client = null!;
    private const string VaultId = "1";

    private const string SearchResultsJson = """
        {
            "pagination": {
                "limit": 100,
                "totalResults": 3,
                "indexingStatus": "IndexingComplete"
            },
            "results": [
                {
                    "id": "101",
                    "name": "Assembly.iam",
                    "entityType": "FileVersion",
                    "version": 2,
                    "state": "Released",
                    "category": "Engineering",
                    "parentFolderId": "10"
                },
                {
                    "id": "202",
                    "name": "Designs",
                    "entityType": "Folder",
                    "fullName": "$/Designs",
                    "subfolderCount": 3
                },
                {
                    "id": "303",
                    "name": "ECO-001",
                    "entityType": "ChangeOrder",
                    "number": "ECO-001",
                    "title": "Update Assembly",
                    "state": "Pending"
                }
            ],
            "included": {
                "folder": {
                    "10": {
                        "id": "10",
                        "name": "Root",
                        "fullName": "$/Root"
                    }
                }
            }
        }
        """;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        _server = WireMockServer.Start();

        // Stub: Basic search
        _server.Given(
            Request.Create()
                .WithPath("/AutodeskDM/Services/api/vault/v2/vaults/1/search-results")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(SearchResultsJson)
        );

        // Stub: Advanced search
        _server.Given(
            Request.Create()
                .WithPath("/AutodeskDM/Services/api/vault/v2/vaults/1:advanced-search")
                .UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(SearchResultsJson)
        );

        _client = new VaultClient(
            () => Task.FromResult("test-token"),
            _server.Url!
        );
    }

    [ClassCleanup]
    public static void Teardown()
    {
        _server?.Stop();
        _server?.Dispose();
    }

    [TestMethod]
    public async Task BasicSearch_ShouldReturnEntityCollection()
    {
        var result = await _client.Search.GetSearchResultsAsync(VaultId, config =>
        {
            config.QueryParameters.Q = "Assembly";
            config.QueryParameters.Limit = 100;
        });

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Pagination);
        Assert.AreEqual(3, result.Pagination.TotalResults);
        Assert.AreEqual("IndexingComplete", result.Pagination.IndexingStatus?.ToString());
        Assert.IsNotNull(result.Results);
        Assert.AreEqual(3, result.Results.Count);
    }

    [TestMethod]
    public async Task BasicSearch_WithOptions_ShouldReturnResults()
    {
        var result = await _client.Search.GetSearchResultsAsync(VaultId, config =>
        {
            config.QueryParameters.Q = "Assembly";
            config.QueryParameters.OptionextendedModels = true;
            config.QueryParameters.OptionlatestOnly = true;
            config.QueryParameters.OptionsearchContent = false;
        });

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Results);
        Assert.IsTrue(result.Results.Count > 0);
    }

    [TestMethod]
    public async Task AdvancedSearch_ShouldReturnEntityCollection()
    {
        var searchBody = new WithVaultIdAdvancedSearchPostRequestBody
        {
            EntityTypesToSearch = [
                WithVaultIdAdvancedSearchPostRequestBody_entityTypesToSearch.File,
                WithVaultIdAdvancedSearchPostRequestBody_entityTypesToSearch.Folder
            ],
            SearchCriterias =
            [
                new SearchCriteria
                {
                    Operator = SearchCriteria_operator.Contains,
                    SearchString = "Assembly"
                }
            ]
        };

        var result = await _client.Search.PerformAdvancedSearchAsync(VaultId, searchBody);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Pagination);
        Assert.AreEqual(3, result.Pagination.TotalResults);
        Assert.IsNotNull(result.Results);
        Assert.AreEqual(3, result.Results.Count);
    }

    [TestMethod]
    public async Task AdvancedSearch_WithSortCriteria_ShouldReturnResults()
    {
        var searchBody = new WithVaultIdAdvancedSearchPostRequestBody
        {
            EntityTypesToSearch = [
                WithVaultIdAdvancedSearchPostRequestBody_entityTypesToSearch.File
            ],
            SearchCriterias =
            [
                new SearchCriteria
                {
                    Operator = SearchCriteria_operator.Contains,
                    SearchString = "Assembly"
                }
            ],
            SortCriterias =
            [
                new SortCriteria
                {
                    Ascending = true
                }
            ]
        };

        var result = await _client.Search.PerformAdvancedSearchAsync(VaultId, searchBody, config =>
        {
            config.QueryParameters.Limit = 50;
            config.QueryParameters.OptionextendedModels = true;
        });

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Results);
        Assert.IsTrue(result.Results.Count > 0);
    }

    [TestMethod]
    public async Task BasicSearch_PaginationInfo_ShouldBeCorrect()
    {
        var result = await _client.Search.GetSearchResultsAsync(VaultId, config =>
        {
            config.QueryParameters.Q = "Assembly";
        });

        Assert.IsNotNull(result?.Pagination);
        Assert.AreEqual(100, result.Pagination.Limit);
        Assert.AreEqual(3, result.Pagination.TotalResults);
    }

    [TestMethod]
    public async Task BasicSearch_IncludedData_ShouldBePresent()
    {
        var result = await _client.Search.GetSearchResultsAsync(VaultId, config =>
        {
            config.QueryParameters.Q = "Assembly";
        });

        Assert.IsNotNull(result?.Included);
        Assert.IsNotNull(result.Included.Folder);
    }
}
