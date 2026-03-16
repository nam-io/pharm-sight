/**
 * @component HospitalBarChart
 * @description 처방전 발행 의료기관 TOP 6 수평 바 차트.
 * 약국의 병원 의존도 파악에 핵심적인 차트 — 매출 집중 리스크 시각화.
 *
 * [차트 구성]
 * - 수평 바: 처방 건수 오름차순 정렬, 리니어 그라디언트 컬러
 * - 우측 라벨: "N건" 형식 표시
 *
 * [엣지 케이스] 빈 배열 또는 모든 count 0 → "데이터 없음" UI 표시
 * [에러 처리] 공통 isEmpty 패턴 적용, sorted 복사본으로 원본 불변성 보장
 *
 * @props {HospitalPrescription[]} data - 기관별 처방 건수 (hospitalName, count)
 */
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

/** 빈 데이터 엣지 케이스 */
const isEmpty = computed(() =>
  props.data.length === 0 || props.data.every(d => d.count === 0)
)

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
  <div v-if="isEmpty" class="flex flex-col items-center justify-center h-full text-slate-500 gap-2">
    <span class="text-3xl opacity-40">🏥</span>
    <p class="text-xs">처방 기관 데이터가 없습니다.</p>
  </div>
  <VChart v-else :option="option" autoresize class="w-full h-full" />
</template>
