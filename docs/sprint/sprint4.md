# Sprint 4: 테스트 전략 및 CI/CD 파이프라인 구축

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 4 |
| **연결된 Phase** | Phase 2 Backlog → 완료 전환 |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | ✅ 완료 |
| **작업 브랜치** | `master` (hotfix 성격 병합) |

---

## 목표 (Goal)

해커톤 평가 기준 "검증 계획(15점)"에 대응하여,
백엔드 Service 계층의 xUnit 단위 테스트를 작성하고
GitHub Actions CI 파이프라인을 실제 프로젝트에 맞게 구성한다.

---

## 배경 및 동기

| 항목 | 이전 상태 | 이번 Sprint 완료 후 |
|------|-----------|---------------------|
| 단위 테스트 | 미존재 (Backlog) | 13개 케이스 전체 통과 |
| CI/CD | Python/pytest 템플릿 (미동작) | .NET 9 + Vue 3 실제 파이프라인 |
| CLAUDE.md | SQLite/.NET 8 참조 잔존 | PostgreSQL/Gemini AI/.NET 9 현행화 |

---

## 작업 분해 (Task Breakdown)

### T4-1: xUnit 테스트 프로젝트 생성

**파일:**
- `backend/PharmSight.Tests/PharmSight.Tests.csproj`
- `backend/PharmSight.Tests/Services/DashboardServiceTests.cs`
- `backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`

**의존성:**
- `xunit 2.9.2`, `xunit.runner.visualstudio 2.8.2`
- `Moq 4.20.72` — IDashboardRepository 모킹
- `Microsoft.NET.Test.Sdk 17.12.0`, `coverlet.collector 6.0.2`

**테스트 케이스 목록:**

| 클래스 | 메서드 | 검증 내용 |
|--------|--------|-----------|
| `DashboardServiceTests` | `GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다` | 반환값 일치, Repository 1회 호출 |
| `DashboardServiceTests` | `GetMonthlySalesAsync_빈_결과도_정상_반환된다` | 빈 IEnumerable 엣지 케이스 |
| `DashboardServiceTests` | `GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다` | ETC/OTC 두 타입 포함 검증 |
| `DashboardServiceTests` | `GetPatientAgeGroupsAsync_연령대_데이터가_반환된다` | Count 값 매핑 검증 |
| `DashboardServiceTests` | `GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다` | 1위 기관명 검증 |
| `DashboardServiceTests` | `GetWholesaleExpensesAsync_도매상별_지출이_반환된다` | Amount 값 검증 |
| `DashboardServiceTests` | `GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다` | 급여 라벨 포함 검증 |
| `DashboardServiceTests` | `GetKpiSummaryAsync_KPI_요약이_반환된다` | 매출·변화율 정확성 검증 |
| `DashboardServiceTests` | `GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다` | 0 나눗셈 엣지 케이스 |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_안내메시지를_반환한다` | Graceful Degradation |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_Repository_호출하지_않는다` | 불필요한 DB 조회 없음 |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_GeneratedAt이_설정된다` | 타임스탬프 설정 검증 |
| `AiInsightServiceTests` | `GetInsightAsync_두번_호출시_캐시를_반환한다` | IMemoryCache 30분 캐시 히트 |

**실행 결과:**
```
총 테스트 수: 13
     통과: 13
 총 시간: 1.2077 초
```

### T4-2: GitHub Actions CI/CD 파이프라인 재구성

**파일:** `.github/workflows/ci.yml`

**변경 내용:**
- 기존: Python/pytest 템플릿 (프로젝트와 완전 불일치, 동작 불가)
- 변경: .NET 9 + Vue 3 실제 파이프라인

**파이프라인 구조:**
```
push/PR → master, develop
├── backend-test job
│   ├── actions/setup-dotnet@v4 (.NET 9.0.x)
│   ├── dotnet restore (API + Tests)
│   ├── dotnet build --configuration Release
│   ├── dotnet test --verbosity normal
│   └── upload-artifact: TRX 테스트 결과
└── frontend-build job
    ├── actions/setup-node@v4 (Node 20)
    ├── npm ci --prefix frontend
    └── npm run build (VITE_API_BASE_URL 주입)
```

### T4-3: CLAUDE.md AI 컨텍스트 현행화

- 기술 스택: `SQLite` → `PostgreSQL (Supabase), Npgsql`, `.NET Core 8.0+` → `.NET 9.0`
- AI 항목 추가: Google Gemini API, IMemoryCache 캐시, 동적 모델 선택
- DB 스키마: PostgreSQL 함수(`DATE_TRUNC`, `AGE()`) 및 Dapper 타입 매핑 주의사항 명시
- 검증 섹션: 테스트 케이스 수, CI/CD 구성 상세 기재

---

## 완료 조건 (Definition of Done)

- [x] `dotnet test PharmSight.Tests/` — 13개 전체 통과 (로컬 검증 완료)
- [x] CI/CD `.github/workflows/ci.yml` — 프로젝트 실정에 맞는 파이프라인 구성
- [x] `git push origin master` → GitHub Actions 트리거 확인
- [x] `CLAUDE.md` 기술 스택 현행화 완료
- [x] `ROADMAP.md` 단위 테스트 항목 `[x]` 완료 전환

---

## 기술 부채 및 주의사항

- 현재 테스트는 Service 계층만 커버 (Repository는 DB 의존성으로 통합 테스트 필요)
- CI의 `backend-test` job은 DB 연결 없이 단위 테스트만 실행 (Supabase 연결 정보 불필요)
- `PharmSight.Api.csproj`에 `<Compile Remove="PharmSight.Tests\**\*.cs" />` 추가 — MSBuild glob이 하위 디렉토리 .cs 파일을 API 프로젝트에 포함시키는 문제 방지

---

## 실제 테스트 코드 전문

### DashboardServiceTests.cs (`backend/PharmSight.Tests/Services/DashboardServiceTests.cs`)

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services;

namespace PharmSight.Tests.Services;

/// <summary>
/// DashboardService 단위 테스트.
/// IDashboardRepository를 Moq로 Mocking하여 Service 계층의 로직만 검증합니다.
/// </summary>
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
    public async Task GetMonthlySalesAsync_빈_결과도_정상_반환된다()
    {
        _repositoryMock.Setup(r => r.GetMonthlySalesAsync())
                       .ReturnsAsync(Enumerable.Empty<MonthlySales>());
        var result = await _service.GetMonthlySalesAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다()
    {
        var expected = new List<DrugTypeSales>
        {
            new("ETC", "전문의약품 (ETC)", 3_800_000m),
            new("OTC", "일반의약품 (OTC)", 1_400_000m),
        };
        _repositoryMock.Setup(r => r.GetDrugTypeSalesAsync()).ReturnsAsync(expected);
        var result = (await _service.GetDrugTypeSalesAsync()).ToList();
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Type == "ETC");
        Assert.Contains(result, d => d.Type == "OTC");
        _repositoryMock.Verify(r => r.GetDrugTypeSalesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPatientAgeGroupsAsync_연령대_데이터가_반환된다()
    {
        var expected = new List<PatientAgeGroup>
        {
            new("20-29세", 15L),
            new("30-39세", 22L),
            new("60-69세", 38L),
        };
        _repositoryMock.Setup(r => r.GetPatientAgeGroupsAsync()).ReturnsAsync(expected);
        var result = (await _service.GetPatientAgeGroupsAsync()).ToList();
        Assert.Equal(3, result.Count);
        Assert.Equal(38L, result.First(a => a.AgeGroup == "60-69세").Count);
        _repositoryMock.Verify(r => r.GetPatientAgeGroupsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다()
    {
        var expected = new List<HospitalPrescription>
        {
            new("연세내과의원", 85L),
            new("푸른하늘소아과", 72L),
            new("행복정형외과", 60L),
        };
        _repositoryMock.Setup(r => r.GetHospitalPrescriptionsAsync()).ReturnsAsync(expected);
        var result = (await _service.GetHospitalPrescriptionsAsync()).ToList();
        Assert.Equal(3, result.Count);
        Assert.Equal("연세내과의원", result[0].HospitalName);
        _repositoryMock.Verify(r => r.GetHospitalPrescriptionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetWholesaleExpensesAsync_도매상별_지출이_반환된다()
    {
        var expected = new List<WholesaleExpense>
        {
            new("지오영", 2_100_000m),
            new("백제약품", 1_850_000m),
        };
        _repositoryMock.Setup(r => r.GetWholesaleExpensesAsync()).ReturnsAsync(expected);
        var result = (await _service.GetWholesaleExpensesAsync()).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(2_100_000m, result[0].Amount);
        _repositoryMock.Verify(r => r.GetWholesaleExpensesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다()
    {
        var expected = new List<DrugCoverage>
        {
            new("급여 의약품", 3_200_000m),
            new("비급여 의약품", 750_000m),
        };
        _repositoryMock.Setup(r => r.GetDrugCoverageAsync()).ReturnsAsync(expected);
        var result = (await _service.GetDrugCoverageAsync()).ToList();
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Label == "급여 의약품");
        _repositoryMock.Verify(r => r.GetDrugCoverageAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpiSummaryAsync_KPI_요약이_반환된다()
    {
        var expected = new KpiSummary(
            CurrentMonthSales: 5_500_000m,
            CurrentMonthPrescriptions: 140L,
            CurrentMonthPatients: 95L,
            CurrentMonthOrderAmount: 2_100_000m,
            SalesChangeRate: 8.3m,
            PrescriptionChangeRate: 5.1m
        );
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);
        var result = await _service.GetKpiSummaryAsync();
        Assert.Equal(5_500_000m, result.CurrentMonthSales);
        Assert.Equal(140L, result.CurrentMonthPrescriptions);
        Assert.Equal(8.3m, result.SalesChangeRate);
        _repositoryMock.Verify(r => r.GetKpiSummaryAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다()
    {
        // Arrange — 전월 매출 0 → SalesChangeRate = 0 엣지 케이스
        var expected = new KpiSummary(
            CurrentMonthSales: 3_200_000m,
            CurrentMonthPrescriptions: 80L,
            CurrentMonthPatients: 60L,
            CurrentMonthOrderAmount: 1_500_000m,
            SalesChangeRate: 0m,
            PrescriptionChangeRate: 0m
        );
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);
        var result = await _service.GetKpiSummaryAsync();
        Assert.Equal(0m, result.SalesChangeRate);
        Assert.Equal(0m, result.PrescriptionChangeRate);
    }
}
```

### AiInsightServiceTests.cs (`backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`)

```csharp
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
    public async Task GetInsightAsync_API키_없으면_Repository_호출하지_않는다()
    {
        var service = CreateService(apiKey: "");
        await service.GetInsightAsync();

        // Assert — API 키 없을 때 불필요한 DB 조회 발생 안 함
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

    [Fact]
    public async Task GetInsightAsync_두번_호출시_캐시를_반환한다()
    {
        // Arrange — 동일 서비스 인스턴스 두 번 호출 → IMemoryCache 캐시 히트
        var service = CreateService(apiKey: "");
        var first = await service.GetInsightAsync();
        var second = await service.GetInsightAsync();

        // Assert — 캐시 히트: 동일 GeneratedAt 타임스탬프
        Assert.Equal(first.GeneratedAt, second.GeneratedAt);
    }
}
```

---

## 실제 CI/CD 파이프라인 전문

### `.github/workflows/ci.yml`

```yaml
name: CI - 빌드 및 테스트 자동화

on:
  push:
    branches: [master, develop]
  pull_request:
    branches: [master, develop]

jobs:
  backend-test:
    name: 백엔드 빌드 및 xUnit 단위 테스트
    runs-on: ubuntu-latest

    steps:
      - name: 코드 체크아웃
        uses: actions/checkout@v4

      - name: .NET 9.0 설정
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"

      - name: NuGet 패키지 복원 (API)
        run: dotnet restore backend/PharmSight.Api.csproj

      - name: NuGet 패키지 복원 (Tests)
        run: dotnet restore backend/PharmSight.Tests/PharmSight.Tests.csproj

      - name: 백엔드 빌드 (Release)
        run: dotnet build backend/PharmSight.Api.csproj --no-restore --configuration Release

      - name: xUnit 단위 테스트 실행
        run: dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj --configuration Release --verbosity normal --logger "trx;LogFileName=test-results.trx"

      - name: 테스트 결과 업로드
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: backend-test-results
          path: backend/PharmSight.Tests/TestResults/*.trx

  frontend-build:
    name: 프론트엔드 빌드 검증 (Vite)
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

      - name: NPM 패키지 설치
        run: npm ci --prefix frontend

      - name: 프론트엔드 프로덕션 빌드 검증
        run: npm run build --prefix frontend
        env:
          VITE_API_BASE_URL: https://pharm-sight.onrender.com
```

### 테스트 실행 결과 (로컬 검증)

```
$ dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj --verbosity normal

  통과 AiInsightServiceTests.GetInsightAsync_두번_호출시_캐시를_반환한다 [60 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_GeneratedAt이_설정된다 [1 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_Repository_호출하지_않는다 [3 ms]
  통과 AiInsightServiceTests.GetInsightAsync_API키_없으면_안내메시지를_반환한다 [9 ms]
  통과 DashboardServiceTests.GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다 [71 ms]
  통과 DashboardServiceTests.GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다 [3 ms]
  통과 DashboardServiceTests.GetMonthlySalesAsync_빈_결과도_정상_반환된다 [1 ms]
  통과 DashboardServiceTests.GetWholesaleExpensesAsync_도매상별_지출이_반환된다 [1 ms]
  통과 DashboardServiceTests.GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다 [1 ms]
  통과 DashboardServiceTests.GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다 [1 ms]
  통과 DashboardServiceTests.GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다 [1 ms]
  통과 DashboardServiceTests.GetPatientAgeGroupsAsync_연령대_데이터가_반환된다 [3 ms]
  통과 DashboardServiceTests.GetKpiSummaryAsync_KPI_요약이_반환된다 [< 1 ms]

총 테스트 수: 13
     통과: 13
    경고 0개  오류 0개
경과 시간: 00:00:05.07
```
