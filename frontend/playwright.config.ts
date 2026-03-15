import { defineConfig, devices } from '@playwright/test'

/**
 * Playwright E2E 테스트 설정
 * 배포된 프로덕션 서비스에 대한 실제 브라우저 E2E 테스트를 수행합니다.
 *
 * [테스트 대상]
 * - 프론트엔드: https://pharm-sight-frontend.vercel.app
 * - 백엔드 API: https://pharm-sight.onrender.com
 *
 * [실행 방법]
 * npx playwright test              # 전체 E2E 테스트
 * npx playwright test --reporter=html  # HTML 리포트 생성
 */
export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [['html', { open: 'never' }], ['list']],
  timeout: 60_000,

  use: {
    baseURL: 'https://pharm-sight-frontend.vercel.app',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'mobile-chrome',
      use: { ...devices['Pixel 5'] },
    },
  ],
})
