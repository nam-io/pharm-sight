using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PharmSight.Api.Controllers;
using PharmSight.Api.Models;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Tests.Controllers;

/// <summary>
/// DashboardController 통합 테스트.
/// Controller → Service 계층 간 의존성 주입 및 HTTP 응답 형식을 검증합니다.
/// 아키텍처 패턴(Thin Controller)이 올바르게 적용되었는지 확인합니다.
/// </summary>
public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _serviceMock;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _serviceMock = new Mock<IDashboardService>();
        var loggerMock = new Mock<ILogger<DashboardController>>();
        _controller = new DashboardController(_serviceMock.Object, loggerMock.Object);
    }

    // ─────────────────────────────────────────────────────────
    // Controller → Service 계층 연결 검증
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMonthlySales_Service를_호출하고_OkResult를_반환한다()
    {
        // Arrange
        var expected = new List<MonthlySales> { new("2026-03", 5_500_000m, 140L) };
        _serviceMock.Setup(s => s.GetMonthlySalesAsync()).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetMonthlySales();

        // Assert — Controller는 Service 결과를 그대로 OK 200으로 래핑
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expected, okResult.Value);
        _serviceMock.Verify(s => s.GetMonthlySalesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetKpi_Service를_호출하고_OkResult를_반환한다()
    {
        // Arrange
        var expected = new KpiSummary(5_500_000m, 140L, 95L, 2_100_000m, 8.3m, 5.1m);
        _serviceMock.Setup(s => s.GetKpiSummaryAsync()).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetKpi();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetDrugTypeSales_OkResult를_반환한다()
    {
        var expected = new List<DrugTypeSales>
        {
            new("ETC", "전문의약품 (ETC)", 3_800_000m),
            new("OTC", "일반의약품 (OTC)", 1_400_000m),
        };
        _serviceMock.Setup(s => s.GetDrugTypeSalesAsync()).ReturnsAsync(expected);

        var result = await _controller.GetDrugTypeSales();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetPatientAges_OkResult를_반환한다()
    {
        var expected = new List<PatientAgeGroup> { new("30-39세", 22L) };
        _serviceMock.Setup(s => s.GetPatientAgeGroupsAsync()).ReturnsAsync(expected);

        var result = await _controller.GetPatientAges();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetHospitalPrescriptions_OkResult를_반환한다()
    {
        var expected = new List<HospitalPrescription> { new("연세내과의원", 85L) };
        _serviceMock.Setup(s => s.GetHospitalPrescriptionsAsync()).ReturnsAsync(expected);

        var result = await _controller.GetHospitalPrescriptions();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetWholesaleExpenses_OkResult를_반환한다()
    {
        var expected = new List<WholesaleExpense> { new("지오영", 2_100_000m) };
        _serviceMock.Setup(s => s.GetWholesaleExpensesAsync()).ReturnsAsync(expected);

        var result = await _controller.GetWholesaleExpenses();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetDrugCoverage_OkResult를_반환한다()
    {
        var expected = new List<DrugCoverage> { new("급여 의약품", 3_200_000m) };
        _serviceMock.Setup(s => s.GetDrugCoverageAsync()).ReturnsAsync(expected);

        var result = await _controller.GetDrugCoverage();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    // ─────────────────────────────────────────────────────────
    // Thin Controller 패턴 검증
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Controller는_ApiController_어트리뷰트를_가진다()
    {
        var attrs = typeof(DashboardController).GetCustomAttributes(typeof(ApiControllerAttribute), true);
        Assert.NotEmpty(attrs);
    }

    [Fact]
    public void Controller의_모든_액션이_IActionResult를_반환한다()
    {
        // Assert — Thin Controller 원칙: 모든 메서드가 IActionResult를 반환
        var methods = typeof(DashboardController).GetMethods()
            .Where(m => m.DeclaringType == typeof(DashboardController) && m.IsPublic)
            .Where(m => m.Name != "get_" && !m.IsSpecialName)
            .ToList();

        Assert.Equal(7, methods.Count);
        foreach (var method in methods)
        {
            Assert.Equal(typeof(Task<IActionResult>), method.ReturnType);
        }
    }
}
