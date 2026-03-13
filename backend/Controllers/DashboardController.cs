using Microsoft.AspNetCore.Mvc;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Api.Controllers;

/// <summary>
/// 약국 경영 대시보드 API 컨트롤러.
/// 6개 차트 패널 및 KPI 요약 데이터를 제공합니다.
///
/// [아키텍처 패턴: Thin Controller]
/// 이 컨트롤러는 HTTP 요청/응답과 라우팅만 담당합니다.
/// 비즈니스 로직은 IDashboardService에 완전히 위임하며,
/// 예외 처리는 GlobalExceptionMiddleware가 전역으로 처리합니다.
/// 컨트롤러 메서드에 try-catch가 없는 것은 의도적 설계입니다.
///
/// [의존성 주입]
/// IDashboardService: Program.cs에서 AddScoped로 등록된 DashboardService 주입.
///   인터페이스를 통한 주입으로 xUnit 테스트에서 Mock 교체가 용이합니다.
/// ILogger: ASP.NET Core 내장 로깅 프레임워크 — 구조화된 로그 출력.
///
/// [응답 형식]
/// 성공: HTTP 200 + JSON 배열/객체
/// 오류: GlobalExceptionMiddleware → HTTP 400/500 + { "error": "...", "statusCode": N }
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// Constructor Injection — IDashboardService, ILogger를 DI 컨테이너에서 주입받습니다.
    /// </summary>
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

    /// <summary>Rx/OTC 약품 유형별 매출 비중 조회</summary>
    [HttpGet("drug-type-sales")]
    public async Task<IActionResult> GetDrugTypeSales()
    {
        var data = await _service.GetDrugTypeSalesAsync();
        return Ok(data);
    }

    /// <summary>방문 환자 연령대 분포 조회</summary>
    [HttpGet("patient-ages")]
    public async Task<IActionResult> GetPatientAges()
    {
        var data = await _service.GetPatientAgeGroupsAsync();
        return Ok(data);
    }

    /// <summary>처방전 발행 의료기관별 유입 건수 TOP 6 조회</summary>
    [HttpGet("hospital-prescriptions")]
    public async Task<IActionResult> GetHospitalPrescriptions()
    {
        var data = await _service.GetHospitalPrescriptionsAsync();
        return Ok(data);
    }

    /// <summary>도매상별 누적 발주 지출 현황 조회</summary>
    [HttpGet("wholesale-expenses")]
    public async Task<IActionResult> GetWholesaleExpenses()
    {
        var data = await _service.GetWholesaleExpensesAsync();
        return Ok(data);
    }

    /// <summary>급여/비급여 의약품 지출 비율 조회</summary>
    [HttpGet("drug-coverage")]
    public async Task<IActionResult> GetDrugCoverage()
    {
        var data = await _service.GetDrugCoverageAsync();
        return Ok(data);
    }

    /// <summary>이번 달 KPI 요약 조회</summary>
    [HttpGet("kpi")]
    public async Task<IActionResult> GetKpi()
    {
        var data = await _service.GetKpiSummaryAsync();
        return Ok(data);
    }
}
