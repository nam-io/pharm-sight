<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { BarChart } from 'echarts/charts'
import { GridComponent, TooltipComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { HospitalPrescription } from '@/types'

use([BarChart, GridComponent, TooltipComponent, CanvasRenderer])

const props = defineProps<{ data: HospitalPrescription[] }>()

const option = computed(() => {
  const sorted = [...props.data].sort((a, b) => a.count - b.count)
  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      backgroundColor: '#1e293b',
      borderColor: '#334155',
      textStyle: { color: '#e2e8f0' },
      formatter: '{b}: <b>{c}건</b>',
    },
    grid: { left: '3%', right: '8%', top: '4%', bottom: '3%', containLabel: true },
    xAxis: {
      type: 'value',
      axisLabel: { color: '#64748b' },
      splitLine: { lineStyle: { color: '#1e293b' } },
    },
    yAxis: {
      type: 'category',
      data: sorted.map(d => d.hospitalName),
      axisLabel: { color: '#94a3b8', fontSize: 11 },
      axisLine: { lineStyle: { color: '#334155' } },
    },
    series: [
      {
        type: 'bar',
        data: sorted.map((d) => ({
          value: d.count,
          itemStyle: {
            color: {
              type: 'linear',
              x: 0, y: 0, x2: 1, y2: 0,
              colorStops: [
                { offset: 0, color: '#1d4ed8' },
                { offset: 1, color: '#3b82f6' },
              ],
            },
            borderRadius: [0, 4, 4, 0],
          },
        })),
        barMaxWidth: 20,
        label: { show: true, position: 'right', color: '#94a3b8', formatter: '{c}건' },
      },
    ],
  }
})
</script>

<template>
  <VChart :option="option" autoresize class="w-full h-full" />
</template>
