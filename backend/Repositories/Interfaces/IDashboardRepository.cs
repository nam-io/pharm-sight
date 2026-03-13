using PharmSight.Api.Models;

namespace PharmSight.Api.Repositories.Interfaces;

/// <summary>대시보드 통계 데이터 접근 인터페이스 (Dapper 기반 순수 SQL)</summary>
public interface IDashboardRepository
{
    /// <summary>최근 12개월 월별 매출 및 조제 건수 조회</summary>
    Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync();

    /// <summary>전체 기간 Rx/OTC 약품 유형별 매출 비중 조회</summary>
    Task<IEnumerable<DrugTypeSales>> GetDrugTypeSalesAsync();

    /// <summary>전체 방문 환자 연령대 분포 조회</summary>
    Task<IEnumerable<PatientAgeGroup>> GetPatientAgeGroupsAsync();

    /// <summary>처방전 발행 의료기관별 유입 건수 TOP 6 조회</summary>
    Task<IEnumerable<HospitalPrescription>> GetHospitalPrescriptionsAsync();

    /// <summary>도매상별 누적 발주 지출 현황 조회</summary>
    Task<IEnumerable<WholesaleExpense>> GetWholesaleExpensesAsync();

    /// <summary>급여/비급여 의약품 지출 비율 조회</summary>
    Task<IEnumerable<DrugCoverage>> GetDrugCoverageAsync();

    /// <summary>이번 달 및 전월 대비 KPI 요약 조회</summary>
    Task<KpiSummary> GetKpiSummaryAsync();
}
