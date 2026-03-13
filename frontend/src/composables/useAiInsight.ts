/**
 * @composable useAiInsight
 * @description Google Gemini AI가 생성한 약국 경영 인사이트를 백엔드 API에서 조회하는 composable.
 * VITE_API_BASE_URL이 없으면 요청을 건너뜁니다.
 */
import { ref } from 'vue'
import type { AiInsight } from '@/types/api'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''

export function useAiInsight() {
  const insight = ref<AiInsight | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function loadInsight() {
    if (!API_BASE) return

    isLoading.value = true
    error.value = null
    try {
      const res = await fetch(`${API_BASE}/api/ai/insight`)
      if (!res.ok) throw new Error(`AI 분석 오류 [${res.status}]`)
      insight.value = await res.json()
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'AI 분석을 불러오지 못했습니다.'
      console.error('[useAiInsight] AI 인사이트 로드 실패:', e)
    } finally {
      isLoading.value = false
    }
  }

  return { insight, isLoading, error, loadInsight }
}
