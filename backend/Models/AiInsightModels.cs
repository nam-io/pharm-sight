namespace PharmSight.Api.Models;

/// <summary>AI가 생성한 약국 경영 인사이트 응답 모델</summary>
public record AiInsight(
    string Summary,
    List<string> Highlights,
    List<string> Warnings,
    string Recommendation,
    DateTime GeneratedAt
);
