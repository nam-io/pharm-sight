/**
 * @component DrugTypePieChart
 * @description 전문의약품(ETC) vs 일반의약품(OTC) 매출 비중을 도넛 차트로 시각화.
 *
 * [차트 구성]
 * - 도넛 차트: ETC(파랑) / OTC(초록) 2개 세그먼트
 * - 내부 라벨: 퍼센트(%) 표시, 범례: 하단 수평 배치
 *
 * [엣지 케이스] 빈 배열 또는 모든 금액 0 → "데이터 없음" UI 표시
 * [에러 처리] props 미전달 시 Vue 기본값 적용, ECharts 내부 안전 처리
 *
 * @props {DrugTypeSales[]} data - ETC/OTC 매출 데이터 (type, label, amount)
 */
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

const COLORS = ['#396EFF', '#FD8200']

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'item',
    backgroundColor: '#fff',
    borderColor: '#DDDDDD',
    textStyle: { color: '#131313' },
    extraCssText: 'box-shadow: 0 4px 12px rgba(0,0,0,0.1); border-radius: 8px',
    formatter: '{b}: {c}원 ({d}%)',
  },
  legend: {
    orient: 'horizontal',
    bottom: 0,
    left: 'center',
    textStyle: { color: '#555555' },
  },
  series: [
    {
      type: 'pie',
      radius: ['30%', '75%'],
      center: ['50%', '45%'],
      avoidLabelOverlap: false,
      label: {
        show: true,
        position: 'inside',
        formatter: '{d}%',
        color: '#fff',
        fontWeight: 'bold',
        fontSize: 10,
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
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-[#999999] gap-2">
    <span class="text-3xl opacity-40">💊</span>
    <p class="text-xs">의약품 매출 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
