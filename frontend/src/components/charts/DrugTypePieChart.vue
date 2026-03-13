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
    orient: 'vertical',
    right: '5%',
    top: 'center',
    textStyle: { color: '#94a3b8' },
  },
  series: [
    {
      type: 'pie',
      radius: ['45%', '72%'],
      center: ['42%', '50%'],
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
  <VChart :option="option" autoresize class="w-full h-full" />
</template>
