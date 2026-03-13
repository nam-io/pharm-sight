using Microsoft.AspNetCore.Mvc;

namespace PharmSight.Api.Controllers;

/// <summary>
/// 서버 상태 확인 컨트롤러.
/// Render 무료 플랜의 15분 비활성 슬립을 방지하기 위해 주기적으로 호출됩니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>서버 생존 여부 및 현재 시각 반환 (Keep-Alive 핑 용도)</summary>
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("헬스체크 호출됨: {Time}", DateTime.UtcNow);
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            message = "PharmSight API 서버가 정상 동작 중입니다."
        });
    }
}
