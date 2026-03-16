<script setup lang="ts">
/**
 * @component AiInsightPanel
 * @description AI 경영 분석 결과를 표시하는 패널 컴포넌트.
 *
 * [사용자 흐름]
 * 1. 로딩 중 → 스켈레톤 UI 표시 (사용자가 콘텐츠 영역을 예측 가능)
 * 2. 성공 → 요약 + 하이라이트 배지 + 경고 배지 + 추천 액션 표시
 * 3. 에러 → 에러 유형별 안내 메시지 + [다시 시도] 버튼 제공
 * 4. AI 미설정 → "준비 중" 안내 (Graceful Degradation)
 *
 * AI 기능은 대시보드의 부가 기능이므로, 실패해도 나머지 6개 차트와 KPI는 정상 동작합니다.
 */
import { computed } from 'vue'
import type { AiInsight } from '@/types/api'

const props = defineProps<{
  insight: AiInsight | null
  isLoading: boolean
  error: string | null
}>()

const emit = defineEmits<{
  retry: []
}>()

const generatedAtLabel = computed(() => {
  if (!props.insight) return ''
  const d = new Date(props.insight.generatedAt)
  return `${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')} 분석`
})
</script>

<template>
  <section
    class="relative overflow-hidden rounded-xl border border-[#396EFF]/20 bg-gradient-to-br from-white to-[#EEF3FF] p-6"
    aria-label="AI 경영 분석 패널"
  >
    <!-- 배경 장식 -->
    <div class="pointer-events-none absolute -right-16 -top-16 h-48 w-48 rounded-full bg-[#396EFF]/5" />
    <div class="pointer-events-none absolute -bottom-10 -left-10 h-32 w-32 rounded-full bg-[#396EFF]/3" />

    <!-- ── 상태 1: 로딩 스켈레톤 ────────────────────────────────────────── -->
    <div v-if="isLoading" class="animate-pulse" aria-busy="true" aria-label="AI 분석 로딩 중">
      <div class="mb-4 flex items-center gap-2">
        <div class="h-5 w-5 rounded-full bg-[#E8E8E8]" />
        <div class="h-4 w-40 rounded bg-[#E8E8E8]" />
        <div class="ml-auto h-3 w-20 rounded bg-[#F0F0F0]" />
      </div>
      <div class="mb-2 h-3 w-full rounded bg-[#F0F0F0]" />
      <div class="mb-4 h-3 w-4/5 rounded bg-[#F0F0F0]" />
      <div class="mb-4 flex gap-2">
        <div class="h-6 w-24 rounded-full bg-[#F0F0F0]" />
        <div class="h-6 w-28 rounded-full bg-[#F0F0F0]" />
        <div class="h-6 w-20 rounded-full bg-[#F0F0F0]" />
      </div>
      <div class="h-8 w-full rounded-lg bg-[#F0F0F0]" />
    </div>

    <!-- ── 상태 2: 에러 발생 — 안내 메시지 + 재시도 버튼 ────────────────── -->
    <div v-else-if="error" class="flex items-start gap-3">
      <span class="text-xl opacity-60 mt-0.5 flex-shrink-0">⚠️</span>
      <div class="flex-1 min-w-0">
        <p class="text-xs font-semibold text-[#FD8200]">AI 경영 분석 일시 중단</p>
        <p class="mt-1 text-xs text-[#555555]">{{ error }}</p>
        <p class="mt-1 text-xs text-[#777777]">AI 분석은 부가 기능입니다. 아래 차트와 KPI는 정상 동작합니다.</p>
        <button
          @click="emit('retry')"
          class="mt-3 text-xs bg-[#EEF3FF] hover:bg-[#396EFF] hover:text-white text-[#396EFF] border border-[#396EFF]/30 px-3 py-1.5 rounded-lg transition-colors"
        >
          다시 분석 요청
        </button>
      </div>
    </div>

    <!-- ── 상태 3: AI 미설정 / 데이터 없음 ───────────────────────────────── -->
    <div v-else-if="!insight" class="flex items-center gap-3 text-[#777777]">
      <span class="text-xl opacity-60">✨</span>
      <div>
        <p class="text-xs font-semibold text-[#555555]">PharmSight AI 경영 분석</p>
        <p class="mt-0.5 text-xs">AI 분석 기능이 준비 중입니다. Gemini API 키 설정 후 이용할 수 있습니다.</p>
      </div>
    </div>

    <!-- ── 상태 4: 정상 — AI 인사이트 표시 ───────────────────────────────── -->
    <div v-else>
      <!-- 헤더 -->
      <div class="mb-3 flex items-center gap-2">
        <span class="text-base">✨</span>
        <h2 class="text-sm font-semibold text-[#396EFF]">PharmSight AI 경영 분석</h2>
        <span class="ml-auto text-xs text-[#999999]">{{ generatedAtLabel }}</span>
      </div>

      <!-- 요약 -->
      <p class="mb-4 text-sm leading-relaxed text-[#343434]">{{ insight.summary }}</p>

      <!-- 하이라이트 + 경고 배지 -->
      <div v-if="insight.highlights.length > 0 || insight.warnings.length > 0" class="mb-4 flex flex-wrap gap-2">
        <span
          v-for="h in insight.highlights"
          :key="'h-' + h"
          class="rounded-full border border-[#28A745]/30 bg-[#F0FFF4] px-2.5 py-1 text-xs text-[#28A745]"
        >
          ✓ {{ h }}
        </span>
        <span
          v-for="w in insight.warnings"
          :key="'w-' + w"
          class="rounded-full border border-[#FD8200]/30 bg-[#FFF8F0] px-2.5 py-1 text-xs text-[#FD8200]"
        >
          ⚠ {{ w }}
        </span>
      </div>

      <!-- 추천 액션 -->
      <div v-if="insight.recommendation" class="flex items-start gap-2 rounded-lg bg-[#EEF3FF] px-3 py-2.5">
        <span class="mt-0.5 flex-shrink-0 text-[#396EFF]">💡</span>
        <p class="text-xs leading-relaxed text-[#555555]">{{ insight.recommendation }}</p>
      </div>
    </div>
  </section>
</template>
