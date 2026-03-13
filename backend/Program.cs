// =============================================================================
// PharmSight API — Composition Root (Program.cs)
//
// 아키텍처: Controller → Service → Repository 3계층 + 인터페이스 기반 DI
//   - Controller: HTTP 요청/응답 처리만 담당 (Thin Controller 패턴)
//   - Service:    비즈니스 로직 처리 및 Repository 조합
//   - Repository: Dapper 순수 SQL로 PostgreSQL 집계 쿼리 전담
//
// DI 수명 선택 — AddScoped 사용 이유:
//   HTTP 요청당 1개 인스턴스 생성 → NpgsqlConnection 수명을 요청 단위로 관리
//   Singleton은 DB 연결 공유로 동시성 문제 발생 위험 / Transient은 불필요한 인스턴스 생성
//
// 미들웨어 파이프라인 순서 (변경 금지 — 순서가 동작을 결정):
//   1. GlobalExceptionMiddleware — 모든 예외를 JSON 응답으로 변환 (최선두 필수)
//   2. CORS                     — 프론트엔드 도메인 허용 (Options 프리플라이트 처리)
//   3. Authorization            — 인증 토큰 검증
//   4. Controllers              — API 라우팅 및 실제 요청 처리
//
// 인터페이스 기반 DI의 이점:
//   IDashboardRepository를 Moq로 교체 → DB 없이 13개 xUnit 단위 테스트 실행 가능
// =============================================================================

using PharmSight.Api.Middleware;
using PharmSight.Api.Repositories;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services;
using PharmSight.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── 컨트롤러 및 OpenAPI ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ── CORS: Vercel 프론트엔드 허용 ──────────────────────────────────────────
// 설정 파일(appsettings.json) 또는 환경변수로 허용 Origin 주입 가능
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["https://pharm-sight-frontend.vercel.app"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── DI 등록: Repository / Service (AddScoped — 요청 단위 수명) ────────────
// IDashboardRepository → DashboardRepository: Dapper + NpgsqlConnection 집계 쿼리
// IDashboardService    → DashboardService:    비즈니스 로직 + Repository 조합
// IAiInsightService    → AiInsightService:    Gemini API 호출 + IMemoryCache 30분 캐시
// ※ 인터페이스 기반 등록으로 xUnit 테스트 시 Mock 교체 가능 (테스트 용이성 확보)
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAiInsightService, AiInsightService>();

// ── AI 인사이트: 메모리 캐시 + Named HttpClient ────────────────────────────
// IMemoryCache: AI 응답 30분 캐시 (Gemini API 할당량 절약 + 2초→10ms 응답 개선)
// Named HttpClient "Gemini": Timeout 30초 설정 (Gemini API 응답 지연 대응)
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("Gemini", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// ── 전역 예외 처리 미들웨어 (파이프라인 최선두 등록 필수) ─────────────────
// 모든 미들웨어/컨트롤러에서 발생하는 예외를 캐치하여 일관된 JSON 오류 응답 반환
// 응답 형식: { "error": "...", "statusCode": 400|500 }
app.UseMiddleware<GlobalExceptionMiddleware>();

// ── HTTP 파이프라인 ──────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("ViteFrontend");
app.UseAuthorization();
app.MapControllers();

// ── 헬스체크 엔드포인트 (Render 슬립 방지 Keep-Alive용) ──────────────────
// Render 무료 플랜은 15분 무활동 시 슬립 → /health 핑으로 유지
// 프론트엔드 useKeepAlive.ts가 10분 주기로 호출
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

app.Run();
