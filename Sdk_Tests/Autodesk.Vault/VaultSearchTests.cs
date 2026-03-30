using System.Text.Json;
using Autodesk.Vault;
using Autodesk.Vault.Models;
using Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Serialization;
using static Autodesk.Vault.Vaults.Item.SearchResults.SearchResultsRequestBuilder;
using static Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch.WithVaultIdAdvancedSearchRequestBuilder;
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
                "totalResults": 5,
                "indexingStatus": "IndexingComplete",
                "nextUrl": "/AutodeskDM/Services/api/vault/v2/vaults/1/search-results?bookmark=abc123"
            },
            "results": [
                {
                    "id": "101",
                    "name": "Assembly.iam",
                    "entityType": "FileVersion",
                    "category": "Engineering",
                    "categoryColor": 255,
                    "classification": "DesignDocument",
                    "version": 2,
                    "createDate": "2025-01-15T10:30:00Z",
                    "lastModifiedDate": "2025-03-10T14:20:00Z",
                    "revision": "A",
                    "state": "Released",
                    "stateColor": 65280,
                    "parentFolderId": "10",
                    "file": {
                        "id": "50",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/files/50",
                        "versionType": "Latest"
                    },
                    "parent": {
                        "id": "10",
                        "name": "Root",
                        "fullName": "$/Root"
                    },
                    "isCheckedOut": false,
                    "createUserName": "admin",
                    "checkinDate": "2025-03-10T14:20:00Z",
                    "checkoutDate": "2025-03-09T09:00:00Z",
                    "checkoutUserName": "admin",
                    "size": 1048576,
                    "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/files/50/versions/101",
                    "hasVisualizationAttachment": true,
                    "visualizationAttachmentStatus": "Syncronized",
                    "isReadOnly": false,
                    "isCloaked": false,
                    "isOnSite": true,
                    "properties": [
                        {
                            "propertyDefinitionId": "1001",
                            "value": "Assembly.iam"
                        }
                    ]
                },
                {
                    "id": "202",
                    "name": "Designs",
                    "entityType": "Folder",
                    "fullName": "$/Designs",
                    "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/folders/202",
                    "createDate": "2024-06-01T08:00:00Z",
                    "createUserName": "admin",
                    "category": "Project",
                    "categoryColor": 128,
                    "state": "Active",
                    "stateColor": 65280,
                    "subfolderCount": 3,
                    "children": "$/Designs/SubFolder1",
                    "isLibrary": false,
                    "isReadOnly": false,
                    "isCloaked": false,
                    "properties": [
                        {
                            "propertyDefinitionId": "1002",
                            "value": "Designs"
                        }
                    ]
                },
                {
                    "id": "303",
                    "name": "ECO-001",
                    "entityType": "ChangeOrder",
                    "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/change-orders/303",
                    "createDate": "2025-02-20T11:00:00Z",
                    "number": "ECO-001",
                    "title": "Update Assembly",
                    "description": "Update the main assembly with revised components",
                    "approveDeadline": "2025-04-01T17:00:00Z",
                    "lastModifiedDate": "2025-03-15T16:45:00Z",
                    "lastModifiedUserId": "5",
                    "lastTouchedDate": "2025-03-15T16:45:00Z",
                    "numberOfAttachments": 2,
                    "state": "Pending",
                    "stateColor": 16776960,
                    "isReadOnly": false,
                    "properties": [
                        {
                            "propertyDefinitionId": "1003",
                            "value": "ECO-001"
                        }
                    ]
                },
                {
                    "id": "404",
                    "name": "ITEM-001",
                    "entityType": "ItemVersion",
                    "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/items/404",
                    "number": "ITEM-001",
                    "version": 1,
                    "revision": "A",
                    "comment": "Initial release",
                    "title": "Main Assembly Item",
                    "state": "Released",
                    "stateColor": 65280,
                    "category": "Engineering",
                    "categoryColor": 255,
                    "isReadOnly": false,
                    "isCloaked": false,
                    "isLatestObsolete": false,
                    "item": {
                        "id": "60",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/items/60",
                        "versionType": "Latest"
                    },
                    "properties": [
                        {
                            "propertyDefinitionId": "1004",
                            "value": "ITEM-001"
                        }
                    ]
                },
                {
                    "id": "505",
                    "name": "Link-Assembly-ECO",
                    "entityType": "LinkEntity",
                    "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/links/505",
                    "createDate": "2025-03-01T09:30:00Z",
                    "createUserName": "admin",
                    "toEntity": {
                        "id": "303",
                        "name": "ECO-001",
                        "entityType": "ChangeOrder"
                    },
                    "fromEntity": {
                        "id": "10",
                        "name": "Root",
                        "fullName": "$/Root"
                    }
                }
            ],
            "included": {
                "folder": {
                    "10": {
                        "id": "10",
                        "name": "Root",
                        "fullName": "$/Root",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/folders/10",
                        "createDate": "2024-01-01T00:00:00Z",
                        "createUserName": "admin",
                        "category": "Project",
                        "categoryColor": 128,
                        "state": "Active",
                        "stateColor": 65280,
                        "subfolderCount": 5,
                        "children": "$/Root/SubFolder1",
                        "isLibrary": false,
                        "isReadOnly": false,
                        "isCloaked": false,
                        "properties": [
                            {
                                "propertyDefinitionId": "1002",
                                "value": "Root"
                            }
                        ]
                    }
                },
                "propertyDefinition": {
                    "1001": {
                        "id": "1001",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/property-definitions/1001",
                        "displayName": "File Name",
                        "systemName": "FileName",
                        "dataType": "String",
                        "active": true,
                        "isSystem": true,
                        "initialValue": ""
                    },
                    "1002": {
                        "id": "1002",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/property-definitions/1002",
                        "displayName": "Folder Name",
                        "systemName": "FolderName",
                        "dataType": "String",
                        "active": true,
                        "isSystem": true,
                        "initialValue": ""
                    },
                    "1003": {
                        "id": "1003",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/property-definitions/1003",
                        "displayName": "Number",
                        "systemName": "Number",
                        "dataType": "String",
                        "active": true,
                        "isSystem": true,
                        "initialValue": ""
                    },
                    "1004": {
                        "id": "1004",
                        "url": "/AutodeskDM/Services/api/vault/v2/vaults/1/property-definitions/1004",
                        "displayName": "Item Number",
                        "systemName": "ItemNumber",
                        "dataType": "String",
                        "active": true,
                        "isSystem": false,
                        "initialValue": "NEW"
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
        var result = await _client.Api.Vaults[VaultId].SearchResults
            .GetAsync(r =>
            {
                r.QueryParameters.Q = "Assembly";
                r.QueryParameters.Limit = 100;
            });

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Pagination);
        Assert.AreEqual(5, result.Pagination.TotalResults);
        Assert.AreEqual("IndexingComplete", result.Pagination.IndexingStatus?.ToString());
        Assert.IsNotNull(result.Results);
        Assert.AreEqual(5, result.Results.Count);
    }

    [TestMethod]
    public async Task BasicSearch_WithOptions_ShouldReturnResults()
    {
        var results = new List<EntityCollection.EntityCollection_results>();
        await foreach (var item in _client.Search.GetSearchResultsAsync(VaultId,
            new RequestConfiguration<SearchResultsRequestBuilderGetQueryParameters>
            {
                QueryParameters = new SearchResultsRequestBuilderGetQueryParameters
                {
                    Q = "Assembly",
                    OptionextendedModels = true,
                    OptionlatestOnly = true,
                    OptionsearchContent = false
                }
            }))
        {
            results.Add(item);
        }

        Assert.IsTrue(results.Count > 0);
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

        var result = await _client.Api.Vaults.WithVaultIdAdvancedSearch(VaultId)
            .PostAsync(searchBody);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Pagination);
        Assert.AreEqual(5, result.Pagination.TotalResults);
        Assert.IsNotNull(result.Results);
        Assert.AreEqual(5, result.Results.Count);
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

        var results = new List<EntityCollection.EntityCollection_results>();
        await foreach (var item in _client.Search.PerformAdvancedSearchAsync(VaultId, searchBody,
            new RequestConfiguration<WithVaultIdAdvancedSearchRequestBuilderPostQueryParameters>
            {
                QueryParameters = new WithVaultIdAdvancedSearchRequestBuilderPostQueryParameters
                {
                    Limit = 50,
                    OptionextendedModels = true
                }
            }))
        {
            results.Add(item);
        }

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task BasicSearch_PaginationInfo_ShouldBeCorrect()
    {
        var result = await _client.Api.Vaults[VaultId].SearchResults
            .GetAsync(r =>
            {
                r.QueryParameters.Q = "Assembly";
            });

        Assert.IsNotNull(result?.Pagination);
        Assert.AreEqual(100, result.Pagination.Limit);
        Assert.AreEqual(5, result.Pagination.TotalResults);
    }

    [TestMethod]
    public async Task BasicSearch_IncludedData_ShouldBePresent()
    {
        var result = await _client.Api.Vaults[VaultId].SearchResults
            .GetAsync(r =>
            {
                r.QueryParameters.Q = "Assembly";
            });

        Assert.IsNotNull(result?.Included);
        Assert.IsNotNull(result.Included.Folder);
    }

    [TestMethod]
    public async Task IncludedFolder_ParseAsync_ShouldReturnTypedFolders()
    {
        var result = await _client.Api.Vaults[VaultId].SearchResults
            .GetAsync(r =>
            {
                r.QueryParameters.Q = "Assembly";
            });

        Assert.IsNotNull(result?.Included?.Folder);

        var folders = await result.Included.Folder.ParseAsync();

        Assert.IsNotNull(folders);
        Assert.IsTrue(folders.Count > 0);
        Assert.IsTrue(folders.ContainsKey("10"));

        var root = folders["10"];
        Assert.AreEqual("10", root.Id);
        Assert.AreEqual("Root", root.Name);
        Assert.AreEqual("$/Root", root.FullName);
        Assert.AreEqual("admin", root.CreateUserName);
        Assert.AreEqual("Project", root.Category);
        Assert.AreEqual(128, root.CategoryColor);
        Assert.AreEqual("Active", root.State);
        Assert.AreEqual(65280, root.StateColor);
        Assert.AreEqual(5, root.SubfolderCount);
        Assert.AreEqual(false, root.IsLibrary);
        Assert.AreEqual(false, root.IsReadOnly);
        Assert.AreEqual(false, root.IsCloaked);
        Assert.IsNotNull(root.Properties);
        Assert.AreEqual(1, root.Properties.Count);
        Assert.AreEqual("1002", root.Properties[0].PropertyDefinitionId);
        Assert.AreEqual("Root", root.Properties[0].Value);
    }
}
