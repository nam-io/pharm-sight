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

    // ─────────────────────────────────────────────────────────
    // 월별 매출
    // ─────────────────────────────────────────────────────────

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
        // Arrange
        _repositoryMock.Setup(r => r.GetMonthlySalesAsync())
                       .ReturnsAsync(Enumerable.Empty<MonthlySales>());

        // Act
        var result = await _service.GetMonthlySalesAsync();

        // Assert
        Assert.Empty(result);
    }

    // ─────────────────────────────────────────────────────────
    // 약품 유형별 매출
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다()
    {
        // Arrange
        var expected = new List<DrugTypeSales>
        {
            new("ETC", "전문의약품 (ETC)", 3_800_000m),
            new("OTC", "일반의약품 (OTC)", 1_400_000m),
        };
        _repositoryMock.Setup(r => r.GetDrugTypeSalesAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetDrugTypeSalesAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Type == "ETC");
        Assert.Contains(result, d => d.Type == "OTC");
        _repositoryMock.Verify(r => r.GetDrugTypeSalesAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // 환자 연령대 분포
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPatientAgeGroupsAsync_연령대_데이터가_반환된다()
    {
        // Arrange
        var expected = new List<PatientAgeGroup>
        {
            new("20-29세", 15L),
            new("30-39세", 22L),
            new("60-69세", 38L),
        };
        _repositoryMock.Setup(r => r.GetPatientAgeGroupsAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetPatientAgeGroupsAsync()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(38L, result.First(a => a.AgeGroup == "60-69세").Count);
        _repositoryMock.Verify(r => r.GetPatientAgeGroupsAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // 의료기관별 처방전
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다()
    {
        // Arrange
        var expected = new List<HospitalPrescription>
        {
            new("연세내과의원", 85L),
            new("푸른하늘소아과", 72L),
            new("행복정형외과", 60L),
        };
        _repositoryMock.Setup(r => r.GetHospitalPrescriptionsAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetHospitalPrescriptionsAsync()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("연세내과의원", result[0].HospitalName);
        _repositoryMock.Verify(r => r.GetHospitalPrescriptionsAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // 도매상별 지출
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetWholesaleExpensesAsync_도매상별_지출이_반환된다()
    {
        // Arrange
        var expected = new List<WholesaleExpense>
        {
            new("지오영", 2_100_000m),
            new("백제약품", 1_850_000m),
        };
        _repositoryMock.Setup(r => r.GetWholesaleExpensesAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetWholesaleExpensesAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2_100_000m, result[0].Amount);
        _repositoryMock.Verify(r => r.GetWholesaleExpensesAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // 급여/비급여 지출
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다()
    {
        // Arrange
        var expected = new List<DrugCoverage>
        {
            new("급여 의약품", 3_200_000m),
            new("비급여 의약품", 750_000m),
        };
        _repositoryMock.Setup(r => r.GetDrugCoverageAsync()).ReturnsAsync(expected);

        // Act
        var result = (await _service.GetDrugCoverageAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Label == "급여 의약품");
        _repositoryMock.Verify(r => r.GetDrugCoverageAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // KPI 요약
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetKpiSummaryAsync_KPI_요약이_반환된다()
    {
        // Arrange
        var expected = new KpiSummary(
            CurrentMonthSales: 5_500_000m,
            CurrentMonthPrescriptions: 140L,
            CurrentMonthPatients: 95L,
            CurrentMonthOrderAmount: 2_100_000m,
            SalesChangeRate: 8.3m,
            PrescriptionChangeRate: 5.1m
        );
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);

        // Act
        var result = await _service.GetKpiSummaryAsync();

        // Assert
        Assert.Equal(5_500_000m, result.CurrentMonthSales);
        Assert.Equal(140L, result.CurrentMonthPrescriptions);
        Assert.Equal(8.3m, result.SalesChangeRate);
        _repositoryMock.Verify(r => r.GetKpiSummaryAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다()
    {
        // Arrange — 전월 매출 0 → 변화율 0 시나리오
        var expected = new KpiSummary(
            CurrentMonthSales: 3_200_000m,
            CurrentMonthPrescriptions: 80L,
            CurrentMonthPatients: 60L,
            CurrentMonthOrderAmount: 1_500_000m,
            SalesChangeRate: 0m,
            PrescriptionChangeRate: 0m
        );
        _repositoryMock.Setup(r => r.GetKpiSummaryAsync()).ReturnsAsync(expected);

        // Act
        var result = await _service.GetKpiSummaryAsync();

        // Assert
        Assert.Equal(0m, result.SalesChangeRate);
        Assert.Equal(0m, result.PrescriptionChangeRate);
    }
}
