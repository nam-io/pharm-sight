<script setup lang="ts">
/**
 * @view DashboardView
 * @description 약국 경영 대시보드 메인 뷰.
 *
 * [사용자 흐름 — 5단계 상태 전이]
 * 1. 초기 로딩  → 스켈레톤 UI 표시 (KPI 4개 + 차트 6개 각각 형태별 스켈레톤)
 * 2. 데이터 표시 → fade-in 트랜지션으로 자연스럽게 콘텐츠 전환
 * 3. 에러 발생  → 에러 유형별(NETWORK/API/PARSE) 안내 + [다시 시도] 버튼
 *                  Mock 데이터로 자동 폴백 → 서비스 중단 없음
 * 4. 사용자 상호작용 → 기간 필터(3/6/12개월), CSV 내보내기, AI 재시도
 * 5. 피드백    → CSV 내보내기 완료 시 토스트 알림 3초 표시
 *
 * [엣지 케이스 대응]
 * - 빈 데이터: 각 차트 컴포넌트가 isEmpty computed로 "데이터 없음" 안내 표시
 * - API 실패: Mock 폴백으로 UI 유지 + 에러 패널로 사용자 안내
 * - AI 실패: AiInsightPanel에서 독립 에러 처리 + 재시도 버튼 (나머지 차트 영향 없음)
 * - 네트워크 불안정: NETWORK 에러 자동 1회 재시도 (config.ts MAX_NETWORK_RETRIES)
 */
import { onMounted, computed, ref } from 'vue'
import { useDashboardData } from '@/composables/useDashboardData'
import { useAiInsight } from '@/composables/useAiInsight'
import AiInsightPanel from '@/components/AiInsightPanel.vue'
import SalesLineChart from '@/components/charts/SalesLineChart.vue'
import DrugTypePieChart from '@/components/charts/DrugTypePieChart.vue'
import PatientAgeChart from '@/components/charts/PatientAgeChart.vue'
import HospitalBarChart from '@/components/charts/HospitalBarChart.vue'
import WholesaleBarChart from '@/components/charts/WholesaleBarChart.vue'
import DrugCoverageChart from '@/components/charts/DrugCoverageChart.vue'

const { dashboardData, kpiCards, isLoading, error, errorType, loadAll } = useDashboardData()
const { insight, isLoading: aiLoading, error: aiError, loadInsight } = useAiInsight()

// ── 로딩 진행률 표시 ─────────────────────────────────────────────────────
const loadingProgress = ref(0)
const loadingStage = ref('')

/** 로딩 진행률을 단계적으로 표시합니다 (7개 API + AI = 총 8단계) */
function simulateProgress() {
  loadingProgress.value = 0
  loadingStage.value = '대시보드 데이터 요청 중...'
  const stages = [
    { pct: 15, label: 'KPI 지표 로드 중...' },
    { pct: 30, label: '매출 데이터 집계 중...' },
    { pct: 45, label: '환자 데이터 분석 중...' },
    { pct: 60, label: '도매 지출 집계 중...' },
    { pct: 75, label: '차트 데이터 구성 중...' },
    { pct: 90, label: 'AI 인사이트 요청 중...' },
    { pct: 100, label: '로딩 완료' },
  ]
  let i = 0
  const interval = setInterval(() => {
    if (!isLoading.value || i >= stages.length) {
      loadingProgress.value = 100
      loadingStage.value = '로딩 완료'
      clearInterval(interval)
      return
    }
    loadingProgress.value = stages[i].pct
    loadingStage.value = stages[i].label
    i++
  }, 400)
}

// ── 차트 드릴다운 상태 ──────────────────────────────────────────────────
const drilldownMonth = ref<string | null>(null)
const drilldownData = computed(() => {
  if (!drilldownMonth.value) return null
  const sale = dashboardData.value.monthlySales.find(d => d.month === drilldownMonth.value)
  if (!sale) return null
  return {
    month: sale.month,
    totalAmount: sale.totalAmount.toLocaleString('ko-KR'),
    prescriptionCount: sale.prescriptionCount,
    avgPerPrescription: sale.prescriptionCount > 0
      ? Math.round(sale.totalAmount / sale.prescriptionCount).toLocaleString('ko-KR')
      : '0',
  }
})

/** 월별 매출 차트 바 클릭 시 드릴다운 상세 표시 */
function handleChartClick(params: any) {
  if (params?.dataIndex !== undefined && filteredMonthlySales.value[params.dataIndex]) {
    const clicked = filteredMonthlySales.value[params.dataIndex]
    drilldownMonth.value = drilldownMonth.value === clicked.month ? null : clicked.month
  }
}

// ── 터치 제스처(스와이프) — 기간 필터 전환 ────────────────────────────────
let touchStartX = 0
let touchStartY = 0

function handleTouchStart(e: TouchEvent) {
  touchStartX = e.touches[0].clientX
  touchStartY = e.touches[0].clientY
}

function handleTouchEnd(e: TouchEvent) {
  const dx = e.changedTouches[0].clientX - touchStartX
  const dy = e.changedTouches[0].clientY - touchStartY
  // 수평 스와이프만 감지 (수직 스크롤 무시)
  if (Math.abs(dx) < 50 || Math.abs(dx) < Math.abs(dy)) return

  const periods = [3, 6, 12]
  const idx = periods.indexOf(selectedPeriod.value)
  if (dx < 0 && idx < periods.length - 1) {
    // 왼쪽 스와이프 → 더 긴 기간
    selectedPeriod.value = periods[idx + 1]
  } else if (dx > 0 && idx > 0) {
    // 오른쪽 스와이프 → 더 짧은 기간
    selectedPeriod.value = periods[idx - 1]
  }
}

// ── 현재 날짜 표시 ───────────────────────────────────────────────────────
const currentDateLabel = computed(() => {
  const now = new Date()
  return `${now.getFullYear()}년 ${now.getMonth() + 1}월 기준`
})

// ── 기간 필터 — 월별 매출 차트 표시 개월 수 선택 ─────────────────────────
const periodOptions = [
  { label: '최근 3개월', value: 3 },
  { label: '최근 6개월', value: 6 },
  { label: '최근 12개월', value: 12 },
]
const selectedPeriod = ref(12)

/**
 * 선택된 기간에 따라 월별 매출 데이터를 필터링합니다.
 * 백엔드 재호출 없이 이미 로드된 12개월 데이터를 클라이언트에서 슬라이싱합니다.
 */
const filteredMonthlySales = computed(() =>
  dashboardData.value.monthlySales.slice(-selectedPeriod.value)
)

// ── 에러 유형별 사용자 친화적 안내 메시지 ────────────────────────────────
const errorGuideMessage = computed(() => {
  switch (errorType.value) {
    case 'NETWORK':
      return '인터넷 연결 또는 서버 상태를 확인해주세요. 아래 [다시 시도] 버튼으로 재요청할 수 있습니다.'
    case 'API':
      return '서버에서 오류가 발생했습니다. 잠시 후 다시 시도해주세요.'
    case 'PARSE':
      return '데이터 형식 오류가 발생했습니다. 지속될 경우 관리자에게 문의하세요.'
    default:
      return '실시간 데이터를 불러오지 못했습니다. 현재 샘플 데이터가 표시되고 있습니다.'
  }
})

// ── 에러 알림 닫기 상태 ──────────────────────────────────────────────────
const isErrorDismissed = ref(false)

// ── 토스트 알림 ──────────────────────────────────────────────────────────
const toastMessage = ref('')
const isToastVisible = ref(false)

function showToast(message: string) {
  toastMessage.value = message
  isToastVisible.value = true
  setTimeout(() => { isToastVisible.value = false }, 3000)
}

// ── 대시보드 + AI 재로드 ─────────────────────────────────────────────────
function retryLoad() {
  isErrorDismissed.value = false
  loadAll()
  loadInsight()
}

/** AI 인사이트만 독립적으로 재시도 (AiInsightPanel의 retry 이벤트 핸들러) */
function retryAiInsight() {
  loadInsight()
}

// ── CSV 내보내기 (BOM UTF-8 — Excel 한글 호환) ──────────────────────────
function exportToCsv() {
  const rows: string[] = []
  const now = new Date()
  const fileName = `pharm-sight-${now.getFullYear()}${String(now.getMonth() + 1).padStart(2, '0')}.csv`

  rows.push('# 월별 매출 및 조제 건수')
  rows.push('월,총매출(원),조제건수')
  for (const d of dashboardData.value.monthlySales) {
    rows.push(`${d.month},${d.totalAmount},${d.prescriptionCount}`)
  }
  rows.push('')

  rows.push('# 의약품 유형별 매출')
  rows.push('유형,매출액(원)')
  for (const d of dashboardData.value.drugTypeSales) {
    rows.push(`${d.label},${d.amount}`)
  }
  rows.push('')

  rows.push('# 방문 환자 연령대 분포')
  rows.push('연령대,환자수')
  for (const d of dashboardData.value.patientAgeGroups) {
    rows.push(`${d.ageGroup},${d.count}`)
  }
  rows.push('')

  rows.push('# 처방전 발행 의료기관 TOP 6')
  rows.push('의료기관명,처방건수')
  for (const d of dashboardData.value.hospitalPrescriptions) {
    rows.push(`${d.hospitalName},${d.count}`)
  }
  rows.push('')

  rows.push('# 도매상별 누적 지출')
  rows.push('도매상명,지출액(원)')
  for (const d of dashboardData.value.wholesaleExpenses) {
    rows.push(`${d.wholesaleName},${d.amount}`)
  }
  rows.push('')

  rows.push('# 급여/비급여 지출 비율')
  rows.push('구분,지출액(원)')
  for (const d of dashboardData.value.drugCoverage) {
    rows.push(`${d.label},${d.amount}`)
  }

  const bom = '\uFEFF'
  const blob = new Blob([bom + rows.join('\n')], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  URL.revokeObjectURL(url)
  showToast(`${fileName} 다운로드 완료`)
}

// ── 초기 로딩 — 대시보드 데이터 + AI 인사이트 병렬 요청 ──────────────────
onMounted(() => {
  simulateProgress()
  loadAll()
  loadInsight()
})
</script>

<template>
  <div class="min-h-screen bg-[#F1F2F5] text-[#131313]" style="font-family: 'Spoqa Han Sans Neo', 'Apple SD Gothic Neo', sans-serif;">

    <!-- ── 헤더 (sticky) ────────────────────────────────────────────────── -->
    <header class="border-b border-[#DDDDDD] bg-white sticky top-0 z-10 shadow-sm">
      <div class="max-w-screen-2xl mx-auto px-6 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-lg bg-[#396EFF] flex items-center justify-center text-white font-bold text-sm">P</div>
          <div>
            <h1 class="text-lg font-bold tracking-tight text-[#131313]">PharmSight AI</h1>
            <p class="text-xs text-[#777777]">약국 경영 통합 AI 대시보드</p>
          </div>
        </div>
        <div class="flex items-center gap-2 sm:gap-4">
          <span class="hidden sm:inline text-xs text-[#777777] bg-[#F4F5F7] px-3 py-1.5 rounded-full">
            {{ currentDateLabel }} · Supabase 연동
          </span>
          <!-- 연결 상태 배지 — 3가지 상태 시각화 -->
          <Transition name="fade" mode="out-in">
            <span v-if="error" key="error" class="text-xs bg-[#FFF0F0] text-[#F1636F] border border-[#F1636F]/40 px-3 py-1.5 rounded-full">
              ⚠ API 오류 · 샘플 데이터
            </span>
            <span v-else-if="isLoading" key="loading" class="text-xs bg-[#F4F5F7] text-[#777777] border border-[#DDDDDD] px-3 py-1.5 rounded-full animate-pulse">
              ⟳ 데이터 로딩 중...
            </span>
            <span v-else key="connected" class="text-xs bg-[#F0FFF4] text-[#28A745] border border-[#28A745]/40 px-3 py-1.5 rounded-full">
              ● 실시간 연동
            </span>
          </Transition>
          <!-- CSV 내보내기 버튼 -->
          <button
            @click="exportToCsv"
            :disabled="isLoading"
            class="flex items-center gap-1.5 text-xs bg-[#EEF3FF] hover:bg-[#396EFF] hover:text-white text-[#396EFF] border border-[#396EFF]/40 hover:border-[#396EFF] px-3 py-1.5 rounded-full transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
            title="현재 대시보드 데이터를 CSV 파일로 내보냅니다"
          >
            ⬇ <span class="hidden sm:inline">데이터 내보내기</span><span class="sm:hidden">CSV</span>
          </button>
        </div>
      </div>
    </header>

    <main class="max-w-screen-2xl mx-auto px-6 py-6 space-y-6">

      <!-- ── 로딩 진행률 표시바 (ARIA: progressbar + aria-valuenow) ──────── -->
      <Transition name="slide-fade">
        <div v-if="isLoading" class="rounded-xl border border-[#DDDDDD] bg-white shadow-sm px-5 py-3" aria-busy="true">
          <div class="flex items-center justify-between mb-2">
            <span class="text-xs text-[#555555]" aria-live="polite">{{ loadingStage }}</span>
            <span class="text-xs text-[#396EFF] font-mono">{{ loadingProgress }}%</span>
          </div>
          <div
            class="h-1.5 bg-[#F4F5F7] rounded-full overflow-hidden"
            role="progressbar"
            :aria-valuenow="loadingProgress"
            aria-valuemin="0"
            aria-valuemax="100"
            :aria-label="`데이터 로딩 진행률 ${loadingProgress}%`"
          >
            <div
              class="h-full bg-[#396EFF] rounded-full transition-all duration-300 ease-out"
              :style="{ width: loadingProgress + '%' }"
            />
          </div>
        </div>
      </Transition>

      <!-- ── 에러 알림 패널 (NETWORK/API/PARSE 유형별 안내 + 재시도) ───── -->
      <Transition name="slide-fade">
        <div
          v-if="error && !isErrorDismissed"
          class="flex items-start gap-4 rounded-xl border border-[#F1636F]/30 bg-[#FFF5F5] px-5 py-4"
          role="alert"
          aria-live="polite"
        >
          <span class="mt-0.5 flex-shrink-0 text-xl">⚠️</span>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-semibold text-[#F1636F]">실시간 데이터 연결 실패</p>
            <p class="mt-1 text-xs text-[#F1636F]">{{ error }}</p>
            <p class="mt-1 text-xs text-[#555555]">{{ errorGuideMessage }}</p>
            <p class="mt-2 text-xs text-[#777777]">
              💡 현재 샘플 데이터가 표시되고 있습니다. 실제 약국 데이터를 보려면 아래 버튼을 클릭하세요.
            </p>
          </div>
          <div class="flex items-center gap-2 flex-shrink-0">
            <button
              @click="retryLoad"
              :disabled="isLoading"
              class="text-xs bg-[#F1636F] hover:bg-[#d9525e] text-white border border-[#F1636F] px-3 py-1.5 rounded-lg transition-colors disabled:opacity-40"
            >
              {{ isLoading ? '로딩 중...' : '다시 시도' }}
            </button>
            <button
              @click="isErrorDismissed = true"
              class="text-[#999999] hover:text-[#555555] transition-colors text-lg leading-none"
              aria-label="알림 닫기"
            >
              ×
            </button>
          </div>
        </div>
      </Transition>

      <!-- ── AI 경영 인사이트 패널 ────────────────────────────────────────── -->
      <AiInsightPanel :insight="insight" :is-loading="aiLoading" :error="aiError" @retry="retryAiInsight" />

      <!-- ── KPI 카드 4종 (스켈레톤 → 실제 데이터 트랜지션) ───────────── -->
      <section class="grid grid-cols-2 lg:grid-cols-4 gap-4" aria-label="핵심 경영 지표">
        <template v-if="isLoading">
          <div
            v-for="i in 4"
            :key="`kpi-skeleton-${i}`"
            class="animate-pulse bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm"
          >
            <div class="flex items-start justify-between mb-3">
              <div class="h-3 w-24 rounded bg-[#E8E8E8]" />
              <div class="h-6 w-6 rounded-full bg-[#E8E8E8]" />
            </div>
            <div class="h-8 w-28 rounded bg-[#E8E8E8] mb-2" />
            <div class="h-3 w-20 rounded bg-[#F0F0F0]" />
          </div>
        </template>
        <template v-else>
          <div
            v-for="card in kpiCards"
            :key="card.title"
            class="bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm hover:shadow-md hover:border-[#396EFF]/30 transition-all duration-300"
          >
            <div class="flex items-start justify-between mb-3">
              <p class="text-xs text-[#777777] font-medium">{{ card.title }}</p>
              <span class="text-xl">{{ card.icon }}</span>
            </div>
            <div class="flex items-end gap-1">
              <span class="text-2xl font-bold text-[#131313]">{{ card.value }}</span>
              <span class="text-sm text-[#555555] mb-0.5">{{ card.unit }}</span>
            </div>
            <div class="mt-2 flex items-center gap-1">
              <span
                class="text-xs font-medium"
                :class="card.change >= 0 ? 'text-[#28A745]' : 'text-[#F1636F]'"
              >
                {{ card.change >= 0 ? '▲' : '▼' }} {{ Math.abs(card.change) }}%
              </span>
              <span class="text-xs text-[#999999]">전월 대비</span>
            </div>
          </div>
        </template>
      </section>

      <!-- ── 차트 행 1: 매출 추이 (기간 필터 + 드릴다운 + 터치 스와이프) + ETC/OTC ── -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4" aria-label="매출 분석 차트">
        <div
          class="lg:col-span-2 bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm"
          @touchstart="handleTouchStart"
          @touchend="handleTouchEnd"
        >
          <div class="mb-4 flex items-start justify-between gap-2">
            <div>
              <h2 class="text-sm font-semibold text-[#131313]">월별 매출 및 조제 건수 추이</h2>
              <p class="text-xs text-[#777777] mt-0.5">매출(바) · 조제 건수(선) · 기간 선택 가능 · 바 클릭 시 상세 · 좌우 스와이프로 기간 전환</p>
            </div>
            <!-- 기간 필터 버튼 그룹 -->
            <div class="flex items-center gap-1 flex-shrink-0" role="group" aria-label="기간 선택">
              <button
                v-for="opt in periodOptions"
                :key="opt.value"
                @click="selectedPeriod = opt.value"
                class="text-xs px-2.5 py-1 rounded transition-all duration-200"
                :class="selectedPeriod === opt.value
                  ? 'bg-[#396EFF] text-[#131313] shadow-sm shadow-[#396EFF]/30'
                  : 'bg-[#F4F5F7] text-[#555555] hover:bg-[#E8E8E8] hover:text-[#343434]'"
                :aria-pressed="selectedPeriod === opt.value"
              >
                {{ opt.label }}
              </button>
            </div>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full rounded-lg bg-[#F4F5F7] flex items-end gap-1 px-4 pb-4 pt-2">
              <div
                v-for="i in 8" :key="i"
                class="flex-1 rounded-t bg-[#E0E0E0]"
                :style="`height: ${25 + (i % 4) * 18}%`"
              />
            </div>
            <SalesLineChart v-else :data="filteredMonthlySales" @click="handleChartClick" />
          </div>
          <!-- 드릴다운 상세 패널 — 바 클릭 시 월별 상세 데이터 표시 -->
          <Transition name="slide-fade">
            <div
              v-if="drilldownData"
              class="mt-3 rounded-lg border border-[#396EFF]/20 bg-[#EEF3FF] px-4 py-3 flex items-center gap-6 text-xs"
            >
              <span class="text-[#396EFF] font-semibold">{{ drilldownData.month }} 상세</span>
              <span class="text-[#777777]">총매출: <b class="text-[#131313]">{{ drilldownData.totalAmount }}원</b></span>
              <span class="text-[#777777]">조제: <b class="text-[#131313]">{{ drilldownData.prescriptionCount }}건</b></span>
              <span class="text-[#777777]">건당 평균: <b class="text-[#131313]">{{ drilldownData.avgPerPrescription }}원</b></span>
              <button
                @click="drilldownMonth = null"
                class="ml-auto text-[#999999] hover:text-[#555555] transition-colors"
                aria-label="드릴다운 닫기"
              >×</button>
            </div>
          </Transition>
        </div>

        <div class="bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-[#131313]">ETC vs OTC 매출 비중</h2>
            <p class="text-xs text-[#777777] mt-0.5">전문의약품(ETC) vs 일반의약품(OTC)</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-center justify-center">
              <div class="w-40 h-40 rounded-full bg-[#E8E8E8] border-8 border-[#F0F0F0]" />
            </div>
            <DrugTypePieChart v-else :data="dashboardData.drugTypeSales" />
          </div>
        </div>
      </section>

      <!-- ── 차트 행 2: 연령대 분포 + 처방 기관 TOP 6 ─────────────────── -->
      <section class="grid grid-cols-1 lg:grid-cols-2 gap-4" aria-label="환자 및 처방 분석">
        <div class="bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-[#131313]">방문 환자 연령대 분포</h2>
            <p class="text-xs text-[#777777] mt-0.5">연령대별 내원 환자 수</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 8" :key="i" class="flex-1 rounded-t bg-[#F0F0F0]" :style="`height: ${15 + i * 9}%`" />
            </div>
            <PatientAgeChart v-else :data="dashboardData.patientAgeGroups" />
          </div>
        </div>

        <div class="bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-[#131313]">처방전 발행 의료기관 TOP 6</h2>
            <p class="text-xs text-[#777777] mt-0.5">기관별 처방전 유입 건수</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 6" :key="i" class="flex-1 rounded-t bg-[#F0F0F0]" :style="`height: ${35 + (6 - i) * 11}%`" />
            </div>
            <HospitalBarChart v-else :data="dashboardData.hospitalPrescriptions" />
          </div>
        </div>
      </section>

      <!-- ── 차트 행 3: 도매상 지출 + 급여/비급여 ─────────────────────── -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4" aria-label="지출 구조 분석">
        <div class="lg:col-span-2 bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-[#131313]">도매상별 누적 지출 현황</h2>
            <p class="text-xs text-[#777777] mt-0.5">거래 도매상별 의약품 발주 지출액</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 5" :key="i" class="flex-1 rounded-t bg-[#F0F0F0]" :style="`height: ${25 + (5 - i) * 13}%`" />
            </div>
            <WholesaleBarChart v-else :data="dashboardData.wholesaleExpenses" />
          </div>
        </div>

        <div class="bg-white border border-[#DDDDDD] rounded-xl p-5 shadow-sm">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-[#131313]">급여 · 비급여 지출 비율</h2>
            <p class="text-xs text-[#777777] mt-0.5">건강보험 적용 여부별 지출</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-center justify-center">
              <div class="w-40 h-40 rounded-full bg-[#E8E8E8] border-8 border-[#F0F0F0]" />
            </div>
            <DrugCoverageChart v-else :data="dashboardData.drugCoverage" />
          </div>
        </div>
      </section>

    </main>

    <!-- ── 푸터 ───────────────────────────────────────────────────────────── -->
    <footer class="border-t border-[#DDDDDD] mt-8 py-4">
      <p class="text-center text-xs text-[#999999]">
        PharmSight AI © 2026 · Powered by Google Gemini AI · Supabase 실데이터 연동
      </p>
    </footer>

    <!-- ── 토스트 알림 (CSV 내보내기 등 사용자 피드백) ─────────────────── -->
    <Transition name="toast">
      <div
        v-if="isToastVisible"
        class="fixed bottom-6 left-1/2 -translate-x-1/2 z-50 flex items-center gap-2 bg-[#396EFF] border border-[#2d5de0] text-white text-sm px-5 py-3 rounded-xl shadow-lg backdrop-blur"
        role="status"
        aria-live="polite"
      >
        <span>✅</span>
        <span>{{ toastMessage }}</span>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* ── 트랜지션: fade (연결 상태 배지 전환) ───────────────── */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* ── 트랜지션: slide-fade (에러 패널 등장/퇴장) ─────────── */
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}
.slide-fade-leave-active {
  transition: all 0.2s ease-in;
}
.slide-fade-enter-from {
  opacity: 0;
  transform: translateY(-12px);
}
.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

/* ── 트랜지션: toast (하단 알림 팝업) ────────────────────── */
.toast-enter-active {
  transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1);
}
.toast-leave-active {
  transition: all 0.25s ease-in;
}
.toast-enter-from {
  opacity: 0;
  transform: translate(-50%, 16px);
}
.toast-leave-to {
  opacity: 0;
  transform: translate(-50%, 8px);
}
</style>
