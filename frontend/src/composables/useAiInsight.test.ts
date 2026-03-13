import { describe, it, expect, vi, beforeEach } from 'vitest'

// API_BASE 미설정 상태 모킹 (모듈 임포트 전에 설정)
vi.stubEnv('VITE_API_BASE_URL', '')

describe('useAiInsight (Mock 모드)', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
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
})
