/**
 * @composable useDashboardData
 * @description 약국 경영 대시보드 데이터를 백엔드 API에서 조회하는 Vue Composable.
 *
 * [에러 처리 전략 — 3단계 분류 + 유형별 차별 대응]
 *
 *   NETWORK (TypeError: Failed to fetch / AbortError)
 *     → 원인: 인터넷 단절, DNS 오류, 서버 다운, 요청 타임아웃
 *     → 대응: 500ms 지연 후 1회 자동 재시도 → 실패 시 Mock 폴백
 *     → 로그:  retryCount 포함 구조화 로깅
 *
 *   API (HTTP 4xx / 5xx)
 *     → 원인: 서버 오류, 라우팅 실패, 인증 오류
 *     → 대응: 재시도 없음 (서버 측 문제는 재시도가 무의미) → Mock 폴백
 *     → 로그:  HTTP 상태코드 포함
 *
 *   PARSE (JSON 역직렬화 실패)
 *     → 원인: 응답 형식 불일치, 빈 응답
 *     → 대응: 재시도 없음 → Mock 폴백
 *     → 로그:  원본 에러 포함
 *
 * [타임아웃 전략]
 * AbortController로 10초 타임아웃을 적용합니다.
 * fetch 자체에 timeout 옵션이 없으므로 AbortSignal로 직접 구현합니다.
 * 타임아웃 발생 시 AbortError → NETWORK 유형으로 분류되어 재시도합니다.
 *
 * [폴백 전략]
 * VITE_API_BASE_URL 미설정 또는 모든 재시도 실패 시 MOCK_DATA를 표시합니다.
 * dashboardData는 초기값이 MOCK_DATA이므로 API 실패 시에도 UI가 깨지지 않습니다
 * (Graceful Degradation).
 *
 * [로깅 전략]
 * console.error에 구조화된 컨텍스트 객체를 포함합니다:
 * { errorType, message, retryCount, timestamp, fallback, raw }
 * 브라우저 DevTools에서 에러 유형 · 재시도 횟수 · 발생 시각을 즉시 파악할 수 있습니다.
 *
 * @throws 에러를 throw하지 않습니다. 오류 상태는 반환되는 `error` / `errorType` ref로 전파됩니다.
 */
import { ref, computed } from 'vue'
import type { DashboardData, KpiCard } from '@/types'
import type { KpiSummary } from '@/types/api'
import {
  API_BASE_URL,
  USE_MOCK,
  DASHBOARD_TIMEOUT_MS,
  MAX_NETWORK_RETRIES,
  RETRY_DELAY_MS,
} from '@/config'

// ── 에러 분류 타입 ───────────────────────────────────────────────────────
/** API 에러 유형. 유형별로 재시도 여부와 사용자 메시지가 다릅니다. */
export type ApiErrorType = 'NETWORK' | 'API' | 'PARSE'

interface ClassifiedError {
  type: ApiErrorType
  message: string
  /** 사용자에게 표시할 친화적 메시지 */
  userMessage: string
  /** NETWORK 유형인 경우 재시도 가능 여부 */
  retryable: boolean
}

/**
 * 발생한 예외를 유형별로 분류합니다.
 * AbortError(타임아웃)는 NETWORK로 분류하여 재시도 대상에 포함합니다.
 */
function classifyError(e: unknown): ClassifiedError {
  // AbortError: AbortController 타임아웃 또는 명시적 중단
  if (e instanceof DOMException && e.name === 'AbortError') {
    return {
      type: 'NETWORK',
      message: `요청 타임아웃 (${DASHBOARD_TIMEOUT_MS / 1000}초 초과)`,
      userMessage: `서버 응답이 지연되고 있습니다 (${DASHBOARD_TIMEOUT_MS / 1000}초 초과). 잠시 후 다시 시도해주세요.`,
      retryable: true,
    }
  }
  // TypeError: fetch 자체가 실패 (네트워크 단절, DNS 오류)
  if (e instanceof TypeError && e.message.toLowerCase().includes('fetch')) {
    return {
      type: 'NETWORK',
      message: e.message,
      userMessage: '네트워크 연결을 확인하거나 잠시 후 다시 시도해주세요.',
      retryable: true,
    }
  }
  // HTTP 4xx / 5xx: 서버 측 오류 — 재시도 무의미
  if (e instanceof Error && e.message.startsWith('API 오류')) {
    return {
      type: 'API',
      message: e.message,
      userMessage: e.message,
      retryable: false,
    }
  }
  // JSON 파싱 실패 등
  return {
    type: 'PARSE',
    message: e instanceof Error ? e.message : String(e),
    userMessage: '데이터를 불러오는 중 오류가 발생했습니다.',
    retryable: false,
  }
}

/**
 * AbortController로 타임아웃을 적용한 fetch 래퍼.
 * DASHBOARD_TIMEOUT_MS 초과 시 AbortError를 throw합니다.
 */
async function fetchWithTimeout(input: RequestInfo, timeoutMs = DASHBOARD_TIMEOUT_MS): Promise<Response> {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs)
  try {
    return await fetch(input, { signal: controller.signal })
  } finally {
    clearTimeout(timeoutId) // 성공/실패 무관하게 타이머 정리
  }
}

/**
 * 타임아웃 + HTTP 상태 검사를 적용한 API 호출 함수.
 * NETWORK 에러는 상위에서 재시도 로직으로 처리합니다.
 */
async function fetchApi<T>(path: string): Promise<T> {
  const res = await fetchWithTimeout(`${API_BASE_URL}${path}`)
  if (!res.ok) throw new Error(`API 오류 [${res.status}]: ${path}`)
  return res.json()
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

export function useDashboardData() {
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  /** 에러 유형 — UI에서 NETWORK/API/PARSE에 따라 다른 안내 메시지를 표시할 수 있습니다. */
  const errorType = ref<ApiErrorType | null>(null)
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

  /**
   * 7개 대시보드 API를 Promise.all로 병렬 호출합니다.
   * NETWORK 에러(타임아웃 포함)는 MAX_NETWORK_RETRIES회 자동 재시도합니다.
   * API/PARSE 에러는 서버 측 문제이므로 재시도하지 않고 즉시 Mock 폴백합니다.
   */
  async function loadAll() {
    if (USE_MOCK) return // Mock 데이터 사용 시 API 호출 생략

    isLoading.value = true
    error.value = null
    errorType.value = null

    let retryCount = 0

    while (true) {
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
        break // 성공 시 루프 탈출
      } catch (e) {
        const classified = classifyError(e)

        // NETWORK 에러이고 재시도 횟수가 남아있으면 재시도
        if (classified.retryable && retryCount < MAX_NETWORK_RETRIES) {
          retryCount++
          console.warn('[useDashboardData] NETWORK 에러 — 재시도 중', {
            errorType: classified.type,
            retryCount,
            maxRetries: MAX_NETWORK_RETRIES,
            delayMs: RETRY_DELAY_MS,
            timestamp: new Date().toISOString(),
          })
          await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS))
          continue // 재시도
        }

        // 재시도 소진 또는 재시도 불가 에러 → Mock 폴백
        error.value = classified.userMessage
        errorType.value = classified.type
        console.error('[useDashboardData] API 호출 실패 — Mock 데이터 폴백', {
          errorType: classified.type,     // NETWORK | API | PARSE
          message: classified.message,
          retryCount,                      // 실제 재시도 횟수
          timestamp: new Date().toISOString(),
          fallback: 'Mock 데이터로 자동 폴백',
          raw: e,
        })
        break
      }
    }

    isLoading.value = false
  }

  return { isLoading, error, errorType, dashboardData, kpiCards, loadAll }
}
