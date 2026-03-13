<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { PieChart } from 'echarts/charts'
import { TooltipComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { DrugTypeSales } from '@/types'

use([PieChart, TooltipComponent, LegendComponent, CanvasRenderer])

const props = defineProps<{ data: DrugTypeSales[] }>()

/** 빈 데이터 엣지 케이스: 배열이 비어있거나 모든 금액이 0인 경우 */
const isEmpty = computed(() =>
  props.data.length === 0 || props.data.every(d => d.amount === 0)
)

const COLORS = ['#3b82f6', '#10b981']

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'item',
    backgroundColor: '#1e293b',
    borderColor: '#334155',
    textStyle: { color: '#e2e8f0' },
    formatter: '{b}: {c}원 ({d}%)',
  },
  legend: {
    orient: 'horizontal',
    bottom: 0,
    left: 'center',
    textStyle: { color: '#94a3b8' },
  },
  series: [
    {
      type: 'pie',
      radius: ['45%', '72%'],
      center: ['50%', '45%'],
      avoidLabelOverlap: false,
      label: {
        show: true,
        position: 'inside',
        formatter: '{d}%',
        color: '#fff',
        fontWeight: 'bold',
        fontSize: 13,
      },
      data: props.data.map((d, i) => ({
        name: d.label,
        value: d.amount,
        itemStyle: { color: COLORS[i] },
      })),
      emphasis: {
        itemStyle: { shadowBlur: 10, shadowOffsetX: 0, shadowColor: 'rgba(0,0,0,0.5)' },
      },
    },
  ],
}))
</script>

<template>
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-slate-500 gap-2">
    <span class="text-3xl opacity-40">💊</span>
    <p class="text-xs">의약품 매출 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
