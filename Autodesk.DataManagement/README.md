# Autodesk Data Management SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

The `Adsk.Platform.DataManagement` toolkit provides a set of APIs to interact with the [Autodesk Data Management Service](https://aps.autodesk.com/developer/overview/data-management-api).

## Usage
See [here](../README.md#how-to-use-the-sdks) for details.

### Urn vs Id
The SDK uses `urn` and `id` interchangeably. Basically, the `id` is an unique identifier for an `folder`, `item`, `version` this `id` is a string starting with prefix `urn`.

### Filtering
Few endpoints support filters as query parameters (see [Filtering](https://aps.autodesk.com/en/docs/data/v2/developers_guide/filtering/) in the APS documentation).

Some of them are accessible with Intellisense, for example:

````csharp
public async Task<List<FolderContent>> getFolderContentByPageAsync(string projectId, string folderId)
{
    return await _dataMgtClient.Projects[projectId].Folders[folderId].Contents
                        .GetAsync(r =>
                        {
                            r.QueryParameters.Filtertype = ["item"];
                            r.QueryParameters.FilterextensionType = ["items:autodesk.bim360:File"];
                        });

}
````

But for more complex filters, you need to override the default URL:

````csharp

public async Task<Models.Search?> SearchAdvancedAsync(string projectId, string folderId)
    {
        var filters = new List<(string, string)>
		{
			( "filter[extension.type]", "items:autodesk.bim360:File" ),
			( "filter[type]", "item" ),
            ( "filter[attributes.fileName]-contains","Floor")
		};

        var requestInfo = _dataMgtClient.Api.Projects[projectId].Folders[folderId].Search.ToGetRequestInformation();

        var searchWithFiltersURI = _dataMgtClient.Helper.CreateRequestWithFilters(requestInfo, filters);

        var searchResult = await _dataMgtClient.Api.Projects[projectId].Folders[folderId].Search.WithUrl(searchWithFiltersURI).GetAsync();

        return searchResult;

    }

````