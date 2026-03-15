/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  build: {
    rollupOptions: {
      output: {
        /**
         * 번들 분할 전략 (Tree-shaking + Code Splitting)
         * - echarts: 전체 번들의 ~80%를 차지하는 차트 라이브러리를 별도 청크로 분리
         *   → 메인 번들 로드 후 비동기 로드, 초기 로딩 속도 개선
         * - vue-vendor: Vue 런타임을 별도 청크로 분리 → 장기 캐싱 활용
         */
        manualChunks: {
          'echarts-vendor': ['echarts', 'vue-echarts'],
          'vue-vendor': ['vue'],
        },
      },
    },
  },
  test: {
    environment: 'happy-dom',
    globals: true,
    include: ['src/**/*.{test,spec}.{ts,tsx}'],
  },
})
