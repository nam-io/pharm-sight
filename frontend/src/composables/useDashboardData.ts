/**
 * @composable useDashboardData
 * @description 약국 경영 대시보드 데이터를 백엔드 API에서 조회하는 Vue Composable.
 *
 * [에러 처리 전략]
 * 3단계 에러 분류를 통해 사용자에게 정확한 피드백을 제공합니다:
 *   - NETWORK: fetch 자체가 실패 (인터넷 끊김, DNS 오류)
 *   - API:     HTTP 비정상 응답 (4xx, 5xx — 서버 오류 또는 경로 오류)
 *   - PARSE:   JSON 역직렬화 실패 (응답 형식 불일치)
 *
 * [폴백 전략]
 * 환경변수 VITE_API_BASE_URL이 없거나 API 호출 실패 시 Mock 데이터를 표시합니다.
 * 이 폴백으로 백엔드 장애 시에도 UI가 완전히 깨지지 않습니다 (Graceful Degradation).
 *
 * [로깅 전략]
 * console.error에 구조화된 컨텍스트 객체를 포함합니다:
 * { errorType, message, timestamp, fallback, raw }
 * 브라우저 DevTools에서 오류 유형과 발생 시각을 즉시 파악할 수 있습니다.
 *
 * @throws 에러를 throw하지 않습니다. 오류 상태는 반환되는 `error` ref를 통해 전파됩니다.
 */
import { ref, computed } from 'vue'
import type { DashboardData, KpiCard } from '@/types'
import type { KpiSummary } from '@/types/api'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''
const USE_MOCK = !API_BASE

// ── 에러 분류 타입 ───────────────────────────────────────────────────────
type ApiErrorType = 'NETWORK' | 'API' | 'PARSE'

interface ClassifiedError {
  type: ApiErrorType
  message: string
  userMessage: string
}

/**
 * 발생한 예외를 유형별로 분류합니다.
 * 분류된 에러는 사용자 표시 메시지와 로깅 컨텍스트 생성에 사용됩니다.
 */
function classifyError(e: unknown): ClassifiedError {
  if (e instanceof TypeError && e.message.toLowerCase().includes('fetch')) {
    return {
      type: 'NETWORK',
      message: e.message,
      userMessage: '네트워크 연결을 확인하거나 잠시 후 다시 시도해주세요.',
    }
  }
  if (e instanceof Error && e.message.startsWith('API 오류')) {
    return {
      type: 'API',
      message: e.message,
      userMessage: e.message,
    }
  }
  return {
    type: 'PARSE',
    message: e instanceof Error ? e.message : String(e),
    userMessage: '데이터를 불러오는 중 오류가 발생했습니다.',
  }
}

// ── Mock 데이터 (API 미연결 시 폴백) ────────────────────────────────────
const MOCK_DATA: DashboardData = {
  monthlySales: [
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
  ],
  drugTypeSales: [
    { type: 'ETC', label: '전문의약품 (ETC)', amount: 118600000 },
    { type: 'OTC', label: '일반의약품 (OTC)', amount: 42200000 },
  ],
  patientAgeGroups: [
    { ageGroup: '0-9세', count: 187 },
    { ageGroup: '10-19세', count: 134 },
    { ageGroup: '20-29세', count: 218 },
    { ageGroup: '30-39세', count: 312 },
    { ageGroup: '40-49세', count: 428 },
    { ageGroup: '50-59세', count: 516 },
    { ageGroup: '60-69세', count: 489 },
    { ageGroup: '70세 이상', count: 374 },
  ],
  hospitalPrescriptions: [
    { hospitalName: '한빛소아과의원', count: 487 },
    { hospitalName: '서울내과클리닉', count: 412 },
    { hospitalName: '미래정형외과', count: 356 },
    { hospitalName: '하늘가정의학과', count: 298 },
    { hospitalName: '연세이비인후과', count: 267 },
    { hospitalName: '그린피부과의원', count: 198 },
  ],
  wholesaleExpenses: [
    { wholesaleName: '한국의약품유통', amount: 28400000 },
    { wholesaleName: '대원제약물류', amount: 22100000 },
    { wholesaleName: '지오영', amount: 19800000 },
    { wholesaleName: '백제약품', amount: 14600000 },
    { wholesaleName: '신풍제약유통', amount: 11200000 },
  ],
  drugCoverage: [
    { label: '급여 의약품', amount: 89400000 },
    { label: '비급여 의약품', amount: 31400000 },
  ],
}

async function fetchApi<T>(path: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`)
  if (!res.ok) throw new Error(`API 오류 [${res.status}]: ${path}`)
  return res.json()
}

export function useDashboardData() {
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const dashboardData = ref<DashboardData>({ ...MOCK_DATA })
  const kpiRaw = ref<KpiSummary | null>(null)

  const kpiCards = computed<KpiCard[]>(() => {
    if (!kpiRaw.value) {
      return [
        { title: '이번 달 총 매출',   value: '1,680', unit: '만원', change: 9.1,  icon: '💰' },
        { title: '이번 달 조제 건수', value: '420',   unit: '건',   change: 9.1,  icon: '💊' },
        { title: '이번 달 방문 환자', value: '287',   unit: '명',   change: 4.7,  icon: '🏥' },
        { title: '이번 달 발주 지출', value: '960',   unit: '만원', change: -2.3, icon: '📦' },
      ]
    }
    const k = kpiRaw.value
    return [
      {
        title: '이번 달 총 매출',
        value: (k.currentMonthSales / 10000).toLocaleString('ko-KR', { maximumFractionDigits: 0 }),
        unit: '만원',
        change: k.salesChangeRate,
        icon: '💰',
      },
      {
        title: '이번 달 조제 건수',
        value: k.currentMonthPrescriptions.toLocaleString(),
        unit: '건',
        change: k.prescriptionChangeRate,
        icon: '💊',
      },
      {
        title: '이번 달 방문 환자',
        value: k.currentMonthPatients.toLocaleString(),
        unit: '명',
        change: 0,
        icon: '🏥',
      },
      {
        title: '이번 달 발주 지출',
        value: (k.currentMonthOrderAmount / 10000).toLocaleString('ko-KR', { maximumFractionDigits: 0 }),
        unit: '만원',
        change: 0,
        icon: '📦',
      },
    ]
  })

  async function loadAll() {
    if (USE_MOCK) return // Mock 데이터 사용 시 API 호출 생략

    isLoading.value = true
    error.value = null
    try {
      const [monthly, drugType, ages, hospitals, wholesale, coverage, kpi] = await Promise.all([
        fetchApi<DashboardData['monthlySales']>('/api/dashboard/monthly-sales'),
        fetchApi<DashboardData['drugTypeSales']>('/api/dashboard/drug-type-sales'),
        fetchApi<DashboardData['patientAgeGroups']>('/api/dashboard/patient-ages'),
        fetchApi<DashboardData['hospitalPrescriptions']>('/api/dashboard/hospital-prescriptions'),
        fetchApi<DashboardData['wholesaleExpenses']>('/api/dashboard/wholesale-expenses'),
        fetchApi<DashboardData['drugCoverage']>('/api/dashboard/drug-coverage'),
        fetchApi<KpiSummary>('/api/dashboard/kpi'),
      ])
      dashboardData.value = {
        monthlySales: monthly,
        drugTypeSales: drugType,
        patientAgeGroups: ages,
        hospitalPrescriptions: hospitals,
        wholesaleExpenses: wholesale,
        drugCoverage: coverage,
      }
      kpiRaw.value = kpi
    } catch (e) {
      const classified = classifyError(e)
      error.value = classified.userMessage
      // 구조화된 에러 로깅: 오류 유형 · 발생 시각 · 폴백 상태 포함
      console.error('[useDashboardData] API 호출 실패', {
        errorType: classified.type,        // NETWORK | API | PARSE
        message: classified.message,
        timestamp: new Date().toISOString(),
        fallback: 'Mock 데이터로 자동 폴백',
        raw: e,
      })
    } finally {
      isLoading.value = false
    }
  }

  return { isLoading, error, dashboardData, kpiCards, loadAll }
}
