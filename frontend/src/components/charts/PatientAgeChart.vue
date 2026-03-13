<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { PieChart } from 'echarts/charts'
import { TooltipComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { PatientAgeGroup } from '@/types'

use([PieChart, TooltipComponent, LegendComponent, CanvasRenderer])

const props = defineProps<{ data: PatientAgeGroup[] }>()

/** 빈 데이터 엣지 케이스 */
const isEmpty = computed(() =>
  props.data.length === 0 || props.data.every(d => d.count === 0)
)

const COLORS = ['#6366f1','#8b5cf6','#a78bfa','#3b82f6','#06b6d4','#10b981','#f59e0b','#f97316']

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'item',
    backgroundColor: '#1e293b',
    borderColor: '#334155',
    textStyle: { color: '#e2e8f0' },
    formatter: '{b}: {c}명 ({d}%)',
  },
  legend: {
    orient: 'vertical',
    right: '3%',
    top: 'middle',
    textStyle: { color: '#94a3b8', fontSize: 11 },
  },
  series: [
    {
      type: 'pie',
      radius: ['38%', '65%'],
      center: ['40%', '50%'],
      label: { show: false },
      data: props.data.map((d, i) => ({
        name: d.ageGroup,
        value: d.count,
        itemStyle: { color: COLORS[i % COLORS.length] },
      })),
      emphasis: {
        label: { show: true, fontSize: 13, fontWeight: 'bold', color: '#fff' },
        itemStyle: { shadowBlur: 10, shadowColor: 'rgba(0,0,0,0.5)' },
      },
    },
  ],
}))
</script>

<template>
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-slate-500 gap-2">
    <span class="text-3xl opacity-40">👥</span>
    <p class="text-xs">환자 연령대 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
