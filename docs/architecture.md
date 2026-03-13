# 백엔드 아키텍처 및 코드 품질 상세

> **주요 파일 경로:** `backend/` 디렉토리
> **패턴:** Controller → Service → Repository (SRP/SoC)
> **DI:** 인터페이스 기반 의존성 주입 (`Program.cs`)

---

## 1. 전체 아키텍처 다이어그램

```
HTTP 요청
    │
    ▼
GlobalExceptionMiddleware   ← 전역 예외 처리, 일관된 JSON 오류 응답
    │
    ▼
DashboardController         ← 요청/응답만 담당 (비즈니스 로직 없음)
AiInsightController
HealthController
    │ IDashboardService / IAiInsightService (인터페이스)
    ▼
DashboardService            ← 비즈니스 로직, 로깅
AiInsightService            ← Gemini API 호출, IMemoryCache(30분 캐시)
    │ IDashboardRepository (인터페이스)
    ▼
DashboardRepository         ← Dapper + Npgsql 순수 SQL
    │
    ▼
Supabase PostgreSQL          ← 클라우드 RDB (DATE_TRUNC, CTE, AGE())
```

---

## 2. 전역 예외 처리 미들웨어 (`GlobalExceptionMiddleware.cs`)

```csharp
// backend/Middleware/GlobalExceptionMiddleware.cs

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
            ArgumentNullException     => (int)HttpStatusCode.BadRequest,   // 400
            InvalidOperationException => (int)HttpStatusCode.BadRequest,   // 400
            _                         => (int)HttpStatusCode.InternalServerError, // 500
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
```

**오류 응답 형식 (일관된 JSON):**

```json
// ArgumentNullException / InvalidOperationException → 400
{ "error": "데이터베이스 연결 문자열이 설정되지 않았습니다.", "statusCode": 400 }

// 그 외 처리되지 않은 예외 → 500
{ "error": "예상치 못한 오류가 발생했습니다.", "statusCode": 500 }
```

---

## 3. DI 등록 (`Program.cs`)

```csharp
// backend/Program.cs

// ── DI 등록: Repository / Service ────────────────────────────────────────
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAiInsightService, AiInsightService>();

// ── AI 인사이트: 메모리 캐시 + Named HttpClient ────────────────────────
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("Gemini", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ── CORS: Vercel 프론트엔드 + 로컬 개발 허용 ───────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins").Get<string[]>()
    ?? ["https://pharm-sight-frontend.vercel.app"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── 미들웨어 파이프라인 ───────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();  // 전역 예외 처리 (최선두)
app.UseCors("ViteFrontend");
app.UseAuthorization();
app.MapControllers();
```

---

## 4. Controller 계층 (`DashboardController.cs`)

```csharp
// backend/Controllers/DashboardController.cs

/// <summary>
/// 약국 경영 대시보드 API 컨트롤러.
/// 6개 차트 패널 및 KPI 요약 데이터를 제공합니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;  // 인터페이스 주입
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>최근 12개월 월별 매출 및 조제 건수 조회</summary>
    [HttpGet("monthly-sales")]
    public async Task<IActionResult> GetMonthlySales()
    {
        var data = await _service.GetMonthlySalesAsync();
        return Ok(data);
    }

    // ... GetDrugTypeSales / GetPatientAges / GetHospitalPrescriptions
    //     GetWholesaleExpenses / GetDrugCoverage / GetKpi 동일 패턴

    /// <summary>이번 달 KPI 요약 조회</summary>
    [HttpGet("kpi")]
    public async Task<IActionResult> GetKpi()
    {
        var data = await _service.GetKpiSummaryAsync();
        return Ok(data);
    }
}
```

**Controller 설계 원칙:**
- HTTP 요청 수신 / 응답 반환 역할만 수행 → 비즈니스 로직 없음
- `IDashboardService` 인터페이스를 통해 Service 주입 → 결합도 최소화
- 모든 예외는 `GlobalExceptionMiddleware`가 처리 → Controller에 try-catch 없음

---

## 5. Service 계층 (`DashboardService.cs`)

```csharp
// backend/Services/DashboardService.cs

/// <summary>
/// 대시보드 비즈니스 로직 서비스 구현체.
/// Repository를 통해 데이터를 조회하고 필요한 가공 처리를 수행합니다.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;  // 인터페이스 주입
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IDashboardRepository repository, ILogger<DashboardService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>최근 12개월 월별 매출 및 조제 건수 반환</summary>
    public async Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync()
    {
        _logger.LogInformation("월별 매출 데이터 조회 시작");
        return await _repository.GetMonthlySalesAsync();
    }

    /// <summary>KPI 요약 반환</summary>
    public async Task<KpiSummary> GetKpiSummaryAsync()
    {
        _logger.LogInformation("KPI 요약 데이터 조회 시작");
        return await _repository.GetKpiSummaryAsync();
    }

    // ... 나머지 6개 메서드 동일 패턴
}
```

---

## 6. Repository 계층 — Dapper + PostgreSQL 고급 쿼리

### 6-1. KPI 요약 (CTE 3중 조인)

```csharp
// backend/Repositories/DashboardRepository.cs

/// <summary>이번 달 및 전월 대비 KPI 요약 집계</summary>
public async Task<KpiSummary> GetKpiSummaryAsync()
{
    const string sql = """
        WITH current_month AS (
            SELECT
                COALESCE(SUM(s."Amount"), 0)  AS sales,
                COUNT(DISTINCT pr."Id")        AS prescriptions,
                COUNT(DISTINCT pr."PatientId") AS patients
            FROM "Sales" s
            LEFT JOIN "Prescriptions" pr ON s."PrescriptionId" = pr."Id"
            WHERE DATE_TRUNC('month', s."SaleDate"::date) = DATE_TRUNC('month', CURRENT_DATE)
        ),
        prev_month AS (
            SELECT
                COALESCE(SUM(s."Amount"), 0) AS sales,
                COUNT(DISTINCT pr."Id")       AS prescriptions
            FROM "Sales" s
            LEFT JOIN "Prescriptions" pr ON s."PrescriptionId" = pr."Id"
            WHERE DATE_TRUNC('month', s."SaleDate"::date) =
                  DATE_TRUNC('month', CURRENT_DATE - INTERVAL '1 month')
        ),
        current_orders AS (
            SELECT COALESCE(SUM(o."Amount"), 0) AS amount
            FROM "Orders" o
            WHERE DATE_TRUNC('month', o."OrderDate"::date) = DATE_TRUNC('month', CURRENT_DATE)
        )
        SELECT
            c.sales            AS "CurrentMonthSales",
            c.prescriptions    AS "CurrentMonthPrescriptions",
            c.patients         AS "CurrentMonthPatients",
            co.amount          AS "CurrentMonthOrderAmount",
            CASE WHEN p.sales = 0 THEN 0
                 ELSE ROUND(((c.sales - p.sales) / p.sales * 100)::numeric, 1)
            END                AS "SalesChangeRate",
            CASE WHEN p.prescriptions = 0 THEN 0
                 ELSE ROUND(((c.prescriptions - p.prescriptions)::numeric
                             / p.prescriptions * 100)::numeric, 1)
            END                AS "PrescriptionChangeRate"
        FROM current_month c, prev_month p, current_orders co;
        """;

    await using var conn = new NpgsqlConnection(_connectionString);
    return await conn.QuerySingleAsync<KpiSummary>(sql);
}
```

### 6-2. 환자 연령대 분포 (`DATE_PART` + `AGE()` 활용)

```csharp
const string sql = """
    SELECT
        CASE
            WHEN age < 10  THEN '0-9세'
            WHEN age < 20  THEN '10-19세'
            WHEN age < 30  THEN '20-29세'
            WHEN age < 40  THEN '30-39세'
            WHEN age < 50  THEN '40-49세'
            WHEN age < 60  THEN '50-59세'
            WHEN age < 70  THEN '60-69세'
            ELSE                '70세 이상'
        END AS "AgeGroup",
        COUNT(*) AS "Count"
    FROM (
        SELECT DISTINCT p."Id",
            DATE_PART('year', AGE(p."DateOfBirth"::date)) AS age
        FROM "Patients" p
        JOIN "Prescriptions" pr ON pr."PatientId" = p."Id"
    ) sub
    GROUP BY "AgeGroup"
    ORDER BY MIN(age);
    """;
```

### 6-3. PostgreSQL URI → Npgsql 연결 문자열 자동 변환

```csharp
/// <summary>
/// Render 환경변수에 주입되는 postgresql:// URI 형식을
/// Npgsql 키=값 형식으로 변환합니다.
/// </summary>
private static string NormalizeConnectionString(string cs)
{
    if (!cs.StartsWith("postgresql://") && !cs.StartsWith("postgres://"))
        return cs;  // 이미 키=값 형식이면 그대로 반환

    var uri = new Uri(cs);
    var userInfo = uri.UserInfo.Split(':', 2);
    var builder = new NpgsqlConnectionStringBuilder
    {
        Host     = uri.Host,
        Port     = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
        SslMode  = SslMode.Require,  // Supabase는 SSL 필수
    };
    return builder.ConnectionString;
}
```

---

## 7. Dapper 선택 근거와 성능 이점

### EF Core를 사용하지 않는 이유

**대시보드 집계 쿼리 특성:**
- 여러 테이블 JOIN + GROUP BY + CASE WHEN + DATE_TRUNC + CTE 복합 사용
- 결과를 DTO에 직접 매핑 (도메인 객체 불필요)
- 단방향 읽기 전용 조회 → 변경 추적(Change Tracking) 불필요

**EF Core로 동일 로직 구현 시 문제:**

```csharp
// ❌ EF Core 방식 - N+1 문제 발생 예시
var result = context.Sales
    .Include(s => s.Prescription)
        .ThenInclude(p => p.Hospital)
    .GroupBy(s => /* ... */)  // 복잡한 집계는 클라이언트 사이드로 넘어갈 수 있음
    .ToListAsync();
// → 최적화되지 않은 SQL 생성 가능, 디버깅 어려움
```

```csharp
// ✅ Dapper 방식 - 개발자가 최적 SQL 직접 제어
const string sql = """
    WITH current_month AS ( ... ),
    prev_month         AS ( ... ),
    current_orders     AS ( ... )
    SELECT ... FROM current_month c, prev_month p, current_orders co;
    """;
return await conn.QuerySingleAsync<KpiSummary>(sql);
// → 단 1회 DB 왕복으로 KPI 전체 계산 완료
```

**Dapper 성능 특성:**
- ORM 레이어 없이 ADO.NET 직접 사용 → 오버헤드 최소
- `QueryAsync<T>`: SQL 결과를 C# 타입에 직접 매핑 (Reflection 캐싱)
- KPI 쿼리: CTE 3개 → 1회 DB 왕복으로 이번 달/전월/발주금액 모두 계산

---

## 8. AiInsightService — Graceful Degradation + 캐시

```csharp
// backend/Services/AiInsightService.cs (핵심 로직)

private const string CacheKey = "ai_insight";
private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

/// <summary>캐시된 인사이트를 반환하거나 새로 생성합니다.</summary>
public async Task<AiInsight> GetInsightAsync()
{
    // 1. 캐시 히트 확인 → 30분 이내 재호출 시 즉시 반환
    if (_cache.TryGetValue(CacheKey, out AiInsight? cached) && cached is not null)
    {
        _logger.LogInformation("AI 인사이트 캐시 히트: {Time}", cached.GeneratedAt);
        return cached;
    }

    var insight = await GenerateInsightAsync();
    _cache.Set(CacheKey, insight, CacheDuration);
    return insight;
}

private async Task<AiInsight> GenerateInsightAsync()
{
    // 2. Graceful Degradation: API 키 미설정 시 예외 없이 안내 메시지 반환
    if (string.IsNullOrEmpty(_apiKey))
    {
        return new AiInsight(
            "AI 경영 분석 기능을 사용하려면 Gemini API 키 설정이 필요합니다.",
            ["대시보드 데이터 정상 수집 중"], [], "Render 환경변수에 Gemini__ApiKey 등록 필요",
            DateTime.UtcNow);
    }

    // 3. 동적 모델 선택: ListModels API로 실제 사용 가능한 모델 자동 선택 (1시간 캐시)
    var model = await ResolveModelNameAsync(); // "gemini-1.5-flash" 등

    // 4. 데이터 수집 → 프롬프트 생성 → API 호출 → 파싱
    var kpi      = await _repository.GetKpiSummaryAsync();
    var monthly  = (await _repository.GetMonthlySalesAsync()).ToList();
    var drugType = (await _repository.GetDrugTypeSalesAsync()).ToList();
    var hospitals = (await _repository.GetHospitalPrescriptionsAsync()).ToList();

    var prompt       = BuildPrompt(kpi, monthly, drugType, hospitals);
    var responseText = await CallGeminiAsync(prompt);
    return ParseInsight(responseText);
}
```

**Gemini API 동적 모델 선택 이유:**
- Gemini 모델명은 버전 업데이트 시 자주 변경됨
- `ListModels` 엔드포인트로 계정에서 실제 사용 가능한 모델을 런타임에 조회
- `gemini-*-flash` 우선 선택 (비용 효율), 없으면 `gemini-*-pro` 폴백

---

## 9. Frontend — Vue 3 Composable 패턴

```typescript
// frontend/src/composables/useDashboardData.ts

/**
 * @composable useDashboardData
 * @description 약국 경영 대시보드 데이터를 백엔드 API에서 조회하는 Vue Composable.
 * 환경변수 VITE_API_BASE_URL이 없으면 Mock 데이터로 폴백합니다.
 */
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''
const USE_MOCK = !API_BASE  // 빌드 타임 결정

export function useDashboardData() {
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const dashboardData = ref<DashboardData>({ ...MOCK_DATA })
  const kpiRaw = ref<KpiSummary | null>(null)

  // KPI 카드 데이터 computed 변환 (원 → 만원, 변화율 부호 표시)
  const kpiCards = computed<KpiCard[]>(() => {
    if (!kpiRaw.value) return MOCK_KPI_CARDS
    const k = kpiRaw.value
    return [
      { title: '이번 달 총 매출',
        value: (k.currentMonthSales / 10000).toLocaleString('ko-KR', { maximumFractionDigits: 0 }),
        unit: '만원', change: k.salesChangeRate, icon: '💰' },
      // ... 3개 KPI 카드 동일 패턴
    ]
  })

  async function loadAll() {
    if (USE_MOCK) return  // 환경변수 없으면 Mock 사용

    isLoading.value = true
    error.value = null
    try {
      // 7개 API 병렬 호출 (Promise.all)
      const [monthly, drugType, ages, hospitals, wholesale, coverage, kpi]
        = await Promise.all([
          fetchApi<DashboardData['monthlySales']>('/api/dashboard/monthly-sales'),
          fetchApi<DashboardData['drugTypeSales']>('/api/dashboard/drug-type-sales'),
          // ...
        ])
      dashboardData.value = { monthlySales: monthly, drugTypeSales: drugType, /* ... */ }
      kpiRaw.value = kpi
    } catch (e) {
      error.value = e instanceof Error ? e.message : '데이터를 불러오는 중 오류가 발생했습니다.'
      // API 실패 시 Mock 데이터 유지 → 서비스 중단 없음
    } finally {
      isLoading.value = false
    }
  }

  return { isLoading, error, dashboardData, kpiCards, loadAll }
}
```

**Composable 설계 원칙:**
- `useDashboardData` / `useAiInsight` / `useKeepAlive` 관심사 분리
- `Promise.all` 병렬 호출로 7개 API 동시 요청 → 순차 호출 대비 응답시간 ~6배 단축
- Mock 폴백으로 API 연결 없는 환경에서도 UI 개발/데모 가능
