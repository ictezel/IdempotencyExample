using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Text;

namespace IdempotencyExample.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IdempotentAttribute : ActionFilterAttribute
{
    public string[] KeyValues { get; set; }
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private const string CacheKeyPrefix = "Idempotent_";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var cacheKey = GenerateCacheKey(request);

        if (_memoryCache.TryGetValue(cacheKey, out var cachedResponse))
        {
            context.Result = cachedResponse as IActionResult;
            return;
        }

        var executedContext = await next();
        if (executedContext.Result is ObjectResult objectResult)
        {
            _memoryCache.Set(cacheKey, objectResult, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        }
    }

    private string GenerateCacheKey(HttpRequest request)
    {
        string cacheKey;

        if (KeyValues.Any())
        {
            var requestBody = GetRequestBody(request);
            var jsonObject = JObject.Parse(requestBody);
            var selectedFields = KeyValues.Select(fieldName =>
                jsonObject[fieldName]?.ToString() ?? string.Empty).ToArray();

            cacheKey = CacheKeyPrefix + string.Join("_", selectedFields);
        }
        else
        {
            var requestBody = GetRequestBody(request);
            cacheKey = CacheKeyPrefix + Convert.ToBase64String(Encoding.UTF8.GetBytes(requestBody));
        }

        cacheKey += request.QueryString;
        cacheKey += string.Join("_", request.Headers.OrderBy(h => h.Key).Select(h => $"{h.Key}:{string.Join(",", h.Value)}"));

        return cacheKey;
    }

    private string GetRequestBody(HttpRequest request)
    {
        if (request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(request.Body))
            {
                return reader.ReadToEnd();
            }
        }

        return string.Empty;
    }
}