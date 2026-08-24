using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Activitylog.Issues;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Association_import;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Association_terminate;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Attribute_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Conflict_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Datatyping_errors_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Eic_impact_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Eic_related_obj_assoc;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Eic_related_obj_assoc.Liveview;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Item;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Narrow_import;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Object_association_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Parent_tag_inconsistency_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Reference.Item;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Register_import;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Remove_eic_items;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Snapshot;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Source_conflict_report;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Terminate_items;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Unterminate_items;
using Autodesk.Datum.PIM360.Etl_queue.Activities.Wide_import;
using Autodesk.Datum.PIM360.Rundeltareport;
using Autodesk.Datum.PIM360.Timeline;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using ActivityExportRequestBody = Autodesk.Datum.PIM360.Etl_queue.Activities.Export.ExportPostRequestBody;
using ActivityExportResponse = Autodesk.Datum.PIM360.Etl_queue.Activities.Export.ExportPostResponse;
using PivotExportRequestBody = Autodesk.Datum.PIM360.Pivot.Export.ExportPostRequestBody;
using PivotExportResponse = Autodesk.Datum.PIM360.Pivot.Export.ExportPostResponse;
using TimelineModel = Autodesk.Datum.PIM360.Timeline.Timeline;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for PIM360 activity operations: timeline, activity log, and ETL activities.</summary>
public sealed class ActivityManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="ActivityManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public ActivityManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Streams activities using the API offset and limit parameters.</summary>
    /// <remarks>Wraps: GET /timeline</remarks>
    /// <param name="requestConfiguration">(Optional) Activity filters and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of activities.</returns>
    /// <example>
    /// <code>
    /// await foreach (Timeline activity in client.ActivityManager.ListActivitiesAsync())
    /// {
    ///     Console.WriteLine(activity.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TimelineModel> ListActivitiesAsync(
        RequestConfiguration<TimelineRequestBuilder.TimelineRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long offset = requestConfiguration?.QueryParameters?.Offset ?? 0;
        long limit = Math.Clamp(requestConfiguration?.QueryParameters?.Limit ?? 200, 1, 200);

        while (true)
        {
            List<TimelineModel>? activities = await _api.Timeline.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
                r.QueryParameters.Categories = requestConfiguration?.QueryParameters?.Categories;
                r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
                r.QueryParameters.Hdl = requestConfiguration?.QueryParameters?.Hdl;
                r.QueryParameters.Own = requestConfiguration?.QueryParameters?.Own;
                r.QueryParameters.Offset = offset;
                r.QueryParameters.Limit = limit;
            }, cancellationToken);

            if (activities is not { Count: > 0 })
                yield break;

            foreach (TimelineModel activity in activities)
                yield return activity;

            if (activities.Count < limit)
                yield break;

            offset += activities.Count;
        }
    }

    /// <summary>Gets activity issues for a task.</summary>
    /// <remarks>Wraps: GET /activitylog/issues</remarks>
    /// <param name="taskhdl">Task handle.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated activity issues response.</returns>
    public Task<IssuesGetResponse?> ListActivityIssuesAsync(
        string taskhdl,
        RequestConfiguration<IssuesRequestBuilder.IssuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskhdl);
        return _api.Activitylog.Issues.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
            r.QueryParameters.Taskhdl = taskhdl;
            r.QueryParameters.Acthdl = requestConfiguration?.QueryParameters?.Acthdl;
        }, cancellationToken);
    }

    /// <summary>Gets the status and parameters of an activity by handle.</summary>
    /// <remarks>Wraps: GET /etl_queue/activities/{handle}</remarks>
    /// <param name="handle">Activity handle.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated activity parameters response.</returns>
    public Task<WithHandleGetResponse?> GetActivityAsync(string handle, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities[handle].GetAsync(null, cancellationToken);

    /// <summary>Submits a new activity based on an existing activity's parameters.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/{handle}</remarks>
    /// <param name="handle">Activity handle.</param>
    /// <param name="body">The activity parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated created-activity response.</returns>
    public Task<WithHandlePostResponse?> ResubmitActivityAsync(string handle, WithHandlePostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities[handle].PostAsync(body, null, cancellationToken);

    /// <summary>Gets the activity associated with the latest run of a dataset reference.</summary>
    /// <remarks>Wraps: GET /etl_queue/activities/reference/{actRef}</remarks>
    /// <param name="actRef">Dataset reference.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated activity response.</returns>
    public Task<WithActRefGetResponse?> GetActivityByDatasetReferenceAsync(string actRef, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Reference[actRef].GetAsync(null, cancellationToken);

    /// <summary>Gets the file associated with the latest run of a dataset reference.</summary>
    /// <remarks>Wraps: GET /etl_queue/activities/reference/{actRef}/data</remarks>
    /// <param name="actRef">Dataset reference.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Stream"/> containing the file content.</returns>
    public Task<Stream?> GetActivityDataByDatasetReferenceAsync(string actRef, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Reference[actRef].Data.GetAsync(null, cancellationToken);

    /// <summary>Submits a new snapshot with a specified version.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/snapshot</remarks>
    /// <param name="body">The snapshot parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated snapshot activity response.</returns>
    public Task<SnapshotPostResponse?> SubmitSnapshotAsync(SnapshotPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Snapshot.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a wide import activity with a file to import.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/wide_import</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Wide_importPostResponse?> SubmitWideImportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Wide_import.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a register import activity with a file to import.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/register_import</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Register_importPostResponse?> SubmitRegisterImportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Register_import.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a narrow import activity with a file to import.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/narrow_import</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Narrow_importPostResponse?> SubmitNarrowImportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Narrow_import.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an association import activity with a file to import.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/association_import</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Association_importPostResponse?> SubmitAssociationImportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Association_import.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an activity to terminate associations.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/association_terminate</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Association_terminatePostResponse?> SubmitAssociationTerminateAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Association_terminate.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an activity to terminate items.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/terminate_items</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Terminate_itemsPostResponse?> SubmitTerminateItemsAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Terminate_items.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an activity to unterminate items.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/unterminate_items</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Unterminate_itemsPostResponse?> SubmitUnterminateItemsAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Unterminate_items.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an activity to remove items from an EIC.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/remove_eic_items</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Remove_eic_itemsPostResponse?> SubmitRemoveEicItemsAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Remove_eic_items.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an EIC related object association import activity.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/eic_related_obj_assoc</remarks>
    /// <param name="body">The multipart upload body containing the file and import parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Eic_related_obj_assocPostResponse?> SubmitEicRelatedObjAssocImportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Eic_related_obj_assoc.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an EIC related object association import activity sourced from a liveview query.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/eic_related_obj_assoc/liveview</remarks>
    /// <param name="body">The target EIC and liveview query.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<LiveviewPostResponse?> SubmitEicRelatedObjAssocImportLiveviewAsync(LiveviewPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Eic_related_obj_assoc.Liveview.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a change log (EIC conflict) report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/conflict_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Conflict_reportPostResponse?> SubmitConflictReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Conflict_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a source conflict report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/source_conflict_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Source_conflict_reportPostResponse?> SubmitSourceConflictReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Source_conflict_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an EIC impact report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/eic_impact_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Eic_impact_reportPostResponse?> SubmitEicImpactReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Eic_impact_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an object-to-object association report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/object_association_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Object_association_reportPostResponse?> SubmitObjectAssociationReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Object_association_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a parent tag inconsistency report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/parent_tag_inconsistency_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Parent_tag_inconsistency_reportPostResponse?> SubmitParentTagInconsistencyReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Parent_tag_inconsistency_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a datatyping errors report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/datatyping_errors_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Datatyping_errors_reportPostResponse?> SubmitDatatypingErrorsReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Datatyping_errors_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an attribute report.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/attribute_report</remarks>
    /// <param name="body">The multipart report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<Attribute_reportPostResponse?> SubmitAttributeReportAsync(MultipartBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Attribute_report.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an export activity for a liveview.</summary>
    /// <remarks>Wraps: POST /etl_queue/activities/export</remarks>
    /// <param name="body">The export parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<ActivityExportResponse?> SubmitExportAsync(ActivityExportRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Etl_queue.Activities.Export.PostAsync(body, null, cancellationToken);

    /// <summary>Submits an export activity for a pivot view.</summary>
    /// <remarks>Wraps: POST /pivot/export</remarks>
    /// <param name="body">The pivot export configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<PivotExportResponse?> SubmitPivotExportAsync(PivotExportRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Pivot.Export.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a delta report for a domain and version range.</summary>
    /// <remarks>Wraps: POST /rundeltareport</remarks>
    /// <param name="body">The delta report parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated submitted-activity response.</returns>
    public Task<RundeltareportPostResponse?> SubmitDeltaReportAsync(RundeltareportPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Rundeltareport.PostAsync(body, null, cancellationToken);
}
