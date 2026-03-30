using System.Web;

namespace Autodesk.Vault.Managers;

internal static class PaginationHelper
{
    internal static string? ExtractBookmarkFromNextUrl(string nextUrl)
    {
        try
        {
            var uri = new Uri(nextUrl, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri("http://placeholder" + nextUrl);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            return queryParams["bookmark"] ?? queryParams["cursorState"];
        }
        catch
        {
            return null;
        }
    }
}
