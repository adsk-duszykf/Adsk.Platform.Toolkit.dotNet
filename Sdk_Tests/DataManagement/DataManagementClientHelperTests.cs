using System.Collections.ObjectModel;
using System.Net.Http.Json;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;
using Autodesk.DataManagement;
using Autodesk.DataManagement.Helpers.Models;
using Autodesk.DataManagement.Models;
using Sdk_Tests;

namespace Tests.DataManagement;

[TestClass]
public class DataManagementClientHelperTests
{
    private static readonly Settings config = Settings.Load();

    private DataManagementClient DMclient { get; init; }

    public DataManagementClientHelperTests()
    {
        DMclient = InitializeDMclient();
    }

    [ClassInitialize]
    public static void StartMockServer(TestContext context)
    {
        APSmockServer.StartMockServer();
    }

    [ClassCleanup]
    public static void StopMockServer()
    {
        APSmockServer.Dispose();
    }

    [TestMethod]
    public async Task ShouldReturnRFIsContainerId()
    {
        var hubId = (await DMclient.Helper.GetHubsByNameAsync(config.HUB_NAME))[0].Id ?? "";
        var containerId = await DMclient.Helper.GetRFIsContainerIdAsync(hubId, config.PROJECT_ID);

        Assert.IsNotNull(containerId);
    }
    [TestMethod]
    public async Task ShouldReturnHubs()
    {
        var hubs = await DMclient.DataMgtApi.Project.V1.Hubs.GetAsync();

        Assert.IsNotNull(hubs);
    }

    [TestMethod]
    public async Task ShouldReturnHubIdGivingItsName()
    {
        var hubs = await DMclient.Helper.GetHubsByNameAsync(config.HUB_NAME);

        Assert.IsNotNull(hubs);
    }

    [TestMethod]
    public async Task ShouldReturnProjectsGivingHubName()
    {
        var projects = await DMclient.Helper.GetAllProjectsByHubNameAsync(config.HUB_NAME);

        Assert.IsNotNull(projects);
    }


    [TestMethod]
    public async Task ShouldReturnFileListWithinFolder()
    {
        var files = await DMclient.Helper.GetAllLatestFilesVersionAsync(config.PROJECT_ID, config.FOLDER_ID);

        Assert.IsNotNull(files);
    }
    [TestMethod]
    public async Task ShouldReturnRevitFileListWithinFolder()
    {
        var files = await DMclient.Helper.GetAllLatestFilesVersionAsync(config.PROJECT_ID, config.FOLDER_ID, ["rvt"]);

        Assert.IsNotNull(files);
    }

    [TestMethod]
    public async Task ShouldFindFileByPath()
    {
        var file = await DMclient.Helper.GetFileItemByPathAsync(config.SAMPLE_FILE_PATH);

        Assert.IsNotNull(file);
    }
    [TestMethod]
    public async Task ShouldReturnFolderGivingItsPath()
    {
        var folderPath = Path.GetDirectoryName(config.SAMPLE_FILE_PATH) ?? throw new ArgumentException($"'{nameof(config.SAMPLE_FILE_PATH)}' is invalid");
        var folder = await DMclient.Helper.GetFolderByPathAsync(folderPath);

        //Minus 2 because the first element in the path is the hub name and the next one is the project name
        var subFoldersCount = folderPath.Split(['\\', '/']).Length - 2;

        Assert.IsTrue(folder.Folders.Count == subFoldersCount);
    }

    [TestMethod]
    public async Task ShouldReturnFoldersWithinFolder()
    {
        var folderPath = Path.GetDirectoryName(config.SAMPLE_FILE_PATH) ?? throw new ArgumentException($"'{nameof(config.SAMPLE_FILE_PATH)}' is invalid");
        var folders = await DMclient.Helper.GetAllSubFoldersByPathAsync(folderPath);

        Assert.IsNotNull(folders);

    }

    [TestMethod]
    public async Task ShouldReturnFilesWithinFolder()
    {
        var folderPath = Path.GetDirectoryName(config.SAMPLE_FILE_PATH) ?? throw new ArgumentException($"'{nameof(config.SAMPLE_FILE_PATH)}' is invalid");
        var files = await DMclient.Helper.GetAllFilesByFolderPathAsync(folderPath);

        Assert.IsNotNull(files);

    }

    [TestMethod]
    public async Task ShouldReturnFilesWithinFolderRecursively()
    {
        var folderPath = Path.GetDirectoryName(config.SAMPLE_FILE_PATH) ?? throw new ArgumentException($"'{nameof(config.SAMPLE_FILE_PATH)}' is invalid");
        var filesEnumerator = DMclient.Helper.GetFilesByFolderPathAsync(folderPath, true);

        var files = new List<(FolderPath, FileItem)>();
        var filesCount = 0;
        await foreach (var file in filesEnumerator)
        {
            files.Add(file);
            filesCount++;
        }

        Assert.IsTrue(filesCount > 0);

    }
    [TestMethod]
    public async Task ShouldUploadFileInBucketWithMultiParts()
    {
        var bucketName = Guid.NewGuid().ToString();

        var newBucket = await DMclient.OssApi.Oss.V2.Buckets.PostAsync(
            new Autodesk.DataManagement.OSS.Models.Create_buckets_payload()
            {
                PolicyKey = Autodesk.DataManagement.OSS.Models.Create_buckets_payload_policyKey.Transient,
                BucketKey = bucketName
            },
            r => { r.Headers.Add("x-ads-region", "US"); });

        //Create a random file content
        var uniqueFileName = $"{Guid.NewGuid()}.txt";

        //ChunkSize < FileSize =>  4 chunks
        var chuncSize = 6 * 1024 * 1024;

        byte[] byteArray = new byte[20 * 1024 * 1024];
        var rnd = new Random();
        rnd.NextBytes(byteArray);

        var fileContent = new MemoryStream(byteArray);

        //Upload the file by chunks
        var files = await DMclient.Helper.UploadFileAsync(bucketName, uniqueFileName, fileContent, chuncSize);

        Assert.IsNotNull(files);

        //Might take few seconds to be really deleted on the server
        await DMclient.OssApi.Oss.V2.Buckets[bucketName].DeleteAsync();
    }

    [TestMethod]
    public async Task ShouldUploadFileInBucketWithoutMultiParts()
    {
        var bucketName = Guid.NewGuid().ToString();

        var newBucket = await DMclient.OssApi.Oss.V2.Buckets.PostAsync(
            new Autodesk.DataManagement.OSS.Models.Create_buckets_payload()
            {
                PolicyKey = Autodesk.DataManagement.OSS.Models.Create_buckets_payload_policyKey.Transient,
                BucketKey = bucketName
            },
            r => { r.Headers.Add("x-ads-region", "US"); });

        //Create a random file content
        var uniqueFileName = $"{Guid.NewGuid()}.txt";

        //ChunkSize > FileSize => 1 chunk
        var chuncSize = 5 * 1024 * 1024;
        byte[] byteArray = new byte[2 * 1024 * 1024];
        var rnd = new Random();
        rnd.NextBytes(byteArray);

        var fileContent = new MemoryStream(byteArray);

        //Upload the file by chunks
        var files = await DMclient.Helper.UploadFileAsync(bucketName, uniqueFileName, fileContent, chuncSize);

        Assert.IsNotNull(files);

        //Might take few seconds to be really deleted on the server
        await DMclient.OssApi.Oss.V2.Buckets[bucketName].DeleteAsync();
    }
    [TestMethod]
    public async Task ShouldCreateTwoVersionsOfFileAndDeleteIt()
    {
        //Create a random file content
        var uniqueFileName = $"{Guid.NewGuid()}.txt";

        byte[] byteArray = new byte[11 * 1024];
        var rnd = new Random();
        rnd.NextBytes(byteArray);

        var fileContent = new MemoryStream(byteArray);

        //Upload the file by chunks
        var (fileItemId, versionId, _) = await DMclient.Helper.UploadFileAsync(config.PROJECT_ID, config.FOLDER_ID, uniqueFileName, fileContent, 50000);

        Assert.IsNotNull(fileItemId);
        Assert.IsTrue(versionId.EndsWith("1"));

        //Upload a new version of the file by chunks
        rnd.NextBytes(byteArray);

        (fileItemId, versionId, _) = await DMclient.Helper.UploadFileAsync(config.PROJECT_ID, config.FOLDER_ID, uniqueFileName, fileContent, 50000);

        Assert.IsNotNull(fileItemId);
        Assert.IsTrue(versionId.EndsWith("2"));

        //Might take few seconds to be really deleted on the server
        var result = await DMclient.Helper.DeleteFileAsync(config.PROJECT_ID, fileItemId);

        //ACC doesn't delete the file but create a new version marked "deleted"
        Assert.IsTrue(result.versionId.EndsWith("3"));
    }

    [TestMethod]
    public async Task ShouldRenameFile()
    {

        //Create a random file content
        var uniqueFileName = $"{Guid.NewGuid()}.txt";

        byte[] byteArray = new byte[11 * 1024];
        var rnd = new Random();
        rnd.NextBytes(byteArray);

        var fileContent = new MemoryStream(byteArray);

        //Upload the file
        var (fileItemId, versionId, _) = await DMclient.Helper.UploadFileAsync(config.PROJECT_ID, config.FOLDER_ID, uniqueFileName, fileContent, 50000);
        Assert.IsNotNull(fileItemId);
        Assert.IsTrue(versionId.EndsWith("1"));

        //Rename the file using PATCH items
        var newFileName = $"{Guid.NewGuid()}.txt";

        //Initialize a new client without proxy 
        //Because WireMock proxy decode the URL before forwarding the request to the real APS endpoint, 
        //which makes the encoded URN not work for the rename operation
        var DMclientNoProxy = InitializeDMclient(false);

        VersionObject renameResult;

        renameResult = await DMclientNoProxy.Helper.RenameFileAsync(config.PROJECT_ID, versionId, newFileName);

        Assert.IsNotNull(renameResult);
        Assert.AreEqual(versionId, renameResult.Data?.Id);
        Assert.AreEqual(newFileName, renameResult.Data?.Attributes?.DisplayName);

        //Cleanup
        await DMclient.Helper.DeleteFileAsync(config.PROJECT_ID, fileItemId);
    }

    [TestMethod]
    public async Task ShouldReturnFileIsDeleted()
    {
        var isDeleted = await DMclient.Helper.IsFileDeletedAsync(config.PROJECT_ID, config.DELETED_FILE_URN);
        Assert.IsTrue(isDeleted);
    }

    private static DataManagementClient InitializeDMclient(bool useMockServer = true)
    {

        var authProxyClient = APSmockServer.CreateProxyHttpClient(AdskService.Authentication);

        var authClient = new AuthenticationClient(authProxyClient);

        var scope = new List<string>
        {
            AuthenticationScopeDefaults.BucketCreate,
            AuthenticationScopeDefaults.BucketDelete,
            AuthenticationScopeDefaults.DataRead,
            AuthenticationScopeDefaults.DataCreate,
            AuthenticationScopeDefaults.DataWrite,
            AuthenticationScopeDefaults.DataSearch
        };

        async Task<string> getAccessToken()
        {
            var token = await authClient.Helper.GetTwoLeggedToken(config.APS_CLIENT_ID, config.APS_CLIENT_SECRET, scope);

            return token?.AccessToken is null ? throw new InvalidOperationException() : token.AccessToken;
        }

        var dataMgtProxyClient = APSmockServer.CreateProxyHttpClient(AdskService.DataManagement);

        var DMclient = new DataManagementClient(getAccessToken, dataMgtProxyClient);

        if (!useMockServer)
        {
            authClient = new AuthenticationClient();
            DMclient = new DataManagementClient(getAccessToken);
        }

        return DMclient;
    }
}

