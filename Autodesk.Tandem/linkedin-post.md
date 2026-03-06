📊 **31 endpoints across 26 paths — covering the entire Tandem API surface for managing digital twins, models, time series data, and more.**

- **Complete coverage** — All 31 Tandem API operations
- **Fully typed** — Complete IntelliSense for requests, responses, headers, and query parameters
- **Fluent API** — Code structure mirrors the REST API endpoints for easy navigation
- **Modern .NET** — Built for .NET 8.0+
- **Built-in resilience** — Automatic retry with exponential backoff for rate limits

```csharp
var tandem = new TandemClient(getAccessToken);

// Get all digital twins (facilities)
var twins = await tandem.Twins.GetAsync();

// Get models for a specific twin
var models = await tandem.Models[twinId].GetAsync();

// Query time series streams
var streams = await tandem.Timeseries.Models[modelId].Streams.GetAsync();
```

📦 **Get started today:**

```console
dotnet add package Adsk.Platform.Tandem
```

