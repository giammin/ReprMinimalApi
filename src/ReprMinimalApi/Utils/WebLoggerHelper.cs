using System.Text;

namespace ReprMinimalApi.Utils;

public static class WebLoggerHelper
{
    public static async ValueTask<string> GetRequestDataAsync(HttpRequest request, bool includeBody = false, CancellationToken cancellationToken = default)
    {
        var sb = GetRequestDataSb(request);

        if (includeBody && !MultipartRequestHelper.IsMultipartContentType(request.ContentType))
        {
            var body = await ReadRequestBodyAsync(request, cancellationToken);
            sb.AppendLine("BODY:").AppendLine(body);
        }
        return sb.ToString();
    }

    public static StringBuilder GetRequestDataSb(HttpRequest request)
    {
        var guid = GetRequestUid(request.HttpContext);
        var sb = new StringBuilder($"REQUEST: {guid}{Environment.NewLine}");
        sb.AppendLine($"{request.Protocol} {request.Method} {request.GetUri().AbsoluteUri} {request.HttpContext.Connection.RemoteIpAddress}");
        sb.AppendLine("HEADER:");
        using var enumerator = request.Headers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            sb.AppendLine($"{enumerator.Current.Key}={string.Join(",", enumerator.Current.Value!)}");
        }

        return sb;
    }

    public static string GetResponseDataAsync(HttpResponse response)
    {
        var guid = GetRequestUid(response.HttpContext);
        var sb = new StringBuilder($"RESPONSE: {guid}{Environment.NewLine}");
        sb.AppendLine($"{response.StatusCode} {response.ContentType} {response.HttpContext.Connection.RemoteIpAddress}");
        sb.AppendLine("HEADER:");
        using var enumerator = response.Headers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            sb.AppendLine($"{enumerator.Current.Key}={string.Join(",", enumerator.Current.Value!)}");
        }

        return sb.ToString();
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        request.EnableBuffering();

        request.Body.Seek(0, SeekOrigin.Begin);
        const int maxLength = 32 * 1024;
        var length = request.Body.Length > maxLength ? maxLength : request.Body.Length;
        //using var sr = new StreamReader(response.Body);
        //var rtn = await sr.ReadToEndAsync();
        var buffer = new byte[length];
        await request.Body.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
        var rtn = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return rtn;
    }

    public static Guid GetRequestUid(HttpContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        Guid rtn;
        if (!context.Items.ContainsKey("RequestUid") || context.Items["RequestUid"] is not Guid)
        {
            rtn = Guid.NewGuid();
            context.Items["RequestUid"] = rtn;
        }
        else
        {
            rtn = (Guid)context.Items["RequestUid"];
        }
        return rtn;
    }
}