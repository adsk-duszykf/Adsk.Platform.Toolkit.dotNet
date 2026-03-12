using Autodesk.Vault;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Vault;

[TestClass]
public class VaultClientTests
{
    private static readonly Func<Task<string>> ValidTokenProvider = () => Task.FromResult("test-token");
    private const string ValidServerUrl = "https://vaultserver.example.com";

    [TestMethod]
    public void ShouldCreate3LeggedClient()
    {
        var client = new VaultClient(ValidTokenProvider, ValidServerUrl);

        Assert.IsNotNull(client.Api);
        Assert.AreEqual(ValidServerUrl, client.VaultServerUrl);
    }

    [TestMethod]
    public void ShouldCreate2LeggedClient()
    {
        var client = new VaultClient(ValidTokenProvider, ValidServerUrl, "user@example.com");

        Assert.IsNotNull(client.Api);
        Assert.AreEqual(ValidServerUrl, client.VaultServerUrl);
    }

    [TestMethod]
    public void ShouldCreateClientWithCustomHttpClient()
    {
        using var httpClient = new System.Net.Http.HttpClient();
        var client = new VaultClient(ValidTokenProvider, ValidServerUrl, httpClient);

        Assert.IsNotNull(client.Api);
    }

    [TestMethod]
    public void ShouldCreateClientWithTrailingSlash()
    {
        var client = new VaultClient(ValidTokenProvider, "https://vaultserver.example.com/");

        Assert.IsNotNull(client.Api);
        Assert.AreEqual("https://vaultserver.example.com/", client.VaultServerUrl);
    }

    [TestMethod]
    public void ShouldThrowOnInvalidUrl()
    {
        Assert.ThrowsException<ArgumentException>(
            () => new VaultClient(ValidTokenProvider, "not-a-url"));
    }

    [TestMethod]
    public void ShouldThrowOnEmptyUrl()
    {
        Assert.ThrowsException<ArgumentException>(
            () => new VaultClient(ValidTokenProvider, string.Empty));
    }

    [TestMethod]
    public void ShouldThrowOnFtpUrl()
    {
        Assert.ThrowsException<ArgumentException>(
            () => new VaultClient(ValidTokenProvider, "ftp://vaultserver.example.com"));
    }

    [TestMethod]
    public void ShouldAcceptHttpUrl()
    {
        var client = new VaultClient(ValidTokenProvider, "http://10.148.0.1");

        Assert.IsNotNull(client.Api);
        Assert.AreEqual("http://10.148.0.1", client.VaultServerUrl);
    }

    [TestMethod]
    public void ShouldInitializeAllManagers()
    {
        var client = new VaultClient(ValidTokenProvider, ValidServerUrl);

        Assert.IsNotNull(client.Auth);
        Assert.IsNotNull(client.Accounts);
        Assert.IsNotNull(client.Options);
        Assert.IsNotNull(client.Informational);
        Assert.IsNotNull(client.Properties);
        Assert.IsNotNull(client.FilesAndFolders);
        Assert.IsNotNull(client.Items);
        Assert.IsNotNull(client.ChangeOrders);
        Assert.IsNotNull(client.Links);
        Assert.IsNotNull(client.Search);
        Assert.IsNotNull(client.Jobs);
    }
}
