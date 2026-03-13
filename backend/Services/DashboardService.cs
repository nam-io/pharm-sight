using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;
using PharmSight.Api.Services.Interfaces;

namespace PharmSight.Api.Services;

/// <summary>
/// 대시보드 비즈니스 로직 서비스 구현체.
/// Repository를 통해 데이터를 조회하고 필요한 가공 처리를 수행합니다.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IDashboardRepository repository, ILogger<DashboardService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>최근 12개월 월별 매출 및 조제 건수 반환</summary>
    public async Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync()
    {
        _logger.LogInformation("월별 매출 데이터 조회 시작");
        return await _repository.GetMonthlySalesAsync();
    }

    /// <summary>Rx/OTC 약품 유형별 매출 비중 반환</summary>
    public async Task<IEnumerable<DrugTypeSales>> GetDrugTypeSalesAsync()
    {
        _logger.LogInformation("약품 유형별 매출 데이터 조회 시작");
        return await _repository.GetDrugTypeSalesAsync();
    }

    /// <summary>환자 연령대 분포 반환</summary>
    public async Task<IEnumerable<PatientAgeGroup>> GetPatientAgeGroupsAsync()
    {
        _logger.LogInformation("환자 연령대 분포 조회 시작");
        return await _repository.GetPatientAgeGroupsAsync();
    }

    /// <summary>의료기관별 처방전 유입 건수 TOP 6 반환</summary>
    public async Task<IEnumerable<HospitalPrescription>> GetHospitalPrescriptionsAsync()
    {
        _logger.LogInformation("의료기관별 처방전 유입 건수 조회 시작");
        return await _repository.GetHospitalPrescriptionsAsync();
    }

    /// <summary>도매상별 누적 지출 현황 반환</summary>
    public async Task<IEnumerable<WholesaleExpense>> GetWholesaleExpensesAsync()
    {
        _logger.LogInformation("도매상별 지출 현황 조회 시작");
        return await _repository.GetWholesaleExpensesAsync();
    }

    /// <summary>급여/비급여 의약품 지출 비율 반환</summary>
    public async Task<IEnumerable<DrugCoverage>> GetDrugCoverageAsync()
    {
        _logger.LogInformation("급여/비급여 지출 비율 조회 시작");
        return await _repository.GetDrugCoverageAsync();
    }

    /// <summary>KPI 요약 반환</summary>
    public async Task<KpiSummary> GetKpiSummaryAsync()
    {
        _logger.LogInformation("KPI 요약 데이터 조회 시작");
        return await _repository.GetKpiSummaryAsync();
    }
}
