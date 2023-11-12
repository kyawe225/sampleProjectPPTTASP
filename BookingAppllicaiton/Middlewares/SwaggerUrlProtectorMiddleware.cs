using System.Text.Json;

namespace BookingAppllicaiton.Middlewares;

public class SwaggerUrlProtectorMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerUrlProtectorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.Path.Value != null && httpContext.Request.Path.HasValue &&
            httpContext.Request.Path.Value.Contains("swagger.json", StringComparison.InvariantCultureIgnoreCase))
        {
            if (httpContext.User.Identity is { IsAuthenticated: false })
            {
                var originalStream = httpContext.Response.Body;
                using (var memStream = new MemoryStream())
                {
                    //Change default unreadable stream with memory stream to be able to read the response afterwards
                    httpContext.Response.Body = memStream;
                    await _next(httpContext);
                    var response = ProtectResponse(httpContext.Response);
                    await originalStream.WriteAsync(response);
                    httpContext.Response.Body = originalStream;
                    return;
                }
            }
        }

        await _next(httpContext);
    }


    private byte[] ProtectResponse(HttpResponse response)
    {
        response.Body.Position = 0;
        var sr = new StreamReader(response.Body);
        var json = sr.ReadToEnd();

        using var writer = new Utf8JsonWriter(response.Body);
        byte[] result;

        using (var memoryStream1 = new MemoryStream())
        {
            using (var utf8JsonWriter1 = new Utf8JsonWriter(memoryStream1))
            {
                using (var jsonDocument = JsonDocument.Parse(json))
                {
                    utf8JsonWriter1.WriteStartObject();

                    foreach (var element in jsonDocument.RootElement.EnumerateObject())
                    {
                        if (element.Name == "paths")
                        {
                            utf8JsonWriter1.WritePropertyName(element.Name);
                            utf8JsonWriter1.WriteStartObject();
                            utf8JsonWriter1.WriteEndObject();
                        }
                        else
                        {
                            element.WriteTo(utf8JsonWriter1);
                        }
                    }

                    utf8JsonWriter1.WriteEndObject();
                }
            }

            result = memoryStream1.ToArray();
        }

        return result;
    }
}