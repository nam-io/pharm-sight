using PharmSight.Api.Models;

namespace PharmSight.Api.Services.Interfaces;

/// <summary>Claude AI API를 활용한 약국 경영 인사이트 생성 서비스 인터페이스</summary>
public interface IAiInsightService
{
    /// <summary>현재 약국 데이터를 기반으로 AI 경영 인사이트를 반환합니다.</summary>
    Task<AiInsight> GetInsightAsync();
}
