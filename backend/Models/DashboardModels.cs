namespace PharmSight.Api.Models;

/// <summary>월별 매출 및 조제 건수 집계 모델</summary>
public record MonthlySales(string Month, decimal TotalAmount, int PrescriptionCount);

/// <summary>약품 유형별(Rx/OTC) 매출 비중 모델</summary>
public record DrugTypeSales(string Type, string Label, decimal Amount);

/// <summary>환자 연령대 분포 모델</summary>
public record PatientAgeGroup(string AgeGroup, int Count);

/// <summary>의료기관별 처방전 유입 건수 모델</summary>
public record HospitalPrescription(string HospitalName, int Count);

/// <summary>도매상별 누적 지출 현황 모델</summary>
public record WholesaleExpense(string WholesaleName, decimal Amount);

/// <summary>급여/비급여 지출 비율 모델</summary>
public record DrugCoverage(string Label, decimal Amount);

/// <summary>대시보드 KPI 요약 모델</summary>
public record KpiSummary(
    decimal CurrentMonthSales,
    int CurrentMonthPrescriptions,
    int CurrentMonthPatients,
    decimal CurrentMonthOrderAmount,
    double SalesChangeRate,
    double PrescriptionChangeRate
);
