import { describe, it, expect } from 'vitest'
import {
  DASHBOARD_TIMEOUT_MS,
  MAX_NETWORK_RETRIES,
  RETRY_DELAY_MS,
  AI_TIMEOUT_MS,
  KEEP_ALIVE_INTERVAL_MS,
} from './config'

describe('config — 프론트엔드 설정값 검증', () => {
  it('대시보드 타임아웃은 양수이다', () => {
    expect(DASHBOARD_TIMEOUT_MS).toBeGreaterThan(0)
  })

  it('AI 타임아웃은 대시보드 타임아웃보다 크다', () => {
    // Gemini API 응답이 더 느리므로 AI 타임아웃이 더 길어야 함
    expect(AI_TIMEOUT_MS).toBeGreaterThan(DASHBOARD_TIMEOUT_MS)
  })

  it('재시도 횟수는 0 이상이다', () => {
    expect(MAX_NETWORK_RETRIES).toBeGreaterThanOrEqual(0)
  })

  it('재시도 대기 시간은 양수이다', () => {
    expect(RETRY_DELAY_MS).toBeGreaterThan(0)
  })

  it('Keep-Alive 주기는 Render 슬립 시간(15분)보다 짧다', () => {
    const RENDER_SLEEP_MS = 15 * 60 * 1000
    expect(KEEP_ALIVE_INTERVAL_MS).toBeLessThan(RENDER_SLEEP_MS)
  })
})
