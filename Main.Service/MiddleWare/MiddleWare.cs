using Microsoft.Extensions.Primitives;

namespace Main.Service.MiddleWare;

public class AuthMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    public AuthMiddleware(RequestDelegate next, ILoggerFactory logFactory) {
        _next = next;
        _logger = logFactory.CreateLogger("MyMiddleware");
    }
    public async Task Invoke(HttpContext httpContext) {

        const string HeaderKeyName = "Auth-Token";
        httpContext.Request.Headers.TryGetValue(HeaderKeyName, out StringValues headerValue);
        if (httpContext.Items.ContainsKey(HeaderKeyName))
        {
            httpContext.Items[HeaderKeyName] = headerValue;
        }
        else
        {
            httpContext.Items.Add(HeaderKeyName, $"{headerValue}-received");
        }
        
        await _next(httpContext); 
    }
}

public static class MyMiddlewareExtensions {
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware < AuthMiddleware > ();
    }
}