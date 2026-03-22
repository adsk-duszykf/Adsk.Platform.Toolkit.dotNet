using Autodesk.ACC.Construction.Photos.V1.Projects.Item.Photos.Item;
using Autodesk.ACC.Construction.Photos.V1.Projects.Item.PhotosFilter;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Photos.V1.Projects.Item.Photos.Item.WithPhotoItemRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Photos operations — retrieves and searches for media (photos and videos)
/// within ACC projects.
/// </summary>
public class PhotosManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhotosManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public PhotosManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a single media item (photo or video) by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/photos/v1/projects/{projectId}/photos/{photoId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/photos-getphoto-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="photoId">The ID of the photo or video</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (use Include query parameter for signed URLs)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithPhotoGetResponse"/> containing the media details</returns>
    /// <example>
    /// <code>
    /// WithPhotoGetResponse? photo = await client.PhotosManager.GetPhotoAsync("projectId", "photoId");
    /// </code>
    /// </example>
    public async Task<WithPhotoGetResponse?> GetPhotoAsync(
        string projectId,
        string photoId,
        RequestConfiguration<WithPhotoItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Photos.V1.Projects[projectId]
            .Photos[photoId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for and returns all specified media (photos and videos) within a project
    /// visible to the authenticated user. Supports filtering, sorting, and cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/photos/v1/projects/{projectId}/photos:filter
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/photos-getfilteredphotos-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The filter request body containing cursor state, filters, includes, limit, and sort options</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PhotosFilterPostResponse"/> containing the filtered photos response</returns>
    /// <example>
    /// <code>
    /// PhotosFilterPostResponse? result = await client.PhotosManager.SearchPhotosAsync("projectId", new PhotosFilterPostRequestBody
    /// {
    ///     Limit = 50
    /// });
    /// </code>
    /// </example>
    public async Task<PhotosFilterPostResponse?> SearchPhotosAsync(
        string projectId,
        PhotosFilterPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Photos.V1.Projects[projectId]
            .PhotosFilter
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}
