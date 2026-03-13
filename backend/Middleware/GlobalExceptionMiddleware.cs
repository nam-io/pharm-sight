using System.Net;
using System.Text.Json;

namespace PharmSight.Api.Middleware;

/// <summary>
/// 전역 예외 처리 미들웨어.
/// 처리되지 않은 모든 예외를 캐치하여 일관된 JSON 오류 응답을 반환합니다.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "처리되지 않은 예외 발생: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException      => (int)HttpStatusCode.BadRequest,
            InvalidOperationException  => (int)HttpStatusCode.BadRequest,
            _                          => (int)HttpStatusCode.InternalServerError,
        };

        var response = new
        {
            error = exception.Message,
            statusCode = context.Response.StatusCode,
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await context.Response.WriteAsync(json);
    }
}
