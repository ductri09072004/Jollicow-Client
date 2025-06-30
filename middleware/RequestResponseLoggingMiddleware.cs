using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RequestResponseLoggingOptions _options;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IOptions<RequestResponseLoggingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.ToString();

        bool shouldLog = _options.IncludedPaths.Count == 0 ||
                         _options.IncludedPaths.Any(p => path.StartsWith(p));

        _logger.LogInformation($"Path: {path}");
        _logger.LogInformation($"Should log: {shouldLog}");
        _logger.LogInformation($"Enable request body logging: {_options.EnableRequestBodyLogging}");
        _logger.LogInformation($"Enable response body logging: {_options.EnableResponseBodyLogging}");
        _logger.LogInformation($"Max body length: {_options.MaxBodyLength}");
        _logger.LogInformation($"Included paths: {string.Join(", ", _options.IncludedPaths)}");
        _logger.LogInformation($"Allowed content types: {string.Join(", ", _options.AllowedContentTypes)}");

        // Ghi log request
        if (shouldLog && _options.EnableRequestBodyLogging &&
            context.Request.ContentLength > 0 &&
            IsAllowedContentType(context.Request.ContentType))
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (requestBody.Length > _options.MaxBodyLength)
                requestBody = requestBody.Substring(0, _options.MaxBodyLength) + "...(truncated)";

            _logger.LogInformation($"Request: {context.Request.Method} {path} | Body: {requestBody}");
        }

        // Bọc response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context); // Chạy middleware tiếp theo

            if (shouldLog && _options.EnableResponseBodyLogging &&
                IsAllowedContentType(context.Response.ContentType))
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

                if (responseText.Length > _options.MaxBodyLength)
                    responseText = responseText.Substring(0, _options.MaxBodyLength) + "...(truncated)";

                _logger.LogInformation($"📤 Response: {context.Response.StatusCode} | Body: {responseText}");
                context.Response.Body.Seek(0, SeekOrigin.Begin);
            }

            context.Response.Headers.Remove("Content-Length"); // Xóa Content-Length để tránh lỗi mismatch
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
    private bool IsAllowedContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return false;
        return _options.AllowedContentTypes.Any(type => contentType.Contains(type));
    }
}


// Class cho middleware
public class RequestResponseLoggingOptions
{
    public bool EnableRequestBodyLogging { get; set; } = true;
    public bool EnableResponseBodyLogging { get; set; } = true;
    public int MaxBodyLength { get; set; } = 5000; // Giới hạn độ dài log
    public List<string> IncludedPaths { get; set; } = new(); // VD: "/api"
    public List<string> AllowedContentTypes { get; set; } = new()
    {
        "application/json",
        "text/plain",
        "text/html"
    };
}