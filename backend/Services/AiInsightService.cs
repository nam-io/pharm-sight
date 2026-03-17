using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Api.Services;

/// <summary>
/// Claude AI API를 호출하여 약국 경영 인사이트를 생성하는 서비스.
/// 응답은 30분간 메모리 캐시에 저장되어 API 비용을 절감합니다.
/// </summary>
public class AiInsightService : IAiInsightService
{
    private readonly IDashboardRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AiInsightService> _logger;
    private readonly string _apiKey;

    private const string CacheKey = "ai_insight";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public AiInsightService(
        IDashboardRepository repository,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<AiInsightService> logger,
        IConfiguration configuration)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
    }

    /// <summary>캐시된 인사이트를 반환하거나 새로 생성합니다.</summary>
    public async Task<AiInsight> GetInsightAsync()
    {
        if (_cache.TryGetValue(CacheKey, out AiInsight? cached) && cached is not null)
        {
            _logger.LogInformation("AI 인사이트 캐시 히트: {Time}", cached.GeneratedAt);
            return cached;
        }

        var insight = await GenerateInsightAsync();
        _cache.Set(CacheKey, insight, CacheDuration);
        return insight;
    }

    /// <summary>대시보드 데이터를 수집하고 Claude API를 호출하여 인사이트를 생성합니다.</summary>
    private async Task<AiInsight> GenerateInsightAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Gemini API 키가 설정되지 않았습니다. 기본 인사이트를 반환합니다.");
            return new AiInsight(
                "AI 경영 분석 기능을 사용하려면 Gemini API 키 설정이 필요합니다.",
                ["대시보드 데이터 정상 수집 중"],
                [],
                "Render 환경변수에 Gemini__ApiKey를 등록하면 AI 분석이 활성화됩니다.",
                DateTime.UtcNow
            );
        }

        try
        {
            var kpi = await _repository.GetKpiSummaryAsync();
            var monthly = (await _repository.GetMonthlySalesAsync()).ToList();
            var drugType = (await _repository.GetDrugTypeSalesAsync()).ToList();
            var hospitals = (await _repository.GetHospitalPrescriptionsAsync()).ToList();

            var prompt = BuildPrompt(kpi, monthly, drugType, hospitals);
            var responseText = await CallGeminiAsync(prompt);
            var insight = ParseInsight(responseText);

            _logger.LogInformation("AI 인사이트 생성 완료");
            return insight;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 인사이트 생성 중 오류 발생");
            return new AiInsight(
                $"[진단] {ex.GetType().Name}: {ex.Message}",
                [],
                [],
                ex.InnerException?.Message ?? "InnerException 없음",
                DateTime.UtcNow
            );
        }
    }

    /// <summary>약국 데이터를 Claude에게 전달할 프롬프트를 생성합니다.</summary>
    private static string BuildPrompt(
        KpiSummary kpi,
        List<MonthlySales> monthly,
        List<DrugTypeSales> drugType,
        List<HospitalPrescription> hospitals)
    {
        var recentMonths = string.Join(" / ", monthly.TakeLast(3)
            .Select(m => $"{m.Month} {m.TotalAmount / 10000:F0}만원({m.PrescriptionCount}건)"));
        var topHospitals = string.Join(", ", hospitals.Take(3)
            .Select(h => $"{h.HospitalName}({h.Count}건)"));
        var rxSales = drugType.FirstOrDefault(d => d.Type == "ETC")?.Amount ?? 0;
        var otcSales = drugType.FirstOrDefault(d => d.Type == "OTC")?.Amount ?? 0;
        var rxRatio = rxSales + otcSales > 0
            ? (double)rxSales / (double)(rxSales + otcSales) * 100 : 0;

        var jsonSchema =
            "{\n" +
            "  \"summary\": \"약사에게 전달하는 2~3문장의 이번 달 경영 현황 종합 요약 (친근하고 전문적으로)\",\n" +
            "  \"highlights\": [\"긍정적인 주목 포인트 2~3개 (각 15자 이내로 간결하게)\"],\n" +
            "  \"warnings\": [\"주의가 필요한 사항 1~2개 (각 15자 이내, 없으면 빈 배열 [])\"],\n" +
            "  \"recommendation\": \"약사에게 드리는 실용적인 경영 조언 1~2문장\"\n" +
            "}";

        return
            "당신은 약국 경영 전문 AI 어시스턴트 'PharmSight AI'입니다.\n" +
            "다음 약국 운영 데이터를 분석하여 약사에게 친절하고 전문적인 한국어로 경영 인사이트를 제공해주세요.\n\n" +
            "=== 이번 달 핵심 지표 ===\n" +
            $"- 총 매출: {kpi.CurrentMonthSales / 10000:F0}만원 (전월 대비 {kpi.SalesChangeRate:F1}%)\n" +
            $"- 조제 건수: {kpi.CurrentMonthPrescriptions}건 (전월 대비 {kpi.PrescriptionChangeRate:F1}%)\n" +
            $"- 방문 환자: {kpi.CurrentMonthPatients}명\n" +
            $"- 발주 지출: {kpi.CurrentMonthOrderAmount / 10000:F0}만원\n\n" +
            "=== 최근 3개월 매출 추이 ===\n" +
            $"{recentMonths}\n\n" +
            "=== 약품 유형별 매출 ===\n" +
            $"- 전문의약품(ETC): {rxSales / 10000:F0}만원 ({rxRatio:F0}%)\n" +
            $"- 일반의약품(OTC): {otcSales / 10000:F0}만원 ({100 - rxRatio:F0}%)\n\n" +
            "=== 주요 처방 의료기관 TOP 3 ===\n" +
            $"{topHospitals}\n\n" +
            "다음 JSON 형식으로만 응답해주세요 (마크다운 코드블록 없이 순수 JSON만):\n" +
            jsonSchema;
    }

    /// <summary>
    /// 계정에서 실제 사용 가능한 Gemini 모델명을 동적으로 조회합니다.
    /// 2.5 계열 제외, 1.5-flash 계열 우선 선택. 결과는 1시간 캐시합니다.
    /// </summary>
    private async Task<string> ResolveModelNameAsync(HashSet<string>? excludeModels = null)
    {
        const string modelCacheKey = "gemini_model_name";
        bool useCache = excludeModels == null || excludeModels.Count == 0;

        if (useCache && _cache.TryGetValue(modelCacheKey, out string? cached) && cached is not null)
            return cached;

        try
        {
            var client = _httpClientFactory.CreateClient("Gemini");
            var res = await client.GetAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}");
            var body = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                var flashCandidates = new List<string>();

                foreach (var model in doc.RootElement.GetProperty("models").EnumerateArray())
                {
                    var name = model.GetProperty("name").GetString() ?? "";
                    if (!model.TryGetProperty("supportedGenerationMethods", out var methods)) continue;
                    if (!methods.EnumerateArray().Any(m => m.GetString() == "generateContent")) continue;

                    var shortName = name.Replace("models/", "");
                    // 1.5 계열만 허용 (2.0/2.5 무료 할당량 0 또는 매우 낮음)
                    if (shortName.Contains("flash") && shortName.Contains("1.5"))
                        flashCandidates.Add(shortName);
                }

                // 제외 모델 필터링
                if (excludeModels?.Count > 0)
                    flashCandidates.RemoveAll(m => excludeModels.Contains(m));

                // 1.5-flash 계열 중 선택, 없으면 안전 기본값
                var chosen = flashCandidates.FirstOrDefault()
                    ?? "gemini-1.5-flash-latest";

                _logger.LogInformation("Gemini 사용 모델 선택: {Model}", chosen);
                if (useCache)
                    _cache.Set(modelCacheKey, chosen, TimeSpan.FromHours(1));
                return chosen;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gemini 모델 목록 조회 실패, 기본값 사용");
        }

        return "gemini-1.5-flash-latest";
    }

    /// <summary>Google Gemini API를 호출하고 텍스트 응답을 반환합니다.</summary>
    private async Task<string> CallGeminiAsync(string prompt)
    {
        var model = await ResolveModelNameAsync();
        var client = _httpClientFactory.CreateClient("Gemini");
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var response = await client.PostAsJsonAsync(url, requestBody);
        var responseBody = await response.Content.ReadAsStringAsync();

        if ((int)response.StatusCode == 429)
        {
            // 할당량 초과 → 현재 모델 제외 후 ListModels로 다음 후보 선택해 재시도
            _logger.LogWarning("Gemini 429 할당량 초과 (model={Model}), 다른 모델로 재시도", model);
            _cache.Remove("gemini_model_name");
            var fallbackModel = await ResolveModelNameAsync(excludeModels: [model]);
            var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{fallbackModel}:generateContent?key={_apiKey}";
            response = await client.PostAsJsonAsync(fallbackUrl, requestBody);
            responseBody = await response.Content.ReadAsStringAsync();
            model = fallbackModel;
        }

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Gemini API {(int)response.StatusCode} (model={model}): {responseBody}");

        using var result = JsonDocument.Parse(responseBody);
        return result.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";
    }

    /// <summary>Claude 응답 텍스트를 AiInsight 모델로 파싱합니다.</summary>
    private static AiInsight ParseInsight(string responseText)
    {
        // 마크다운 코드 펜스 제거
        var json = responseText.Trim();
        if (json.StartsWith("```"))
        {
            json = json.TrimStart('`');
            if (json.StartsWith("json")) json = json[4..];
            json = json.Trim().TrimEnd('`').Trim();
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var highlights = root.GetProperty("highlights").EnumerateArray()
            .Select(e => e.GetString() ?? "").Where(s => s.Length > 0).ToList();
        var warnings = root.GetProperty("warnings").EnumerateArray()
            .Select(e => e.GetString() ?? "").Where(s => s.Length > 0).ToList();

        return new AiInsight(
            Summary: root.GetProperty("summary").GetString() ?? "",
            Highlights: highlights,
            Warnings: warnings,
            Recommendation: root.GetProperty("recommendation").GetString() ?? "",
            GeneratedAt: DateTime.UtcNow
        );
    }
}
