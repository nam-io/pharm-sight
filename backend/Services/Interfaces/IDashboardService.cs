using PharmSight.Api.Models;

namespace PharmSight.Api.Services.Interfaces;

/// <summary>대시보드 비즈니스 로직 서비스 인터페이스</summary>
public interface IDashboardService
{
    Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync();
    Task<IEnumerable<DrugTypeSales>> GetDrugTypeSalesAsync();
    Task<IEnumerable<PatientAgeGroup>> GetPatientAgeGroupsAsync();
    Task<IEnumerable<HospitalPrescription>> GetHospitalPrescriptionsAsync();
    Task<IEnumerable<WholesaleExpense>> GetWholesaleExpensesAsync();
    Task<IEnumerable<DrugCoverage>> GetDrugCoverageAsync();
    Task<KpiSummary> GetKpiSummaryAsync();
}
