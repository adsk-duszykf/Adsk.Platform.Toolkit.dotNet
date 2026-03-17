# Adsk.Platform.ModelDerivative

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.ModelDerivative` provides a set of APIs to interact with the [Autodesk Model Derivative Service](https://aps.autodesk.com/en/docs/model-derivative/v2/developers_guide/overview/).

## Documentation

- [API Reference](xref:Autodesk.ModelDerivative): Strongly typed API
- [Helpers](xref:Autodesk.ModelDerivative.Helpers.ModelDerivativeClientHelper): Set of helper methods

## Installation

```bash
dotnet add package Adsk.Platform.ModelDerivative
```

## Usage

See the  [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`ModelDerivativeClient`](xref:Autodesk.ModelDerivative.ModelDerivativeClient). This object provides access to the `Model Derivative APIs` and the `Helpers` method.

### Querying specific model properties

The  [Fetch Specific Properties endpoint](https://aps.autodesk.com/en/docs/model-derivative/v2/reference/http/metadata/urn-metadata-guid-properties-query-POST/) needs a query to filter the properties to be returned. The query is a JSON object that can be created using the `UntypedObject`.

Here is an example of how to query the project attributes of an IFC model, using a helper method `GetSpecificPropertiesAsync`. Compared to the default endpoint, this method waits until the server completes the processing

```csharp
using Autodesk.ModelDerivative;
using Autodesk.ModelDerivative.Helpers.Models;

public async Task<ParsedSpecificProperties> GetIFCprojectAttributes()
{

    async Task<string> getAccessToken()
    {
        //return access token with your logic
    }

    var MDclient = new ModelDerivativeClient(Location.US, getAccessToken);

    var fileURN = ""; // Replace with your file version urn, like 'urn:adsk.wipprod:fs.file:vf.w9MS3MDBQaat6ObHffTA?version=1'
    var modelGuid = ""; // Replace with your model guid like 'c0337487-5b66-422b-a284-c273b424af54'

    //The query in the body request is {"$in":["objectid",2]}"} which is equivalent to the following
    var query = new UntypedObject(new Dictionary<string, UntypedNode> {
        { "$in",new UntypedArray(
                    [
                        new UntypedString("objectid"),
                        new UntypedInteger(2)
                    ])
        }
    });

    var properties = await MDclient.Helper.GetSpecificPropertiesAsync(
        fileURN, modelGuid, query);

    return properties;
}

````