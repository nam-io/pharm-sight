/**
 * @composable useAiInsight
 * @description Google Gemini AI가 생성한 약국 경영 인사이트를 백엔드 API에서 조회하는 composable.
 *
 * [에러 처리 전략]
 * AI 기능은 대시보드의 부가 기능이므로 실패해도 전체 화면을 차단하지 않습니다.
 * 오류 시 AiInsightPanel에 안내 메시지만 표시하고 나머지 차트는 정상 동작합니다 (Graceful Degradation).
 *
 * 에러 유형별 처리:
 *   - NETWORK (TypeError: Failed to fetch): 네트워크 연결 오류
 *   - API (HTTP 4xx/5xx): 백엔드 AI 서비스 오류 (API 키 미설정, Gemini 할당량 초과 등)
 *   - PARSE: JSON 역직렬화 실패
 *
 * [폴백 동작]
 * VITE_API_BASE_URL이 없으면 API 호출 자체를 생략합니다.
 * 백엔드에서 Gemini API 키가 없으면 안내 메시지가 담긴 AiInsight 객체를 반환합니다.
 *
 * [로깅]
 * 구조화된 컨텍스트 객체로 에러 유형과 발생 시각을 기록합니다.
 *
 * @throws 에러를 throw하지 않습니다. 오류 상태는 반환되는 `error` ref를 통해 전파됩니다.
 */
import { ref } from 'vue'
import type { AiInsight } from '@/types/api'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''

// ── 에러 분류 ─────────────────────────────────────────────────────────────
type AiErrorType = 'NETWORK' | 'API' | 'PARSE'

function classifyAiError(e: unknown): { type: AiErrorType; message: string } {
  if (e instanceof TypeError && e.message.toLowerCase().includes('fetch')) {
    return { type: 'NETWORK', message: 'AI 서버에 연결할 수 없습니다.' }
  }
  if (e instanceof Error && e.message.startsWith('AI 분석 오류')) {
    return { type: 'API', message: e.message }
  }
  return { type: 'PARSE', message: 'AI 분석 결과를 처리하지 못했습니다.' }
}

export function useAiInsight() {
  const insight = ref<AiInsight | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function loadInsight() {
    // VITE_API_BASE_URL 미설정 시 API 호출 생략 (로컬 개발 환경 대응)
    if (!API_BASE) return

    isLoading.value = true
    error.value = null
    try {
      const res = await fetch(`${API_BASE}/api/ai/insight`)
      if (!res.ok) throw new Error(`AI 분석 오류 [${res.status}]`)
      insight.value = await res.json()
    } catch (e) {
      const classified = classifyAiError(e)
      error.value = classified.message
      // 구조화된 에러 로깅: 에러 유형 · 발생 시각 · 폴백 상태 포함
      console.error('[useAiInsight] AI 인사이트 로드 실패', {
        errorType: classified.type,     // NETWORK | API | PARSE
        message: classified.message,
        timestamp: new Date().toISOString(),
        fallback: 'AiInsightPanel 오류 메시지 표시',
        raw: e,
      })
    } finally {
      isLoading.value = false
    }
  }

  return { insight, isLoading, error, loadInsight }
}
