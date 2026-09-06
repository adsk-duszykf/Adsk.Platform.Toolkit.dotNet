# Changelog - Adsk.Platform.Datum.PIM360

All notable changes to this package are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [1.1.0]

### Added

- Multi-targeting: package now also targets `net10.0` in addition to `net8.0`.

---

## [1.0.0]

### Added

- `DatumPIM360Client` with tenant-aware base URL configuration and access-token support.
- Manager entry points for items, EICs, views, queries, activities, files, comments, and metadata.
- Automatic `IAsyncEnumerable<T>` pagination for item handles and query results.
- Access to the complete Kiota-generated fluent API through `client.Api`.
- Package README and NuGet metadata.
