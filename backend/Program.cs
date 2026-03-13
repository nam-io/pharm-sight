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
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["https://pharm-sight-frontend.vercel.app"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── DI 등록: Repository / Service ────────────────────────────────────────
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// ── 전역 예외 처리 미들웨어 ───────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

// ── HTTP 파이프라인 ──────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("ViteFrontend");
app.UseAuthorization();
app.MapControllers();

// ── 헬스체크 엔드포인트 ──────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

app.Run();
