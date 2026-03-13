# Sprint 1: 프론트엔드 UI 개발 및 Vercel 배포

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 1 |
| **연결된 Phase** | Phase 1: 프론트엔드 UI 개발 및 Vercel 배포 (보너스 10점 타겟) |
| **목표 시간** | 1h ~ 3h (120분 이내 완료) |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | ✅ 완료 |
| **작업 브랜치** | `sprint/sprint1` |
| **배포 URL** | https://pharm-sight-frontend.vercel.app (완료) |

---

## 목표 (Goal)

Mock 데이터 기반으로 약국 경영 대시보드 UI를 완성하고, Vercel에 정적 배포하여 데모 URL을 확보한다.

- `AppLayout` 컴포넌트가 6개 ECharts 패널을 포함한 대시보드 화면을 렌더링
- `useDashboardData.ts` composable이 현실감 있는 Mock 데이터를 제공
- `npm run build` 성공 및 Vercel 배포 URL 접속 가능
- `README.md` 최상단에 배포 URL 기록 완료

---

## 범위 (Scope)

### In Scope
- `frontend/src/layouts/AppLayout.vue`: 사이드바 또는 상단 헤더를 포함한 전체 레이아웃 컴포넌트
- `frontend/src/composables/useDashboardData.ts`: 6개 차트의 Mock 데이터 및 타입 정의
- `frontend/src/components/charts/`: 6개 ECharts 패널 컴포넌트 (각 차트 유형별 파일)
- `frontend/src/views/DashboardView.vue`: 레이아웃과 패널을 조립하는 뷰 컴포넌트
- `frontend/src/App.vue`: DashboardView를 마운트하도록 업데이트
- Vercel 배포 및 `README.md` URL 기록

### Out of Scope
- 실제 백엔드 API 연동 → Phase 2에서 처리
- Axios 또는 HTTP 클라이언트 설정 → Phase 2에서 처리
- 날짜/기간 필터 기능 → Backlog
- 사용자 인증/로그인 → Won't Have

---

## 작업 분해 (Task Breakdown)

### T1-1: useDashboardData.ts 작성 (20분)

**파일:** `frontend/src/composables/useDashboardData.ts`

**구현 내용:**
- 6개 차트 각각의 데이터 타입 인터페이스 정의 (TypeScript)
- 현실감 있는 약국 경영 Mock 데이터 상수 정의
- `useDashboardData()` composable 함수로 데이터 반환

**Mock 데이터 명세:**

| 차트 | 타입 | 데이터 구조 |
|------|------|------------|
| 월별 매출·조제건수 추이 | 라인 차트 | `{ month: string, revenue: number, dispensingCount: number }[]` (12개월) |
| Rx vs OTC 매출 비중 | 파이 차트 | `{ name: string, value: number }[]` (Rx/OTC 2개) |
| 방문 환자 연령대 분포 | 도넛 차트 | `{ name: string, value: number }[]` (0~9세/10대/…/70대+) |
| 처방전 발행 의료기관별 유입 | 바 차트 | `{ hospital: string, count: number }[]` (5~7개 의료기관) |
| 도매상별 누적 지출 현황 | 바 차트 | `{ wholesaler: string, amount: number }[]` (4~6개 도매상) |
| 급여/비급여 지출 비율 | 파이 차트 | `{ name: string, value: number }[]` (급여/비급여 2개) |

**완료 조건:** `import { useDashboardData } from '@/composables/useDashboardData'` 호출 시 오류 없이 데이터 반환

---

### T1-2: AppLayout.vue 구현 (15분)

**파일:** `frontend/src/layouts/AppLayout.vue`

**구현 내용:**
- 상단 헤더: `PharmSight` 타이틀, 약국명(고정 텍스트), 현재 날짜 표시
- 메인 콘텐츠 영역: `<slot />`으로 하위 뷰를 렌더링
- Tailwind CSS v4 유틸리티 클래스 활용 (`bg-gray-50`, `shadow`, `px-6` 등)

**완료 조건:** 헤더가 화면 상단에 고정되고 슬롯 콘텐츠가 하단에 표시됨

---

### T1-3: ECharts 패널 컴포넌트 6개 구현 (50분)

**디렉토리:** `frontend/src/components/charts/`

#### T1-3-1: MonthlySalesTrendChart.vue (월별 매출·조제건수 추이)
- **차트 유형:** 라인 차트 (이중 Y축)
- **ECharts 옵션 핵심:**
  ```
  yAxis: [{ name: '매출(원)' }, { name: '조제건수' }]
  series: [{ type: 'line', name: '총 매출' }, { type: 'line', name: '조제 건수', yAxisIndex: 1 }]
  ```
- **Props:** `data: { month: string, revenue: number, dispensingCount: number }[]`

#### T1-3-2: RxOtcSalesRatioChart.vue (Rx vs OTC 매출 비중)
- **차트 유형:** 파이 차트
- **ECharts 옵션 핵심:** `series[0].type: 'pie'`, 범례(Rx/OTC) 표시
- **Props:** `data: { name: string, value: number }[]`

#### T1-3-3: PatientAgeDistributionChart.vue (방문 환자 연령대 분포)
- **차트 유형:** 도넛 차트 (파이 차트의 `radius: ['40%', '70%']`)
- **Props:** `data: { name: string, value: number }[]`

#### T1-3-4: HospitalReferralChart.vue (처방전 발행 의료기관별 유입 건수)
- **차트 유형:** 가로 바 차트 (`xAxis.type: 'value'`, `yAxis.type: 'category'`)
- **Props:** `data: { hospital: string, count: number }[]`

#### T1-3-5: WholesalerExpenditureChart.vue (도매상별 누적 지출 현황)
- **차트 유형:** 세로 바 차트 (`xAxis.type: 'category'`)
- **Props:** `data: { wholesaler: string, amount: number }[]`

#### T1-3-6: CoverageRatioChart.vue (급여/비급여 지출 비율)
- **차트 유형:** 파이 차트
- **Props:** `data: { name: string, value: number }[]`

**공통 구현 규칙:**
- 각 컴포넌트는 `vue-echarts`의 `<v-chart>` 컴포넌트를 사용
- `use-client-directive` 비활성화: SSR 미사용이므로 `VChart` 전역 등록
- `option`은 `computed`로 Props 데이터를 ECharts 형식으로 변환
- 차트 높이: `class="h-72"` 또는 `style="height: 288px"` 고정
- 툴팁(`tooltip`) 기본 활성화

**완료 조건:** 각 컴포넌트에서 `<v-chart :option="option" />` 렌더링 시 차트 정상 표시

---

### T1-4: DashboardView.vue 구현 (15분)

**파일:** `frontend/src/views/DashboardView.vue`

**구현 내용:**
- `useDashboardData()` composable에서 6개 데이터 세트 추출
- 6개 패널을 `grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6` 레이아웃으로 배치
- 각 패널은 카드 형태의 래퍼(`bg-white rounded-xl shadow p-4`)로 감쌈
- 카드 상단에 차트 제목 표시

**레이아웃 구조:**
```
DashboardView
└── 카드 그리드 (2열/3열 반응형)
    ├── 카드 1: MonthlySalesTrendChart (월별 매출·조제건수)
    ├── 카드 2: RxOtcSalesRatioChart (Rx vs OTC)
    ├── 카드 3: PatientAgeDistributionChart (연령대 분포)
    ├── 카드 4: HospitalReferralChart (의료기관별 유입)
    ├── 카드 5: WholesalerExpenditureChart (도매상별 지출)
    └── 카드 6: CoverageRatioChart (급여/비급여)
```

**완료 조건:** 브라우저에서 6개 차트가 그리드 레이아웃으로 모두 표시됨

---

### T1-5: App.vue 업데이트 (5분)

**파일:** `frontend/src/App.vue`

**변경 내용:**
- 기존 `HelloWorld` 컴포넌트 임포트 제거
- `AppLayout`과 `DashboardView`를 조합하여 렌더링
- 기존 `style scoped` 내 불필요한 로고 스타일 제거

**완료 조건:** `npm run dev` 실행 후 로컬호스트에서 대시보드 화면 확인

---

### T1-6: Vercel 배포 (10분) — 완료

**상태:** 완료

**배포 URL:** https://pharm-sight-frontend.vercel.app

**완료 조건:**
- Vercel 배포 URL 접속 시 대시보드 화면 정상 표시 ✅
- `README.md` 최상단에 배포 URL 기록 ✅

---

### T1-7: ECharts 전역 컴포넌트 등록 (5분)

**파일:** `frontend/src/main.ts`

**구현 내용:**
- `vue-echarts`에서 `VChart` 임포트
- ECharts 필요 기능(라인, 파이, 바, 그리드, 툴팁, 범례) 개별 등록 (`use()`)
- `app.component('VChart', VChart)` 전역 등록

**완료 조건:** 모든 차트 컴포넌트에서 별도 임포트 없이 `<v-chart>` 태그 사용 가능

---

## 기술 접근법 (Technical Approach)

### vue-echarts 사용 방식

```typescript
// main.ts에서 전역 등록
import VChart, { THEME_KEY } from 'vue-echarts'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { LineChart, BarChart, PieChart } from 'echarts/charts'
import {
  GridComponent, TooltipComponent, LegendComponent, TitleComponent
} from 'echarts/components'

use([CanvasRenderer, LineChart, BarChart, PieChart,
     GridComponent, TooltipComponent, LegendComponent, TitleComponent])

app.component('VChart', VChart)
```

### composable 패턴 (useDashboardData.ts)

```typescript
// JSDoc 주석 필수 (CLAUDE.md 규칙)
/**
 * @description 대시보드 6개 패널의 Mock 데이터를 제공하는 composable
 * @returns 각 차트 데이터 객체 (monthlySales, rxOtcRatio, ageDistribution, ...)
 */
export function useDashboardData() {
  // reactive 또는 ref로 반응형 데이터 래핑
  // 실제 백엔드 연동 시 이 함수 내부만 교체하면 됨 (인터페이스 유지)
}
```

### Tailwind CSS v4 클래스 활용

- 헤더: `bg-indigo-700 text-white px-6 py-4 shadow-lg`
- 카드 래퍼: `bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow`
- 그리드: `grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6 p-6`

### TypeScript 타입 설계

```typescript
// 공유 타입은 composable 파일 내 또는 types/ 폴더에 정의
export interface MonthlySalesData {
  month: string      // 'YYYY-MM' 형식
  revenue: number    // 원 단위
  dispensingCount: number
}

export interface ChartPieData {
  name: string
  value: number
}
```

---

## 의존성 및 리스크 (Dependencies & Risks)

| 항목 | 내용 | 대응 방안 |
|------|------|----------|
| **의존성** | Sprint 0 완료 (Vue 3 + Vite + ECharts 설치) | Sprint 0 package.json 확인 완료 |
| **리스크 1** | ECharts 트리셰이킹 설정 누락 시 번들 크기 증가 | `use()` 함수로 필요 컴포넌트만 개별 등록 |
| **리스크 2** | `vue-echarts` v8.x API 변경 | 공식 문서 기준 `VChart` 컴포넌트 방식 사용 |
| **리스크 3** | Tailwind v4 클래스 자동완성 미작동 | `@tailwindcss/vite` 플러그인 확인, IDX/VSCode 재시작 |
| **리스크 4** | Vercel 빌드 실패 (`vue-tsc` 타입 오류) | `npm run build` 로컬 선행 검증 필수 |

---

## 완료 조건 (Definition of Done)

- [x] `frontend/src/composables/useDashboardData.ts` 작성 완료, 6개 차트 Mock 데이터 반환
- [x] `frontend/src/layouts/AppLayout.vue` 구현 완료 (헤더 + 슬롯)
- [x] `frontend/src/components/charts/` 내 6개 차트 컴포넌트 구현 완료
  - [x] `MonthlySalesTrendChart.vue` (라인 차트)
  - [x] `RxOtcSalesRatioChart.vue` (파이 차트)
  - [x] `PatientAgeDistributionChart.vue` (도넛 차트)
  - [x] `HospitalReferralChart.vue` (가로 바 차트)
  - [x] `WholesalerExpenditureChart.vue` (세로 바 차트)
  - [x] `CoverageRatioChart.vue` (파이 차트)
- [x] `frontend/src/views/DashboardView.vue` 구현 완료 (6개 패널 그리드 배치)
- [x] `frontend/src/App.vue` 업데이트 완료 (DashboardView 마운트)
- [x] `frontend/src/main.ts` ECharts 전역 컴포넌트 등록 완료
- [x] `npm run build` 성공 (TypeScript 오류 0건, 빌드 경고 없음)
- [x] Vercel 배포 URL(https://pharm-sight-frontend.vercel.app) 접속 시 6개 차트 정상 표시
- [x] `README.md` 최상단에 배포 URL 기록 완료
- [x] `docs/sprint/sprint1.md` 문서 작성 완료 (현재 파일)

---

## 산출물 (Deliverables)

| 파일/폴더 | 설명 |
|-----------|------|
| `frontend/src/composables/useDashboardData.ts` | 6개 차트 Mock 데이터 및 타입 정의 composable |
| `frontend/src/layouts/AppLayout.vue` | 헤더 포함 전체 레이아웃 컴포넌트 |
| `frontend/src/components/charts/MonthlySalesTrendChart.vue` | 월별 매출·조제건수 라인 차트 |
| `frontend/src/components/charts/RxOtcSalesRatioChart.vue` | Rx vs OTC 매출 파이 차트 |
| `frontend/src/components/charts/PatientAgeDistributionChart.vue` | 연령대 분포 도넛 차트 |
| `frontend/src/components/charts/HospitalReferralChart.vue` | 의료기관별 유입 가로 바 차트 |
| `frontend/src/components/charts/WholesalerExpenditureChart.vue` | 도매상별 지출 세로 바 차트 |
| `frontend/src/components/charts/CoverageRatioChart.vue` | 급여/비급여 파이 차트 |
| `frontend/src/views/DashboardView.vue` | 6개 패널 그리드 조립 뷰 |
| `frontend/src/App.vue` | DashboardView 마운트 (업데이트) |
| `frontend/src/main.ts` | ECharts 전역 등록 (업데이트) |
| Vercel 배포 | https://pharm-sight-frontend.vercel.app |

---

## 다음 단계 (Next Phase)

Sprint 1 완료 후 **Phase 2 (3h ~ 6h)** 시작:
- `backend/` 에서 Dapper Repository 통계 쿼리 구현 (CTE, GROUP BY)
- Service 계층 비즈니스 로직 및 전역 에러 처리(`GlobalExceptionMiddleware`) 구현
- `PharmSight.Tests` (xUnit, Moq) 단위 테스트 작성
- (선택) 프론트엔드 `useDashboardData.ts`의 Mock 데이터를 실제 API 호출로 교체
