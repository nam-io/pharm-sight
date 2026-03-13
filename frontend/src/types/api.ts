/** 백엔드 KPI 요약 API 응답 타입 */
export interface KpiSummary {
  currentMonthSales: number
  currentMonthPrescriptions: number
  currentMonthPatients: number
  currentMonthOrderAmount: number
  salesChangeRate: number
  prescriptionChangeRate: number
}
