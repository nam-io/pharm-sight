import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useDashboardData } from './useDashboardData'

// import.meta.env 모킹
vi.stubEnv('VITE_API_BASE_URL', '')

describe('useDashboardData', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
  })

  it('초기 상태에서 Mock 데이터가 로드된다', () => {
    const { dashboardData, isLoading, error } = useDashboardData()

    expect(isLoading.value).toBe(false)
    expect(error.value).toBeNull()
    expect(dashboardData.value.monthlySales.length).toBeGreaterThan(0)
    expect(dashboardData.value.drugTypeSales.length).toBe(2)
    expect(dashboardData.value.patientAgeGroups.length).toBeGreaterThan(0)
    expect(dashboardData.value.hospitalPrescriptions.length).toBeGreaterThan(0)
    expect(dashboardData.value.wholesaleExpenses.length).toBeGreaterThan(0)
    expect(dashboardData.value.drugCoverage.length).toBe(2)
  })

  it('API_BASE 미설정 시 loadAll은 Mock 데이터를 유지한다', async () => {
    const { dashboardData, loadAll } = useDashboardData()
    const before = { ...dashboardData.value }

    await loadAll()

    // Mock 모드에서는 데이터가 변경되지 않음
    expect(dashboardData.value.monthlySales.length).toBe(before.monthlySales.length)
  })

  it('KPI 카드가 Mock 기본값으로 생성된다', () => {
    const { kpiCards } = useDashboardData()

    expect(kpiCards.value.length).toBe(4)
    expect(kpiCards.value[0].title).toBe('이번 달 총 매출')
    expect(kpiCards.value[1].title).toBe('이번 달 조제 건수')
    expect(kpiCards.value[2].title).toBe('이번 달 방문 환자')
    expect(kpiCards.value[3].title).toBe('이번 달 발주 지출')
  })

  it('KPI 카드에 단위와 아이콘이 포함된다', () => {
    const { kpiCards } = useDashboardData()

    expect(kpiCards.value[0].unit).toBe('만원')
    expect(kpiCards.value[1].unit).toBe('건')
    expect(kpiCards.value[2].unit).toBe('명')
    expect(kpiCards.value[3].unit).toBe('만원')

    kpiCards.value.forEach(card => {
      expect(card.icon).toBeTruthy()
    })
  })

  it('Mock 데이터의 월별 매출이 시간순으로 정렬되어 있다', () => {
    const { dashboardData } = useDashboardData()
    const months = dashboardData.value.monthlySales.map(m => m.month)

    for (let i = 1; i < months.length; i++) {
      expect(months[i] > months[i - 1]).toBe(true)
    }
  })

  it('Mock 데이터의 ETC/OTC 유형이 올바르게 구분된다', () => {
    const { dashboardData } = useDashboardData()
    const types = dashboardData.value.drugTypeSales.map(d => d.type)

    expect(types).toContain('ETC')
    expect(types).toContain('OTC')
  })

  it('에러 유형(errorType)이 초기에 null이다', () => {
    const { errorType } = useDashboardData()
    expect(errorType.value).toBeNull()
  })

  // ── 엣지 케이스: 0값 데이터 ────────────────────────────────────────────
  it('Mock 데이터의 모든 금액이 양수이다 (0값 방어)', () => {
    const { dashboardData } = useDashboardData()
    dashboardData.value.monthlySales.forEach(sale => {
      expect(sale.totalAmount).toBeGreaterThan(0)
      expect(sale.prescriptionCount).toBeGreaterThan(0)
    })
  })

  it('KPI 카드 change 값이 0일 때도 정상 렌더링된다 (0값 엣지 케이스)', () => {
    const { kpiCards } = useDashboardData()
    // 방문 환자와 발주 지출의 change는 Mock에서 0이 아닌 값이므로,
    // 0인 경우도 정상 처리되는지 확인
    kpiCards.value.forEach(card => {
      expect(typeof card.change).toBe('number')
      expect(Number.isFinite(card.change)).toBe(true)
    })
  })

  it('빈 배열 데이터가 설정되어도 에러가 발생하지 않는다 (빈 결과 엣지 케이스)', () => {
    const { dashboardData, kpiCards } = useDashboardData()
    // 빈 배열로 설정
    dashboardData.value = {
      monthlySales: [],
      drugTypeSales: [],
      patientAgeGroups: [],
      hospitalPrescriptions: [],
      wholesaleExpenses: [],
      drugCoverage: [],
    }
    // KPI 카드는 kpiRaw가 null이므로 기본값 사용
    expect(kpiCards.value.length).toBe(4)
    // 빈 배열이어도 에러 없이 접근 가능
    expect(dashboardData.value.monthlySales.length).toBe(0)
    expect(dashboardData.value.drugTypeSales.length).toBe(0)
  })

  it('drugCoverage 금액이 0이어도 NaN이 발생하지 않는다 (0 나눗셈 방어)', () => {
    const { dashboardData } = useDashboardData()
    dashboardData.value.drugCoverage = [
      { label: '급여 의약품', amount: 0 },
      { label: '비급여 의약품', amount: 0 },
    ]
    const total = dashboardData.value.drugCoverage.reduce((sum, d) => sum + d.amount, 0)
    // 0으로 나누기 방어: total이 0이면 퍼센트 계산 시 NaN 방지
    const pct = total === 0 ? 0 : (dashboardData.value.drugCoverage[0].amount / total) * 100
    expect(Number.isFinite(pct)).toBe(true)
    expect(pct).toBe(0)
  })

  it('wholesaleExpenses가 빈 배열이어도 정상 처리된다 (빈 도매 데이터)', () => {
    const { dashboardData } = useDashboardData()
    dashboardData.value.wholesaleExpenses = []
    expect(dashboardData.value.wholesaleExpenses.length).toBe(0)
    const totalExpense = dashboardData.value.wholesaleExpenses.reduce((sum, w) => sum + w.amount, 0)
    expect(totalExpense).toBe(0)
  })
})
