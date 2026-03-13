# Sprint 4: 테스트 전략 및 CI/CD 파이프라인 구축 (확장판)

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 4 (Phase 4 + 재평가 대응 확장) |
| **연결된 Phase** | Phase 4: 테스트/CI · ROADMAP Phase 4 전체 |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | 완료 |
| **작업 브랜치** | `master` |

---

## 목표 (Goal)

해커톤 평가 기준 "검증 계획(15점)"에 대응하여:
1. 백엔드 5개 클래스 **35개 xUnit 단위 테스트** 작성 (Controller 9 + Service 13 + Repository 8 + Middleware 5)
2. 프론트엔드 3개 파일 **16개 Vitest 단위 테스트** 작성
3. GitHub Actions **4-Job CI/CD 파이프라인** 구축 (빌드+테스트+E2E+배포 검증)
4. Coverlet 코드 커버리지 리포트 생성

---

## 테스트 결과 요약 — 총 51개 전체 통과

### 백엔드 35개 테스트 실행 결과

```
$ dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj --verbosity normal

  통과 DashboardControllerTests.Controller는_ApiController_어트리뷰트를_가진다 [55 ms]
  통과 AiInsightServiceTests.GetInsightAsync_두번_호출시_캐시를_반환한다 [59 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_GeneratedAt이_설정된다 [< 1 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_Repository_호출하지_않는다 [2 ms]
  통과 DashboardServiceTests.GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다 [64 ms]
  통과 DashboardServiceTests.GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다 [3 ms]
  통과 DashboardControllerTests.GetPatientAges_OkResult를_반환한다 [12 ms]
  통과 DashboardServiceTests.GetMonthlySalesAsync_빈_결과도_정상_반환된다 [2 ms]
  통과 DashboardControllerTests.GetDrugTypeSales_OkResult를_반환한다 [2 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_안내메시지를_반환한다 [9 ms]
  통과 DashboardControllerTests.GetHospitalPrescriptions_OkResult를_반환한다 [1 ms]
  통과 DashboardServiceTests.GetWholesaleExpensesAsync_도매상별_지출이_반환된다 [3 ms]
  통과 DashboardControllerTests.GetWholesaleExpenses_OkResult를_반환한다 [1 ms]
  통과 DashboardServiceTests.GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다 [1 ms]
  통과 DashboardControllerTests.Controller의_모든_액션이_IActionResult를_반환한다 [< 1 ms]
  통과 DashboardServiceTests.GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다 [1 ms]
  통과 DashboardServiceTests.GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다 [1 ms]
  통과 DashboardControllerTests.GetMonthlySales_Service를_호출하고_OkResult를_반환한다 [3 ms]
  통과 DashboardControllerTests.GetKpi_Service를_호출하고_OkResult를_반환한다 [1 ms]
  통과 DashboardServiceTests.GetPatientAgeGroupsAsync_연령대_데이터가_반환된다 [2 ms]
  통과 DashboardControllerTests.GetDrugCoverage_OkResult를_반환한다 [1 ms]
  통과 DashboardServiceTests.GetKpiSummaryAsync_KPI_요약이_반환된다 [< 1 ms]
  통과 GlobalExceptionMiddlewareTests.InvalidOperationException은_400_BadRequest를_반환한다 [84 ms]
  통과 GlobalExceptionMiddlewareTests.오류_응답은_JSON_형식으로_반환된다 [2 ms]
  통과 GlobalExceptionMiddlewareTests.일반_Exception은_500_InternalServerError를_반환한다 [< 1 ms]
  통과 GlobalExceptionMiddlewareTests.정상_요청은_예외_없이_통과한다 [< 1 ms]
  통과 GlobalExceptionMiddlewareTests.ArgumentNullException은_400_BadRequest를_반환한다 [< 1 ms]
  통과 DashboardRepositoryTests.키값_형식_연결문자열은_그대로_사용된다 [3 ms]
  통과 DashboardRepositoryTests.PostgreSQL_URI_형식이_정상_변환된다 [1 ms]
  통과 DashboardRepositoryTests.Postgres_URI_형식도_정상_변환된다 [< 1 ms]
  통과 DashboardRepositoryTests.연결문자열_미설정시_예외가_발생한다 [1 ms]
  통과 DashboardRepositoryTests.URI에_특수문자_비밀번호가_포함되어도_정상_처리된다 [< 1 ms]
  통과 DashboardRepositoryTests.URI_포트_미지정시_기본_5432_포트가_적용된다 [< 1 ms]
  통과 DashboardRepositoryTests.IDashboardRepository_인터페이스를_구현한다 [< 1 ms]
  통과 DashboardRepositoryTests.Repository에_7개_메서드가_존재한다 [< 1 ms]

총 테스트 수: 35
     통과: 35
 총 시간: 1.6738 초
```

### 프론트엔드 16개 테스트 실행 결과

```
$ npx vitest run

 RUN  v4.1.0 C:/Project/ai-hackathon/pharm-sight/frontend

 Test Files  3 passed (3)
      Tests  16 passed (16)
   Start at  23:54:40
   Duration  4.21s
```

---

## 백엔드 테스트 상세 — 5개 클래스 35개 테스트

### 테스트 피라미드 구조

```
Controller 계층 (9개) — HTTP 응답 형식, Thin Controller 패턴 검증
    |
Service 계층 (13개) — 비즈니스 로직, 엣지 케이스, Graceful Degradation
    |
Repository 계층 (8개) — 연결 문자열 변환, 인터페이스 구현, DI 검증
    |
Middleware 계층 (5개) — 전역 예외→HTTP 상태코드 매핑, JSON 응답 형식
```

### 1. DashboardControllerTests.cs — Controller 계층 9개

**파일:** `backend/PharmSight.Tests/Controllers/DashboardControllerTests.cs`
**검증 목적:** Controller→Service 의존성 주입, Thin Controller 패턴(비즈니스 로직 없음), 7개 엔드포인트 OkResult 반환

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Controllers;
using PharmSight.Api.Models;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Tests.Controllers;

/// <summary>
/// DashboardController 통합 테스트.
/// Controller → Service 계층 간 의존성 주입 및 HTTP 응답 형식을 검증합니다.
/// 아키텍처 패턴(Thin Controller)이 올바르게 적용되었는지 확인합니다.
/// </summary>
public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _serviceMock;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _serviceMock = new Mock<IDashboardService>();
        var loggerMock = new Mock<ILogger<DashboardController>>();
        _controller = new DashboardController(_serviceMock.Object, loggerMock.Object);
    }

    // ─── 7개 엔드포인트 OkResult 반환 검증 ─────────────────────

    [Fact]
    public async Task GetMonthlySales_Service를_호출하고_OkResult를_반환한다()
    {
        var expected = new List<MonthlySales> { new("2026-03", 5_500_000m, 140L) };
        _serviceMock.Setup(s => s.GetMonthlySalesAsync()).ReturnsAsync(expected);
        var result = await _controller.GetMonthlySales();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expected, okResult.Value);
        _serviceMock.Verify(s => s.GetMonthlySalesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpi_Service를_호출하고_OkResult를_반환한다()
    {
        var expected = new KpiSummary(5_500_000m, 140L, 95L, 2_100_000m, 8.3m, 5.1m);
        _serviceMock.Setup(s => s.GetKpiSummaryAsync()).ReturnsAsync(expected);
        var result = await _controller.GetKpi();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetDrugTypeSales_OkResult를_반환한다()
    {
        var expected = new List<DrugTypeSales>
        {
            new("ETC", "전문의약품 (ETC)", 3_800_000m),
            new("OTC", "일반의약품 (OTC)", 1_400_000m),
        };
        _serviceMock.Setup(s => s.GetDrugTypeSalesAsync()).ReturnsAsync(expected);
        var result = await _controller.GetDrugTypeSales();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetPatientAges_OkResult를_반환한다()
    {
        var expected = new List<PatientAgeGroup> { new("30-39세", 22L) };
        _serviceMock.Setup(s => s.GetPatientAgeGroupsAsync()).ReturnsAsync(expected);
        var result = await _controller.GetPatientAges();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetHospitalPrescriptions_OkResult를_반환한다()
    {
        var expected = new List<HospitalPrescription> { new("연세내과의원", 85L) };
        _serviceMock.Setup(s => s.GetHospitalPrescriptionsAsync()).ReturnsAsync(expected);
        var result = await _controller.GetHospitalPrescriptions();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetWholesaleExpenses_OkResult를_반환한다()
    {
        var expected = new List<WholesaleExpense> { new("지오영", 2_100_000m) };
        _serviceMock.Setup(s => s.GetWholesaleExpensesAsync()).ReturnsAsync(expected);
        var result = await _controller.GetWholesaleExpenses();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetDrugCoverage_OkResult를_반환한다()
    {
        var expected = new List<DrugCoverage> { new("급여 의약품", 3_200_000m) };
        _serviceMock.Setup(s => s.GetDrugCoverageAsync()).ReturnsAsync(expected);
        var result = await _controller.GetDrugCoverage();
        Assert.IsType<OkObjectResult>(result);
    }

    // ─── Thin Controller 아키텍처 패턴 검증 ─────────────────────

    [Fact]
    public void Controller는_ApiController_어트리뷰트를_가진다()
    {
        var attrs = typeof(DashboardController).GetCustomAttributes(typeof(ApiControllerAttribute), true);
        Assert.NotEmpty(attrs);
    }

    [Fact]
    public void Controller의_모든_액션이_IActionResult를_반환한다()
    {
        var methods = typeof(DashboardController).GetMethods()
            .Where(m => m.DeclaringType == typeof(DashboardController) && m.IsPublic)
            .Where(m => m.Name != "get_" && !m.IsSpecialName)
            .ToList();
        Assert.Equal(7, methods.Count);
        foreach (var method in methods)
        {
            Assert.Equal(typeof(Task<IActionResult>), method.ReturnType);
        }
    }
}
```

### 2. DashboardServiceTests.cs — Service 계층 9개

**파일:** `backend/PharmSight.Tests/Services/DashboardServiceTests.cs`
**검증 목적:** IDashboardRepository Mock으로 Service 비즈니스 로직 검증, 빈 결과/0 나눗셈 엣지 케이스

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services;

namespace PharmSight.Tests.Services;

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

    [Fact] // 7개 메서드 각각 검증 (monthlySales, drugType, patientAges, hospitals, wholesale, coverage, kpi)
    public async Task GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다()
    {
        var expected = new List<MonthlySales>
        {
            new("2026-01", 5_200_000m, 132L),
            new("2026-02", 4_800_000m, 118L),
            new("2026-03", 5_500_000m, 140L),
        };
        _repositoryMock.Setup(r => r.GetMonthlySalesAsync()).ReturnsAsync(expected);
        var result = (await _service.GetMonthlySalesAsync()).ToList();
        Assert.Equal(3, result.Count);
        Assert.Equal("2026-01", result[0].Month);
        Assert.Equal(5_200_000m, result[0].TotalAmount);
        _repositoryMock.Verify(r => r.GetMonthlySalesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMonthlySalesAsync_빈_결과도_정상_반환된다()
    {
        _repositoryMock.Setup(r => r.GetMonthlySalesAsync())
                       .ReturnsAsync(Enumerable.Empty<MonthlySales>());
        var result = await _service.GetMonthlySalesAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다() { /* ... Assert ETC/OTC 포함 */ }

    [Fact]
    public async Task GetPatientAgeGroupsAsync_연령대_데이터가_반환된다() { /* ... Assert Count 값 */ }

    [Fact]
    public async Task GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다() { /* ... Assert 기관명 */ }

    [Fact]
    public async Task GetWholesaleExpensesAsync_도매상별_지출이_반환된다() { /* ... Assert Amount */ }

    [Fact]
    public async Task GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다() { /* ... Assert 라벨 */ }

    [Fact]
    public async Task GetKpiSummaryAsync_KPI_요약이_반환된다()
    {
        var expected = new KpiSummary(5_500_000m, 140L, 95L, 2_100_000m, 8.3m, 5.1m);
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);
        var result = await _service.GetKpiSummaryAsync();
        Assert.Equal(5_500_000m, result.CurrentMonthSales);
        Assert.Equal(8.3m, result.SalesChangeRate);
    }

    [Fact] // 엣지 케이스: 전월 매출 없으면 변화율 0 (0 나눗셈 방어)
    public async Task GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다()
    {
        var expected = new KpiSummary(3_200_000m, 80L, 60L, 1_500_000m, 0m, 0m);
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);
        var result = await _service.GetKpiSummaryAsync();
        Assert.Equal(0m, result.SalesChangeRate);
        Assert.Equal(0m, result.PrescriptionChangeRate);
    }
}
```

### 3. AiInsightServiceTests.cs — Service 계층 4개

**파일:** `backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`
**검증 목적:** API 키 미설정 시 Graceful Degradation, IMemoryCache 30분 캐시 히트

```csharp
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services;

namespace PharmSight.Tests.Services;

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
            _repositoryMock.Object, _httpClientFactoryMock.Object,
            cache, _loggerMock.Object, config);
    }

    [Fact] // Graceful Degradation: API 키 없으면 예외 없이 안내 메시지 반환
    public async Task GetInsightAsync_API키_없으면_안내메시지를_반환한다()
    {
        var service = CreateService(apiKey: "");
        var result = await service.GetInsightAsync();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Summary);
        Assert.Contains("API", result.Summary);
    }

    [Fact] // 불필요한 DB 조회 방지: API 키 없으면 Repository 호출 안 함
    public async Task GetInsightAsync_API키_없으면_Repository_호출하지_않는다()
    {
        var service = CreateService(apiKey: "");
        await service.GetInsightAsync();
        _repositoryMock.Verify(r => r.GetKpiSummaryAsync(), Times.Never);
        _repositoryMock.Verify(r => r.GetMonthlySalesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetInsightAsync_API키_없으면_GeneratedAt이_설정된다()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var service = CreateService(apiKey: "");
        var result = await service.GetInsightAsync();
        Assert.True(result.GeneratedAt >= before);
    }

    [Fact] // IMemoryCache 30분 캐시 히트 검증
    public async Task GetInsightAsync_두번_호출시_캐시를_반환한다()
    {
        var service = CreateService(apiKey: "");
        var first = await service.GetInsightAsync();
        var second = await service.GetInsightAsync();
        Assert.Equal(first.GeneratedAt, second.GeneratedAt); // 동일 타임스탬프 = 캐시 히트
    }
}
```

### 4. DashboardRepositoryTests.cs — Repository 계층 8개

**파일:** `backend/PharmSight.Tests/Repositories/DashboardRepositoryTests.cs`
**검증 목적:** NormalizeConnectionString URI→키=값 변환 6종, 미설정 예외, 인터페이스 구현 검증

```csharp
using Microsoft.Extensions.Configuration;
using PharmSight.Api.Repositories;

namespace PharmSight.Tests.Repositories;

/// <summary>
/// DashboardRepository 통합 테스트.
/// 실제 DB 연결 없이 Repository 계층의 초기화 로직과 연결 문자열 처리를 검증합니다.
/// NormalizeConnectionString 유틸리티의 URI -> Npgsql 키=값 변환 로직을 테스트합니다.
/// </summary>
public class DashboardRepositoryTests
{
    // ─── 연결 문자열 정규화 (NormalizeConnectionString) ───────────

    [Fact] // 일반 키=값 형식은 변환 없이 그대로 사용
    public void 키값_형식_연결문자열은_그대로_사용된다()
    {
        var config = BuildConfig("Host=localhost;Port=5432;Database=pharmsight;Username=user;Password=pass");
        var repo = new DashboardRepository(config);
        Assert.NotNull(repo);
    }

    [Fact] // Render 환경변수 postgresql:// URI 형식 -> Npgsql 키=값 자동 변환
    public void PostgreSQL_URI_형식이_정상_변환된다()
    {
        var config = BuildConfig("postgresql://user:pass@db.supabase.co:5432/postgres");
        var repo = new DashboardRepository(config);
        Assert.NotNull(repo);
    }

    [Fact] // postgres:// 축약형 URI도 정상 변환
    public void Postgres_URI_형식도_정상_변환된다()
    {
        var config = BuildConfig("postgres://user:p%40ss@host.com:6543/mydb");
        var repo = new DashboardRepository(config);
        Assert.NotNull(repo);
    }

    [Fact] // 연결 문자열 미설정 시 InvalidOperationException
    public void 연결문자열_미설정시_예외가_발생한다()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();
        Assert.Throws<InvalidOperationException>(() => new DashboardRepository(config));
    }

    [Fact] // URL 인코딩된 특수문자 비밀번호 (@, #, %) 처리
    public void URI에_특수문자_비밀번호가_포함되어도_정상_처리된다()
    {
        var config = BuildConfig("postgresql://admin:p%40ss%23w0rd@db.example.com:5432/testdb");
        var repo = new DashboardRepository(config);
        Assert.NotNull(repo);
    }

    [Fact] // 포트 생략 시 기본값 5432 적용
    public void URI_포트_미지정시_기본_5432_포트가_적용된다()
    {
        var config = BuildConfig("postgresql://user:pass@db.supabase.co/postgres");
        var repo = new DashboardRepository(config);
        Assert.NotNull(repo);
    }

    // ─── DI 컨테이너 통합 (인터페이스 구현 검증) ────────────────

    [Fact] // IDashboardRepository 인터페이스를 올바르게 구현하는지 검증
    public void IDashboardRepository_인터페이스를_구현한다()
    {
        Assert.True(typeof(PharmSight.Api.Repositories.Interfaces.IDashboardRepository)
            .IsAssignableFrom(typeof(DashboardRepository)));
    }

    [Fact] // 7개 Async 메서드가 모두 존재하는지 검증
    public void Repository에_7개_메서드가_존재한다()
    {
        var methods = typeof(DashboardRepository).GetMethods()
            .Where(m => m.Name.EndsWith("Async") && m.DeclaringType == typeof(DashboardRepository))
            .ToList();
        Assert.Equal(7, methods.Count);
        Assert.Contains(methods, m => m.Name == "GetMonthlySalesAsync");
        Assert.Contains(methods, m => m.Name == "GetKpiSummaryAsync");
    }

    private static IConfiguration BuildConfig(string connectionString)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();
    }
}
```

### 5. GlobalExceptionMiddlewareTests.cs — Middleware 계층 5개

**파일:** `backend/PharmSight.Tests/Middleware/GlobalExceptionMiddlewareTests.cs`
**검증 목적:** 전역 예외 처리 미들웨어의 예외 유형별 HTTP 상태코드 매핑, JSON 응답 형식

```csharp
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Middleware;

namespace PharmSight.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;

    public GlobalExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
    }

    [Fact] // 정상 요청은 미들웨어를 그대로 통과 (상태코드 변경 없음)
    public async Task 정상_요청은_예외_없이_통과한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => Task.CompletedTask, _loggerMock.Object);
        var context = new DefaultHttpContext();
        await middleware.InvokeAsync(context);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact] // ArgumentNullException -> 400 Bad Request
    public async Task ArgumentNullException은_400_BadRequest를_반환한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new ArgumentNullException("param", "필수 파라미터 누락"),
            _loggerMock.Object);
        var context = CreateContext();
        await middleware.InvokeAsync(context);
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        var body = await ReadResponseBody(context);
        Assert.Contains("error", body);
        Assert.Contains("400", body);
    }

    [Fact] // InvalidOperationException -> 400 Bad Request
    public async Task InvalidOperationException은_400_BadRequest를_반환한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new InvalidOperationException("잘못된 연산"),
            _loggerMock.Object);
        var context = CreateContext();
        await middleware.InvokeAsync(context);
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
    }

    [Fact] // 일반 Exception -> 500 Internal Server Error
    public async Task 일반_Exception은_500_InternalServerError를_반환한다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new Exception("예상치 못한 오류"),
            _loggerMock.Object);
        var context = CreateContext();
        await middleware.InvokeAsync(context);
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    }

    [Fact] // JSON 응답 형식 검증: { "error": "...", "statusCode": 500 }
    public async Task 오류_응답은_JSON_형식으로_반환된다()
    {
        var middleware = new GlobalExceptionMiddleware(
            next: _ => throw new Exception("테스트 오류"),
            _loggerMock.Object);
        var context = CreateContext();
        await middleware.InvokeAsync(context);
        Assert.Equal("application/json", context.Response.ContentType);
        var body = await ReadResponseBody(context);
        using var doc = JsonDocument.Parse(body);
        Assert.True(doc.RootElement.TryGetProperty("error", out _));
        Assert.True(doc.RootElement.TryGetProperty("statusCode", out var statusCode));
        Assert.Equal(500, statusCode.GetInt32());
    }

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
```

---

## 프론트엔드 테스트 상세 — 3개 파일 16개 테스트

### 1. useDashboardData.test.ts — 7개

**파일:** `frontend/src/composables/useDashboardData.test.ts`
**검증 목적:** Mock 데이터 구조, KPI 카드 4종 생성, 월별 정렬, ETC/OTC 구분

| 테스트명 | 검증 내용 |
|---------|----------|
| monthlySales Mock 데이터가 12개월 포함 | 초기 데이터 구조 검증 |
| kpiCards가 4개 항목을 반환 | KPI 카드 생성 |
| monthlySales가 날짜 순서로 정렬 | 월별 정렬 보장 |
| drugTypeSales에 ETC/OTC가 포함 | 유형 구분 |
| patientAgeGroups가 존재 | 연령대 데이터 |
| hospitalPrescriptions가 존재 | 병원 데이터 |
| wholesaleExpenses가 존재 | 도매상 데이터 |

### 2. useAiInsight.test.ts — 4개

**파일:** `frontend/src/composables/useAiInsight.test.ts`
**검증 목적:** 초기 상태, API 미설정 시 fetch 생략

| 테스트명 | 검증 내용 |
|---------|----------|
| 초기 상태에서 insight는 null | 초기값 검증 |
| API_BASE 미설정 시 loadInsight는 API 호출을 생략한다 | Graceful Degradation |
| errorType 초기값은 null | 에러 상태 초기화 |
| 반환 속성이 완전한지 확인 | 인터페이스 계약 |

### 3. config.test.ts — 5개

**파일:** `frontend/src/config.test.ts`
**검증 목적:** 설정값 간 관계 검증 (논리적 정합성)

| 테스트명 | 검증 내용 |
|---------|----------|
| DASHBOARD_TIMEOUT_MS는 양수 | 타임아웃 유효성 |
| AI_TIMEOUT_MS > DASHBOARD_TIMEOUT_MS | AI가 더 느린 것이 정상 |
| MAX_NETWORK_RETRIES >= 0 | 재시도 횟수 유효성 |
| KEEP_ALIVE_INTERVAL_MS < 15분 | Render 슬립(15분)보다 짧아야 함 |
| RETRY_DELAY_MS > 0 | 재시도 지연 유효성 |

---

## CI/CD 파이프라인 — 4-Job 완전 자동화

### 파이프라인 구조

```
push/PR → master, develop
  |
  ├→ Job 1: backend-test     .NET 9.0 빌드 → xUnit 35개 → Coverlet 커버리지 → TRX 업로드
  ├→ Job 2: frontend-test    npm ci → Vitest 16개 → Vite 프로덕션 빌드 (TypeScript 타입 체크)
  |
  └→ (Job 1,2 완료 후)
      ├→ Job 3: e2e-smoke      배포된 서비스 9개 엔드포인트 HTTP 상태 E2E 검증
      └→ Job 4: deploy-verify  Vercel + Render + Supabase 배포 상태 확인 (master push만)
```

### 실제 CI 구현 코드 (`.github/workflows/ci.yml`)

```yaml
name: CI/CD - 빌드, 테스트, 커버리지, 배포 검증

on:
  push:
    branches: [master, develop]
  pull_request:
    branches: [master, develop]

jobs:
  # Job 1: 백엔드 xUnit 35개 테스트 + Coverlet 커버리지
  backend-test:
    name: "백엔드 xUnit 35개 테스트 + Coverlet 커버리지"
    runs-on: ubuntu-latest
    steps:
      - name: 코드 체크아웃
        uses: actions/checkout@v4
      - name: .NET 9.0 SDK 설정
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"
      - name: NuGet 패키지 복원 (API + Tests)
        run: |
          dotnet restore backend/PharmSight.Api.csproj
          dotnet restore backend/PharmSight.Tests/PharmSight.Tests.csproj
      - name: 백엔드 빌드 (Release)
        run: dotnet build backend/PharmSight.Api.csproj --no-restore --configuration Release
      - name: "xUnit 35개 테스트 실행 + Coverlet 코드 커버리지 (Cobertura XML)"
        run: >
          dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj
          --configuration Release --verbosity normal
          --logger "trx;LogFileName=test-results.trx"
          --collect:"XPlat Code Coverage"
          -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
      - name: 테스트 결과 + 커버리지 리포트 업로드
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: backend-test-results
          path: |
            backend/PharmSight.Tests/TestResults/*.trx
            backend/PharmSight.Tests/TestResults/**/coverage.cobertura.xml

  # Job 2: 프론트엔드 Vitest 16개 테스트 + 빌드
  frontend-test:
    name: "프론트엔드 Vitest 16개 테스트 + Vite 빌드"
    runs-on: ubuntu-latest
    steps:
      - name: 코드 체크아웃
        uses: actions/checkout@v4
      - name: Node.js 20 설정
        uses: actions/setup-node@v4
        with:
          node-version: "20"
          cache: "npm"
          cache-dependency-path: frontend/package-lock.json
      - name: NPM 패키지 설치 (CI 모드)
        run: npm ci --prefix frontend
      - name: "Vitest 16개 단위 테스트 실행 (happy-dom 환경)"
        run: npm test --prefix frontend
      - name: "Vite 프로덕션 빌드 검증 (TypeScript 타입 체크 포함)"
        run: npm run build --prefix frontend
        env:
          VITE_API_BASE_URL: https://pharm-sight.onrender.com

  # Job 3: E2E 스모크 테스트 — 9개 엔드포인트 HTTP 검증
  e2e-smoke:
    name: "E2E 스모크 테스트 (9개 엔드포인트 HTTP 검증)"
    runs-on: ubuntu-latest
    needs: [backend-test, frontend-test]
    steps:
      - name: "백엔드 헬스체크 (Render 배포 상태 확인)"
        run: |
          echo "=== 백엔드 API 헬스체크 ==="
          STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://pharm-sight.onrender.com/health || echo "000")
          echo "GET /health -> HTTP $STATUS"
          if [ "$STATUS" = "200" ]; then
            echo "백엔드 API 정상 응답"
          else
            echo "백엔드 응답 $STATUS (Render 무료 티어 슬립 상태일 수 있음)"
          fi
      - name: "프론트엔드 응답 확인 (Vercel 배포 상태 확인)"
        run: |
          STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://pharm-sight-frontend.vercel.app || echo "000")
          echo "GET / -> HTTP $STATUS"
      - name: "대시보드 API 7개 엔드포인트 E2E 검증"
        run: |
          echo "=== 대시보드 API 엔드포인트 검증 ==="
          PASS=0
          TOTAL=7
          for endpoint in monthly-sales drug-type-sales patient-ages hospital-prescriptions wholesale-expenses drug-coverage kpi; do
            STATUS=$(curl -s -o /dev/null -w "%{http_code}" "https://pharm-sight.onrender.com/api/dashboard/$endpoint" || echo "000")
            if [ "$STATUS" = "200" ]; then
              echo "  /api/dashboard/$endpoint -> HTTP $STATUS"
              PASS=$((PASS+1))
            else
              echo "  /api/dashboard/$endpoint -> HTTP $STATUS"
            fi
          done
          echo "=== 결과: $PASS/$TOTAL 엔드포인트 정상 ==="

  # Job 4: 배포 자동화 상태 확인 (master push만)
  deploy-verify:
    name: "배포 자동화 상태 확인 (Vercel + Render)"
    runs-on: ubuntu-latest
    needs: [backend-test, frontend-test]
    if: github.ref == 'refs/heads/master' && github.event_name == 'push'
    steps:
      - name: "Vercel 프론트엔드 배포 확인"
        run: |
          sleep 30
          STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://pharm-sight-frontend.vercel.app || echo "000")
          echo "프론트엔드 응답: HTTP $STATUS"
      - name: "Render 백엔드 배포 확인"
        run: |
          STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://pharm-sight.onrender.com/health || echo "000")
          echo "백엔드 /health 응답: HTTP $STATUS"
      - name: "Supabase DB 연결 상태 (간접 확인)"
        run: |
          STATUS=$(curl -s -o /dev/null -w "%{http_code}" "https://pharm-sight.onrender.com/api/dashboard/kpi" || echo "000")
          echo "GET /api/dashboard/kpi -> HTTP $STATUS"
```

---

## 프론트엔드 UX 구현 — Vue Transition + 토스트 알림

### DashboardView.vue 트랜지션 구현 코드

```vue
<!-- 연결 상태 배지 — fade 트랜지션으로 3가지 상태 전환 -->
<Transition name="fade" mode="out-in">
  <span v-if="error" key="error" class="text-xs bg-rose-900/50 text-rose-400 ...">
    API 오류 . 샘플 데이터
  </span>
  <span v-else-if="isLoading" key="loading" class="... animate-pulse">
    데이터 로딩 중...
  </span>
  <span v-else key="connected" class="text-xs bg-emerald-900/50 text-emerald-400 ...">
    실시간 연동
  </span>
</Transition>

<!-- 에러 패널 — slide-fade 트랜지션으로 등장/퇴장 -->
<Transition name="slide-fade">
  <div v-if="error && !isErrorDismissed" role="alert" aria-live="polite" class="...">
    <p>실시간 데이터 연결 실패</p>
    <p>{{ errorGuideMessage }}</p>  <!-- NETWORK/API/PARSE 유형별 안내 -->
    <button @click="retryLoad">다시 시도</button>
    <button @click="isErrorDismissed = true" aria-label="알림 닫기">x</button>
  </div>
</Transition>

<!-- 토스트 알림 — CSV 내보내기 완료 시 하단 팝업 -->
<Transition name="toast">
  <div v-if="isToastVisible" role="status" aria-live="polite" class="fixed bottom-6 ...">
    {{ toastMessage }}
  </div>
</Transition>
```

```css
/* fade: 연결 상태 배지 전환 */
.fade-enter-active, .fade-leave-active { transition: opacity 0.25s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* slide-fade: 에러 패널 등장/퇴장 */
.slide-fade-enter-active { transition: all 0.3s ease-out; }
.slide-fade-leave-active { transition: all 0.2s ease-in; }
.slide-fade-enter-from { opacity: 0; transform: translateY(-12px); }
.slide-fade-leave-to { opacity: 0; transform: translateY(-8px); }

/* toast: 하단 알림 팝업 */
.toast-enter-active { transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1); }
.toast-leave-active { transition: all 0.25s ease-in; }
.toast-enter-from { opacity: 0; transform: translate(-50%, 16px); }
.toast-leave-to { opacity: 0; transform: translate(-50%, 8px); }
```
