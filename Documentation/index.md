# Autodesk Platform Services Toolkit

This toolkit provides a collection of .NET libraries, featuring a [Fluent API](https://dzone.com/articles/java-fluent-api) derived from the [API specifications](https://swagger.io/specification/) and a set of convenient functions designed to enhance the developer experience when using Autodesk Platform Services (APS) in C#.

## Helper functions

Reduce the number of API calls and simplify the code by using the helper functions. The helper functions are designed to cover common scenarios combining multiple API calls.

Example:
**Get the file id by the file path**

```csharp
using Autodesk.DataManagement;

// Initialize the DataManagementClient
var authToken() => Task.FromResult("YOUR_ACCESS_TOKEN");

var DMclient = new DataManagementClient(authToken);

// A single method getting the file id by path
var file = await DMclient.Helper.GetFileItemByPathAsync("Account/Project/Folder/SubFolder/FileName.ext");

Console.WriteLine($"File ID: {file.Id}");    
```

## Fluent API

The Fluent API is derived from the API specifications and provides a more intuitive way to interact with the APS services. The Fluent API is designed to be easy to use and understand, making it easier to work with the APS services in C#.

The Fluent API reflects the Rest API endpoint structure. Here is an example showing the contrast between the Rest API endpoint and the Fluent API:

````csharp
//https://developer.api.autodesk.com / project / v1 / hubs  (GET)
                   client.DataMgtApi . Project . V1 . Hubs . GetAsync()
````

Example: **Get the hub ids**

```csharp
using Autodesk.DataManagement;

// Initialize the DataManagementClient
var authToken() => Task.FromResult("YOUR_ACCESS_TOKEN");

var DMclient = new DataManagementClient(authToken);

// Fluent API reproducing the API endpoint to get the hub ids
var hubs = await DMclient.DataMgtApi.Project.V1.Hubs.GetAsync();

var hubIds = hubs?.Data?.Select(h => h?.Id ?? "")?.ToArray() ?? [];

Console.WriteLine($"Hubs: {string.Join(';', hubIds)}");  
```

## Services available in the toolkit:

| Service | SDK | Helper functions | Package |
|--|--|--|--|
| Authentication | Stable | [Yes](https://adsk-duszykf.github.io/Adsk.Platform.Toolkit/api/Autodesk.Authentication.Helpers.AuthenticationClientHelper.html) | [Adsk.Platform.Authentication](https://www.nuget.org/packages/Adsk.Platform.Authentication) |
| Data Management | Stable | [Yes](https://adsk-duszykf.github.io/Adsk.Platform.Toolkit/api/Autodesk.DataManagement.Helpers.DataManagementClientHelper.html) | [Adsk.Platform.DataManagement](https://www.nuget.org/packages/Adsk.Platform.DataManagement) |
| Model Derivative | Stable | [Yes](https://adsk-duszykf.github.io/Adsk.Platform.Toolkit/api/Autodesk.ModelDerivative.Helpers.ModelDerivativeClientHelper.html) | [Adsk.Platform.ModelDerivative](https://www.nuget.org/packages/Adsk.Platform.ModelDerivative) |
| ACC | Stable | No | [Adsk.Platform.ACC](https://www.nuget.org/packages/Adsk.Platform.ACC) |
| BIM360 | Stable | No | [Adsk.Platform.BIM360](https://www.nuget.org/packages/Adsk.Platform.BIM360) |
| Vault Data | Stable | No | [Adsk.Platform.VaultData](https://www.nuget.org/packages/Adsk.Platform.VaultData) |
| Tandem | Stable | No | [Adsk.Platform.Tandem](https://www.nuget.org/packages/Adsk.Platform.Tandem) |
| Automation | Stable | No | [Adsk.Platform.Automation](https://www.nuget.org/packages/Adsk.Platform.Automation) |
| BuildingConnected | Stable | No | [Adsk.Platform.BuildingConnected](https://www.nuget.org/packages/Adsk.Platform.BuildingConnected) |


## Get started

The root object for each service is `{service}Client`. For example, the root object for the Data Management service is `DataManagementClient`.
This object contains 2 properties:

- `Api`: Contains the root object for the Fluent API.
- `Helper`: Contains methods that cover common scenarios combining multiple API calls.

The Fluent API reflects the Rest API endpoint structure:

The fluent API is generated with [Kiota](https://learn.microsoft.com/en-us/openapi/kiota/request-builders). Look at its documentation to understand it in depth.

For code examples look at the files `{service}ClientHelper.cs`.

## Motivations

### Why this toolkit?

- Needs full typed SDKs with a more complete and comprehensive API from the API specifications compared to existing solution. Thanks to [Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview) by Microsoft
- Needs higher level functions to cover common scenarios combining multiple API calls:
  - Authentication: Create and refresh automatically a 2 legged token
  - DataManagement: Download/Upload a file
  - Model Derivative: Get large model tree
  - Model Properties: Run the query and wait until done
  - ...

### More about the SDK generator [Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview)

There are other SDK generators like [OpenAPI Generator](https://github.com/OpenAPITools/openapi-generator), [AutoRest](https://github.com/Azure/autorest), [NSwag](https://github.com/RicoSuter/NSwag), etc.

[Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview) offers several benefits:

- Support all OpenAPI specifications => Full typed SDK including:
  - Request and response bodies
  - Headers
  - Query parameters
  - Path parameters
- Easy to map the API provider documentation to the SDK => Easy to use
- Log error in reports and continue during SDK generation => Robust
- Can exclude some endpoints for the SDK generation => Customizable
- Open source => Transparent
- Developed by Microsoft and used for their Graph SDK (very large API) => Reliable/scalable
- Coded in C# => Easy to understand, extend and execute

Here is an introduction by the authors:
> [!Video https://www.youtube.com/embed/sQ9Pv-rQ1s8]
