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
  <VChart :option="option" autoresize class="w-full h-full" />
</template>
