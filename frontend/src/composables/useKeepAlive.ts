/**
 * @composable useKeepAlive
 * @description Render 무료 플랜의 15분 비활성 슬립을 방지하기 위해
 * 백엔드 헬스체크 엔드포인트를 10분마다 자동 호출합니다.
 */
import { onMounted, onUnmounted } from 'vue'

const PING_INTERVAL_MS = 10 * 60 * 1000 // 10분
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''

async function ping() {
  if (!API_BASE) return
  try {
    const res = await fetch(`${API_BASE}/api/health`)
    if (res.ok) {
      console.info(`[KeepAlive] 백엔드 핑 성공 ${new Date().toLocaleTimeString('ko-KR')}`)
    }
  } catch {
    console.warn('[KeepAlive] 백엔드 핑 실패 - 서버 슬립 중일 수 있습니다.')
  }
}

export function useKeepAlive() {
  let timer: ReturnType<typeof setInterval> | null = null

  onMounted(() => {
    ping() // 즉시 1회 호출
    timer = setInterval(ping, PING_INTERVAL_MS)
  })

  onUnmounted(() => {
    if (timer !== null) clearInterval(timer)
  })
}
