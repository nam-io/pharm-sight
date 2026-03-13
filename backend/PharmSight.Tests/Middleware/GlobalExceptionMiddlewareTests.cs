using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Middleware;

namespace PharmSight.Tests.Middleware;

/// <summary>
/// GlobalExceptionMiddleware 단위 테스트.
/// 전역 예외 처리 미들웨어가 예외 유형별로 올바른 HTTP 상태코드와
/// 일관된 JSON 오류 응답을 반환하는지 검증합니다.
/// </summary>
public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;

    public GlobalExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
    }

    [Fact]
    public async Task 정상_요청은_예외_없이_통과한다()
    {
        // Arrange — 예외를 던지지 않는 정상 파이프라인
        var middleware = new GlobalExceptionMiddleware(
            next: _ => Task.CompletedTask,
            _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert — 상태코드 변경 없음 (200 기본값)
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task ArgumentNullException은_400_BadRequest를_반환한다()
    {
        // Arrange
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new ArgumentNullException("param", "필수 파라미터 누락"),
            _loggerMock.Object);
        var context = CreateContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert — 400 Bad Request + JSON 오류 응답
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        var body = await ReadResponseBody(context);
        Assert.Contains("error", body);
        Assert.Contains("400", body);
    }

    [Fact]
    public async Task InvalidOperationException은_400_BadRequest를_반환한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new InvalidOperationException("잘못된 연산"),
            _loggerMock.Object);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task 일반_Exception은_500_InternalServerError를_반환한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new Exception("예상치 못한 오류"),
            _loggerMock.Object);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task 오류_응답은_JSON_형식으로_반환된다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new Exception("테스트 오류"),
            _loggerMock.Object);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        // Assert — Content-Type이 application/json
        Assert.Equal("application/json", context.Response.ContentType);

        // Assert — JSON 파싱 가능하고 error/statusCode 필드 존재
        var body = await ReadResponseBody(context);
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.TryGetProperty("error", out _));
        Assert.True(doc.RootElement.TryGetProperty("statusCode", out var statusCode));
        Assert.Equal(500, statusCode.GetInt32());
    }

    // ─────────────────────────────────────────────────────────
    // 헬퍼
    // ─────────────────────────────────────────────────────────

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponseBody(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
