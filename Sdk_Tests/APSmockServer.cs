using System.Net;
using Sdk_Tests;
using WireMock.Handlers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Tests
{
    internal class APSmockServer
    {
        private static readonly string BaseMockServerURL = "http://localhost:4200";
        private static readonly Settings config = Settings.Load();
        public static WireMockServer? MockServer { get; private set; }
        private static Dictionary<AdskService, string> ServiceRootUrls => new()
        {
            {AdskService.DataManagement, "https://developer.api.autodesk.com"},
            {AdskService.Authentication, "https://developer.api.autodesk.com"},
            {AdskService.ModelDerivative, "https://developer.api.autodesk.com/modelderivative/v2"},
            {AdskService.ACC_CustomAttributes, "https://developer.api.autodesk.com/bim360/docs/v1"},
            {AdskService.ACC_RFIs, "https://developer.api.autodesk.com/bim360/rfis"},
        };

        public static System.Net.Http.HttpClient CreateProxyHttpClient(AdskService service)
        {

            var handler = new Autodesk.Common.HttpClientLibrary.Middleware.CustomErrorHandler()
            {
                InnerHandler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                }
            };

            var httpClient = new System.Net.Http.HttpClient(handler)
            {
                BaseAddress = new Uri(GetMockAPSserviceUrl(service))
            };

            return httpClient;
        }

        public static string GetMockAPSserviceUrl(AdskService service)
        {
            return $"{BaseMockServerURL}/{service}";
        }

        /// <summary>
        /// Create a mock
        /// </summary>
        /// <param name="recordingPath">Path to the folder containing the recorded responses</param>
        /// <param name="proxyUrl">URL used by HttpClient in replacement of APS host</param>
        /// <param name="serviceRootUrl">URL of the APS host</param>
        /// <param name="urlPathReplacements">Settings for replacing the URL in the request before performing it</param>
        /// <returns>Mock server</returns>
        public static WireMockServer StartMockServer()
        {

            if (MockServer is not null)
            {
                return MockServer;
            }

            var recordingPath = Path.Combine(config.RECORDED_ENDPOINTS_FOLDER_PATH);

            if (string.IsNullOrEmpty(recordingPath))
            {
                throw new ArgumentNullException($"'{nameof(recordingPath)}' is undefined in the config file");
            }

            if (Path.Exists(recordingPath) == false)
            {
                throw new DirectoryNotFoundException($"'{recordingPath}' not found");
            }


            MockServer = WireMockServer.Start(new WireMockServerSettings
            {

                //ReadStaticMappings = true,
                Urls = [BaseMockServerURL],
                FileSystemHandler = new LocalFileSystemHandler(recordingPath),

            });

            var adskServices = Enum.GetValues(typeof(AdskService)).Cast<AdskService>().ToList();

            foreach (var service in adskServices)
            {
                MockServer
                    .Given(Request.Create().WithPath($"/{service}/*"))
                    .RespondWith(Response.Create()
                        .WithProxy(new ProxyAndRecordSettings
                        {
                            Url = ServiceRootUrls[service],

                            //SaveMapping = true,
                            SaveMappingToFile = true,
                            ExcludedHeaders = ["Authorization"],
                            ReplaceSettings = new ProxyUrlReplaceSettings
                            {
                                OldValue = $"/{service}",
                                NewValue = ""
                            }
                        })
                    );
            }

            // Specific mock for rate limiting testing
            MockServer.Given(Request.Create().WithPath("/ratelimit/*"))
                .RespondWith(Response.Create()
                                .WithStatusCode(System.Net.HttpStatusCode.OK)
                                .WithDelay(2)
                                .WithHeader("Retry-After", "120"));

            // Specific mock for error testing
            MockServer.Given(Request.Create().WithPath("/error/*"))
                .RespondWith(Response.Create()
                                .WithStatusCode(System.Net.HttpStatusCode.InternalServerError)
                                .WithBodyAsJson(new { message = "Expected error" })
                                );
            // Specific mock for custom status code testing (returns 302 without redirect)
            MockServer.Given(Request.Create().WithPath("/status/302"))
                .RespondWith(Response.Create()
                                .WithStatusCode(System.Net.HttpStatusCode.Found)
                                .WithBodyAsJson(new { message = "Found" })
                                );

            // Specific mock for query parameter testing
            MockServer.Given(Request.Create().WithPath("/queryParam/*").WithParam(p =>
            {
                var testParam = p.Keys.FirstOrDefault(k => k == "testparam");
                return testParam != null && p.Count == 1;
            })).RespondWith(Response.Create()
                                .WithStatusCode(System.Net.HttpStatusCode.OK)
                                .WithBodyAsJson(new { message = "Expected query parameter" })
                                );


            return MockServer;
        }

        public static void Dispose()
        {
            MockServer?.Stop();
            MockServer?.Dispose();
        }
    }

    internal enum AdskService
    {
        Authentication,
        ModelDerivative,
        DataManagement,
        ACC_CustomAttributes,
        ACC_RFIs
    }
}
