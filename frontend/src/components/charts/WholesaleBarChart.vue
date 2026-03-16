/**
 * @component WholesaleBarChart
 * @description 도매상별 누적 지출 현황 세로 바 차트. 비용 구조 파악용.
 *
 * [차트 구성]
 * - 세로 바: 지출액 내림차순 정렬, 오렌지 계열 5색 팔레트
 * - 상단 라벨: "N만" 형식 (만원 단위 변환)
 * - Y축: 만원 단위 포맷
 *
 * [엣지 케이스] 빈 배열 또는 모든 amount 0 → "데이터 없음" UI 표시
 * [에러 처리] 공통 isEmpty 패턴, sorted 복사본으로 원본 불변성 보장
 *
 * @props {WholesaleExpense[]} data - 도매상별 지출 (wholesaleName, amount)
 */
<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { BarChart } from 'echarts/charts'
import { GridComponent, TooltipComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { WholesaleExpense } from '@/types'

use([BarChart, GridComponent, TooltipComponent, CanvasRenderer])

const props = defineProps<{ data: WholesaleExpense[] }>()

/** 빈 데이터 엣지 케이스 */
const isEmpty = computed(() =>
  props.data.length === 0 || props.data.every(d => d.amount === 0)
)

const COLORS = ['#f97316', '#fb923c', '#fdba74', '#fcd34d', '#fde68a']

const option = computed(() => {
  const sorted = [...props.data].sort((a, b) => b.amount - a.amount)
  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      backgroundColor: '#1e293b',
      borderColor: '#334155',
      textStyle: { color: '#e2e8f0' },
      formatter: (params: any[]) =>
        `${params[0].name}<br/><b>${(params[0].value / 10000).toLocaleString()}만원</b>`,
    },
    grid: { left: '3%', right: '5%', top: '4%', bottom: '3%', containLabel: true },
    xAxis: {
      type: 'category',
      data: sorted.map(d => d.wholesaleName),
      axisLabel: { color: '#94a3b8', fontSize: 10, interval: 0 },
      axisLine: { lineStyle: { color: '#334155' } },
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: '#64748b',
        formatter: (v: number) => (v / 10000).toFixed(0) + '만',
      },
      splitLine: { lineStyle: { color: '#1e293b' } },
    },
    series: [
      {
        type: 'bar',
        data: sorted.map((d, i) => ({
          value: d.amount,
          itemStyle: {
            color: COLORS[i % COLORS.length],
            borderRadius: [4, 4, 0, 0],
          },
        })),
        barMaxWidth: 40,
        label: {
          show: true,
          position: 'top',
          color: '#94a3b8',
          formatter: (p: any) => (p.value / 10000).toLocaleString() + '만',
          fontSize: 10,
        },
      },
    ],
  }
})
</script>

<template>
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-slate-500 gap-2">
    <span class="text-3xl opacity-40">📦</span>
    <p class="text-xs">도매상 지출 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
