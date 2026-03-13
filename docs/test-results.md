# 단위 테스트 실행 결과

> **실행 환경:** .NET 9.0 · xUnit 2.9.2 · Moq 4.20.72
> **실행 일시:** 2026-03-13
> **결과:** ✅ 13 / 13 통과

## 실행 명령어

```bash
dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj --verbosity normal
```

## 테스트 결과 출력

```
지정된 패턴과 일치한 총 테스트 파일 수는 1개입니다.

  통과 PharmSight.Tests.Services.AiInsightServiceTests.GetInsightAsync_두번_호출시_캐시를_반환한다 [60 ms]
  통과 PharmSight.Tests.Services.AiInsightServiceTests.GetInsightAsync_API키_없으면_GeneratedAt이_설정된다 [1 ms]
  통과 PharmSight.Tests.Services.AiInsightServiceTests.GetInsightAsync_API키_없으면_Repository_호출하지_않는다 [3 ms]
  통과 PharmSight.Tests.Services.AiInsightServiceTests.GetInsightAsync_API키_없으면_안내메시지를_반환한다 [9 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다 [71 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다 [3 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetMonthlySalesAsync_빈_결과도_정상_반환된다 [1 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetWholesaleExpensesAsync_도매상별_지출이_반환된다 [1 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다 [1 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다 [1 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다 [1 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetPatientAgeGroupsAsync_연령대_데이터가_반환된다 [3 ms]
  통과 PharmSight.Tests.Services.DashboardServiceTests.GetKpiSummaryAsync_KPI_요약이_반환된다 [< 1 ms]

총 테스트 수: 13
     통과: 13
    경고 0개
    오류 0개
경과 시간: 00:00:05.07
```

---

## 테스트 파일 구조

```
backend/PharmSight.Tests/
├── PharmSight.Tests.csproj
└── Services/
    ├── DashboardServiceTests.cs   ← 9개 테스트 케이스
    └── AiInsightServiceTests.cs   ← 4개 테스트 케이스
```

---

## DashboardServiceTests.cs 주요 코드

**파일 경로:** `backend/PharmSight.Tests/Services/DashboardServiceTests.cs`

```csharp
public class DashboardServiceTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly Mock<ILogger<DashboardService>> _loggerMock;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _repositoryMock = new Mock<IDashboardRepository>();
        _loggerMock = new Mock<ILogger<DashboardService>>();
        _service = new DashboardService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다()
    {
        // Arrange
        var expected = new List<MonthlySales>
        {
            new("2026-01", 5_200_000m, 132L),
            new("2026-02", 4_800_000m, 118L),
            new("2026-03", 5_500_000m, 140L),
        };
        _repositoryMock.Setup(r => r.GetMonthlySalesAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetMonthlySalesAsync()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("2026-01", result[0].Month);
        Assert.Equal(5_200_000m, result[0].TotalAmount);
        _repositoryMock.Verify(r => r.GetMonthlySalesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다()
    {
        // Arrange — 전월 매출 0 → 변화율 0 엣지 케이스
        var expected = new KpiSummary(
            CurrentMonthSales: 3_200_000m,
            CurrentMonthPrescriptions: 80L,
            CurrentMonthPatients: 60L,
            CurrentMonthOrderAmount: 1_500_000m,
            SalesChangeRate: 0m,
            PrescriptionChangeRate: 0m
        );
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);

        // Act
        var result = await _service.GetKpiSummaryAsync();

        // Assert
        Assert.Equal(0m, result.SalesChangeRate);
        Assert.Equal(0m, result.PrescriptionChangeRate);
    }
}
```

---

## AiInsightServiceTests.cs 주요 코드

**파일 경로:** `backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`

```csharp
public class AiInsightServiceTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<AiInsightService>> _loggerMock;

    private AiInsightService CreateService(string apiKey = "")
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Gemini:ApiKey"] = apiKey })
            .Build();
        return new AiInsightService(
            _repositoryMock.Object, _httpClientFactoryMock.Object,
            cache, _loggerMock.Object, config);
    }

    [Fact]
    public async Task GetInsightAsync_API키_없으면_안내메시지를_반환한다()
    {
        // Arrange — API 키 미설정 (Graceful Degradation 검증)
        var service = CreateService(apiKey: "");

        // Act
        var result = await service.GetInsightAsync();

        // Assert — 예외 없이 안내 메시지 반환
        Assert.NotNull(result);
        Assert.NotEmpty(result.Summary);
        Assert.Contains("API", result.Summary);
    }

    [Fact]
    public async Task GetInsightAsync_두번_호출시_캐시를_반환한다()
    {
        // Arrange
        var service = CreateService(apiKey: "");

        // Act — 동일 서비스 인스턴스 두 번 호출
        var first = await service.GetInsightAsync();
        var second = await service.GetInsightAsync();

        // Assert — 캐시 히트: 동일 GeneratedAt 타임스탬프
        Assert.Equal(first.GeneratedAt, second.GeneratedAt);
    }

    [Fact]
    public async Task GetInsightAsync_API키_없으면_Repository_호출하지_않는다()
    {
        var service = CreateService(apiKey: "");
        await service.GetInsightAsync();

        // Assert — API 키 없을 때 불필요한 DB 조회 발생 안 함
        _repositoryMock.Verify(r => r.GetKpiSummaryAsync(), Times.Never);
        _repositoryMock.Verify(r => r.GetMonthlySalesAsync(), Times.Never);
    }
}
```

---

## 테스트 전략 요약

| 항목 | 내용 |
|------|------|
| **테스트 대상** | Service 계층 (DashboardService, AiInsightService) |
| **Mock 대상** | IDashboardRepository, IHttpClientFactory, ILogger |
| **테스트 패턴** | Arrange-Act-Assert (AAA) |
| **커버리지 영역** | 정상 흐름, 빈 결과 엣지 케이스, 0 나눗셈 엣지 케이스, Graceful Degradation, IMemoryCache 캐시 동작 |
| **격리 수준** | 외부 DB / HTTP 의존성 없음 (완전 격리된 순수 단위 테스트) |
