using System.Diagnostics;
using System.Web;
using Autodesk.DataManagement.Data.V1;
using Autodesk.DataManagement.Data.V1.Projects.Item.Folders.Item.Contents;
using Autodesk.DataManagement.Helpers.Models;
using Autodesk.DataManagement.Models;
using Autodesk.DataManagement.OSS;
using Autodesk.DataManagement.OSS.Models;
using Autodesk.DataManagement.OSS.Oss.V2;
using CommonUtils;
using Microsoft.Kiota.Abstractions;
using SDKmodels = Autodesk.DataManagement.Models;

namespace Autodesk.DataManagement.Helpers;

/// <summary>
/// Helper class for Data Management API. It provides methods to perform common operations such as getting file item by path, downloading file, etc. It uses the generated clients but also implements some custom logic to handle specific cases such as ACC/BIM360 limitations, pagination, etc.
/// </summary>
public class DataManagementClientHelper
{
    private static readonly HttpClient httpClient = new(new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 64,
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        EnableMultipleHttp2Connections = true,
    });

    private readonly V1RequestBuilder _dataMgtClient;
    private readonly V2RequestBuilder _ossClient;
    private static readonly char[] separator = ['\\', '/'];
    /// <summary>
    /// The Data Management API client.
    /// </summary>
    public BaseDataManagementClient DataMgtApi { get; init; }
    /// <summary>
    /// The OSS API client.
    /// </summary>
    public BaseOSSClient OssApi { get; init; }
    internal DataManagementClientHelper(BaseDataManagementClient dataMgtApi, BaseOSSClient ossApi)
    {
        DataMgtApi = dataMgtApi;
        OssApi = ossApi;
        _dataMgtClient = dataMgtApi.Data.V1;
        _ossClient = ossApi.Oss.V2;
    }

    /// <summary>
    /// Get the container id of the RFIs
    /// </summary>
    /// <param name="hubId">hub id.</param>
    /// <param name="projectId">project id</param>
    /// <returns>The RFI container ID</returns>
    /// <remarks>The 'b.' for the <paramref name="hubId"/> and <paramref name="projectId"/> is managed automatically</remarks>
    /// <exception cref="InvalidOperationException">If RFIs container not found</exception>
    public async Task<string> GetRFIsContainerIdAsync(string hubId, string projectId)
    {
        hubId = FixHubId(hubId);
        projectId = FixProjectId(projectId);

        var projectDetails = await DataMgtApi.Project.V1.Hubs[hubId].Projects[projectId].GetAsync();

        return projectDetails?.Data?.Relationships?.Rfis?.Data?.Id
                ?? throw new InvalidOperationException("RFIs container not found");
    }

    /// <summary>
    /// Delete a file item (all versions) from Docs
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' handled automatically</param>
    /// <param name="fileItemUrn">File Item Urn like '</param>
    /// <returns>The deleted file</returns>
    /// <remarks>File is not really deleted but marked as "deleted" and can be restored</remarks>
    public async Task<(string fileItemId, string versionId)> DeleteFileAsync(string projectId, string fileItemUrn)
    {
        projectId = FixProjectId(projectId);

        var result = await DataMgtApi.Data.V1.Projects[projectId].Versions
            .PostAsync(
            new()
            {
                Jsonapi = new()
                {
                    Version = "1.0"
                },
                Data = new()
                {
                    Type = "versions",
                    Attributes = new()
                    {
                        Extension = new()
                        {
                            Type = "versions:autodesk.core:Deleted",
                            Version = "1.0"
                        },
                    },
                    Relationships = new()
                    {
                        Item = new()
                        {
                            Data = new()
                            {
                                Type = "items",
                                Id = fileItemUrn
                            }
                        }
                    }
                }
            });

        return (
                result?.Included?.FirstOrDefault()?.Id ?? throw new UnreachableException("Version 'id' is not defined"),
                result?.Data?.Id ?? throw new UnreachableException("Item 'id' is not defined")
                );
    }

    /// <summary>
    /// Rename a file item in ACC/BIM360 by patching the item's displayName
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' handled automatically</param>
    /// <param name="fileVersionUrn">File Item URN (lineage ID like 'urn:adsk.wipprod:dm.lineage:...')</param>
    /// <param name="newFileName">New file name including extension</param>
    /// <returns>The updated item</returns>
    /// <remarks>
    /// Uses PATCH items/:item_id to update the displayName.
    /// Only supported for BIM 360 Docs and ACC.
    /// </remarks>
    public async Task<VersionObject> RenameFileAsync(string projectId, string fileVersionUrn, string newFileName)
    {
        projectId = FixProjectId(projectId);

        var result = await _dataMgtClient.Projects[projectId].Versions[fileVersionUrn]
            .PatchAsync(new()
            {
                Jsonapi = new()
                {
                    Version = "1.0"
                },
                Data = new()
                {
                    Type = "versions",
                    Id = fileVersionUrn,
                    Attributes = new()
                    {
                        Name = newFileName
                    }

                }
            });

        return result ?? throw new InvalidOperationException("Rename failed: no response returned");
    }

    /// <summary>
    /// Determines whether the specified item in a project is marked as deleted.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project containing the item. Cannot be null or empty.</param>
    /// <param name="itemUrn">The unique identifier (URN) of the item to check. Cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the item is marked as deleted;  otherwise, <see langword="false"/>.</returns>
    public async Task<bool> IsFileDeletedAsync(string projectId, string itemUrn)
    {
        projectId = FixProjectId(projectId);

        var result = await DataMgtApi.Data.V1.Projects[projectId].Items[itemUrn].GetAsync();

        var isDeleted = result!.Data!.Attributes!.Hidden ?? false;

        if (isDeleted)
        {
            return true;
        }


        // ACC Limitation: The "hidden" item attribute remains "true" if the file has not been deleted directly
        // that happens when a parent folder has been deleted, that's why we have to check if the parent folders are hidden or not

        var parentFolderUrn = result!.Data!.Relationships!.Parent!.Data!.Id!;

        return await IsFolderDeletedAsync(projectId, parentFolderUrn);

    }

    /// <summary>
    /// Determines whether the specified folder has been deleted in the project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project containing the folder. Cannot be null or empty.</param>
    /// <param name="folderUrn">The unique resource name (URN) of the folder to check. Cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the folder has been deleted; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> IsFolderDeletedAsync(string projectId, string folderUrn)
    {
        projectId = FixProjectId(projectId);

        var folderInfo = await DataMgtApi.Data.V1.Projects[projectId].Folders[folderUrn].GetAsync();


        var isDeleted = folderInfo!.Data!.Attributes!.Hidden ?? false;

        var isRootFolder = false;

        // ACC Limitation: The "hidden" item attribute remains "true" if the file has not been deleted directly
        // that happens when a parent folder has been deleted, that's why we have to check if the parent folders are hidden or not

        while (isDeleted == false && isRootFolder == false)
        {

            var parentFolder = await DataMgtApi.Data.V1.Projects[projectId].Folders[folderUrn].Parent.GetAsync();

            folderUrn = parentFolder!.Data!.Id!;

            isRootFolder = parentFolder!.Data!.Attributes!.Name!.EndsWith("root-folder", StringComparison.InvariantCultureIgnoreCase);
            isDeleted = parentFolder!.Data!.Attributes!.Hidden ?? false;
        }

        return isDeleted;

    }

    /// <summary>
    /// Download file version from ACC/BIM360
    /// </summary>
    /// <param name="projectId">Id of the project. The prefix 'b.' is handled automatically</param>
    /// <param name="fileVersionUrn">Id of the file version</param>
    /// <returns>File content</returns>
    public async Task<Stream> DownloadFileVersionAsync(string projectId, string fileVersionUrn)
    {
        projectId = FixProjectId(projectId);

        var fileVersion = await _dataMgtClient.Projects[projectId].Versions[fileVersionUrn].GetAsync();

        var storageUrl = (fileVersion?.Data?.Relationships?.Storage?.Meta?.Link?.Href)
                            ?? throw new InvalidOperationException("Storage url is null");

        return await DownloadFromStorageAsync(storageUrl);
    }

    /// <summary>
    /// Download file version from ACC/BIM360
    /// </summary>
    /// <param name="projectId">Id of the project. The prefix 'b.' is handled automatically</param>
    /// <param name="itemUrn">Id of the file item</param>
    /// <param name="version">Version number starting at 1</param>
    /// <returns>File content</returns>
    public async Task<Stream> DownloadFileVersionAsync(string projectId, string itemUrn, int version)
    {
        projectId = FixProjectId(projectId);

        var item = await _dataMgtClient.Projects[projectId].Items[itemUrn].GetAsync();

        var fileVersion = await _dataMgtClient.Projects[projectId].Items[itemUrn].Versions
                                    .GetAsync(r => { r.QueryParameters.FilterversionNumber = [version]; });

        var storageUrl = (fileVersion?.Data?.FirstOrDefault()?.Relationships?.Storage?.Meta?.Link?.Href)
                            ?? throw new InvalidOperationException("Storage url is null");

        return await DownloadFromStorageAsync(storageUrl);
    }

    /// <summary>
    /// Download the latest file version from ACC/BIM360
    /// </summary>
    /// <param name="projectId">Id of the project. The prefix 'b.' is handled automatically</param>
    /// <param name="itemUrn">Id of the file item</param>
    /// <returns>File content</returns>
    public async Task<Stream> DownloadLatestFileVersionAsync(string projectId, string itemUrn)
    {
        projectId = FixProjectId(projectId);

        var item = await _dataMgtClient.Projects[projectId].Items[itemUrn].GetAsync();

        var storageUrl = (item?.Included?.FirstOrDefault()?.Relationships?.Storage?.Meta?.Link?.Href)
                            ?? throw new InvalidOperationException("Storage url is null");

        return await DownloadFromStorageAsync(storageUrl);
    }

    /// <summary>
    /// Download file from storage url using CDN-accelerated S3 signed download.
    /// </summary>
    /// <param name="storageUrl">Storage URL from version/item relationships</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    /// <remarks>
    /// <para>Uses CloudFront CDN (<c>useCdn=true</c>) and <c>public-resource-fallback=true</c> for optimal performance.</para>
    /// <para>Handles both <c>complete</c>/<c>fallback</c> (single URL) and <c>chunked</c> (byte-range URLs for unmerged resumable uploads) statuses.</para>
    /// </remarks>
    public async Task<Stream> DownloadFromStorageAsync(string storageUrl, CancellationToken cancellationToken = default)
    {
        var (bucketKey, objectKey) = ParseStorageUrl(storageUrl);

        var signedUrls = await _ossClient.Buckets[bucketKey].Objects[objectKey].Signeds3download
            .GetAsync(r =>
            {
                r.QueryParameters.PublicResourceFallback=false;
                r.QueryParameters.UseCdn = true;
            });

        var status = signedUrls?.Status;

        // Complete or fallback: single signed URL
        if (status == Signeds3download_response_status.Complete )
        {
            var signedUrl = signedUrls?.Url;
            return await httpClient.GetStreamAsync(
                signedUrl ?? throw new InvalidOperationException($"Signed URL is null for bucket '{bucketKey}', object '{objectKey}' (status: {status})"),
                cancellationToken);
        }

        throw new InvalidOperationException(
            $"Cannot download object from bucket '{bucketKey}', object '{objectKey}'. Unexpected status: '{status}'");
    }

    /// <summary>
    /// Parses a storage URL and extracts the bucket key and object key.
    /// </summary>
    /// <param name="storageUrl">The storage URL to parse</param>
    /// <returns>A tuple containing the bucket key and object key</returns>
    private static (string BucketKey, string ObjectKey) ParseStorageUrl(string storageUrl)
    {
        var storageUrlAsUri = new Uri(storageUrl);
        var bucketKey = storageUrlAsUri.Segments[4].TrimEnd('/');
        var objectKey = storageUrlAsUri.Segments[6];
        return (bucketKey, objectKey);
    }

    /// <summary>
    /// Return the files in a folder
    /// </summary>
    /// <param name="folderPath">The path of the folder to get the files from.</param>
    /// <param name="recursive">Optional: 'true' returns files within the sub folders. Default: 'false'</param>
    /// <returns>List of file ids</returns>
    public async Task<(FolderPath Folder, List<FileItem> Files)> GetAllFilesByFolderPathAsync(string folderPath, bool recursive = false)
    {
        var files = await GetFilesByFolderPathAsync(folderPath, recursive).GetAll();

        return (files.First().Folder, files.Select(f => f.File).ToList());
    }


    /// <summary>
    /// Return the files in a folder (optional recursively)
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="recursive">Optional: 'true' returns files within the sub folders. Default: 'false'</param>
    /// <returns></returns>
    public async IAsyncEnumerable<(FolderPath Folder, FileItem File)> GetFilesByFolderPathAsync(string folderPath, bool recursive = false)
    {

        var folder = await GetFolderByPathAsync(folderPath);

        var files = GetFilesInFoldersAsync(folder, recursive);

        await foreach (var item in files)
        {
            yield return item;
        }

    }

    private async IAsyncEnumerable<(FolderPath Folder, FileItem File)> GetFilesInFoldersAsync(FolderPath parentFolder, bool recursive = false)
    {
        var projectId = parentFolder.Project.Id;
        var parentFolderId = parentFolder.Folders.Last().Id;

        var files = GetFileItemByFolderIdAsync(projectId, parentFolderId);

        await foreach (var item in files)
        {
            yield return (parentFolder, item);
        }

        if (!recursive)
            yield break;


        var subFolders = GetSubFoldersAsync(projectId, parentFolderId);

        await foreach (var subFolder in subFolders)
        {
            var folder = new FolderPath
            {
                Hub = parentFolder.Hub,
                Project = parentFolder.Project,
                Folders = [.. parentFolder.Folders, subFolder]
            };

            await foreach (var item in GetFilesInFoldersAsync(folder, recursive))
            {
                yield return item;
            }

        }

    }

    /// <summary>
    /// Get the file item by its path
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>File item and path data</returns>
    /// <exception cref="FileNotFoundException">Thrown when file is not found. File name included in the exception</exception>
    public async Task<(FolderPath PathData, FileItem FileData)> GetFileItemByPathAsync(string filePath)
    {
        string[] pathElements = filePath.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        var subFolderPath = string.Join(separator.First(), pathElements.SkipLast(1).ToArray());
        var fileName = pathElements.Last();

        FileItem file;
        FolderPath folder;

        try
        {
            folder = await GetFolderByPathAsync(subFolderPath);

            //Search for the file
            var projectId = FixProjectId(folder.Project.Id);
            var parentFolderId = folder.Folders.Last().Id;

            var fileItems = GetFileItemByFolderIdAsync(projectId, parentFolderId);

            file = await fileItems.FirstOrDefault(f => string.Equals(f.Included.Attributes?.Name, fileName, StringComparison.InvariantCultureIgnoreCase))
                        ?? throw new FileNotFoundException($"File '{fileName}' not found in folder '{subFolderPath}'", filePath);
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"File '{fileName}' not found in folder '{subFolderPath}'", filePath, ex);
        }


        return (folder, file);
    }

    /// <summary>
    /// Return the ids of all elements in the path
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>The ids of all elements in the path </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<FolderPath> GetFolderByPathAsync(string folderPath)
    {
        var subFolderList = new SubFolderList();

        string[] folders = folderPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (folders.Length < 3)
            throw new InvalidOperationException("Invalid file path. Path must be like '{AccountName}\\{ProjectName}\\{FolderNames}\\{FileName}'");

        //Get the hub and project
        var hubName = folders[0];
        var projectName = folders[1];

        var projectIdsByName = await GetProjectIdsByNameAsync(hubName, projectName);

        var hubId = projectIdsByName.hubId;

        var projects = projectIdsByName.projects;

        if (projects.Count == 0)
            throw new InvalidOperationException($"Project '{projectName}' not found in hub '{hubName}'");

        if (projects.Count > 1)
            throw new InvalidOperationException($"'{projects.Count}' found with the name '{projectName}' in hub '{hubName}'. This method assumes the 'projectName' is unique");

        var projectId = FixProjectId(projects[0].Id);

        //Get the top folder
        var topFolderName = folders[2];
        var topFolders = await DataMgtApi.Project.V1.Hubs[hubId].Projects[projectId].TopFolders.GetAsync();

        var topFolder = topFolders?.Data?.FirstOrDefault(f => string.Equals(f?.Attributes?.Name, topFolderName, StringComparison.InvariantCultureIgnoreCase));

        if (topFolder is null)
            throw new InvalidOperationException($"Folder '{topFolderName}' not found in project '{projectName}'");

        var folderIds = new List<IdNameMap>() { new() { Name = topFolderName, Id = topFolder.Id ?? "" } };

        //Go through the sub folders
        var firstSubFolderPathPosition = 3;

        for (int i = firstSubFolderPathPosition; i < folders.Length; i++)
        {
            var subFolders = GetSubFoldersAsync(projectId, folderIds.Last().Id);

            var subFolderName = folders[i];

            var folder = await subFolders.FirstOrDefault(f => string.Equals(f.Name, subFolderName, StringComparison.InvariantCultureIgnoreCase));

            if (folder?.Id is null)
                throw new InvalidOperationException($"Folder '{subFolderName}' not found in folder '{folders[^(i - 1)]}'");

            folderIds.Add(folder);
        }

        //Format result
        var hub = new IdNameMap { Name = hubName, Id = hubId };
        var project = new IdNameMap { Name = projectName, Id = projectId };

        return new FolderPath
        {
            Hub = hub,
            Project = project,
            Folders = folderIds,
        };
    }

    /// <summary>
    /// Get the ids of all sub folders in a folder
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>List of folder ids</returns>
    public async Task<(FolderPath Folder, List<IdNameMap> SubFolders)> GetAllSubFoldersByPathAsync(string folderPath)
    {
        var subFolders = await GetSubFoldersByPathAsync(folderPath).GetAll();

        return (subFolders.First().Folder, subFolders.Select(f => f.SubFolder).ToList());
    }

    /// <summary>
    /// Get the ids of sub folders in a folder
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>Stream of sub folders name and their id</returns>
    /// <remarks>Not recursive</remarks>
    public async IAsyncEnumerable<(FolderPath Folder, IdNameMap SubFolder)> GetSubFoldersByPathAsync(string folderPath)
    {
        var folder = await GetFolderByPathAsync(folderPath);

        var subFolder = GetSubFoldersAsync(folder.Project.Id, folder.Folders.Last().Id);

        await foreach (var item in subFolder)
        {
            yield return (folder, item);
        }
    }

    /// <summary>
    /// Return the ids of all sub folders
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="parentFolderId"></param>
    /// <returns>List of the of sub folders path and id</returns>
    /// <remarks>Not recursive</remarks>
    public async Task<List<IdNameMap>> GetAllSubFoldersAsync(string projectId, string parentFolderId)
    {
        return await GetSubFoldersAsync(projectId, parentFolderId).GetAll();
    }

    /// <summary>
    /// Return the ids of all sub folders
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="parentFolderId"></param>
    /// <returns>Stream of the of sub folders path and id</returns>
    /// <remarks>Not recursive</remarks>
    public async IAsyncEnumerable<IdNameMap> GetSubFoldersAsync(string projectId, string parentFolderId)
    {
        projectId = FixProjectId(projectId);

        var isLastPage = false;

        var subFolders = new List<IdNameMap>();

        var pageNumber = 0;

        while (isLastPage == false)
        {
            subFolders = await getSubFolderByPageAsync(pageNumber);

            foreach (var item in subFolders)
            {

                yield return item;

            }

            pageNumber++;
        }


        async Task<List<IdNameMap>> getSubFolderByPageAsync(int pageNumber)
        {
            var subFolders = await _dataMgtClient.Projects[projectId].Folders[parentFolderId].Contents
                                        .GetAsync(r =>
                                        {
                                            r.QueryParameters.FilterextensionType = ["folders"];
                                            r.QueryParameters.Pagenumber = pageNumber;
                                        });

            isLastPage = subFolders?.Links?.Next is null;

            return subFolders?.Data?.Select(f => new IdNameMap { Id = f.Id ?? "", Name = f?.Attributes?.Name ?? "" }).ToList() ?? [];
        }

    }

    /// <summary>
    /// Get the hubs given the name
    /// </summary>
    /// <param name="hubName">Name of the hub. Insensitive case</param>
    /// <returns>Hub data</returns>
    public async Task<List<Hubs_data>> GetHubsByNameAsync(string hubName)
    {
        var hubs = await DataMgtApi.Project.V1.Hubs.GetAsync();

        return hubs?.Data?.Where(h => string.Equals(h.Attributes?.Name, hubName, StringComparison.InvariantCultureIgnoreCase))?.ToList() ?? [];

    }

    /// <summary>
    /// Return the ids of all projects with the same name in a hub
    /// </summary>
    /// <param name="hubName"></param>
    /// <param name="projectName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// /// <exception cref="ArgumentException">Thrown if the hub na</exception>
    public async Task<(string hubId, List<IdNameMap> projects)> GetProjectIdsByNameAsync(string hubName, string projectName)
    {
        var hubs = await GetHubsByNameAsync(hubName);

        if (hubs.Count == 0)
            throw new ArgumentException($"Hub '{hubName}' not found");

        if (hubs.Count > 1)
            throw new InvalidOperationException($"'{hubs.Count}' hubs found with the name '{hubName}'. This method assumes the 'hubName' is unique");

        var hubId = hubs[0].Id ?? "";

        var allProjects = (await GetProjectsByHubIdAsync(hubId).GetAll()).Where(p => string.Equals(p.Name, projectName, StringComparison.InvariantCultureIgnoreCase));

        return (hubId, allProjects.ToList());

    }

    /// <summary>
    /// Get all projects in a hub
    /// </summary>
    /// <param name="hubId">The id of the hub</param>
    /// <returns>List of project Id</returns>
    public async Task<List<IdNameMap>> GetAllProjectsByHubIdAsync(string hubId)
    {
        return await GetProjectsByHubIdAsync(hubId).GetAll();
    }

    /// <summary>
    /// Get all projects in a hub
    /// </summary>
    /// <param name="hubId">The id of the hub</param>
    /// <returns>Stream of project Id</returns>
    public async IAsyncEnumerable<IdNameMap> GetProjectsByHubIdAsync(string hubId)
    {
        var pageNumber = 0;
        var isLastPage = false;
        List<IdNameMap> projects;

        while (isLastPage == false)
        {
            (projects, isLastPage) = await getProjectsByPageAsync(pageNumber);

            foreach (var item in projects)
            {
                yield return item;
            }

            pageNumber++;
        }

        async Task<(List<IdNameMap> projects, bool isLastPage)> getProjectsByPageAsync(int pageNumber)
        {
            var projects = await DataMgtApi.Project.V1.Hubs[hubId].Projects.GetAsync(r => { r.QueryParameters.Pagenumber = pageNumber; });

            var isLastPage = projects?.Links?.Next is null;

            var projectIdsNames = projects?.Data?.Select(p => new IdNameMap(p.Id ?? "", p.Attributes?.Name ?? ""))?.ToList() ?? [];

            return (projectIdsNames, isLastPage);
        }

    }

    /// <summary>
    /// Get all projects in a hub
    /// </summary>
    /// <param name="hubName">Name of the hub</param>
    /// <returns>List of projects</returns>
    /// <exception cref="InvalidOperationException"> <para>hubName</para> is not unique or not found</exception>
    public async Task<(string HubId, List<IdNameMap> Projects)> GetAllProjectsByHubNameAsync(string hubName)
    {
        var projects = await GetProjectsByHubNameAsync(hubName).GetAll();

        return (projects.First().HubId, projects.Select(p => p.Project).ToList());
    }

    /// <summary>
    /// Get all projects in a hub
    /// </summary>
    /// <param name="hubName">Name of the hub</param>
    /// <returns>Stream of projects</returns>
    /// <exception cref="InvalidOperationException"> <para>hubName</para> is not unique or not found</exception>
    public async IAsyncEnumerable<(string HubId, IdNameMap Project)> GetProjectsByHubNameAsync(string hubName)
    {
        var hubs = await GetHubsByNameAsync(hubName);

        if (hubs.Count == 0)
            throw new InvalidOperationException($"Hub {hubName} not found");

        if (hubs.Count > 1)
            throw new InvalidOperationException($"{hubs.Count} found with the name '{hubName}'. This method assumes the 'hubName' is unique");

        var hubId = hubs[0].Id ?? "";

        await foreach (var project in GetProjectsByHubIdAsync(hubId))
        {
            yield return (hubId, project);
        }

    }

    /// <summary>
    /// Get file items from a folder (not recursive)
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' is handled</param>
    /// <param name="folderId">Folder urn like: <c>urn:adsk.wipprod:dm.folder:hC6k4hndRWaeIVhIjvHu8w</c></param>
    /// <param name="fileExtensions">File extensions filter</param>
    /// <returns>List of file items</returns>
    public async Task<List<FileItem>> GetAllFileItemIdsByFolderIdAsync(string projectId, string folderId, IEnumerable<string>? fileExtensions = null)
    {
        return await GetFileItemByFolderIdAsync(projectId, folderId, fileExtensions).GetAll();
    }

    /// <summary>
    /// Get file items from a folder (not recursive)
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' is handled</param>
    /// <param name="folderId">Folder urn like: <c>urn:adsk.wipprod:dm.folder:hC6k4hndRWaeIVhIjvHu8w</c></param>
    /// <param name="fileExtensions">File extensions filter</param>
    /// <returns>List of file items</returns>
    public async IAsyncEnumerable<FileItem> GetFileItemByFolderIdAsync(string projectId, string folderId, IEnumerable<string>? fileExtensions = null)
    {
        projectId = FixProjectId(projectId);

        var pageNumber = 0;
        var isLastPage = false;
        IEnumerable<FileItem> fileItemIds = [];


        while (isLastPage == false)
        {

            (fileItemIds, isLastPage) = await getFileItemIdsByFolderIdByPageAsync(pageNumber);

            foreach (var item in fileItemIds)
            {
                yield return item;
            }

            pageNumber++;
        }


        async Task<(IEnumerable<FileItem> data, bool isLastPage)> getFileItemIdsByFolderIdByPageAsync(int pageNumber)
        {

            var folderContents = await _dataMgtClient.Projects[projectId].Folders[folderId].Contents
                                    .GetAsync(r =>
                                    {
                                        r.QueryParameters.FiltertypeAsGetFilterTypeQueryParameterType = [GetFilterTypeQueryParameterType.Items];
                                        r.QueryParameters.FilterextensionType = ["items:autodesk.bim360:File"];
                                        r.QueryParameters.Pagenumber = pageNumber;
                                    });

            isLastPage = folderContents?.Links?.Next is null;

            var data = folderContents?.Data?
                        .Select(f => new FileItem(
                                        f,
                                        folderContents?.Included?.FirstOrDefault(v => v.Id == f?.Relationships?.Tip?.Data?.Id)
                                        ?? throw new InvalidDataException()))
                        ?? [];


            //No file extension filter
            if (fileExtensions is null || !fileExtensions.Any())
            {
                return (data, isLastPage);
            }

            var filteredFileItem = data.Where(file => file.Data?.Attributes?.Name != null && fileExtensions.Any(ext => file.Data.Attributes.Name.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase))).ToList();

            return (filteredFileItem, isLastPage);
        }
    }

    /// <summary>
    /// Upload file by chunks to a bucket
    /// </summary>
    /// <param name="bucketId">File destination</param>
    /// <param name="uniqueFileName">Unique file name like {{GUID}}.ifc</param>
    /// <param name="fileContent">Content of the file to upload</param>
    /// <param name="defaultChunkSize">Optional: Chunk size in bytes value 10 000 000</param>
    /// <returns>Upload results</returns>
    /// <exception cref="InvalidDataException">No signed url was created</exception>
    public async Task<Completes3upload_response_200> UploadFileAsync(string bucketId, string uniqueFileName, Stream fileContent, int defaultChunkSize = 10000000)
    {
        //Enforce the S3 minimum chunk size 5MB
        var minimalcChunkSize = 5 * 1024 * 1024;
        if (defaultChunkSize < minimalcChunkSize)
            defaultChunkSize = minimalcChunkSize;

        //S3 maximum chunk size is 5GB, but 500MB makes more sense
        var maximalcChunkSize = 500 * 1024 * 1024;
        if (defaultChunkSize > maximalcChunkSize)
            defaultChunkSize = maximalcChunkSize;

        var fileChunks = await CreateFIleChunks(fileContent, defaultChunkSize);

        //Create signed urls
        var signedUrls = await _ossClient.Buckets[bucketId].Objects[uniqueFileName].Signeds3upload.GetAsync(r => { r.QueryParameters.Parts = fileChunks.Count; });

        if (signedUrls?.Urls is null)
        {
            throw new InvalidDataException("SignedUrls is null");
        }

        //Upload the file by chunks in parallel
        fileContent.Position = 0;
        var uploadTasks = new List<Task<HttpResponseMessage>>();

        for (int i = 0; i < fileChunks.Count; i++)
        {
            var url = signedUrls.Urls[i];

            var uploadTask = httpClient.PutAsync(url, new ByteArrayContent(fileChunks[i]));

            uploadTasks.Add(uploadTask);
        }

        await Task.WhenAll(uploadTasks);

        //Complete the upload
        var result = await _ossClient.Buckets[bucketId].Objects[uniqueFileName].Signeds3upload
            .PostAsync(
                new()
                {
                    UploadKey = signedUrls.UploadKey
                });

        return result ?? new Completes3upload_response_200();
    }

    /// <summary>
    /// Create a new file version in a ACC/BIM360 folder. If the file exists, create a new version. 
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' handled automatically</param>
    /// <param name="folderId">Folder id like: </param>
    /// <param name="fileName">File name as displayed in ACC/BIM360</param>
    /// <param name="fileContent"></param>
    /// <param name="defaultChunkSize">Optional: Chunk size in bytes value 10 000 000</param>
    /// <returns>File item id and version Id of the file version created in ACC/BIM360</returns>
    /// <exception cref="InvalidDataException">Upload failed</exception>
    /// <exception cref="InvalidOperationException">Version creation failed</exception>
    public async Task<(string fileItemId, string versionId, Completes3upload_response_200 uploadResult)> UploadFileAsync(string projectId, string folderId, string fileName, Stream fileContent, int defaultChunkSize = 10000000)
    {
        var newVersion = await UploadNewVersionAsync(projectId, folderId, fileName, fileContent, defaultChunkSize);
        if (newVersion?.ObjectId is null || newVersion?.ObjectKey is null)
        {
            throw new InvalidDataException();
        }

        var items = GetFileItemByFolderIdAsync(projectId, folderId);


        var fileItem = await items.FirstOrDefault(i => string.Equals(i.Included.Attributes?.Name, fileName, StringComparison.InvariantCultureIgnoreCase));

        var versionId = string.Empty;
        var fileItemId = string.Empty;

        if (fileItem is null)
        {
            var result = await CreateNewFileAsync(projectId, folderId, newVersion.ObjectId, fileName);
            fileItemId = result?.Data?.Id ?? throw new InvalidOperationException("File id is null");
            versionId = result?.Included?.FirstOrDefault()?.Id ?? throw new InvalidOperationException("Version id is null");
        }
        else
        {
            var result = await UpdateFileAsync(projectId, fileItem.Data.Id ?? throw new InvalidDataException(), fileName, newVersion.ObjectId);
            versionId = result?.Data?.Id ?? throw new InvalidOperationException("Version id is null");
            fileItemId = result?.Included?.FirstOrDefault()?.Id ?? throw new InvalidOperationException("File id is null");
        }

        return (fileItemId, versionId, newVersion);
    }

    /// <summary>
    /// Use Search endpoint to search for files in a folder recursively based on advanced search filters
    /// </summary>
    /// <param name="projectId">Project Id. Prefix 'b.' is handled</param>
    /// <param name="folderId">Folder urn like: <c>urn:adsk.wipprod:dm.folder:hC6k4hndRWaeIVhIjvHu8w</c></param>
    /// <param name="filters">List of filters used as query parameters like: ("filter[fileTYpe]","rvt,mwd") <see href="https://aps.autodesk.com/en/docs/data/v2/developers_guide/filtering/"/></param>
    /// <returns>First 100 search results. Use page[number] to get the next results</returns>
    /// <remarks>Required 3 legged authentication</remarks>
    public async Task<SDKmodels.Search?> SearchAdvancedAsync(string projectId, string folderId, List<(string Name, string Value)> filters)
    {
        projectId = FixProjectId(projectId);

        var requestInfo = _dataMgtClient.Projects[projectId].Folders[folderId].Search.ToGetRequestInformation();

        var searchWithFiltersURI = CreateRequestWithFilters(requestInfo, filters);

        var searchResult = await _dataMgtClient.Projects[projectId].Folders[folderId].Search.WithUrl(searchWithFiltersURI).GetAsync();

        return searchResult;
    }

    /// <summary>
    /// Update the request URI with advanced search filters that includes comparison type or custom filter like 'filter[fileType]=rvt,jpg&amp;filter[attributes.fileName]-contains=Floor'
    /// </summary>
    /// <param name="requestInfo">The request info for the original endpoint. Use 'ToGetRequestInformation()' to retrieve it </param>
    /// <param name="filters">Filters to include as query parameters</param>
    /// <returns>New URL combining the original one and filters</returns>
    public string CreateRequestWithFilters(RequestInformation requestInfo, List<(string Name, string Value)> filters)
    {
        var uriBuilder = new UriBuilder(requestInfo.URI);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var filter in filters)
        {
            query.Add(filter.Name, filter.Value);
        }

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Search for files in a folder recursively and return the latest version of each file
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="skipDeleted">(Optional) Ignore deleted files</param>
    /// <param name="filters">(Optional) Query string like: 'filter[fileType]=txt,jpg'.See documentation https://aps.autodesk.com/en/docs/data/v2/developers_guide/filtering/. </param>
    /// <returns>List of file version</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<List<FileVersion>> SearchAllLatestFileVersionAsync(string projectId, string folderId, bool skipDeleted = true, List<(string Name, string Value)>? filters = null)
    {
        return await SearchLatestFileVersionAsync(projectId, folderId, skipDeleted, filters).GetAll();
    }

    /// <summary>
    /// Search for files in a folder recursively and return the latest version of each file
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="skipDeleted">(Optional) Ignore deleted files</param>
    /// <param name="filters">(Optional) Query string like: 'filter[fileType]=txt,jpg'.See documentation https://aps.autodesk.com/en/docs/data/v2/developers_guide/filtering/. </param>
    /// <returns>Stream of file version</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async IAsyncEnumerable<FileVersion> SearchLatestFileVersionAsync(string projectId, string folderId, bool skipDeleted = true, List<(string Name, string Value)>? filters = null)
    {
        List<(string Name, string Value)> customFilters = filters is null ? [] : [.. filters];
        customFilters.RemoveAll(f => f.Name.StartsWith("page[number]", StringComparison.InvariantCultureIgnoreCase));

        List<FileVersion> fileVersions;

        projectId = FixProjectId(projectId);

        var isLastPage = false;
        var pageNumber = 0;
        while (isLastPage == false)
        {
            (fileVersions, isLastPage) = await searchByPageAsync(pageNumber);

            foreach (var item in fileVersions)
            {
                yield return item;
            }

            pageNumber++;

        }

        //Search for files in a folder recursively and return the latest version of each file
        async Task<(List<FileVersion> FileVersion, bool IsLastPage)> searchByPageAsync(int page)
        {

            var folderContents = await SearchAdvancedAsync(projectId, folderId, [("page[number]", page.ToString()), .. customFilters]);
            var files = folderContents?.Included ?? [];
            if (skipDeleted)
            {
                files = files.Where(f => f.Attributes?.Hidden == false).ToList();
            }

            var fileIds = files.Select(c =>
            {
                if (c.Attributes?.DisplayName is null || c?.Id is null)
                    throw new InvalidOperationException("Invalid file");

                var parentFolderId = c.Relationships?.Parent?.Data?.Id;

                return new FileVersion(parentFolderId, c.Attributes.DisplayName, c?.Relationships?.Tip?.Data?.Id ?? throw new InvalidOperationException("File Version urn is null"));
            });

            isLastPage = folderContents?.Links?.Next is null;

            return (fileIds?.ToList() ?? [], isLastPage);
        }

    }
    /// <summary>
    /// Get the latest version of each file within a folder
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="filters"></param>
    /// <remarks>Use search for recursive search</remarks>
    /// <returns>List of file version</returns>
    /// <exception cref="InvalidOperationException">Unexpected output</exception>
    public async Task<List<FileVersion>> GetAllLatestFilesVersionAsync(string projectId, string folderId, List<(string Name, string Value)>? filters = null)
    {
        return await GetLatestFilesVersionAsync(projectId, folderId, filters).GetAll();
    }

    /// <summary>
    /// Get the latest version of each file within a folder
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="filters"></param>
    /// <remarks>Use search for recursive search</remarks>
    /// <returns>Stream of file version</returns>
    /// <exception cref="InvalidOperationException">Unexpected output</exception>
    public async IAsyncEnumerable<FileVersion> GetLatestFilesVersionAsync(string projectId, string folderId, List<(string Name, string Value)>? filters = null)
    {
        List<(string Name, string Value)> customFilters = filters is null ? [] : [.. filters];
        customFilters.RemoveAll(f => f.Name.StartsWith("page[number]", StringComparison.InvariantCultureIgnoreCase));
        customFilters.RemoveAll(f => f.Name.StartsWith("filter[extension.type]", StringComparison.InvariantCultureIgnoreCase));

        customFilters.Add(("filter[extension.type]", "items:autodesk.bim360:File"));

        projectId = FixProjectId(projectId);

        var fileIds = new List<FileVersion>();

        var isLastPage = false;
        var pageNumber = 0;
        while (isLastPage == false)
        {
            var newFileIds = await getFolderContentByPageAsync(pageNumber);
            foreach (var item in newFileIds)
            {
                yield return item;
            }

            pageNumber++;
        }


        //Get files in a folder and return the latest version of each file
        async Task<List<FileVersion>> getFolderContentByPageAsync(int page)
        {
            var folderContentsReq = _dataMgtClient.Projects[projectId].Folders[folderId].Contents.ToGetRequestInformation();

            var searchWithFiltersURI = CreateRequestWithFilters(folderContentsReq, customFilters);

            var folderContents = await _dataMgtClient.Projects[projectId].Folders[folderId].Contents.WithUrl(searchWithFiltersURI).GetAsync();

            var tmpFileIds = folderContents?.Data?.Select(c =>
            {
                if (c?.Id is null)
                    throw new InvalidOperationException("Invalid file");

                var parentFolderId = c.Relationships?.Parent?.Data?.Id;

                var file = folderContents?.Included?.FirstOrDefault(v => v.Relationships?.Item?.Data?.Id == c.Id);
                if (file?.Attributes?.Name is null)
                    throw new InvalidOperationException("Invalid file");

                return new FileVersion(parentFolderId, file.Attributes.Name, c?.Relationships?.Tip?.Data?.Id ?? throw new InvalidOperationException("File Version urn is null"));
            }) ??
                                throw new InvalidOperationException("Folder contents is null");


            isLastPage = folderContents?.Links?.Next is null;

            return tmpFileIds?.ToList() ?? [];
        }
    }

    /// <summary>
    /// Get files in a folder (not recursive) and return the latest version of each file
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="fileExtensionFilter"></param>
    /// <remarks>Use search for recursive search</remarks>
    /// <returns>List of file version</returns>
    /// <exception cref="InvalidOperationException">Unexpected output</exception>
    public async Task<List<FileVersion>> GetAllLatestFilesVersionAsync(string projectId, string folderId, IEnumerable<string> fileExtensionFilter)
    {
        return await GetLatestFilesVersionAsync(projectId, folderId, fileExtensionFilter).GetAll();
    }
    /// <summary>
    /// Get files in a folder (not recursive) and return the latest version of each file
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="folderId"></param>
    /// <param name="fileExtensionFilter"></param>
    /// <remarks>Use search for recursive search</remarks>
    /// <returns>Stream of file version</returns>
    /// <exception cref="InvalidOperationException">Unexpected output</exception>
    public async IAsyncEnumerable<FileVersion> GetLatestFilesVersionAsync(string projectId, string folderId, IEnumerable<string> fileExtensionFilter)
    {
        await foreach (var file in GetLatestFilesVersionAsync(projectId, folderId))
        {
            if (fileExtensionFilter.Any(ext => file.FileName.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)) == false)
            {
                continue;
            }

            yield return file;
        }

    }

    /// <summary>
    /// Fix project id by adding the prefix 'b.' if it is missing
    /// </summary>
    /// <param name="projectId">Current project id</param>
    /// <returns>Project id with 'b.' prefix</returns>
    public string FixProjectId(string projectId)
    {
        return projectId.StartsWith("b.") ? projectId : $"b.{projectId}";
    }

    /// <summary>
    /// Fix hub id by adding the prefix 'b.' if it is missing
    /// </summary>
    /// <param name="hubId">Hub id to fix</param>
    /// <returns>Hub id with 'b.' prefix</returns>
    public string FixHubId(string hubId)
    {
        return hubId.StartsWith("b.") ? hubId : $"b.{hubId}";
    }
    private async Task<Completes3upload_response_200?> UploadNewVersionAsync(string projectId, string folderId, string fileName, Stream fileContent, int defaultChunkSize = 10000000)
    {

        //Create signed urls
        projectId = FixProjectId(projectId);

        var storage = await _dataMgtClient.Projects[projectId].Storage
            .PostAsync(new()
            {
                Jsonapi = new()
                {
                    Version = "1.0"
                },
                Data = new()
                {
                    Type = "objects",
                    Attributes = new()
                    {
                        Name = fileName,
                    },
                    Relationships = new()
                    {
                        Target = new()
                        {
                            Data = new()
                            {
                                Type = "folders",
                                Id = folderId
                            }
                        }
                    }
                }
            });

        if (storage?.Data?.Id is null)
        {
            throw new InvalidDataException("Storage is null");
        }

        var storageData = storage.Data.Id.Split('/');
        var bucketKey = storageData[0].Split(':').Last();
        var objectKey = storageData[1];

        return await UploadFileAsync(bucketKey, objectKey, fileContent, defaultChunkSize);
    }

    private async Task<SDKmodels.Item?> CreateNewFileAsync(string projectId, string folderId, string storageId, string fileName)
    {
        projectId = FixProjectId(projectId);

        return await _dataMgtClient.Projects[projectId].Items
                        .PostAsync(new SDKmodels.CreateItem()
                        {
                            Jsonapi = new()
                            {
                                Version = "1.0"
                            },
                            Data = new()
                            {
                                Type = "items",
                                Attributes = new()
                                {
                                    DisplayName = fileName,
                                    Extension = new()
                                    {
                                        Type = "items:autodesk.bim360:File",
                                        Version = "1.0"
                                    }
                                },
                                Relationships = new()
                                {
                                    Tip = new()
                                    {
                                        Data = new()
                                        {
                                            Type = "versions",
                                            Id = "1"
                                        }
                                    },
                                    Parent = new()
                                    {
                                        Data = new()
                                        {
                                            Type = "folders",
                                            Id = folderId
                                        }
                                    }
                                }
                            },
                            Included =
                            [
                                new()
                                {
                                    Type = "versions",
                                    Id = "1",
                                    Attributes = new ()
                                    {
                                        Name = fileName,
                                        Extension = new ()
                                        {
                                            Type = "versions:autodesk.bim360:File",
                                            Version = "1.0"
                                        }
                                    },
                                    Relationships = new ()
                                    {
                                        Storage = new ()
                                        {
                                            Data = new ()
                                            {
                                                Type = "objects",
                                                Id = storageId
                                            }
                                        }
                                    }
                                }
                            ]
                        });
    }

    private async Task<SDKmodels.CreatedVersion?> UpdateFileAsync(string projectId, string itemId, string fileName, string objectKey)
    {
        projectId = FixProjectId(projectId);

        return await _dataMgtClient.Projects[projectId].Versions
             .PostAsync(new()
             {
                 Jsonapi = new()
                 {
                     Version = "1.0"
                 },
                 Data = new()
                 {
                     Type = "versions",
                     Attributes = new()
                     {
                         Name = fileName,
                         Extension = new()
                         {
                             Type = "versions:autodesk.bim360:File",
                             Version = "1.0"
                         },
                     },
                     Relationships = new()
                     {
                         Item = new()
                         {
                             Data = new()
                             {
                                 Type = "items",
                                 Id = itemId
                             }
                         },
                         Storage = new()
                         {
                             Data = new()
                             {
                                 Type = "objects",
                                 Id = objectKey
                             }
                         }
                     }
                 }
             });
    }

    private static async Task<List<byte[]>> CreateFIleChunks(Stream fileContent, int defaultChunkSize = 10000000)
    {
        var MAX_CHUNCK = 25; //S3 limit
        var MIN_CHUNCK_SIZE = 5000; //S3 limit

        ArgumentOutOfRangeException.ThrowIfLessThan(defaultChunkSize, MIN_CHUNCK_SIZE);

        var fileSize = fileContent.Length;

        //Recalculate the chunk size to avoid the 25 chunks limit
        if (fileSize / defaultChunkSize > 25)
        {
            defaultChunkSize = (int)fileSize / (MAX_CHUNCK - 1);
        }

        var numberOfChunks = (int)Math.Round((double)(fileSize / defaultChunkSize)) + 1;

        var chunks = new List<byte[]>();

        for (int i = 0; i < numberOfChunks; i++)
        {
            var chunkSize = fileSize - fileContent.Position;
            if (chunkSize > defaultChunkSize)
                chunkSize = defaultChunkSize;

            var buffer = new byte[chunkSize];

            await fileContent.ReadAsync(buffer.AsMemory(0, (int)chunkSize));

            chunks.Add(buffer);
        }

        return chunks;
    }



    private static IEnumerable<T> FilterFilesByExtension<T>(IEnumerable<T> fileNames, IEnumerable<string> extensionsToKeep) where T : IHasFileName
    {
        if (extensionsToKeep.Any() == false)
            return fileNames;

        return fileNames
            .Where(file => extensionsToKeep.Any(ext => file.FileName.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)));
    }
}