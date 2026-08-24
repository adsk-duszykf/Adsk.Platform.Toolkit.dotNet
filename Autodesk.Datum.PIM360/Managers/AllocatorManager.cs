using Autodesk.Datum.PIM360.Allocator;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for tag number allocator operations.</summary>
public sealed class AllocatorManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="AllocatorManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public AllocatorManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Provides access to the generated allocator request builders.</summary>
    public AllocatorRequestBuilder Api => _api.Allocator;
}
