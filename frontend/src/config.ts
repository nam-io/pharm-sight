/**
 * @file 프론트엔드 전역 설정 (config.ts)
 * @description API 호출, 타임아웃, 재시도 관련 설정값을 중앙 관리합니다.
 *
 * [설계 원칙]
 * 하드코딩을 지양하고 설정값을 한 곳에서 관리하여 유지보수성을 확보합니다.
 * 환경변수(VITE_*)는 빌드 타임에 인라인되므로 런타임 변경이 불가합니다.
 * 따라서 타임아웃·재시도 등 튜닝이 필요한 값은 이 파일에서 관리합니다.
 */

// ── API 연결 설정 ────────────────────────────────────────────────────────
/** 백엔드 API Base URL (Vite 빌드 타임 주입) */
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? ''

/** API 연결 여부 — URL 미설정 시 Mock 데이터로 Graceful Degradation */
export const USE_MOCK = !API_BASE_URL

// ── 대시보드 API 설정 ────────────────────────────────────────────────────
/**
 * 대시보드 API 요청 타임아웃 (ms).
 * 7개 API 병렬 호출 기준, Render 무료 플랜 콜드스타트(~5초) 고려하여 10초로 설정.
 */
export const DASHBOARD_TIMEOUT_MS = 10_000

/**
 * NETWORK 에러 자동 재시도 횟수.
 * API/PARSE 에러는 서버 측 문제이므로 재시도하지 않습니다.
 */
export const MAX_NETWORK_RETRIES = 1

/** NETWORK 에러 재시도 전 대기 시간 (ms). */
export const RETRY_DELAY_MS = 500

// ── AI 인사이트 API 설정 ─────────────────────────────────────────────────
/**
 * AI 인사이트 요청 타임아웃 (ms).
 * Gemini API 응답이 2~5초 소요되므로 대시보드(10초)보다 여유 있게 15초로 설정.
 * 백엔드 Named HttpClient "Gemini"의 타임아웃(30초)보다는 짧게 설정합니다.
 */
export const AI_TIMEOUT_MS = 15_000

// ── Keep-Alive 설정 ─────────────────────────────────────────────────────
/**
 * Render 무료 플랜 슬립 방지 핑 주기 (ms).
 * Render 무료 플랜은 15분 비활성 시 슬립 → 10분 간격으로 헬스체크 핑 전송.
 */
export const KEEP_ALIVE_INTERVAL_MS = 10 * 60 * 1000
