<script setup lang="ts">
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

// 현재 날짜 기준 표시
const currentDateLabel = computed(() => {
  const now = new Date()
  return `📅 ${now.getFullYear()}년 ${now.getMonth() + 1}월 기준`
})

// 기간 필터 — 월별 매출 차트 표시 개월 수 선택 (3 / 6 / 12개월)
const periodOptions = [
  { label: '최근 3개월', value: 3 },
  { label: '최근 6개월', value: 6 },
  { label: '최근 12개월', value: 12 },
]
const selectedPeriod = ref(12)

/**
 * 선택된 기간에 따라 월별 매출 데이터를 필터링합니다.
 * 백엔드 재호출 없이 이미 로드된 데이터를 클라이언트에서 슬라이싱합니다.
 */
const filteredMonthlySales = computed(() =>
  dashboardData.value.monthlySales.slice(-selectedPeriod.value)
)

/**
 * 에러 유형별 사용자 친화적 안내 메시지를 반환합니다.
 * errorType은 useDashboardData에서 NETWORK | API | PARSE로 분류됩니다.
 */
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

// 에러 알림 패널 표시 여부 (사용자가 직접 닫을 수 있음)
const isErrorDismissed = ref(false)

/**
 * 대시보드 및 AI 인사이트를 다시 로드합니다.
 * 에러 발생 후 사용자가 수동으로 재시도할 때 호출됩니다.
 */
function retryLoad() {
  isErrorDismissed.value = false
  loadAll()
  loadInsight()
}

/**
 * 대시보드 데이터 전체를 CSV 파일로 내보냅니다.
 * 6개 섹션, BOM 포함 UTF-8 인코딩으로 Excel 한글 깨짐을 방지합니다.
 */
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

  rows.push('# 급여·비급여 지출 비율')
  rows.push('구분,지출액(원)')
  for (const d of dashboardData.value.drugCoverage) {
    rows.push(`${d.label},${d.amount}`)
  }

  // BOM 포함 UTF-8로 다운로드 (Excel 한글 깨짐 방지)
  const bom = '\uFEFF'
  const blob = new Blob([bom + rows.join('\n')], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  URL.revokeObjectURL(url)
}

// 대시보드 데이터와 AI 인사이트를 병렬로 로드
onMounted(() => {
  loadAll()
  loadInsight()
})
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">

    <!-- 헤더 -->
    <header class="border-b border-slate-800 bg-slate-900/80 backdrop-blur sticky top-0 z-10">
      <div class="max-w-screen-2xl mx-auto px-6 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold text-sm">P</div>
          <div>
            <h1 class="text-lg font-bold tracking-tight">PharmSight AI</h1>
            <p class="text-xs text-slate-500">약국 경영 통합 AI 대시보드</p>
          </div>
        </div>
        <div class="flex items-center gap-2 sm:gap-4">
          <span class="hidden sm:inline text-xs text-slate-500 bg-slate-800 px-3 py-1.5 rounded-full">
            {{ currentDateLabel }} · Supabase 연동
          </span>
          <!-- 연결 상태 배지 -->
          <span v-if="error" class="text-xs bg-rose-900/50 text-rose-400 border border-rose-800 px-3 py-1.5 rounded-full">
            ⚠ API 오류 · 샘플 데이터
          </span>
          <span v-else-if="isLoading" class="text-xs bg-slate-800 text-slate-400 border border-slate-700 px-3 py-1.5 rounded-full animate-pulse">
            ⟳ 데이터 로딩 중...
          </span>
          <span v-else class="text-xs bg-emerald-900/50 text-emerald-400 border border-emerald-800 px-3 py-1.5 rounded-full">
            ● 실시간 연동
          </span>
          <!-- CSV 내보내기 버튼 -->
          <button
            @click="exportToCsv"
            :disabled="isLoading"
            class="flex items-center gap-1.5 text-xs bg-blue-900/40 hover:bg-blue-800/60 text-blue-400 border border-blue-800 hover:border-blue-600 px-3 py-1.5 rounded-full transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
            title="현재 대시보드 데이터를 CSV 파일로 내보냅니다"
          >
            ⬇ <span class="hidden sm:inline">데이터 내보내기</span><span class="sm:hidden">CSV</span>
          </button>
        </div>
      </div>
    </header>

    <main class="max-w-screen-2xl mx-auto px-6 py-6 space-y-6">

      <!-- ── 에러 알림 패널 (사용자 친화적 안내 + 재시도 버튼) ──────────── -->
      <div
        v-if="error && !isErrorDismissed"
        class="flex items-start gap-4 rounded-xl border border-rose-800/50 bg-rose-950/30 px-5 py-4"
        role="alert"
        aria-live="polite"
      >
        <span class="mt-0.5 flex-shrink-0 text-xl">⚠️</span>
        <div class="flex-1 min-w-0">
          <p class="text-sm font-semibold text-rose-300">실시간 데이터 연결 실패</p>
          <p class="mt-1 text-xs text-rose-400">{{ error }}</p>
          <p class="mt-1 text-xs text-slate-400">{{ errorGuideMessage }}</p>
          <p class="mt-2 text-xs text-slate-500">
            💡 현재 샘플 데이터가 표시되고 있습니다. 실제 약국 데이터를 보려면 아래 버튼을 클릭하세요.
          </p>
        </div>
        <div class="flex items-center gap-2 flex-shrink-0">
          <button
            @click="retryLoad"
            :disabled="isLoading"
            class="text-xs bg-rose-900/40 hover:bg-rose-800/60 text-rose-300 border border-rose-700 px-3 py-1.5 rounded-lg transition-colors disabled:opacity-40"
          >
            {{ isLoading ? '로딩 중...' : '다시 시도' }}
          </button>
          <button
            @click="isErrorDismissed = true"
            class="text-slate-600 hover:text-slate-400 transition-colors text-lg leading-none"
            aria-label="알림 닫기"
          >
            ×
          </button>
        </div>
      </div>

      <!-- AI 경영 인사이트 패널 -->
      <AiInsightPanel :insight="insight" :is-loading="aiLoading" :error="aiError" />

      <!-- ── KPI 카드 ──────────────────────────────────────────────────── -->
      <section class="grid grid-cols-2 lg:grid-cols-4 gap-4" aria-label="핵심 경영 지표">
        <!-- 로딩 스켈레톤 -->
        <template v-if="isLoading">
          <div
            v-for="i in 4"
            :key="`kpi-skeleton-${i}`"
            class="animate-pulse bg-slate-900 border border-slate-800 rounded-xl p-5"
          >
            <div class="flex items-start justify-between mb-3">
              <div class="h-3 w-24 rounded bg-slate-700" />
              <div class="h-6 w-6 rounded-full bg-slate-700" />
            </div>
            <div class="h-8 w-28 rounded bg-slate-700 mb-2" />
            <div class="h-3 w-20 rounded bg-slate-800" />
          </div>
        </template>
        <!-- 실제 KPI 카드 -->
        <template v-else>
          <div
            v-for="card in kpiCards"
            :key="card.title"
            class="bg-slate-900 border border-slate-800 rounded-xl p-5 hover:border-slate-700 transition-colors"
          >
            <div class="flex items-start justify-between mb-3">
              <p class="text-xs text-slate-500 font-medium">{{ card.title }}</p>
              <span class="text-xl">{{ card.icon }}</span>
            </div>
            <div class="flex items-end gap-1">
              <span class="text-2xl font-bold text-slate-100">{{ card.value }}</span>
              <span class="text-sm text-slate-400 mb-0.5">{{ card.unit }}</span>
            </div>
            <div class="mt-2 flex items-center gap-1">
              <span
                class="text-xs font-medium"
                :class="card.change >= 0 ? 'text-emerald-400' : 'text-rose-400'"
              >
                {{ card.change >= 0 ? '▲' : '▼' }} {{ Math.abs(card.change) }}%
              </span>
              <span class="text-xs text-slate-600">전월 대비</span>
            </div>
          </div>
        </template>
      </section>

      <!-- ── 차트 행 1: 매출 추이 + ETC/OTC ────────────────────────────── -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <div class="lg:col-span-2 bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4 flex items-start justify-between gap-2">
            <div>
              <h2 class="text-sm font-semibold text-slate-200">월별 매출 및 조제 건수 추이</h2>
              <p class="text-xs text-slate-500 mt-0.5">매출(바) · 조제 건수(선) · 기간 선택 가능</p>
            </div>
            <!-- 기간 필터 버튼 -->
            <div class="flex items-center gap-1 flex-shrink-0" role="group" aria-label="기간 선택">
              <button
                v-for="opt in periodOptions"
                :key="opt.value"
                @click="selectedPeriod = opt.value"
                class="text-xs px-2 py-1 rounded transition-colors"
                :class="selectedPeriod === opt.value
                  ? 'bg-blue-600 text-white'
                  : 'bg-slate-800 text-slate-400 hover:bg-slate-700'"
                :aria-pressed="selectedPeriod === opt.value"
              >
                {{ opt.label }}
              </button>
            </div>
          </div>
          <div class="h-64">
            <!-- 로딩 스켈레톤: 막대 차트 형태 -->
            <div v-if="isLoading" class="animate-pulse h-full rounded-lg bg-slate-800/50 flex items-end gap-1 px-4 pb-4 pt-2">
              <div
                v-for="i in 8" :key="i"
                class="flex-1 rounded-t bg-slate-700"
                :style="`height: ${25 + (i % 4) * 18}%`"
              />
            </div>
            <SalesLineChart v-else :data="filteredMonthlySales" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">ETC vs OTC 매출 비중</h2>
            <p class="text-xs text-slate-500 mt-0.5">전문의약품(ETC) vs 일반의약품(OTC)</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-center justify-center">
              <div class="w-40 h-40 rounded-full bg-slate-800 border-8 border-slate-700" />
            </div>
            <DrugTypePieChart v-else :data="dashboardData.drugTypeSales" />
          </div>
        </div>
      </section>

      <!-- ── 차트 행 2: 연령대 + 병원별 ──────────────────────────────────── -->
      <section class="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">방문 환자 연령대 분포</h2>
            <p class="text-xs text-slate-500 mt-0.5">연령대별 내원 환자 수</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 8" :key="i" class="flex-1 rounded-t bg-slate-800" :style="`height: ${15 + i * 9}%`" />
            </div>
            <PatientAgeChart v-else :data="dashboardData.patientAgeGroups" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">처방전 발행 의료기관 TOP 6</h2>
            <p class="text-xs text-slate-500 mt-0.5">기관별 처방전 유입 건수</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 6" :key="i" class="flex-1 rounded-t bg-slate-800" :style="`height: ${35 + (6 - i) * 11}%`" />
            </div>
            <HospitalBarChart v-else :data="dashboardData.hospitalPrescriptions" />
          </div>
        </div>
      </section>

      <!-- ── 차트 행 3: 도매상 지출 + 급여/비급여 ───────────────────────── -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <div class="lg:col-span-2 bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">도매상별 누적 지출 현황</h2>
            <p class="text-xs text-slate-500 mt-0.5">거래 도매상별 의약품 발주 지출액</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-end gap-2 pb-4">
              <div v-for="i in 5" :key="i" class="flex-1 rounded-t bg-slate-800" :style="`height: ${25 + (5 - i) * 13}%`" />
            </div>
            <WholesaleBarChart v-else :data="dashboardData.wholesaleExpenses" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">급여 · 비급여 지출 비율</h2>
            <p class="text-xs text-slate-500 mt-0.5">건강보험 적용 여부별 지출</p>
          </div>
          <div class="h-64">
            <div v-if="isLoading" class="animate-pulse h-full flex items-center justify-center">
              <div class="w-40 h-40 rounded-full bg-slate-800 border-8 border-slate-700" />
            </div>
            <DrugCoverageChart v-else :data="dashboardData.drugCoverage" />
          </div>
        </div>
      </section>

    </main>

    <!-- 푸터 -->
    <footer class="border-t border-slate-800 mt-8 py-4">
      <p class="text-center text-xs text-slate-600">
        PharmSight AI © 2026 · Powered by Google Gemini AI · Supabase 실데이터 연동
      </p>
    </footer>
  </div>
</template>
