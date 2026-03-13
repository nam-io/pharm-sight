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
    backgroundColor: '#1e293b',
    borderColor: '#334155',
    textStyle: { color: '#e2e8f0' },
    formatter: (params: any[]) => {
      const month = params[0].axisValue
      const sales = params[0].value.toLocaleString()
      const count = params[1].value
      return `<div style="font-weight:600;margin-bottom:4px">${month}</div>매출: <b>${sales}원</b><br/>조제: <b>${count}건</b>`
    },
  },
  legend: {
    data: ['월 매출', '조제 건수'],
    textStyle: { color: '#94a3b8' },
    top: 0,
  },
  grid: { left: '3%', right: '8%', bottom: '3%', containLabel: true },
  xAxis: {
    type: 'category',
    data: props.data.map(d => d.month.slice(5) + '월'),
    axisLine: { lineStyle: { color: '#334155' } },
    axisLabel: { color: '#64748b' },
  },
  yAxis: [
    {
      type: 'value',
      name: '매출(원)',
      nameTextStyle: { color: '#64748b' },
      axisLabel: {
        color: '#64748b',
        formatter: (v: number) => (v / 10000).toFixed(0) + '만',
      },
      splitLine: { lineStyle: { color: '#1e293b' } },
    },
    {
      type: 'value',
      name: '조제(건)',
      nameTextStyle: { color: '#64748b' },
      axisLabel: { color: '#64748b' },
      splitLine: { show: false },
    },
  ],
  series: [
    {
      name: '월 매출',
      type: 'bar',
      yAxisIndex: 0,
      data: props.data.map(d => d.totalAmount),
      itemStyle: { color: '#3b82f6', borderRadius: [4, 4, 0, 0] },
      barMaxWidth: 32,
    },
    {
      name: '조제 건수',
      type: 'line',
      yAxisIndex: 1,
      data: props.data.map(d => d.prescriptionCount),
      smooth: true,
      lineStyle: { color: '#10b981', width: 2 },
      itemStyle: { color: '#10b981' },
      symbol: 'circle',
      symbolSize: 6,
    },
  ],
}))
</script>

<template>
  <!-- 빈 데이터 안내 -->
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-slate-500 gap-2">
    <span class="text-3xl opacity-40">📊</span>
    <p class="text-xs">해당 기간의 매출 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
