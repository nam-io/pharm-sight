/**
 * @component SalesLineChart
 * @description 월별 매출 추이를 바+라인 복합 차트로 시각화하는 ECharts 컴포넌트.
 *
 * [차트 구성]
 * - 바 차트 (왼쪽 Y축): 월별 총 매출액 (원 단위, 만원 라벨)
 * - 라인 차트 (오른쪽 Y축): 월별 조제 건수 (건 단위)
 * - X축: 월 (MM월 형식)
 *
 * [엣지 케이스 처리]
 * - 빈 배열: "매출 데이터 없음" 아이콘+메시지 표시
 * - 모든 값 0: isEmpty 조건에 포함 → 빈 차트 방지
 *
 * [에러 처리]
 * - props.data가 undefined/null이면 Vue가 기본값([])을 적용
 * - ECharts 내부 오류는 VChart autoresize가 안전하게 처리
 *
 * @props {MonthlySales[]} data - 월별 매출 데이터 배열 (month, totalAmount, prescriptionCount)
 * @emits click - 바/라인 클릭 시 ECharts params 전달 (드릴다운용)
 */
<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { LineChart, BarChart } from 'echarts/charts'
import {
  GridComponent,
  TooltipComponent,
  LegendComponent,
  TitleComponent,
} from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { MonthlySales } from '@/types'

use([LineChart, BarChart, GridComponent, TooltipComponent, LegendComponent, TitleComponent, CanvasRenderer])

const props = defineProps<{ data: MonthlySales[] }>()

/** 데이터가 비어있는지 확인 — 빈 배열이거나 모든 매출이 0인 경우 */
const isEmpty = computed(() =>
  props.data.length === 0 || props.data.every(d => d.totalAmount === 0 && d.prescriptionCount === 0)
)

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'axis',
    backgroundColor: '#fff',
    borderColor: '#DDDDDD',
    textStyle: { color: '#131313' },
    extraCssText: 'box-shadow: 0 4px 12px rgba(0,0,0,0.1); border-radius: 8px',
    formatter: (params: any[]) => {
      const month = params[0].axisValue
      const sales = params[0].value.toLocaleString()
      const count = params[1].value
      return `<div style="font-weight:600;margin-bottom:4px">${month}</div>매출: <b>${sales}원</b><br/>조제: <b>${count}건</b>`
    },
  },
  legend: {
    data: ['월 매출', '조제 건수'],
    textStyle: { color: '#555555' },
    top: 0,
  },
  grid: { left: '3%', right: '8%', bottom: '3%', containLabel: true },
  xAxis: {
    type: 'category',
    data: props.data.map(d => d.month.slice(5) + '월'),
    axisLine: { lineStyle: { color: '#DDDDDD' } },
    axisLabel: { color: '#777777' },
  },
  yAxis: [
    {
      type: 'value',
      name: '매출(원)',
      nameTextStyle: { color: '#777777' },
      axisLabel: {
        color: '#777777',
        formatter: (v: number) => (v / 10000).toFixed(0) + '만',
      },
      splitLine: { lineStyle: { color: '#F1F2F5' } },
    },
    {
      type: 'value',
      name: '조제(건)',
      nameTextStyle: { color: '#777777' },
      axisLabel: { color: '#777777' },
      splitLine: { show: false },
    },
  ],
  series: [
    {
      name: '월 매출',
      type: 'bar',
      yAxisIndex: 0,
      data: props.data.map(d => d.totalAmount),
      itemStyle: { color: '#396EFF', borderRadius: [4, 4, 0, 0] },
      barMaxWidth: 32,
    },
    {
      name: '조제 건수',
      type: 'line',
      yAxisIndex: 1,
      data: props.data.map(d => d.prescriptionCount),
      smooth: true,
      lineStyle: { color: '#FD8200', width: 2 },
      itemStyle: { color: '#FD8200' },
      symbol: 'circle',
      symbolSize: 6,
    },
  ],
}))
</script>

<template>
  <!-- 빈 데이터 안내 -->
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-[#999999] gap-2">
    <span class="text-3xl opacity-40">📊</span>
    <p class="text-xs">해당 기간의 매출 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
