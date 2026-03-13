# 기술 스택 선택 근거 및 의존성 관리

PharmSight AI의 모든 의존성은 해커톤 제약(비용 0원, 빠른 배포) + 생산 수준 코드 품질을 동시에 만족하도록 선택되었습니다.

---

## 프론트엔드 의존성 (`frontend/package.json`)

### 런타임 의존성

| 패키지 | 버전 | 선택 근거 | 탈락 대안 |
|--------|------|-----------|-----------|
| `vue` | ^3.5 | Composition API + `<script setup>` TypeScript 통합 · `useDashboardData` 등 Composable 3개로 로직 분리 | React 18 (JSX 러닝커브, Context API 복잡성) |
| `vite` | ^6.4 | 즉각 HMR · ESM 네이티브 · 빌드 최적화 (tree-shaking) · `VITE_*` 환경변수 빌드타임 인라인 | CRA (느린 빌드), Webpack (설정 복잡) |
| `echarts` | ^5.6 | 바+라인 복합 차트 네이티브 지원 · `noData` 옵션 · 6종 차트 통일 API · TypeScript 지원 | Chart.js (복합 차트 제한, 타입 정의 불완전) |
| `tailwindcss` | ^3.4 | JIT 컴파일로 번들 크기 최소화 · Breakpoint(`sm:`, `lg:`) 반응형 · 유틸리티 클래스로 CSS 파일 불필요 | Bootstrap (번들 크기 큼, 커스터마이징 어려움) |

### 개발 의존성

| 패키지 | 버전 | 용도 |
|--------|------|------|
| `vue-tsc` | ^2.2 | Vue SFC + TypeScript 타입 검사 (빌드 시 `vue-tsc -b` 실행) |
| `@vitejs/plugin-vue` | ^5.2 | Vite에서 Vue SFC(`.vue`) 처리 플러그인 |
| `typescript` | ~5.8 | 컴파일 타임 타입 검사 · DTO 인터페이스 계약 |

### 버전 선택 기준

- **Vue 3.5**: `useTemplateRef`, 더 나은 반응형 성능 개선 포함 최신 안정 버전
- **Vite 6.4**: Rollup 4 기반 빌드 성능 향상 + ESM 최적화
- **ECharts 5.6**: TypeScript 지원 완성도 + 성능 개선 (Canvas 렌더러 최적화)

---

## 백엔드 의존성 (`backend/PharmSight.Api.csproj`)

### 런타임 의존성

| 패키지 | 버전 | 선택 근거 | 탈락 대안 |
|--------|------|-----------|-----------|
| `Dapper` | 2.1.35 | CTE 3중 집계 1회 DB 왕복 · PostgreSQL 네이티브 함수(`DATE_TRUNC`, `AGE()`) 직접 사용 · Change Tracking 오버헤드 없음 · `QueryAsync<T>` DTO 직접 매핑 | EF Core (LINQ 집계 한계, PG 함수 번역 불완전, Change Tracking 오버헤드) |
| `Npgsql` | 9.0.x | PostgreSQL 네이티브 드라이버 · IPv4 + SSL 지원 · `NpgsqlConnectionStringBuilder`로 URI 형식 변환 | MySqlConnector (PostgreSQL 미지원) |
| `Microsoft.AspNetCore` | .NET 9.0 | 내장 DI 컨테이너 · `IMemoryCache` · `IHttpClientFactory` · Minimal API | Spring Boot (JVM 오버헤드), FastAPI (타입 안전성 약함) |

### Dapper vs EF Core 선택 근거 상세

이 프로젝트의 모든 쿼리는 읽기 전용 집계(Read-only Aggregation)이므로 Dapper가 EF Core보다 우월합니다:

```
[KPI 쿼리 특성 → Dapper 선택 근거]

WITH current_month AS (   ← CTE 1: 이번 달 매출/조제건수/환자수
    ...
),
prev_month AS (           ← CTE 2: 전월 매출/조제건수
    ...
),
current_orders AS (       ← CTE 3: 이번 달 발주 지출
    ...
)
SELECT ...
FROM current_month c, prev_month p, current_orders co;
           ↑
           단 1회 DB 왕복으로 KPI 전체 계산
           EF Core LINQ: 최소 3회 왕복 또는 Raw SQL 강제 사용
```

| 특성 | EF Core | Dapper |
|------|---------|--------|
| CTE 3중 집계 | LINQ 표현 불가 → Raw SQL 강제 → ORM 이점 없음 | 직접 제어, 1회 왕복 |
| `DATE_TRUNC`, `AGE()` | PG 함수 번역 불완전 → 클라이언트 사이드 처리 위험 | 순수 SQL → PG 실행 |
| Change Tracking | 읽기 전용 집계에도 엔티티 추적 오버헤드 | `await using var conn` 즉시 해제 |
| DTO 매핑 | 도메인 객체 → DTO 변환 코드 추가 필요 | `QueryAsync<KpiSummary>` 1줄 |

### 테스트 의존성 (`backend/PharmSight.Tests/PharmSight.Tests.csproj`)

| 패키지 | 버전 | 용도 |
|--------|------|------|
| `xUnit` | 2.9.2 | .NET 표준 단위 테스트 프레임워크 · `[Fact]` / `[Theory]` 어노테이션 |
| `Moq` | 4.20.72 | `IDashboardRepository` Interface Mock → DB 없이 Service 계층 테스트 |
| `Microsoft.NET.Test.Sdk` | 17.12 | `dotnet test` CLI 실행 지원 |
| `xunit.runner.visualstudio` | 2.8.2 | Visual Studio / Rider IDE 통합 |

### 버전 선택 기준

- **.NET 9.0**: 2024년 11월 LTS. `async/await` 성능 개선, Native AOT 지원
- **Dapper 2.1.x**: `QueryAsync` 비동기 완전 지원 + Npgsql 9.x 호환
- **Npgsql 9.0**: .NET 9.0과 메이저 버전 일치, SSL 연결 안정성 개선
- **xUnit 2.9.2 + Moq 4.20.72**: 상호 호환 검증된 최신 안정 버전 조합

---

## 인프라 의존성 (무료 티어 조합)

| 서비스 | 역할 | 무료 제한 | 선택 근거 |
|--------|------|-----------|-----------|
| **Vercel** | 프론트엔드 정적 호스팅 | 무제한 | GitHub 연동 자동 배포 · Edge CDN · `VITE_*` 환경변수 빌드타임 주입 |
| **Render** | 백엔드 API 서버 | 월 750시간, 15분 슬립 | .NET 9.0 Docker 배포 지원 · 환경변수 GUI · Supabase 연결 안정 |
| **Supabase** | PostgreSQL 관리형 DB | 500MB, 월 2GB 대역폭 | Render 에페머럴 파일시스템 문제 해결 · PostgreSQL 네이티브 기능 · SSL 기본 제공 |

### Render 슬립 방지 전략

Render 무료 플랜은 15분 무활동 시 서버 슬립 → 첫 요청 30~40초 지연 발생:
```
frontend/src/composables/useKeepAlive.ts
  → 10분 주기로 GET /health 핑
  → useAiInsight.ts의 15초 타임아웃으로 슬립 중 요청 방어
backend/Program.cs
  → app.MapGet("/health", ...) — 슬립 방지 엔드포인트
```

---

## 기술 스택 변경 이력

| 변경 내용 | Phase | 이유 |
|-----------|-------|------|
| `SQLite → PostgreSQL (Supabase)` | Phase 2 | Render 에페머럴 파일시스템 → 배포 시 SQLite 데이터 소멸 |
| `Anthropic API → Google Gemini API` | Phase 3 | 무료 할당량 소진 → Gemini Flash 무료 티어(1M 토큰/월) 전환 |
| `.NET 8 → .NET 9` | Phase 2 | 성능 개선 + 최신 LTS |
| `하드코딩 모델명 → ListModels 동적 탐색` | Phase 3 | 배포 환경별 모델명 불일치(NOT_FOUND 오류) 근본 해결 |
| `URI 형식 → Npgsql 키=값 형식` | Phase 2 | Render 환경변수 URI → `NormalizeConnectionString()` 자동 변환 |
