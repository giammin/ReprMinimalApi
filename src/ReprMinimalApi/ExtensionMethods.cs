namespace ReprMinimalApi;

public static class ExtensionMethods
{
    public static Uri GetUri(this HttpRequest request)
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port.GetValueOrDefault(80),
            Path = request.Path.ToString(),
            Query = request.QueryString.ToString()
        };
        return uriBuilder.Uri;
    }
}