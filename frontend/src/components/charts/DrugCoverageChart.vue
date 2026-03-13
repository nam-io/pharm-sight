<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { PieChart } from 'echarts/charts'
import { TooltipComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import type { DrugCoverage } from '@/types'

use([PieChart, TooltipComponent, LegendComponent, CanvasRenderer])

const props = defineProps<{ data: DrugCoverage[] }>()

const COLORS = ['#10b981', '#f43f5e']

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'item',
    backgroundColor: '#1e293b',
    borderColor: '#334155',
    textStyle: { color: '#e2e8f0' },
    formatter: (p: any) =>
      `${p.name}<br/><b>${(p.value / 10000).toLocaleString()}만원</b> (${p.percent}%)`,
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
        itemStyle: { color: COLORS[i % COLORS.length] },
      })),
      emphasis: {
        itemStyle: { shadowBlur: 10, shadowColor: 'rgba(0,0,0,0.5)' },
      },
    },
  ],
}))
</script>

<template>
  <VChart :option="option" autoresize class="w-full h-full" />
</template>
