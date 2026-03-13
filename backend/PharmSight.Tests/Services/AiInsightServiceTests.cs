using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services;

namespace PharmSight.Tests.Services;

/// <summary>
/// AiInsightService 단위 테스트.
/// API 키 미설정 시 Graceful Degradation 동작 및 캐시 히트 동작을 검증합니다.
/// </summary>
public class AiInsightServiceTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<AiInsightService>> _loggerMock;

    public AiInsightServiceTests()
    {
        _repositoryMock = new Mock<IDashboardRepository>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<AiInsightService>>();
    }

    private AiInsightService CreateService(string apiKey = "")
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gemini:ApiKey"] = apiKey
            })
            .Build();

        return new AiInsightService(
            _repositoryMock.Object,
            _httpClientFactoryMock.Object,
            cache,
            _loggerMock.Object,
            config
        );
    }

    // ─────────────────────────────────────────────────────────
    // API 키 미설정 시 Graceful Degradation
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetInsightAsync_API키_없으면_안내메시지를_반환한다()
    {
        // Arrange — API 키 미설정
        var service = CreateService(apiKey: "");

        // Act
        var result = await service.GetInsightAsync();

        // Assert — 에러를 던지지 않고 안내 메시지 반환
        Assert.NotNull(result);
        Assert.NotEmpty(result.Summary);
        Assert.Contains("API", result.Summary);
    }

    [Fact]
    public async Task GetInsightAsync_API키_없으면_Repository_호출하지_않는다()
    {
        // Arrange
        var service = CreateService(apiKey: "");

        // Act
        await service.GetInsightAsync();

        // Assert — 불필요한 DB 조회 없음
        _repositoryMock.Verify(r => r.GetKpiSummaryAsync(), Times.Never);
        _repositoryMock.Verify(r => r.GetMonthlySalesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetInsightAsync_API키_없으면_GeneratedAt이_설정된다()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);
        var service = CreateService(apiKey: "");

        // Act
        var result = await service.GetInsightAsync();

        // Assert
        Assert.True(result.GeneratedAt >= before);
    }

    // ─────────────────────────────────────────────────────────
    // 캐시 동작 검증
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetInsightAsync_두번_호출시_캐시를_반환한다()
    {
        // Arrange — API 키 없음 → 첫 호출에서 결과 캐시됨
        var service = CreateService(apiKey: "");

        // Act
        var first = await service.GetInsightAsync();
        var second = await service.GetInsightAsync();

        // Assert — 동일 인스턴스 참조(캐시 히트)
        Assert.Equal(first.GeneratedAt, second.GeneratedAt);
    }
}
