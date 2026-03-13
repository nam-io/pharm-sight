# 기술 의사결정 기록 (Architecture Decision Records)

> **형식:** ADR (Architecture Decision Record) — 마이클 나이가드(Michael Nygard) 제안 형식 기반
> **목적:** 각 기술 선택의 배경·대안·근거·결과를 영구 기록하여 개발 추적성 확보
> **연결:** 각 ADR은 `CHANGELOG.md`의 커밋 이력 및 `ROADMAP.md`의 Phase 항목과 연결됩니다.

---

## 목록

| 번호 | 제목 | 상태 | Phase |
|------|------|------|-------|
| [ADR-001](#adr-001-orm-미사용--dapper-순수-sql-전략) | ORM 미사용 — Dapper 순수 SQL 전략 | 확정 | Phase 2 |
| [ADR-002](#adr-002-sqlite--supabase-postgresql-마이그레이션) | SQLite → Supabase PostgreSQL 마이그레이션 | 확정 | Phase 2 |
| [ADR-003](#adr-003-anthropic-api--google-gemini-api-전환) | Anthropic API → Google Gemini API 전환 | 확정 | Phase 3 |
| [ADR-004](#adr-004-gemini-모델명-하드코딩--listmodels-api-동적-탐색) | Gemini 모델명 하드코딩 → ListModels API 동적 탐색 | 확정 | Phase 3 |
| [ADR-005](#adr-005-imemorycache-기반-ai-응답-캐싱) | IMemoryCache 기반 AI 응답 캐싱 | 확정 | Phase 3 |
| [ADR-006](#adr-006-vue-3-composition-api--react-대신-선택) | Vue 3 Composition API — React 대신 선택 | 확정 | Phase 1 |
| [ADR-007](#adr-007-controller--service--repository-3계층-아키텍처) | Controller → Service → Repository 3계층 아키텍처 | 확정 | Phase 0 |
| [ADR-008](#adr-008-promise-all-병렬-api-호출--순차-호출-대신) | Promise.all 병렬 API 호출 — 순차 호출 대신 | 확정 | Phase 2 |

---

## ADR-001: ORM 미사용 — Dapper 순수 SQL 전략

**날짜:** Phase 2 (2026-03)
**상태:** 확정
**관련 커밋:** `a7b5334` (백엔드 전체 구현)

### 컨텍스트

약국 경영 대시보드의 핵심은 7종의 통계 집계 쿼리입니다:
- 월별 매출 + 조제건수 + 전월 대비 변화율 (KPI)
- 의약품 유형별(ETC/OTC) 매출 비중
- 연령대별(`AGE()`, `EXTRACT(YEAR FROM AGE(...))`) 환자 분포
- 병원별 처방건수 상위 6개 (`GROUP BY`, `ORDER BY`, `LIMIT`)
- 도매상별 누적 지출 (`SUM`, `GROUP BY`)
- 급여/비급여 지출 비율 (`IsCovered` 조건 집계)
- KPI 전월 대비 (`DATE_TRUNC`, CTE 3중)

이런 복잡한 집계 쿼리를 ORM으로 구현하면 어떤 문제가 발생하는지 검토했습니다.

### 검토한 대안

**Entity Framework Core:**
- 장점: 마이그레이션 자동화, LINQ 타입 안전성
- 단점:
  - `DATE_TRUNC`, `AGE()` 등 PostgreSQL 전용 함수를 LINQ로 표현 불가 → 결국 `FromSqlRaw()` 강제 사용
  - Change Tracking: 읽기 전용 집계 쿼리에도 엔티티 추적 오버헤드 발생
  - CTE 3중 구조를 LINQ로 표현 시 최소 3회 DB 왕복 또는 비최적 SQL 생성
  - N+1 문제: `Hospital → Prescriptions` 관계를 `Include()`로 로드 후 메모리 집계 위험

**Dapper:**
- 장점:
  - 순수 SQL → PostgreSQL 네이티브 함수 그대로 실행 (성능 최적화 명확)
  - CTE 3중을 1회 DB 왕복으로 처리 (`QuerySingleAsync<KpiSummary>`)
  - `QueryAsync<T>` 한 줄로 DTO 직접 매핑 (변환 코드 불필요)
  - `await using var conn` — 연결 즉시 해제, Change Tracking 오버헤드 없음
- 단점: 마이그레이션 자동화 없음 (해커톤 환경에서는 `database/schema.sql`로 수동 관리 허용)

### 결정

**Dapper 사용. Entity Framework Core 미사용.**

CLAUDE.md에 `⚠️ 절대 Entity Framework를 사용하지 마세요` 로 명문화.

### 근거

이 프로젝트의 DB 접근은 **쓰기 없는 읽기 전용 집계** 패턴입니다.
EF Core의 핵심 가치(마이그레이션, Change Tracking)가 이 패턴에서 오히려 오버헤드입니다.
Dapper는 "SQL을 직접 작성하되, 파라미터 바인딩과 DTO 매핑은 자동화"하는 정확한 도구입니다.

### 결과

- KPI 집계 쿼리: CTE 3중, 1회 왕복 → 단일 SQL로 이번 달/전월/발주 동시 계산
- `DATE_TRUNC`, `AGE()`, `INTERVAL` PostgreSQL 함수 제한 없이 사용
- 전체 7개 쿼리 평균 응답 시간: Render 배포 환경 기준 200ms 이내

---

## ADR-002: SQLite → Supabase PostgreSQL 마이그레이션

**날짜:** Phase 2 초기 (2026-03)
**상태:** 확정
**관련 커밋:** Phase 2 핵심 구현 `a7b5334`, 연결 오류 수정 `9bf841b`, `acd8e42`

### 컨텍스트

Phase 0에서 SQLite로 로컬 개발 환경을 구성했습니다.
Phase 2에서 Render 클라우드 배포를 시도하자 치명적 문제 발생:

```
Render 무료 플랜 파일시스템: 에페머럴(Ephemeral)
→ 배포(Deploy) 또는 재시작(Restart) 시 /app 디렉토리 초기화
→ SQLite .db 파일 소멸 → 데이터 0건
```

### 검토한 대안

| 옵션 | 비용 | 영속성 | 해커톤 적합성 |
|------|------|--------|-------------|
| SQLite + Render Volume | 유료($7/월) | 영속 | 비용 발생 |
| PlanetScale MySQL | 무료 | 영속 | DATE_TRUNC 미지원 |
| Railway PostgreSQL | 무료 $5 크레딧 | 영속 | 크레딧 소진 위험 |
| **Supabase PostgreSQL** | **무료 무제한** | **영속** | **최적** |

### 결정

**Supabase PostgreSQL 무료 티어 사용.**

연결 방식: Supabase Connection Pooler (IPv4 전용 URL) — Render IPv6 미지원 대응.

### 마이그레이션 과정에서 발생한 기술적 도전 (CHANGELOG Phase 2 참조)

```
도전 1 — URI 형식 연결 문자열 파싱 오류 (커밋 9bf841b):
  원인: Render 환경변수로 주입되는 postgresql:// URI 형식을
        Npgsql이 키=값 형식으로만 파싱 → FormatException
  해결: NormalizeConnectionString() — URI 감지 시 자동 변환

도전 2 — IPv6 연결 실패 (커밋 acd8e42):
  원인: Render 무료 플랜 IPv6 아웃바운드 미지원
        → Supabase 기본 연결 URL이 IPv6 주소로 DNS 해석
  해결: Supabase Connection Pooler URL(IPv4 전용)로 변경

도전 3 — bigint ↔ long 타입 매핑 (커밋 504164d):
  원인: PostgreSQL COUNT() 반환 타입 bigint → C# int 매핑 시 오버플로우 오류
  해결: Dapper 타입 핸들러 + 명시적 CAST(... AS int) 적용
```

### 결과

- 클라우드 배포 후 데이터 영속성 확보
- PostgreSQL 전용 함수 (`DATE_TRUNC`, `AGE()`) 완전 활용 가능
- Supabase 무료 티어: 월 500MB, 2GB 네트워크 — 해커톤 시드 데이터(100명, 1800건) 충분

---

## ADR-003: Anthropic API → Google Gemini API 전환

**날짜:** Phase 3 (2026-03)
**상태:** 확정
**관련 커밋:** `5ff16a7` (전환), `d87c0d8`~`761c3c2` (안정화)

### 컨텍스트

Phase 3 AI 기능 구현 시 Claude API(Anthropic)를 1차 선택으로 구현했습니다.
통합 직후 문제 발생:

```
Anthropic API 무료 할당량: 개인 계정 기준 매우 제한적
→ 해커톤 중 데모/테스트 과정에서 할당량 소진
→ API 응답: {"error": {"type": "rate_limit_error"}}
```

### 검토한 대안

| AI API | 무료 할당량 | 모델 선택 유연성 | 선택 여부 |
|--------|-----------|----------------|---------|
| Anthropic Claude | 낮음 | 고정 모델명 | 제외 |
| OpenAI GPT-4o | 없음(유료) | 고정 모델명 | 제외 |
| **Google Gemini Flash** | **높음(1M 토큰/월)** | **ListModels API 지원** | **선택** |
| Ollama (로컬) | 무제한 | 제한적 | 배포 불가 |

### 결정

**Google Gemini API (Flash 계열) 사용.**
HTTP REST 직접 호출 방식 — SDK 의존성 최소화.

### 근거

1. **무료 할당량:** Gemini 1.5 Flash 기준 분당 15회, 일 1500회 — 해커톤 환경에서 충분
2. **ListModels API:** 사용 가능한 모델을 런타임에 동적 탐색 가능 (ADR-004 연결)
3. **HTTP REST:** SDK 없이 `HttpClient`로 직접 호출 → 의존성 감소, 이식성 향상

### 결과

Gemini 통합 과정에서 API 버전(v1beta vs v1)과 모델명 관련 추가 디버깅 발생.
상세 단계별 기록: `CHANGELOG.md` Phase 3 디버깅 타임라인 참조.
최종 해결: ADR-004 (동적 모델 탐색).

---

## ADR-004: Gemini 모델명 하드코딩 → ListModels API 동적 탐색

**날짜:** Phase 3 디버깅 종반 (2026-03)
**상태:** 확정
**관련 커밋:** `8be6235` (근본 원인 발견), `761c3c2` (구현)

### 컨텍스트

Phase 3 디버깅 과정에서 모델명을 여러 번 변경했으나(`gemini-2.0-flash` → `gemini-1.5-flash` → `gemini-1.5-flash-latest`) 여전히 간헐적 404 발생.

최종 진단 (`8be6235`):

```
Gemini API가 배포 지역(Region)에 따라 사용 가능 모델 목록이 다름
→ 로컬(한국): gemini-1.5-flash 사용 가능
→ Render 배포(미국 Oregon): 동일 모델명이 NOT_FOUND

하드코딩된 모델명 자체가 취약 설계
```

### 결정

**`ResolveModelNameAsync()` — ListModels API 기반 동적 모델 탐색.**

```csharp
private async Task<string> ResolveModelNameAsync()
{
    // 1. IMemoryCache에서 1시간 캐시 확인
    if (_cache.TryGetValue(ModelCacheKey, out string? cached))
        return cached!;

    // 2. GET /v1beta/models → 현재 환경 사용 가능 모델 목록
    var response = await _httpClient.GetAsync(
        $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}");
    var models = // 역직렬화...

    // 3. generateContent 지원 + gemini-1.5-flash 우선 선택
    var selected = models
        .Where(m => m.SupportedGenerationMethods.Contains("generateContent"))
        .OrderByDescending(m => m.Name.Contains("gemini-1.5-flash") ? 1 : 0)
        .First();

    // 4. 1시간 캐시 저장
    _cache.Set(ModelCacheKey, selected.Name, TimeSpan.FromHours(1));
    return selected.Name;
}
```

### 결과

- 배포 환경별 사용 가능 모델 자동 선택 → 404 오류 완전 해소
- ListModels 캐시(1시간): API 호출 비용 최소화
- 향후 Gemini 모델명 변경에도 코드 수정 없이 자동 대응

---

## ADR-005: IMemoryCache 기반 AI 응답 캐싱

**날짜:** Phase 3 (2026-03)
**상태:** 확정
**관련 커밋:** `a7b5334` (DI 등록), Phase 3 구현 커밋들

### 컨텍스트

AI 인사이트 API는 두 가지 비용이 발생합니다:
1. **Gemini API 호출 비용:** 할당량 소진 위험
2. **응답 시간:** Gemini Flash 기준 평균 2~4초

대시보드 방문마다 Gemini API를 호출하면 할당량을 빠르게 소진합니다.
약국 경영 데이터는 **분 단위 변화가 없으므로** 캐싱이 적절합니다.

### 결정

**`IMemoryCache` — AI 응답 30분 캐시, 모델명 1시간 캐시.**

```csharp
// Program.cs
builder.Services.AddMemoryCache();  // IMemoryCache DI 등록

// AiInsightService.cs
_cache.Set(InsightCacheKey, insight, TimeSpan.FromMinutes(30));
_cache.Set(ModelCacheKey, modelName, TimeSpan.FromHours(1));
```

### 근거

- 약국 매출 데이터: Supabase에서 일 1회 이상 변경되지 않음
- 30분 캐시: 대시보드를 여러 번 새로고침해도 API 1회 호출
- 분산 캐시(Redis) 미사용 이유: Render 단일 인스턴스 → in-process 캐시로 충분

### 결과

- Gemini API 호출: 30분에 최대 1회로 제한
- 두 번째 이후 요청: 캐시 반환 → 응답 시간 ~2000ms → ~10ms

---

## ADR-006: Vue 3 Composition API — React 대신 선택

**날짜:** Phase 0 (2026-03)
**상태:** 확정
**관련 커밋:** `43be119` (Vue 3 스캐폴딩)

### 컨텍스트

프론트엔드 프레임워크 선택. 주요 후보: React 18, Vue 3.

### 비교

| 기준 | React 18 | Vue 3 Composition API |
|------|----------|----------------------|
| Composable 패턴 | Custom Hooks | Composables (`use*`) — 동등한 로직 재사용 |
| TypeScript 통합 | 별도 설정 필요 | `<script setup lang="ts">` — 자연스러운 통합 |
| 템플릿 직관성 | JSX — JS 숙련도 필요 | HTML 기반 템플릿 — 약국 경영 도메인 집중 용이 |
| 빌드 도구 | CRA(느림) / Vite | Vite 기본 — 즉각 HMR |
| 반응형 선언 | useState + useEffect | `ref()`, `computed()` — 선언적 표현 명확 |

### 결정

**Vue 3 Composition API + Vite + TypeScript.**

### 결과

- `useDashboardData.ts`, `useAiInsight.ts`, `useKeepAlive.ts` — 기능별 Composable 분리
- `computed(() => dashboardData.value.monthlySales.slice(-selectedPeriod.value))` — 기간 필터를 단 1줄로 선언
- Vite 6 HMR: 차트 컴포넌트 수정 → 즉각 반영 (해커톤 개발 속도 향상)

---

## ADR-007: Controller → Service → Repository 3계층 아키텍처

**날짜:** Phase 0 (2026-03)
**상태:** 확정
**관련 커밋:** `008f5bd` (구조 정의), `a7b5334` (전체 구현)

### 컨텍스트

단일 Controller 파일에 모든 로직을 넣는 단순 구조 vs. 3계층 분리 구조.
해커톤 시간 제약 하에서 아키텍처 분리를 선택한 이유:

### 결정

**3계층 분리 + 인터페이스 기반 DI.**

```
DashboardController (HTTP 계층)
    → IDashboardService (비즈니스 로직 계층)
        → IDashboardRepository (데이터 접근 계층)
```

### 근거

1. **테스트 용이성 (가장 중요한 이유):** `IDashboardRepository` Mock → `DashboardServiceTests` 9개 단위 테스트 작성 가능. 단일 클래스 구조였다면 DB 연결 없이 Service 테스트 불가.

2. **단일 책임 원칙(SRP):** Controller는 HTTP 요청/응답만 처리. Service는 비즈니스 규칙만. Repository는 SQL 쿼리만.

3. **GlobalExceptionMiddleware와의 조화:** 어떤 계층에서 예외가 발생해도 미들웨어가 일관된 JSON 응답 보장.

### 결과

- xUnit + Moq: `IDashboardRepository`를 Mock으로 교체 → DB 없이 9개 단위 테스트 작성
- `IDashboardService`를 `IAiInsightService`로 교체 → AI 서비스도 동일 패턴 4개 테스트
- 총 13개 단위 테스트 모두 통과

---

## ADR-008: Promise.all 병렬 API 호출 — 순차 호출 대신

**날짜:** Phase 2 (2026-03)
**상태:** 확정
**관련 커밋:** `f7871bc` (프론트엔드 API 연동)

### 컨텍스트

대시보드 초기 로드 시 7개 API 엔드포인트를 호출해야 합니다:
- `/api/dashboard/monthly-sales`
- `/api/dashboard/drug-type-sales`
- `/api/dashboard/patient-age-groups`
- `/api/dashboard/hospital-prescriptions`
- `/api/dashboard/wholesale-expenses`
- `/api/dashboard/drug-coverage`
- `/api/dashboard/kpi-summary`

순차 호출 시 각 API 평균 200ms × 7 = **1400ms 이상** 소요.

### 결정

**`Promise.all()` 병렬 호출.**

```typescript
// useDashboardData.ts
const [monthly, drugType, ageGroups, hospital, wholesale, coverage, kpi] =
  await Promise.all([
    api.get('/dashboard/monthly-sales'),
    api.get('/dashboard/drug-type-sales'),
    api.get('/dashboard/patient-age-groups'),
    api.get('/dashboard/hospital-prescriptions'),
    api.get('/dashboard/wholesale-expenses'),
    api.get('/dashboard/drug-coverage'),
    api.get('/dashboard/kpi-summary'),
  ])
```

### 결과

- 순차 호출: ~1400ms
- 병렬 호출: 가장 느린 API 응답 시간 기준 ~250ms
- **최대 5배 초기 로딩 속도 개선**

---

*이 문서는 PharmSight AI 해커톤 개발 과정의 모든 주요 기술 의사결정을 추적합니다.*
*각 ADR은 해당 커밋 시점에 실제로 내려진 결정을 사후 기록한 것입니다.*
