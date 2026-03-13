/** 월별 매출 및 조제 건수 데이터 */
export interface MonthlySales {
  month: string
  totalAmount: number
  prescriptionCount: number
}

/** 약품 유형별 매출 비중 (Rx/OTC) */
export interface DrugTypeSales {
  type: 'ETC' | 'OTC'
  label: string
  amount: number
}

/** 환자 연령대 분포 */
export interface PatientAgeGroup {
  ageGroup: string
  count: number
}

/** 병원별 처방전 유입 건수 */
export interface HospitalPrescription {
  hospitalName: string
  count: number
}

/** 도매상별 누적 지출 */
export interface WholesaleExpense {
  wholesaleName: string
  amount: number
}

/** 급여/비급여 지출 비율 */
export interface DrugCoverage {
  label: string
  amount: number
}

/** 대시보드 전체 데이터 */
export interface DashboardData {
  monthlySales: MonthlySales[]
  drugTypeSales: DrugTypeSales[]
  patientAgeGroups: PatientAgeGroup[]
  hospitalPrescriptions: HospitalPrescription[]
  wholesaleExpenses: WholesaleExpense[]
  drugCoverage: DrugCoverage[]
}

/** KPI 카드 데이터 */
export interface KpiCard {
  title: string
  value: string
  unit: string
  change: number
  icon: string
}
