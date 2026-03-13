using Dapper;
using Npgsql;
using PharmSight.Api.Models;
using PharmSight.Api.Repositories.Interfaces;

namespace PharmSight.Api.Repositories;

/// <summary>
/// 대시보드 통계 데이터 접근 구현체.
/// Dapper를 통한 순수 SQL 쿼리로 PostgreSQL에서 집계 데이터를 조회합니다.
/// </summary>
public class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;

    public DashboardRepository(IConfiguration configuration)
    {
        var raw = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("데이터베이스 연결 문자열이 설정되지 않았습니다.");
        _connectionString = NormalizeConnectionString(raw);
    }

    /// <summary>
    /// postgresql:// 또는 postgres:// URI 형식을 Npgsql 키=값 형식으로 변환합니다.
    /// Render 환경변수에 PostgreSQL URI가 주입되는 경우를 처리합니다.
    /// </summary>
    private static string NormalizeConnectionString(string cs)
    {
        if (!cs.StartsWith("postgresql://") && !cs.StartsWith("postgres://"))
            return cs;

        var uri = new Uri(cs);
        var userInfo = uri.UserInfo.Split(':', 2);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = userInfo[0],
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
            SslMode = SslMode.Require,
        };
        return builder.ConnectionString;
    }


    /// <summary>최근 12개월 월별 매출 및 조제 건수 집계</summary>
    public async Task<IEnumerable<MonthlySales>> GetMonthlySalesAsync()
    {
        const string sql = """
            SELECT
                TO_CHAR(s."SaleDate", 'YYYY-MM') AS "Month",
                SUM(s."Amount")                  AS "TotalAmount",
                COUNT(p."Id")                    AS "PrescriptionCount"
            FROM "Sales" s
            LEFT JOIN "Prescriptions" p ON s."PrescriptionId" = p."Id"
            WHERE s."SaleDate" >= CURRENT_DATE - INTERVAL '12 months'
            GROUP BY TO_CHAR(s."SaleDate", 'YYYY-MM')
            ORDER BY "Month";
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<MonthlySales>(sql);
    }

    /// <summary>Rx/OTC 약품 유형별 매출 비중 집계 (PrescriptionId 유무로 구분)</summary>
    public async Task<IEnumerable<DrugTypeSales>> GetDrugTypeSalesAsync()
    {
        // Sales 테이블에서 PrescriptionId 유무로 Rx/OTC 구분
        const string simpleSql = """
            SELECT
                CASE
                    WHEN s."PrescriptionId" IS NOT NULL THEN 'ETC'
                    ELSE 'OTC'
                END AS "Type",
                CASE
                    WHEN s."PrescriptionId" IS NOT NULL THEN '전문의약품 (ETC)'
                    ELSE '일반의약품 (OTC)'
                END AS "Label",
                SUM(s."Amount") AS "Amount"
            FROM "Sales" s
            GROUP BY (s."PrescriptionId" IS NOT NULL)
            ORDER BY "Type" DESC;
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<DrugTypeSales>(simpleSql);
    }

    /// <summary>전체 방문 환자 연령대 분포 집계</summary>
    public async Task<IEnumerable<PatientAgeGroup>> GetPatientAgeGroupsAsync()
    {
        const string sql = """
            SELECT
                CASE
                    WHEN age < 10  THEN '0-9세'
                    WHEN age < 20  THEN '10-19세'
                    WHEN age < 30  THEN '20-29세'
                    WHEN age < 40  THEN '30-39세'
                    WHEN age < 50  THEN '40-49세'
                    WHEN age < 60  THEN '50-59세'
                    WHEN age < 70  THEN '60-69세'
                    ELSE                '70세 이상'
                END AS "AgeGroup",
                COUNT(*) AS "Count"
            FROM (
                SELECT DISTINCT p."Id",
                    DATE_PART('year', AGE(p."DateOfBirth"::date)) AS age
                FROM "Patients" p
                JOIN "Prescriptions" pr ON pr."PatientId" = p."Id"
            ) sub
            GROUP BY "AgeGroup"
            ORDER BY MIN(age);
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<PatientAgeGroup>(sql);
    }

    /// <summary>의료기관별 처방전 유입 건수 TOP 6 집계</summary>
    public async Task<IEnumerable<HospitalPrescription>> GetHospitalPrescriptionsAsync()
    {
        const string sql = """
            SELECT
                h."Name" AS "HospitalName",
                COUNT(pr."Id") AS "Count"
            FROM "Prescriptions" pr
            JOIN "Hospitals" h ON pr."HospitalId" = h."Id"
            GROUP BY h."Id", h."Name"
            ORDER BY "Count" DESC
            LIMIT 6;
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<HospitalPrescription>(sql);
    }

    /// <summary>도매상별 누적 발주 지출 현황 집계</summary>
    public async Task<IEnumerable<WholesaleExpense>> GetWholesaleExpensesAsync()
    {
        const string sql = """
            SELECT
                o."WholesaleName",
                SUM(o."Amount") AS "Amount"
            FROM "Orders" o
            GROUP BY o."WholesaleName"
            ORDER BY "Amount" DESC
            LIMIT 5;
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<WholesaleExpense>(sql);
    }

    /// <summary>급여/비급여 의약품 지출 비율 집계</summary>
    public async Task<IEnumerable<DrugCoverage>> GetDrugCoverageAsync()
    {
        const string sql = """
            SELECT
                CASE d."IsCovered"
                    WHEN TRUE THEN '급여 의약품'
                    ELSE           '비급여 의약품'
                END AS "Label",
                SUM(o."Amount") AS "Amount"
            FROM "Orders" o
            JOIN "Drugs" d ON o."DrugId" = d."Id"
            GROUP BY d."IsCovered"
            ORDER BY d."IsCovered" DESC;
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<DrugCoverage>(sql);
    }

    /// <summary>이번 달 및 전월 대비 KPI 요약 집계</summary>
    public async Task<KpiSummary> GetKpiSummaryAsync()
    {
        const string sql = """
            WITH current_month AS (
                SELECT
                    COALESCE(SUM(s."Amount"), 0)  AS sales,
                    COUNT(DISTINCT pr."Id")        AS prescriptions,
                    COUNT(DISTINCT pr."PatientId") AS patients
                FROM "Sales" s
                LEFT JOIN "Prescriptions" pr ON s."PrescriptionId" = pr."Id"
                WHERE DATE_TRUNC('month', s."SaleDate"::date) = DATE_TRUNC('month', CURRENT_DATE)
            ),
            prev_month AS (
                SELECT
                    COALESCE(SUM(s."Amount"), 0) AS sales,
                    COUNT(DISTINCT pr."Id")       AS prescriptions
                FROM "Sales" s
                LEFT JOIN "Prescriptions" pr ON s."PrescriptionId" = pr."Id"
                WHERE DATE_TRUNC('month', s."SaleDate"::date) = DATE_TRUNC('month', CURRENT_DATE - INTERVAL '1 month')
            ),
            current_orders AS (
                SELECT COALESCE(SUM(o."Amount"), 0) AS amount
                FROM "Orders" o
                WHERE DATE_TRUNC('month', o."OrderDate"::date) = DATE_TRUNC('month', CURRENT_DATE)
            )
            SELECT
                c.sales            AS "CurrentMonthSales",
                c.prescriptions    AS "CurrentMonthPrescriptions",
                c.patients         AS "CurrentMonthPatients",
                co.amount          AS "CurrentMonthOrderAmount",
                CASE WHEN p.sales = 0 THEN 0
                     ELSE ROUND(((c.sales - p.sales) / p.sales * 100)::numeric, 1)
                END                AS "SalesChangeRate",
                CASE WHEN p.prescriptions = 0 THEN 0
                     ELSE ROUND(((c.prescriptions - p.prescriptions)::numeric / p.prescriptions * 100)::numeric, 1)
                END                AS "PrescriptionChangeRate"
            FROM current_month c, prev_month p, current_orders co;
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleAsync<KpiSummary>(sql);
    }
}
