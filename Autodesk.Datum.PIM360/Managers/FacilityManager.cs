using Autodesk.Datum.PIM360.FacilityExplorer;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for facility explorer operations.</summary>
public sealed class FacilityManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="FacilityManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public FacilityManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Provides access to the generated facility explorer request builders.</summary>
    public FacilityExplorerRequestBuilder Api => _api.FacilityExplorer;
}
