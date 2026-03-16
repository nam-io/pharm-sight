/**
 * @component PatientAgeChart
 * @description 방문 환자 연령대 분포를 8개 연령대별 도넛 차트로 시각화.
 *
 * [차트 구성]
 * - 도넛 차트: 0-9세 ~ 70세 이상 8개 세그먼트, 컬러 팔레트 8색
 * - 범례: 우측 세로 배치, hover 시 라벨 표시
 *
 * [엣지 케이스] 빈 배열 또는 모든 count 0 → "데이터 없음" UI 표시
 * [에러 처리] 공통 패턴: isEmpty computed → 빈 데이터 안내 / VChart autoresize
 *
 * @props {PatientAgeGroup[]} data - 연령대별 환자 수 (ageGroup, count)
 */
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

const COLORS = ['#396EFF','#54B2FF','#FD8200','#FDB44B','#F1636F','#28A745','#9B59B6','#E74C3C']

const option = computed(() => ({
  backgroundColor: 'transparent',
  tooltip: {
    trigger: 'item',
    backgroundColor: '#fff',
    borderColor: '#DDDDDD',
    textStyle: { color: '#131313' },
    extraCssText: 'box-shadow: 0 4px 12px rgba(0,0,0,0.1); border-radius: 8px',
    formatter: '{b}: {c}명 ({d}%)',
  },
  legend: {
    orient: 'vertical',
    right: '3%',
    top: 'middle',
    textStyle: { color: '#555555', fontSize: 11 },
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
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-[#999999] gap-2">
    <span class="text-3xl opacity-40">👥</span>
    <p class="text-xs">환자 연령대 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
