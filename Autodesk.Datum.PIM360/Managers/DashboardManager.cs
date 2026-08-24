using Autodesk.Datum.PIM360.Dashboard;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for dashboard query operations.</summary>
public sealed class DashboardManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="DashboardManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public DashboardManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Provides access to the generated dashboard request builders.</summary>
    public DashboardRequestBuilder Api => _api.Dashboard;
}
