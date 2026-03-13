using Microsoft.AspNetCore.Mvc;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Api.Controllers;

/// <summary>
/// Claude AI 기반 약국 경영 인사이트 컨트롤러.
/// 실시간 대시보드 데이터를 분석하여 AI 경영 요약을 반환합니다.
/// </summary>
[ApiController]
[Route("api/ai")]
public class AiInsightController : ControllerBase
{
    private readonly IAiInsightService _aiInsightService;
    private readonly ILogger<AiInsightController> _logger;

    public AiInsightController(IAiInsightService aiInsightService, ILogger<AiInsightController> logger)
    {
        _aiInsightService = aiInsightService;
        _logger = logger;
    }

    /// <summary>약국 경영 데이터를 AI가 분석한 인사이트를 반환합니다.</summary>
    [HttpGet("insight")]
    public async Task<IActionResult> GetInsight()
    {
        _logger.LogInformation("AI 인사이트 요청 수신");
        var insight = await _aiInsightService.GetInsightAsync();
        return Ok(insight);
    }
}
