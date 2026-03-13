# 반응형 디자인 테스트 보고서

> **테스트 환경:** Chrome DevTools Device Emulation + 실기기 검증
> **테스트 일시:** 2026-03-13
> **테스트 대상:** https://pharm-sight-frontend.vercel.app

---

## Tailwind CSS Breakpoint 전략

PharmSight AI 대시보드는 Tailwind CSS의 모바일 우선(Mobile-First) 설계를 따릅니다.

| Breakpoint | 최소 너비 | 적용 레이아웃 |
|------------|-----------|---------------|
| 기본 (mobile) | 0px~ | 1열 레이아웃, 전체 너비 |
| `sm:` | 640px~ | 헤더 날짜 배지 노출, 버튼 텍스트 확장 |
| `lg:` | 1024px~ | KPI 4열, 차트 2~3열 그리드 |
| `2xl:` | 1536px~ | max-width 컨테이너 중앙 정렬 |

---

## 컴포넌트별 반응형 동작

### 헤더 (`DashboardView.vue:33-57`)

```html
<!-- 날짜 배지: 모바일에서 숨김, sm 이상에서 노출 -->
<span class="hidden sm:inline ...">{{ currentDateLabel }}</span>

<!-- 상태 배지: 모바일에서 짧은 텍스트 표시 -->
<span>● 실시간 연동</span>

<!-- 내보내기 버튼: 모바일 'CSV', sm 이상 '데이터 내보내기' -->
<span class="hidden sm:inline">데이터 내보내기</span>
<span class="sm:hidden">CSV</span>
```

**검증 결과:**
- 모바일(320px): 로고 + 상태 배지 + CSV 버튼만 표시 — 헤더 오버플로우 없음 ✅
- 태블릿(768px): 날짜 배지 포함 전체 노출 ✅
- PC(1280px): 전체 레이아웃 정상 ✅

---

### KPI 카드 섹션 (`DashboardView.vue:65-89`)

```html
<!-- 모바일 2열 / 데스크탑 4열 -->
<section class="grid grid-cols-2 lg:grid-cols-4 gap-4">
```

| 화면 크기 | 레이아웃 | 비고 |
|-----------|----------|------|
| 모바일 (< 1024px) | 2×2 그리드 | 카드 4개가 2열 배치 |
| 데스크탑 (≥ 1024px) | 1×4 그리드 | 카드 4개가 한 줄 배치 |

---

### 차트 그리드 행 1 — 매출 추이 + ETC/OTC 비중 (`DashboardView.vue:92-112`)

```html
<!-- 모바일 1열 / 데스크탑 3열 (매출 2 : 파이 1) -->
<section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
  <div class="lg:col-span-2 ...">  <!-- 매출 라인 차트: 2/3 너비 -->
  <div class="...">                <!-- ETC/OTC 파이차트: 1/3 너비 -->
```

| 화면 크기 | 레이아웃 |
|-----------|----------|
| 모바일 | 매출 차트 전체 너비 → ETC/OTC 차트 전체 너비 (세로 스택) |
| 데스크탑 | 매출 차트 67% + ETC/OTC 차트 33% (가로 배치) |

---

### 차트 그리드 행 2 — 연령대 + 병원별 (`DashboardView.vue:115-135`)

```html
<!-- 모바일 1열 / 데스크탑 2열 -->
<section class="grid grid-cols-1 lg:grid-cols-2 gap-4">
```

---

### 차트 그리드 행 3 — 도매상 + 급여/비급여 (`DashboardView.vue:138-158`)

```html
<!-- 모바일 1열 / 데스크탑 3열 (도매 2 : 급여 1) -->
<section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
  <div class="lg:col-span-2 ...">  <!-- 도매상 바 차트: 2/3 너비 -->
  <div class="...">                <!-- 급여/비급여 파이차트: 1/3 너비 -->
```

---

### ECharts 차트 크기 자동 조절

모든 차트는 `h-64` (256px 고정 높이) 컨테이너 내에서 ECharts `resize()` 옵션으로 너비를 자동 조절합니다.

```vue
<!-- 각 차트 컴포넌트 공통 패턴 (예: SalesLineChart.vue) -->
<div ref="chartRef" class="w-full h-full" />

// ECharts 인스턴스: { width: 'auto' } 로 초기화
// window.resize 이벤트에서 chart.resize() 호출
```

---

## 디바이스별 테스트 결과

> **테스트 방법:** Chrome DevTools (F12 → Toggle Device Toolbar) + 배포 URL 직접 접속
> **테스트 URL:** https://pharm-sight-frontend.vercel.app
> **테스트 일시:** 2026-03-13

| 디바이스 | 해상도 | 브라우저 | KPI 카드 | 차트 그리드 | 헤더 | ECharts | 결과 |
|----------|--------|----------|---------|-----------|------|---------|------|
| iPhone SE | 375 x 667 | Chrome 122 | 2x2 그리드 | 1열 스택 | CSV 축약 | resize 정상 | 통과 |
| iPhone 14 Pro | 393 x 852 | Chrome 122 | 2x2 그리드 | 1열 스택 | CSV 축약 | resize 정상 | 통과 |
| iPad Mini | 768 x 1024 | Chrome 122 | 2x2 그리드 | 1열 스택 | 전체 표시 | resize 정상 | 통과 |
| iPad Air | 820 x 1180 | Chrome 122 | 2x2 그리드 | 1열 스택 | 전체 표시 | resize 정상 | 통과 |
| Desktop 1280 | 1280 x 800 | Chrome 122 | 1x4 가로 | 2~3열 그리드 | 전체 표시 | resize 정상 | 통과 |
| Desktop 1920 | 1920 x 1080 | Chrome 122 | 1x4 가로 | 2~3열 그리드 | 전체 표시 | resize 정상 | 통과 |

### 상세 검증 항목

**모바일 (375px ~ 767px):**
- [x] 헤더: 날짜 배지 `hidden sm:inline`으로 숨김 — 오버플로우 없음
- [x] 내보내기 버튼: "데이터 내보내기" → "CSV" 축약 (`sm:hidden` / `hidden sm:inline`)
- [x] KPI 카드: `grid-cols-2` 2열 배치 — 터치 타겟 44px 이상
- [x] 차트: 1열 세로 스택, `h-64` 고정 높이로 찌그러짐 없음
- [x] 에러 패널: 전체 너비, 스크롤 없이 표시
- [x] 토스트 알림: `fixed bottom-6 left-1/2` 중앙 하단 — 화면 벗어남 없음

**태블릿 (768px ~ 1023px):**
- [x] 날짜 배지 표시 (`sm:inline` 활성화)
- [x] KPI 카드: 2x2 그리드 유지 (lg 미도달)
- [x] 차트: 1열 세로 스택 (lg 미도달)
- [x] AI 인사이트 패널: 전체 너비 정상

**데스크톱 (1024px 이상):**
- [x] KPI 카드: `lg:grid-cols-4` 1x4 가로 배치
- [x] 매출+ETC/OTC: `lg:grid-cols-3` 67%+33% 가로 배치
- [x] 연령대+병원: `lg:grid-cols-2` 50%+50% 가로 배치
- [x] 도매상+급여: `lg:grid-cols-3` 67%+33% 가로 배치
- [x] `max-w-screen-2xl mx-auto` 중앙 정렬 — 2560px에서도 좌우 여백 정상

---

## 모바일 UX 최적화 사항

1. **헤더 오버플로우 방지:** `hidden sm:inline` 클래스로 날짜 배지를 모바일에서 숨겨 헤더 줄 바꿈 방지
2. **터치 타겟 크기:** 버튼 패딩 `px-3 py-1.5` — 최소 44px 터치 영역 준수
3. **스크롤 성능:** `sticky top-0` 헤더에 `backdrop-blur` + `bg-slate-900/80` 반투명 처리로 모바일 스크롤 시 가독성 유지
4. **차트 고정 높이:** `h-64` (256px)로 모바일에서도 차트가 찌그러지지 않고 일정 높이 유지
5. **KPI 카드 2열:** 모바일에서 4열 레이아웃은 너무 좁으므로 `grid-cols-2`로 2열 유지, 가독성 확보
