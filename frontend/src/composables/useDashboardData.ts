/**
 * @composable useDashboardData
 * @description 약국 경영 대시보드 Mock 데이터를 제공하는 Vue Composable.
 * Phase 2에서 실제 백엔드 API 호출로 교체 예정.
 */
import { ref, computed } from 'vue'
import type { DashboardData, KpiCard } from '@/types'

export function useDashboardData() {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  /** 월별 매출 및 조제 건수 (최근 12개월) */
  const monthlySales = ref([
    { month: '2024-03', totalAmount: 12400000, prescriptionCount: 312 },
    { month: '2024-04', totalAmount: 13100000, prescriptionCount: 328 },
    { month: '2024-05', totalAmount: 14200000, prescriptionCount: 355 },
    { month: '2024-06', totalAmount: 13800000, prescriptionCount: 341 },
    { month: '2024-07', totalAmount: 12900000, prescriptionCount: 318 },
    { month: '2024-08', totalAmount: 13500000, prescriptionCount: 337 },
    { month: '2024-09', totalAmount: 14800000, prescriptionCount: 372 },
    { month: '2024-10', totalAmount: 15600000, prescriptionCount: 389 },
    { month: '2024-11', totalAmount: 16200000, prescriptionCount: 405 },
    { month: '2024-12', totalAmount: 17100000, prescriptionCount: 428 },
    { month: '2025-01', totalAmount: 15400000, prescriptionCount: 385 },
    { month: '2025-02', totalAmount: 16800000, prescriptionCount: 420 },
  ])

  /** 조제약(Rx) vs 일반의약품(OTC) 매출 비중 */
  const drugTypeSales = ref([
    { type: 'Rx' as const, label: '전문의약품 (Rx)', amount: 118600000 },
    { type: 'OTC' as const, label: '일반의약품 (OTC)', amount: 42200000 },
  ])

  /** 방문 환자 연령대 분포 */
  const patientAgeGroups = ref([
    { ageGroup: '0-9세', count: 187 },
    { ageGroup: '10-19세', count: 134 },
    { ageGroup: '20-29세', count: 218 },
    { ageGroup: '30-39세', count: 312 },
    { ageGroup: '40-49세', count: 428 },
    { ageGroup: '50-59세', count: 516 },
    { ageGroup: '60-69세', count: 489 },
    { ageGroup: '70세 이상', count: 374 },
  ])

  /** 처방전 발행 의료기관별 유입 건수 (상위 6개) */
  const hospitalPrescriptions = ref([
    { hospitalName: '한빛소아과의원', count: 487 },
    { hospitalName: '서울내과클리닉', count: 412 },
    { hospitalName: '미래정형외과', count: 356 },
    { hospitalName: '하늘가정의학과', count: 298 },
    { hospitalName: '연세이비인후과', count: 267 },
    { hospitalName: '그린피부과의원', count: 198 },
  ])

  /** 도매상별 누적 지출 현황 */
  const wholesaleExpenses = ref([
    { wholesaleName: '한국의약품유통', amount: 28400000 },
    { wholesaleName: '대원제약물류', amount: 22100000 },
    { wholesaleName: '지오영', amount: 19800000 },
    { wholesaleName: '백제약품', amount: 14600000 },
    { wholesaleName: '신풍제약유통', amount: 11200000 },
  ])

  /** 급여/비급여 지출 비율 */
  const drugCoverage = ref([
    { label: '급여 의약품', amount: 89400000 },
    { label: '비급여 의약품', amount: 31400000 },
  ])

  /** KPI 요약 카드 */
  const kpiCards = computed<KpiCard[]>(() => [
    {
      title: '이번 달 총 매출',
      value: '1,680',
      unit: '만원',
      change: 9.1,
      icon: '💰',
    },
    {
      title: '이번 달 조제 건수',
      value: '420',
      unit: '건',
      change: 9.1,
      icon: '💊',
    },
    {
      title: '이번 달 방문 환자',
      value: '287',
      unit: '명',
      change: 4.7,
      icon: '🏥',
    },
    {
      title: '이번 달 발주 지출',
      value: '960',
      unit: '만원',
      change: -2.3,
      icon: '📦',
    },
  ])

  const dashboardData = computed<DashboardData>(() => ({
    monthlySales: monthlySales.value,
    drugTypeSales: drugTypeSales.value,
    patientAgeGroups: patientAgeGroups.value,
    hospitalPrescriptions: hospitalPrescriptions.value,
    wholesaleExpenses: wholesaleExpenses.value,
    drugCoverage: drugCoverage.value,
  }))

  return {
    isLoading,
    error,
    dashboardData,
    kpiCards,
  }
}
