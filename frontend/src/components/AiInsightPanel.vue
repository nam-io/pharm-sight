<script setup lang="ts">
import { computed } from 'vue'
import type { AiInsight } from '@/types/api'

const props = defineProps<{
  insight: AiInsight | null
  isLoading: boolean
  error: string | null
}>()

const generatedAtLabel = computed(() => {
  if (!props.insight) return ''
  const d = new Date(props.insight.generatedAt)
  return `${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')} 분석`
})
</script>

<template>
  <section
    class="relative overflow-hidden rounded-xl border border-blue-800/40 bg-gradient-to-br from-slate-900 via-blue-950/60 to-indigo-950/80 p-6"
  >
    <!-- 배경 장식 -->
    <div class="pointer-events-none absolute -right-16 -top-16 h-48 w-48 rounded-full bg-blue-500/5" />
    <div class="pointer-events-none absolute -bottom-10 -left-10 h-32 w-32 rounded-full bg-indigo-500/5" />

    <!-- 로딩 스켈레톤 -->
    <div v-if="isLoading" class="animate-pulse">
      <div class="mb-4 flex items-center gap-2">
        <div class="h-5 w-5 rounded-full bg-slate-700" />
        <div class="h-4 w-40 rounded bg-slate-700" />
        <div class="ml-auto h-3 w-20 rounded bg-slate-800" />
      </div>
      <div class="mb-2 h-3 w-full rounded bg-slate-800" />
      <div class="mb-4 h-3 w-4/5 rounded bg-slate-800" />
      <div class="mb-4 flex gap-2">
        <div class="h-6 w-24 rounded-full bg-slate-800" />
        <div class="h-6 w-28 rounded-full bg-slate-800" />
        <div class="h-6 w-20 rounded-full bg-slate-800" />
      </div>
      <div class="h-8 w-full rounded-lg bg-slate-800/60" />
    </div>

    <!-- 에러 / API 키 미설정 -->
    <div v-else-if="error || !insight" class="flex items-center gap-3 text-slate-500">
      <span class="text-xl opacity-60">✨</span>
      <div>
        <p class="text-xs font-semibold text-slate-400">PharmSight AI 경영 분석</p>
        <p class="mt-0.5 text-xs">{{ error ?? 'AI 분석 기능이 준비 중입니다.' }}</p>
      </div>
    </div>

    <!-- AI 인사이트 본문 -->
    <div v-else>
      <!-- 헤더 -->
      <div class="mb-3 flex items-center gap-2">
        <span class="text-base">✨</span>
        <h2 class="text-sm font-semibold text-blue-300">PharmSight AI 경영 분석</h2>
        <span class="ml-auto text-xs text-slate-600">{{ generatedAtLabel }}</span>
      </div>

      <!-- 요약 -->
      <p class="mb-4 text-sm leading-relaxed text-slate-200">{{ insight.summary }}</p>

      <!-- 하이라이트 · 경고 배지 -->
      <div class="mb-4 flex flex-wrap gap-2">
        <span
          v-for="h in insight.highlights"
          :key="h"
          class="rounded-full border border-emerald-800/50 bg-emerald-900/40 px-2.5 py-1 text-xs text-emerald-400"
        >
          ✓ {{ h }}
        </span>
        <span
          v-for="w in insight.warnings"
          :key="w"
          class="rounded-full border border-amber-800/50 bg-amber-900/40 px-2.5 py-1 text-xs text-amber-400"
        >
          ⚠ {{ w }}
        </span>
      </div>

      <!-- 추천 -->
      <div class="flex items-start gap-2 rounded-lg bg-blue-900/30 px-3 py-2.5">
        <span class="mt-0.5 flex-shrink-0 text-blue-400">💡</span>
        <p class="text-xs leading-relaxed text-blue-200">{{ insight.recommendation }}</p>
      </div>
    </div>
  </section>
</template>
