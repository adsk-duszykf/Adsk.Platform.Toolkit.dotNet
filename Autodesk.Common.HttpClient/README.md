# Autodesk.Common.HttpClientLibrary

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

A resilient HttpClient library designed to serve the Autodesk Platform Services (APS) Toolkit SDKs. This library provides a robust foundation for making HTTP requests with built-in error handling, rate limiting, query parameter management, and authentication support.

## Overview

The `Autodesk.Common.HttpClientLibrary` is built on top of Microsoft Kiota's HttpClient foundation and extends it with custom middleware handlers specifically designed for Autodesk's API ecosystem. It provides a reliable, configurable HTTP client with enterprise-grade features.

## Features

- **Resilient HTTP Client**: Built-in retry logic and error handling
- **Rate Limiting**: Configurable request rate limiting per endpoint
- **Error Handling**: Automatic error response handling with detailed exception information
- **Query Parameter Management**: Dynamic query parameter injection
- **Authentication Support**: Bearer token authentication with flexible token providers
- **Dependency Injection**: Full support for .NET dependency injection containers
- **Middleware Pipeline**: Extensible middleware architecture

## Installation

```cmd
dotnet add package Adsk.Platform.HttpClient
```

## Quick Start

### Basic Usage

```csharp
using Autodesk.Common.HttpClientLibrary;

// Create a basic HttpClient
var httpClient = HttpClientFactory.Create();

// Create with rate limiting
var rateLimitedClient = HttpClientFactory.Create((maxConcurrentRequests: 10, timeWindow: TimeSpan.FromMinutes(1)));
```

### With Authentication

```csharp
using Autodesk.Common.HttpClientLibrary;

// Define your token provider
Func<Task<string>> getAccessToken = async () => 
{
    // Your token acquisition logic here
    return await GetAccessTokenFromYourAuthProvider();
};

// Create authenticated adapter
var httpClient = HttpClientFactory.Create();
var adapter = HttpClientFactory.CreateAdapter(getAccessToken, httpClient);
```

### Dependency Injection Setup

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

// In your Startup.cs or Program.cs
services.AddAdskToolkitHttpClient("MyHttpClient");

// Use in your services
public class MyService
{
    private readonly HttpClient _httpClient;
    
    public MyService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MyHttpClient");
    }
}
```

## Core Components

### HttpClientFactory

The main entry point for creating HttpClient instances with preconfigured middleware.

```csharp
public static class HttpClientFactory
{
    // Create basic HttpClient
    public static HttpClient Create();
    
    // Create HttpClient with rate limiting
    public static HttpClient Create((int maxConcurrentRequests, TimeSpan timeWindow)? rateLimit);
    
    // Create HttpClient with custom options
    public static HttpClient Create(HttpMessageHandler? finalHandler = null, IRequestOption[]? optionsForHandlers = null);
    
    // Create authenticated request adapter
    public static HttpClientRequestAdapter CreateAdapter(Func<Task<string>> getAccessToken, HttpClient? httpClient);
}
```

## Middleware Components

### RateLimitingHandler

Implements per-endpoint rate limiting to prevent API quota exhaustion.

**Features:**

- Configurable maximum concurrent requests
- Configurable time windows
- Per-endpoint limiting (based on HTTP method and path)
- Automatic request queuing and retry

**Configuration:**

```csharp
var rateLimitOption = new RateLimitingHandlerOption();
rateLimitOption.SetRateLimit(maxConcurrentRequests: 5, timeWindow: TimeSpan.FromMinutes(1));

var httpClient = HttpClientFactory.Create(null, new IRequestOption[] { rateLimitOption });
```

### ErrorHandler

Provides centralized error handling for HTTP responses.

**Features:**

- Automatic exception throwing for non-success status codes
- Detailed error information including response context
- Configurable enable/disable functionality

**Configuration:**

```csharp
var errorOption = new ErrorHandlerOption { Enabled = true };
var httpClient = HttpClientFactory.Create(null, new IRequestOption[] { errorOption });
```

### QueryParameterHandler

Dynamically adds query parameters to HTTP requests.

**Features:**

- Runtime query parameter injection
- Preserves existing query parameters
- Flexible parameter management

**Configuration:**

```csharp
var queryOption = new QueryParameterHandlerOption();
queryOption.QueryParameters.Add("api-version", "1.0");
queryOption.QueryParameters.Add("region", "us-east-1");

var httpClient = HttpClientFactory.Create(null, new IRequestOption[] { queryOption });
```

## Advanced Configuration

### Custom Middleware Pipeline

```csharp
// Get default handler types
var handlerTypes = HttpClientFactory.GetDefaultHandlerActivatableTypes();

// Create custom configuration
var customOptions = new IRequestOption[]
{
    new RateLimitingHandlerOption(),
    new ErrorHandlerOption { Enabled = true },
    new QueryParameterHandlerOption()
};

var httpClient = HttpClientFactory.Create(finalHandler: null, optionsForHandlers: customOptions);
```

### Per-Request Options

You can override default options on a per-request basis:

```csharp
var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/data");

// Add request-specific rate limiting
var requestRateLimit = new RateLimitingHandlerOption();
requestRateLimit.SetRateLimit(1, TimeSpan.FromSeconds(5));
request.Options.Add(requestRateLimit.GetType().Name, requestRateLimit);

var response = await httpClient.SendAsync(request);
```

## Best Practices

### 1. Use Dependency Injection

Register the HttpClient in your DI container for better testability and lifecycle management:

```csharp
services.AddAdskToolkitHttpClient("ApsHttpClient");
```

### 2. Configure Appropriate Rate Limits

Set rate limits based on your API's documented limits:

```csharp
// For APIs with 100 requests per minute limit
var httpClient = HttpClientFactory.Create((maxConcurrentRequests: 100, timeWindow: TimeSpan.FromMinutes(1)));
```

### 3. Implement Proper Token Management

Use a robust token provider that handles token refresh:

```csharp
Func<Task<string>> getAccessToken = async () =>
{
    if (IsTokenExpired())
    {
        await RefreshToken();
    }
    return CurrentAccessToken;
};
```

### 4. Handle Errors Gracefully

While the ErrorHandler provides automatic exception throwing, implement proper error handling in your application:

```csharp
try
{
    var response = await httpClient.GetAsync("https://api.example.com/data");
    // Process response
}
catch (HttpRequestException ex) when (ex.Data.Contains("context"))
{
    var httpResponse = (HttpResponseMessage)ex.Data["context"];
    // Handle specific error cases based on status code
}
```

## Thread Safety

All components in this library are thread-safe and can be used in concurrent scenarios. The HttpClient instances created by the factory are safe to use across multiple threads.

## Performance Considerations

- **HttpClient Reuse**: Reuse HttpClient instances rather than creating new ones for each request
- **Rate Limiting**: Configure appropriate rate limits to balance performance with API compliance
- **Connection Pooling**: The underlying HttpClient uses connection pooling for optimal performance

## Dependencies

- **.NET 8.0**: Target framework
- **Microsoft.Extensions.Http**: HTTP client factory and DI integration
- **Microsoft.Kiota.Http.HttpClientLibrary**: Base HTTP client library with Kiota integration

## Examples

### Complete Example with Authentication and Rate Limiting

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register HttpClient
builder.Services.AddAdskToolkitHttpClient("ApsClient");

// Register your token provider
builder.Services.AddScoped<ITokenProvider, YourTokenProvider>();

var host = builder.Build();

// Use the client
var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
var tokenProvider = host.Services.GetRequiredService<ITokenProvider>();

var httpClient = httpClientFactory.CreateClient("ApsClient");
var adapter = HttpClientFactory.CreateAdapter(
    () => tokenProvider.GetTokenAsync(), 
    httpClient
);

// Use adapter with your SDK clients
var dataManagementClient = new DataManagementClient(adapter);
```

## Troubleshooting

### Common Issues

1. **Rate Limiting Too Aggressive**: Adjust rate limit parameters based on actual API limits
2. **Authentication Failures**: Ensure your token provider returns valid, non-expired tokens
3. **Timeout Issues**: Configure appropriate timeout values for your use case

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
```

## License

This library is licensed under the MIT License. See LICENSE file for details.
