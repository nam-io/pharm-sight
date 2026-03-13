<script setup lang="ts">
import { useDashboardData } from '@/composables/useDashboardData'
import SalesLineChart from '@/components/charts/SalesLineChart.vue'
import DrugTypePieChart from '@/components/charts/DrugTypePieChart.vue'
import PatientAgeChart from '@/components/charts/PatientAgeChart.vue'
import HospitalBarChart from '@/components/charts/HospitalBarChart.vue'
import WholesaleBarChart from '@/components/charts/WholesaleBarChart.vue'
import DrugCoverageChart from '@/components/charts/DrugCoverageChart.vue'

const { dashboardData, kpiCards } = useDashboardData()
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">

    <!-- 헤더 -->
    <header class="border-b border-slate-800 bg-slate-900/80 backdrop-blur sticky top-0 z-10">
      <div class="max-w-screen-2xl mx-auto px-6 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold text-sm">P</div>
          <div>
            <h1 class="text-lg font-bold tracking-tight">PharmSight</h1>
            <p class="text-xs text-slate-500">약국 경영 통합 대시보드</p>
          </div>
        </div>
        <div class="flex items-center gap-4">
          <span class="text-xs text-slate-500 bg-slate-800 px-3 py-1.5 rounded-full">
            📅 2025년 2월 기준 · Mock 데이터
          </span>
          <span class="text-xs bg-emerald-900/50 text-emerald-400 border border-emerald-800 px-3 py-1.5 rounded-full">
            ● 실시간 연동 준비 중
          </span>
        </div>
      </div>
    </header>

    <main class="max-w-screen-2xl mx-auto px-6 py-6 space-y-6">

      <!-- KPI 카드 -->
      <section class="grid grid-cols-2 lg:grid-cols-4 gap-4">
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
      </section>

      <!-- 차트 그리드 행 1: 매출 추이 (넓게) + Rx/OTC 비중 -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <div class="lg:col-span-2 bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">월별 매출 및 조제 건수 추이</h2>
            <p class="text-xs text-slate-500 mt-0.5">최근 12개월 매출(바) · 조제 건수(선)</p>
          </div>
          <div class="h-64">
            <SalesLineChart :data="dashboardData.monthlySales" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">Rx vs OTC 매출 비중</h2>
            <p class="text-xs text-slate-500 mt-0.5">전문의약품 vs 일반의약품</p>
          </div>
          <div class="h-64">
            <DrugTypePieChart :data="dashboardData.drugTypeSales" />
          </div>
        </div>
      </section>

      <!-- 차트 그리드 행 2: 연령대 + 병원별 -->
      <section class="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">방문 환자 연령대 분포</h2>
            <p class="text-xs text-slate-500 mt-0.5">연령대별 내원 환자 수</p>
          </div>
          <div class="h-64">
            <PatientAgeChart :data="dashboardData.patientAgeGroups" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">처방전 발행 의료기관 TOP 6</h2>
            <p class="text-xs text-slate-500 mt-0.5">기관별 처방전 유입 건수</p>
          </div>
          <div class="h-64">
            <HospitalBarChart :data="dashboardData.hospitalPrescriptions" />
          </div>
        </div>
      </section>

      <!-- 차트 그리드 행 3: 도매상 지출 + 급여/비급여 -->
      <section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <div class="lg:col-span-2 bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">도매상별 누적 지출 현황</h2>
            <p class="text-xs text-slate-500 mt-0.5">거래 도매상별 의약품 발주 지출액</p>
          </div>
          <div class="h-64">
            <WholesaleBarChart :data="dashboardData.wholesaleExpenses" />
          </div>
        </div>

        <div class="bg-slate-900 border border-slate-800 rounded-xl p-5">
          <div class="mb-4">
            <h2 class="text-sm font-semibold text-slate-200">급여 · 비급여 지출 비율</h2>
            <p class="text-xs text-slate-500 mt-0.5">건강보험 적용 여부별 지출</p>
          </div>
          <div class="h-64">
            <DrugCoverageChart :data="dashboardData.drugCoverage" />
          </div>
        </div>
      </section>

    </main>

    <!-- 푸터 -->
    <footer class="border-t border-slate-800 mt-8 py-4">
      <p class="text-center text-xs text-slate-600">
        PharmSight © 2025 · 해커톤 데모 · Mock 데이터 기반
      </p>
    </footer>
  </div>
</template>
