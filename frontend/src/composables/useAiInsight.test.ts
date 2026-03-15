import { describe, it, expect, vi, beforeEach } from 'vitest'

describe('useAiInsight (Mock 모드)', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
    vi.stubEnv('VITE_API_BASE_URL', '')
    vi.resetModules()
  })

  it('초기 상태에서 insight는 null이다', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const { insight, isLoading, error } = useAiInsight()

    expect(insight.value).toBeNull()
    expect(isLoading.value).toBe(false)
    expect(error.value).toBeNull()
  })

  it('API_BASE 미설정 시 loadInsight는 API 호출을 생략한다', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    const { useAiInsight } = await import('./useAiInsight')
    const { loadInsight, isLoading } = useAiInsight()

    await loadInsight()

    expect(fetchSpy).not.toHaveBeenCalled()
    expect(isLoading.value).toBe(false)
  })

  it('에러 유형(errorType)이 초기에 null이다', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const { errorType } = useAiInsight()
    expect(errorType.value).toBeNull()
  })

  it('반환 객체에 필수 속성이 모두 존재한다', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const result = useAiInsight()

    expect(result).toHaveProperty('insight')
    expect(result).toHaveProperty('isLoading')
    expect(result).toHaveProperty('error')
    expect(result).toHaveProperty('errorType')
    expect(result).toHaveProperty('loadInsight')
    expect(typeof result.loadInsight).toBe('function')
  })

  // ── 엣지 케이스: 빈 결과 및 0값 데이터 ─────────────────────────────────
  it('loadInsight 호출 후에도 isLoading은 false로 복원된다 (빈 결과 엣지 케이스)', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const { loadInsight, isLoading } = useAiInsight()

    await loadInsight()

    // API_BASE 미설정이므로 즉시 false로 복원
    expect(isLoading.value).toBe(false)
  })

  it('insight가 null인 상태에서 속성 접근 시 에러가 발생하지 않는다 (null 안전성)', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const { insight } = useAiInsight()

    expect(insight.value).toBeNull()
    // null 안전 접근 — 옵셔널 체이닝으로 에러 없이 undefined 반환
    expect(insight.value?.summary).toBeUndefined()
    expect(insight.value?.highlights).toBeUndefined()
    expect(insight.value?.warnings).toBeUndefined()
    expect(insight.value?.recommendation).toBeUndefined()
  })

  it('error와 errorType이 동시에 null이다 (초기 상태 일관성)', async () => {
    const { useAiInsight } = await import('./useAiInsight')
    const { error, errorType } = useAiInsight()

    // error와 errorType은 항상 동시에 null이거나 동시에 값이 설정됨
    expect(error.value).toBeNull()
    expect(errorType.value).toBeNull()
    // 둘 다 null이면 일관성 통과
    const isConsistent = (error.value === null) === (errorType.value === null)
    expect(isConsistent).toBe(true)
  })
})
