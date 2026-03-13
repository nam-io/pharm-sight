# PharmSight AI — 약국 경영 통합 AI 대시보드

약국의 처방·매출·지출 데이터를 한 화면에서 시각화하고, AI가 경영 인사이트를 자동 분석하여 제공하는 통합 대시보드입니다.

[![CI - 빌드 및 테스트](https://github.com/nam-io/pharm-sight/actions/workflows/ci.yml/badge.svg)](https://github.com/nam-io/pharm-sight/actions/workflows/ci.yml)

![PharmSight AI 대시보드 화면](docs/pharm-sight-intro.png)

> **제품 요구사항 정의서(PRD):** [`docs/PRD.md`](docs/PRD.md) — 기능 요구사항 12개(P0 8개 + P1 4개), 비기능 요구사항(성능/안정성/보안/접근성), 사용자 흐름, 데이터 모델, 제약 조건 정의

---

## 문제 정의 — 개인 약국의 생존을 위협하는 데이터 사각지대

### 매년 1,000개 이상의 약국이 문을 닫고 있습니다

대형 드러그스토어와 체인 약국은 POS 데이터 분석으로 재고·마케팅을 최적화합니다. 반면 **국내 25,000개 약국 중 85%를 차지하는 개인 약국은 여전히 감(感)에 의존해 경영**하고 있습니다. 데이터 분석 능력의 격차가 곧 생존 경쟁력의 격차로 이어지는 시대입니다.

**근본 원인: 3개 시스템에 파편화된 경영 데이터**

| 데이터 | 현재 시스템 | 한계 |
|--------|-------------|------|
| 처방·조제 | 건보 청구 프로그램 | 청구 목적만 — 경영 분석 기능 전무 |
| OTC 판매 | POS 단말기 / 수기 | 처방 데이터와 연계 불가 |
| 도매 발주 | 도매상 전용 앱 | 매출 대비 지출 비율 파악 불가 |

이 세 데이터는 서로 연결되지 않습니다. 약사는 **통합 경영 현황을 한 번도 본 적이 없는 상태로** 매일 의사결정을 내리고 있습니다.

**이 파편화가 만드는 실질적 경영 위협:**

1. **병원 폐원 시 매출 급감 무방비**: 매출의 60%가 인근 소아과 1곳에서 오는데 이를 파악하지 못함 — 인근 의원 폐원 시 매출 40~60% 급감 사례 빈번
2. **마진율을 모른 채 발주 반복**: 도매 지출 총액과 매출을 연결해 본 적 없음 — 체인 약국 대비 수익성 열위 심화
3. **데이터 관리 복잡성 폭증**: 코로나 이후 비대면 처방·배달약국 확대로 관리할 채널이 늘었지만, 기존 수작업 방식으로는 한계 도달

**사용자 검증 — 다층적 근거 확보:**

**1차 검증: 약사 심층 인터뷰 (3인, 2026년 3월 · 서울/경기/인천)**

> *"청구 프로그램은 건보 청구만 돼요. 매출 분석은 따로 엑셀로 하는데, 3~4시간 걸리다가 포기했어요."* — 서울 A약국, 경력 8년차

> *"어느 소아과에서 처방이 많이 오는지 알고 싶은데, 지금 방법으로는 알 수가 없어요."* — 경기 B약국, 개국 5년차

> *"AI가 우리 약국 데이터를 분석해서 요약해 준다면 바로 쓰겠어요."* — 인천 C약국, 경력 12년차

**2차 검증: 공공 데이터 및 시장 통계 기반 정량적 근거**

| 검증 출처 | 데이터 | 시사점 |
|----------|--------|--------|
| 건강보험심사평가원 (2024) | 국내 약국 ~25,000개, 85% 개인 약국 | 대규모 잠재 사용자 기반 확인 |
| 대한약사회 통계 (2024) | 연간 약 1,000개 이상 약국 폐업 | 경영 분석 도구 부재가 생존 위협으로 직결 |
| 보건복지부 (2023) | '약국 디지털 전환 지원사업' 시범 개시, 참여 의향 ~32% | 정책적 수요 확인 + 디지털 전환 의향 약국 ~8,000개 |
| 한국벤처투자 (2023) | 디지털헬스케어 투자 4,200억원 (3년 연속 성장) | 시장 성장세 확인 |
| 경쟁 제품 직접 사용 테스트 | 비소프트/유케어 — 경영 분석 기능 전무 확인 | 약국 경영 BI 시장 공백 실증 ([상세](docs/market-analysis.md#6-경쟁사-직접-사용-검증)) |

**지금 이 문제가 더 시급한 이유:**
- 약국 폐업률 증가 추세 — 데이터 분석 능력이 **생존의 필수 조건**으로 전환
- 대형 체인·드러그스토어 확장 — 개인 약국과의 데이터 분석 격차 확대
- 보건복지부 2023년 '약국 디지털 전환 지원사업' 시범 시작 — 정책적 수요 확인
- ChatGPT 대중화로 AI 도구 수용성 급격 상승 — 약사들의 즉각 도입 의향 확인

> 시장 규모(TAM/SAM/SOM), 경쟁사 심층 분석, 포지셔닝 맵 상세: [`docs/market-analysis.md`](docs/market-analysis.md)

---

## 사용자가 얻는 핵심 가치 — 경쟁 제품과 무엇이 다른가

### Before vs After: 약사가 체감하는 변화

| 약사 | 지금 (Before) | PharmSight 도입 후 (After) |
|------|--------------|---------------------------|
| 박약사 (38세, 소아과 인근) | 매출의 60%가 한 소아과에서 오는데 모름 — 폐원하면 대비 불가 | **병원별 매출 의존도**를 한눈에 확인, 리스크 사전 대비 |
| 김약사 (52세, 복합상권) | 월말 엑셀 정리 3~4시간, 결국 포기 — 경영 현황 파악 불가 | 접속만 하면 **이번 달 경영 현황을 5분 안에** 파악 |
| 이약사 (45세, 개국 2년차) | 도매 3곳 지출 합계를 모름 — 수익성 관리 불가 | **도매상별 지출 차트**로 비용 구조 즉시 파악 |

### 3-Way 경쟁 비교

| 구분 | 청구 프로그램 | 엑셀 수기 관리 | **PharmSight AI** |
|------|-------------|--------------|-------------------|
| 처방+매출+발주 통합 | X (청구만) | 수동 (월 3~4시간) | **자동 통합** |
| 경영 현황 파악 시간 | 불가 | 3~4시간/월 | **5분 이내** |
| AI 경영 요약 | 없음 | 없음 | **매월 자동 요약 + 추천 액션** |
| 병원 의존도 파악 | 불가 | 직접 계산 필요 | **자동 산출** |
| 모바일 접속 | 불가 (PC 설치형) | 불편 | **브라우저 즉시 접속** |
| **월 비용** | **15~30만원** + 초기 설치비 50~100만원 | 무료 (시간 비용 월 3~4시간) | **월 3~5만원 목표** |
| 연간 총비용 (TCO) | 280~460만원 (설치비+월비용+유지보수) | 0원 (단, 약사 시급 환산 시 연 150~200만원) | **36~60만원** |

### PharmSight만의 5가지 차별점

1. **흩어진 3개 시스템 데이터를 한 화면에서 확인** — 엑셀 복사-붙여넣기가 더 이상 필요 없음
2. **교육 없이 5분 내 파악 가능한 직관적 화면** — PC, 태블릿, 스마트폰 어디서든 접속
3. **어느 병원에 매출이 의존되는지, 어떤 연령대 고객이 많은지 숫자로 즉시 확인** — 즉각적 경영 액션이 가능한 약국 전용 지표
4. **AI가 매달 경영 상태를 요약하고, 주의사항과 추천 액션을 자동으로 알려줌** — 데이터를 읽을 줄 몰라도 경영 인사이트를 얻을 수 있음
5. **약국 전용 경영 분석 도구는 국내에 아직 없음** — PharmSight AI가 이 시장 공백을 최초로 겨냥

> 국내 약국 25,000개 중 디지털 전환 의향 ~8,000개 — 약국 전용 경영 BI는 국내 미개척 시장 ([상세 시장 분석](docs/market-analysis.md))

---

## 배포 URL

- **프론트엔드:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app) (Vercel)
- **백엔드 API:** [https://pharm-sight.onrender.com](https://pharm-sight.onrender.com) (Render)
- **데이터베이스:** Supabase PostgreSQL (실데이터 연동)

---

## 주요 기능 (구현 완료)

> **라이브 데모:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app)

| 기능 | 설명 |
|------|------|
| AI 경영 분석 | AI가 이번 달 경영 현황을 2~3문장으로 요약, 주의사항·추천 액션 자동 생성 (30분 캐시). 에러 시 [다시 분석 요청] 버튼 제공 |
| KPI 카드 4종 | 이번 달 매출·조제건수·환자수·발주금액, 전월 대비 변화율 ▲/▼ 색상 표시. 로딩 중 스켈레톤 UI |
| 처방 트렌드 | 월별 매출(바)+조제건수(선) 복합 차트. **기간 필터 3/6/12개월** 실시간 전환 (API 재호출 없이 클라이언트 슬라이싱) |
| 의약품 매출 비중 | 전문의약품(ETC) vs 일반의약품(OTC) 도넛 차트, 퍼센트 라벨 |
| 고객 연령대 분포 | 8개 연령대별 도넛 차트, hover 시 상세 정보 |
| 처방 기관 분석 | 처방전 발행 의료기관별 수평 바 차트, 자동 정렬 |
| 도매상 지출 현황 | 도매상별 누적 지출 바 차트, 만원 단위 라벨 |
| CSV 내보내기 | 6개 섹션 전체를 CSV로 다운로드 (BOM UTF-8 — Excel 한글 호환). **완료 시 토스트 알림 표시** |
| 반응형 레이아웃 | 모바일~PC 자동 적응, Tailwind breakpoint (`sm:`, `lg:`). **6개 디바이스 테스트 통과** ([상세 결과](docs/responsive-testing.md)) |

### 사용자 흐름 및 UX 상세 (`DashboardView.vue`)

```
[초기 접속] → 스켈레톤 로딩 (KPI 4개 + 차트 6개 각각 형태별 스켈레톤)
     │
     ▼
[데이터 로드 성공] → fade-in 트랜지션으로 콘텐츠 전환
     │                 연결 상태 배지: "● 실시간 연동" (emerald)
     │
     ├─ [기간 필터 클릭] → 3/6/12개월 즉시 전환 (선택 버튼 하이라이트)
     ├─ [CSV 내보내기]   → 파일 다운로드 + 토스트 알림 3초 표시
     └─ [차트 호버]      → ECharts 툴팁 (한국어 포맷, 만원 단위)

[데이터 로드 실패] → 에러 유형별 안내 패널 (slide-fade 트랜지션)
     │                 NETWORK: "인터넷 연결 확인" + [다시 시도]
     │                 API:     "서버 오류, 잠시 후 재시도"
     │                 PARSE:   "데이터 형식 오류, 관리자 문의"
     │                 Mock 데이터 자동 폴백 → 서비스 중단 없음
     │                 연결 상태 배지: "⚠ API 오류 · 샘플 데이터" (rose)
     └─ [다시 시도 클릭] → 대시보드 + AI 동시 재로드

[AI 분석 실패] → AiInsightPanel 독립 에러 처리 + [다시 분석 요청] 버튼
                  나머지 6개 차트 + KPI 영향 없음 (Graceful Degradation)
```

**엣지 케이스 대응:**
- 빈 데이터: 6개 차트 모두 `isEmpty` computed로 "데이터 없음" 아이콘+메시지 표시
- 0값 데이터: 모든 금액이 0일 때도 빈 데이터로 처리하여 빈 차트 방지
- AI 미설정: "준비 중" 안내 (에러가 아닌 정보 메시지)
- 네트워크 불안정: NETWORK 에러 500ms 지연 후 1회 자동 재시도 → 실패 시 Mock 폴백

**Vue `<Transition>` 3종 구현 (`DashboardView.vue`):**

```vue
<!-- 1. fade: 연결 상태 배지 3가지 상태 전환 (에러/로딩/연결됨) -->
<Transition name="fade" mode="out-in">
  <span v-if="error" key="error" class="bg-rose-900/50 text-rose-400 ...">API 오류</span>
  <span v-else-if="isLoading" key="loading" class="animate-pulse ...">로딩 중</span>
  <span v-else key="connected" class="bg-emerald-900/50 text-emerald-400 ...">실시간 연동</span>
</Transition>

<!-- 2. slide-fade: 에러 안내 패널 등장/퇴장 (role="alert" aria-live="polite") -->
<Transition name="slide-fade">
  <div v-if="error && !isErrorDismissed" role="alert" aria-live="polite">
    <p>{{ errorGuideMessage }}</p>  <!-- NETWORK/API/PARSE 유형별 차별 안내 -->
    <button @click="retryLoad">다시 시도</button>
  </div>
</Transition>

<!-- 3. toast: CSV 내보내기 완료 알림 (3초 자동 소멸) -->
<Transition name="toast">
  <div v-if="isToastVisible" role="status" aria-live="polite" class="fixed bottom-6 ...">
    {{ toastMessage }}
  </div>
</Transition>
```

```css
/* fade 트랜지션 */
.fade-enter-active, .fade-leave-active { transition: opacity 0.25s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* slide-fade 트랜지션 */
.slide-fade-enter-active { transition: all 0.3s ease-out; }
.slide-fade-enter-from { opacity: 0; transform: translateY(-12px); }

/* toast 트랜지션 */
.toast-enter-active { transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1); }
.toast-enter-from { opacity: 0; transform: translate(-50%, 16px); }
```

---

## 기술 스택 — 모든 선택에는 이유가 있습니다

| 레이어 | 채택 기술 | 선택 근거 — 왜 이 기술인가 |
|--------|---------|---------|
| **Frontend** | Vue 3.5 + TypeScript + Vite 6 | Composition API `<script setup>`으로 Composable 3개 관심사 분리, Vite 즉각 HMR + ESM 네이티브 빌드 |
| **차트** | Apache ECharts 5.6 | 바+라인 복합 차트 네이티브 지원 (Chart.js는 불가), 6종 차트 통일 API, TypeScript 타입 완전 지원 |
| **Backend** | C# .NET 9.0 Web API | `record` 타입으로 DTO 불변성 보장 + 컴파일 타임 검증, `async/await` 7개 쿼리 병렬 실행, 내장 DI 컨테이너로 3계층 인터페이스 주입 |
| **DB** | PostgreSQL (Supabase) | `DATE_TRUNC`·`AGE()`·CTE로 집계를 DB 레벨에서 처리 → 애플리케이션 연산 최소화, 클라우드 영속성 |
| **ORM** | Dapper (순수 SQL) | CTE 3중 집계를 1회 DB 왕복으로 완료 (EF Core는 LINQ 표현 불가→Raw SQL 강제→ORM 이점 없음), Change Tracking 오버헤드 제거 |
| **AI** | Google Gemini API | 무료 1M 토큰/월 + `ListModels` API로 사용 가능 모델 런타임 자동 탐색 (하드코딩 모델명 NOT_FOUND 문제 근본 해결) |
| **Infra** | Vercel + Render + Supabase | 3가지 모두 무료 티어로 해커톤 비용 0원 배포, Git push 자동 배포 |

**`.NET 9.0` 구체적 활용 기능:**
- `record` 타입 DTO (`MonthlySales`, `KpiSummary` 등 7개) — 불변성 + `with` 표현식 + 값 기반 동등성
- `async/await` + `Task.WhenAll` — 7개 DB 쿼리 병렬 실행으로 순차 대비 ~6배 응답 단축
- `IMemoryCache` — AI 인사이트 30분 캐시 (API 할당량 절약 + 응답 2초→10ms)
- `IHttpClientFactory` Named Client — Gemini API 전용 HttpClient 수명 관리
- `switch` 표현식 기반 예외 분류 — `GlobalExceptionMiddleware`에서 예외 타입별 HTTP 상태코드 매핑

> 패키지별 버전·탈락 대안 상세: [`docs/tech-stack.md`](docs/tech-stack.md)

---

## 아키텍처 — Controller → Service → Repository 3계층 + 인터페이스 DI

### 요청 처리 흐름

```
HTTP 요청
    │
    ▼
GlobalExceptionMiddleware   ← 전역 예외 처리: 예외 유형별 400/500 JSON 응답
    │
    ▼
DashboardController         ← Thin Controller: HTTP 수신/응답만 (비즈니스 로직 0)
AiInsightController
    │ IDashboardService / IAiInsightService (인터페이스 DI)
    ▼
DashboardService            ← 비즈니스 로직 + 구조화 로깅
AiInsightService            ← Gemini API + IMemoryCache 30분 캐시 + 동적 모델 선택
    │ IDashboardRepository (인터페이스 DI)
    ▼
DashboardRepository         ← Dapper 순수 SQL (CTE 3중 집계, DATE_TRUNC, AGE())
    │
    ▼
Supabase PostgreSQL
```

### 계층 분리 증거 — 디렉토리 구조

```
backend/
├── Controllers/                    ← 계층 1: HTTP 요청/응답
│   ├── DashboardController.cs      ← IDashboardService 주입, 비즈니스 로직 없음
│   ├── AiInsightController.cs      ← IAiInsightService 주입
│   └── HealthController.cs
├── Services/                       ← 계층 2: 비즈니스 로직
│   ├── DashboardService.cs         ← IDashboardRepository 주입, 로깅
│   ├── AiInsightService.cs         ← Gemini API + 캐시 + 동적 모델 탐색
│   └── Interfaces/
│       ├── IDashboardService.cs    ← Service 인터페이스 (Mock 가능)
│       └── IAiInsightService.cs
├── Repositories/                   ← 계층 3: 데이터 접근
│   ├── DashboardRepository.cs      ← Dapper + NpgsqlConnection
│   └── Interfaces/
│       └── IDashboardRepository.cs ← Repository 인터페이스 (Mock 가능)
├── Middleware/
│   └── GlobalExceptionMiddleware.cs ← 전역 예외→JSON 변환
├── Models/
│   └── DashboardModels.cs          ← record 타입 DTO 7개 (불변 값 객체)
├── Program.cs                      ← Composition Root: DI 등록 + 미들웨어 파이프라인
└── PharmSight.Tests/               ← 35개 xUnit 테스트
    ├── Services/                   ← Service 계층 테스트 (Repository Mock)
    ├── Repositories/               ← Repository 계층 테스트 (URI 변환, 인터페이스)
    ├── Controllers/                ← Controller 계층 테스트 (Service Mock, Thin Controller 검증)
    └── Middleware/                  ← 미들웨어 테스트 (예외→상태코드 매핑)
```

### 인터페이스 기반 DI — `Program.cs`에서 등록

```csharp
// AddScoped: HTTP 요청당 1개 인스턴스 → NpgsqlConnection 수명을 요청 단위로 관리
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAiInsightService, AiInsightService>();
```

**인터페이스 DI의 실제 효과:** `IDashboardRepository`를 Moq로 교체하여 DB 없이 Service 테스트 13개 실행 가능. `IDashboardService`를 Mock하여 Controller Thin 패턴 검증 9개 테스트 실행.

### 프론트엔드 — Vue 3 Composable 관심사 분리

```
frontend/src/
├── composables/                    ← 비즈니스 로직 분리 (관심사별 1 Composable)
│   ├── useDashboardData.ts         ← 7개 API 병렬 호출 + Mock 폴백 + 에러 분류
│   ├── useAiInsight.ts             ← AI API + 15초 타임아웃 + Graceful Degradation
│   └── useKeepAlive.ts             ← Render 슬립 방지 10분 핑
├── config.ts                       ← 설정값 중앙 관리 (타임아웃, 재시도, API URL)
├── components/charts/              ← ECharts 차트 컴포넌트 6종
└── types/api.ts                    ← TypeScript 인터페이스 (백엔드 DTO 계약)
```

### 반응형 디자인 — 6개 디바이스 테스트 결과

| 디바이스 | 해상도 | KPI 카드 | 차트 그리드 | 결과 |
|----------|--------|---------|-----------|------|
| iPhone SE | 375x667 | 2x2 그리드 | 1열 스택 | 통과 |
| iPhone 14 Pro | 393x852 | 2x2 그리드 | 1열 스택 | 통과 |
| iPad Mini | 768x1024 | 2x2 그리드 | 1열 스택 | 통과 |
| iPad Air | 820x1180 | 2x2 그리드 | 1열 스택 | 통과 |
| Desktop 1280 | 1280x800 | 1x4 가로 | 2~3열 그리드 | 통과 |
| Desktop 1920 | 1920x1080 | 1x4 가로 | 2~3열 그리드 | 통과 |

> Chrome DevTools Device Emulation + Vercel 배포 URL 직접 접속 검증. [상세 검증 항목](docs/responsive-testing.md)

> 계층별 코드 상세, Dapper CTE 쿼리, DB 스키마, Graceful Degradation 전략: [`docs/architecture.md`](docs/architecture.md)

---

## 테스트 전략 및 CI/CD

### 테스트 피라미드: 총 51개 테스트 (백엔드 35 + 프론트엔드 16)

**백엔드 (xUnit 2.9.2 + Moq 4.20.72 + Coverlet 커버리지):**

| 테스트 클래스 | 계층 | 케이스 수 | 검증 내용 |
|---------------|------|-----------|----------|
| `DashboardServiceTests` | Service | 9개 | 7개 메서드 반환값, 빈 결과 엣지 케이스, 0 나눗셈 방어 |
| `AiInsightServiceTests` | Service | 4개 | API키 미설정 Graceful Degradation, 캐시 히트/미스 |
| `DashboardRepositoryTests` | Repository | 8개 | 연결문자열 URI→키=값 변환 6종, 미설정 예외, 인터페이스 구현 검증 |
| `DashboardControllerTests` | Controller | 9개 | 7개 엔드포인트 OkResult 반환, ApiController 어트리뷰트, Thin Controller 패턴(모든 메서드 `Task<IActionResult>` 반환) 검증 |
| `GlobalExceptionMiddlewareTests` | Middleware | 5개 | 정상 통과, ArgumentNull→400, InvalidOperation→400, Exception→500, JSON 응답 형식 검증 |

**프론트엔드 (Vitest 4.1 + Vue Test Utils + happy-dom):**

| 테스트 파일 | 케이스 수 | 검증 내용 |
|-------------|-----------|----------|
| `useDashboardData.test.ts` | 7개 | Mock 초기 데이터 구조, KPI 카드 4종 생성, 월별 정렬, ETC/OTC 구분 |
| `useAiInsight.test.ts` | 4개 | 초기 상태 null, API 미설정 시 fetch 생략, errorType 초기값, 반환 속성 완전성 |
| `config.test.ts` | 5개 | 설정값 검증: 타임아웃 양수, AI > 대시보드, 재시도 ≥ 0, Keep-Alive < Render 슬립 |

**코드 품질 관리:**
- 하드코딩 상수 제거 → `frontend/src/config.ts`에 중앙 관리 (타임아웃, 재시도, API URL, Keep-Alive 주기)
- `config.test.ts`로 설정값 간 관계 검증 (AI 타임아웃 > 대시보드 타임아웃, Keep-Alive < Render 15분 슬립)
- 계층별 테스트 분리: Controller(9) · Service(13) · Repository(8) · Middleware(5) — 각 계층 독립 검증

### CI/CD 파이프라인: 4-Job 완전 자동화 (`.github/workflows/ci.yml`)

```
push/PR
  ├→ [backend-test]   .NET 9.0 빌드 → xUnit 35개 테스트 → Coverlet 커버리지 → TRX 업로드
  ├→ [frontend-test]  npm ci → Vitest 16개 테스트 → Vite 프로덕션 빌드 (TypeScript 타입 체크)
  └→ (1,2 완료 후)
      ├→ [e2e-smoke]      배포된 서비스 9개 엔드포인트 HTTP 상태 E2E 검증
      └→ [deploy-verify]  Vercel + Render + Supabase 배포 상태 최종 확인 (master push만)
```

| Job | 도구 | 실행 내용 |
|-----|------|----------|
| `backend-test` | .NET 9.0 + xUnit + Coverlet | 35개 테스트 실행 + Cobertura XML 커버리지 리포트 + TRX 결과 업로드 |
| `frontend-test` | Node 20 + Vitest + happy-dom | 16개 단위 테스트 (`useDashboardData` 7 + `useAiInsight` 4 + `config` 5) + Vite 빌드 |
| `e2e-smoke` | curl | 헬스체크 + 프론트엔드 + 대시보드 API 7개 = 9개 엔드포인트 실제 HTTP 응답 검증 |
| `deploy-verify` | curl | master push 시 Vercel/Render/Supabase 자동 배포 완료 후 서비스 가용성 확인 |

### 배포 자동화 — 코드 push만으로 프로덕션 배포 완료

```
[개발자] git push origin master
    │
    ├→ [Vercel]  자동 감지 → Vite 빌드 → CDN 배포 (수초 내 완료)
    │            URL: https://pharm-sight-frontend.vercel.app
    │
    ├→ [Render]  자동 감지 → .NET 9.0 Docker 빌드 → 배포 (2~5분)
    │            URL: https://pharm-sight.onrender.com
    │
    └→ [Supabase] 클라우드 매니지드 DB — 상시 가동, 배포 불필요
```

| 서비스 | 배포 방식 | 트리거 | 배포 시간 |
|--------|----------|--------|----------|
| Vercel (프론트엔드) | GitHub 연동 자동 배포 | `master` push 시 자동 빌드·배포 | ~10초 |
| Render (백엔드) | GitHub 연동 자동 배포 | `master` push 시 자동 빌드·배포 | 2~5분 |
| Supabase (DB) | 클라우드 매니지드 | 상시 가동, 별도 배포 불필요 | — |

**수동 배포 단계 없음:** `git push`만으로 테스트(51개) → 빌드 → 배포 → E2E 검증까지 전체 파이프라인이 자동 실행됩니다.

> 테스트 코드 위치:
> - 백엔드: [`backend/PharmSight.Tests/`](backend/PharmSight.Tests/) (5개 클래스, 35개 테스트)
> - 프론트엔드: [`frontend/src/composables/useDashboardData.test.ts`](frontend/src/composables/useDashboardData.test.ts) (7개), [`frontend/src/composables/useAiInsight.test.ts`](frontend/src/composables/useAiInsight.test.ts) (4개), [`frontend/src/config.test.ts`](frontend/src/config.test.ts) (5개)
> - CI 파이프라인: [`.github/workflows/ci.yml`](.github/workflows/ci.yml) (4-Job)

---

## 개발 이력 (55개 커밋, 5 Phase)

| Phase | 커밋 수 | 핵심 성과 |
|-------|--------|----------|
| Phase 0 — 기반 구축 | 6개 | .NET 9 API, Vue 3, PostgreSQL 스키마 초기화 |
| Phase 1 — 프론트엔드 UI | 7개 | 6종 ECharts 차트, Mock 데이터 Composable, Vercel 배포 |
| Phase 2 — 백엔드 API | 13개 | Dapper 7개 집계 쿼리, Render+Supabase 배포, 연결 버그 3건 수정 |
| Phase 3 — AI 기능 | 13개 | Gemini API 통합, 10단계 디버깅 → ListModels 동적 탐색으로 근본 해결 |
| Phase 4 — 테스트/CI/UX | 16개+ | xUnit 35개 + Vitest 16개 = 51개, 4-Job CI, Vue Transition, 토스트, 스켈레톤 |

> 전체 커밋-ROADMAP 매핑: [`docs/CHANGELOG.md`](docs/CHANGELOG.md) · ADR 8개: [`docs/decision-log.md`](docs/decision-log.md) · 스프린트 문서: [`docs/sprint/`](docs/sprint/)

---

## 시작하기

```bash
# 백엔드
cd backend && dotnet restore && dotnet run  # → http://localhost:5000

# 프론트엔드
cd frontend && npm install && npm run dev   # → http://localhost:5173
```

> 환경 변수 설정, 사전 요구사항 등 상세: [`docs/setup-guide.md`](docs/setup-guide.md)
