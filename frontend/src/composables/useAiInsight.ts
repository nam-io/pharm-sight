/**
 * @composable useAiInsight
 * @description Google Gemini AI가 생성한 약국 경영 인사이트를 백엔드 API에서 조회하는 composable.
 *
 * [에러 처리 전략 — Graceful Degradation]
 * AI 기능은 대시보드의 부가 기능입니다.
 * 실패해도 전체 화면을 차단하지 않고 AiInsightPanel에 안내 메시지만 표시합니다.
 * 나머지 6개 차트와 KPI 카드는 정상 동작합니다.
 *
 * 에러 유형별 처리:
 *   NETWORK (TypeError: Failed to fetch / AbortError: 타임아웃)
 *     → AbortController 15초 타임아웃 (Gemini API 응답 지연 고려, 대시보드의 10초보다 여유)
 *     → AI는 부가 기능이므로 재시도 없음 — 실패 시 즉시 오류 메시지 표시
 *
 *   API (HTTP 4xx/5xx)
 *     → 원인: Gemini API 키 미설정, 할당량 초과, 백엔드 오류
 *     → 백엔드가 키 미설정 시 안내 메시지가 담긴 정상 응답을 반환하므로
 *        이 케이스는 백엔드 자체 오류일 때만 발생합니다.
 *
 *   PARSE (JSON 역직렬화 실패)
 *     → AiInsight 타입과 응답 형식 불일치
 *
 * [폴백 동작]
 * VITE_API_BASE_URL이 없으면 API 호출 자체를 생략합니다 (로컬 개발 환경 대응).
 * 백엔드에서 Gemini API 키가 없으면 안내 메시지가 담긴 AiInsight 객체를 반환합니다
 * (백엔드 레벨 Graceful Degradation).
 *
 * [로깅]
 * 구조화된 컨텍스트 객체: { errorType, message, timestamp, fallback, raw }
 *
 * @throws 에러를 throw하지 않습니다. 오류 상태는 반환되는 `error` / `errorType` ref로 전파됩니다.
 */
import { ref } from 'vue'
import type { AiInsight } from '@/types/api'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''

// ── 상수 ─────────────────────────────────────────────────────────────────
/**
 * AI 요청 타임아웃 (ms).
 * Gemini API 응답이 2~5초 소요되므로 대시보드(10초)보다 여유 있게 설정합니다.
 * Named HttpClient "Gemini"의 백엔드 타임아웃(30초)보다는 짧게 설정합니다.
 */
const AI_REQUEST_TIMEOUT_MS = 15_000

// ── 에러 분류 ─────────────────────────────────────────────────────────────
/** AI 에러 유형. UI에서 유형별 메시지를 다르게 표시할 수 있습니다. */
export type AiErrorType = 'NETWORK' | 'API' | 'PARSE'

interface ClassifiedAiError {
  type: AiErrorType
  message: string
}

/**
 * AI 요청 예외를 유형별로 분류합니다.
 * AbortError(타임아웃)는 NETWORK로 분류합니다.
 */
function classifyAiError(e: unknown): ClassifiedAiError {
  if (e instanceof DOMException && e.name === 'AbortError') {
    return {
      type: 'NETWORK',
      message: `AI 서버 응답 지연 (${AI_REQUEST_TIMEOUT_MS / 1000}초 초과). 잠시 후 새로고침해주세요.`,
    }
  }
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
  /** 에러 유형 — UI에서 NETWORK/API/PARSE에 따라 다른 안내 문구를 표시할 수 있습니다. */
  const errorType = ref<AiErrorType | null>(null)

  async function loadInsight() {
    // VITE_API_BASE_URL 미설정 시 API 호출 생략 (로컬 개발 환경 대응)
    if (!API_BASE) return

    isLoading.value = true
    error.value = null
    errorType.value = null

    // AbortController로 AI_REQUEST_TIMEOUT_MS 타임아웃 적용
    const controller = new AbortController()
    const timeoutId = setTimeout(() => controller.abort(), AI_REQUEST_TIMEOUT_MS)

    try {
      const res = await fetch(`${API_BASE}/api/ai/insight`, { signal: controller.signal })
      if (!res.ok) throw new Error(`AI 분석 오류 [${res.status}]`)
      insight.value = await res.json()
    } catch (e) {
      const classified = classifyAiError(e)
      error.value = classified.message
      errorType.value = classified.type
      // 구조화된 에러 로깅: 에러 유형 · 발생 시각 · 폴백 상태 포함
      console.error('[useAiInsight] AI 인사이트 로드 실패', {
        errorType: classified.type,     // NETWORK | API | PARSE
        message: classified.message,
        timestamp: new Date().toISOString(),
        fallback: 'AiInsightPanel 오류 메시지 표시 (나머지 차트 정상 동작)',
        raw: e,
      })
    } finally {
      clearTimeout(timeoutId) // 성공/실패 무관하게 타이머 정리
      isLoading.value = false
    }
  }

  return { insight, isLoading, error, errorType, loadInsight }
}
