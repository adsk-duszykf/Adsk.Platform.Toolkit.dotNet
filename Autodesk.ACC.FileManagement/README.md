# Autodesk File Management SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

The package `Adsk.Platform.ACC.FileManagement` provides a set of APIs to interact with the [Autodesk Construction Cloud - File Management Service](https://aps.autodesk.com/en/docs/acc/v1/reference/http/v1-files-export-pdf-files-POST/).

## Documentation

- [API Reference](xref:Autodesk.ACC.FileManagement): Strongly typed API
- [Helpers](xref:Autodesk.ACC.FileManagement.Helpers.FileManagementClientHelper): Set of helper methods

## Installation

```bash
dotnet add package Adsk.Platform.ACC.FileManagement
```

## Usage

See the  [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`FileManagementClient`](xref:Autodesk.ACC.FileManagement.FileManagementClient). This object provides access to the `Authentication` API and the `Helpers` method.


### Use the [API](xref:Autodesk.ACC.FileManagement)

```csharp
using Autodesk.ACC.FileManagement;
using Autodesk.ACC.FileManagement.Models;

public async Task<CustomAttributeDefinitions> GetAttributeDefinitions()
{

    async Task<string> getAccessToken()
    {
        //return access token with your logic
    }

    var FileClient = new FileManagementClient(getAccessToken);

    var projectId = ""; // Replace with your project id like 'c0337487-5b66-422b-a284-c273b424af54'
    var folderId = ""; // Replace with your folder id, like 'urn:adsk.wipprod:fs.folder:co.9g7HeA2wRqOxLlgLJ40UGQ'

    // The `FileClient.Api` object refers to the base url `https://developer.api.autodesk.com/bim360/docs/v1/`
    var attrDef = await FileClient.Api.Projects[projectId].Folders[folderId].CustomAttributeDefinitions.GetAsync();

    return attrDef;
}
```

### Use the [Helpers](xref:Autodesk.ACC.FileManagement.Helpers.FileManagementClientHelper)

Helpers simplify the usage of the API by providing a set of methods to interact with the API. Here is an example of how to use a helper method to update a custom attribute value. This method wraps the following steps and returns a list of errors if any.

1. Get the custom attribute definitions 
1. Check if the value can be applied
    - Is attribute exists?
    - Is a value valid?
1. Update the attribute value

```csharp
using Autodesk.ACC.FileManagement.Helpers;
using Autodesk.Authentication.Helpers.Models;

public async Task<List<AttributeUpdateError>> SetCustomAttributeValue()
{

    var projectId = ""; // Replace with your project id like 'c0337487-5b66-422b-a284-c273b424af54'
    var folderId= ""; // Replace with your folder id, like 'urn:adsk.wipprod:fs.folder:co.9g7HeA2wRqOxLlgLJ40UGQ'
    var fileId = ""; // Replace with your file id, like 'urn:adsk.wipprod:dm.lineage:file:co.9g7HeA2wRqOxLlgLJ40UGQ'

    var attributes = new List<CustomAttribute>()
    {
        (Name: "MyTextAttribute", Value: "NewValue"),
    };

    var errors = await CustomAttrClient.Helper.UpdateCustomAttributesAsync(projectId, fileId, fileId, attributes);
    
    return errors;
}
```
