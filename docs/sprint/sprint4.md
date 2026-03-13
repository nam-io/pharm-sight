# Sprint 4: 테스트 전략 및 CI/CD 파이프라인 구축

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 4 |
| **연결된 Phase** | Phase 2 Backlog → 완료 전환 |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | ✅ 완료 |
| **작업 브랜치** | `master` (hotfix 성격 병합) |

---

## 목표 (Goal)

해커톤 평가 기준 "검증 계획(15점)"에 대응하여,
백엔드 Service 계층의 xUnit 단위 테스트를 작성하고
GitHub Actions CI 파이프라인을 실제 프로젝트에 맞게 구성한다.

---

## 배경 및 동기

| 항목 | 이전 상태 | 이번 Sprint 완료 후 |
|------|-----------|---------------------|
| 단위 테스트 | 미존재 (Backlog) | 13개 케이스 전체 통과 |
| CI/CD | Python/pytest 템플릿 (미동작) | .NET 9 + Vue 3 실제 파이프라인 |
| CLAUDE.md | SQLite/.NET 8 참조 잔존 | PostgreSQL/Gemini AI/.NET 9 현행화 |

---

## 작업 분해 (Task Breakdown)

### T4-1: xUnit 테스트 프로젝트 생성

**파일:**
- `backend/PharmSight.Tests/PharmSight.Tests.csproj`
- `backend/PharmSight.Tests/Services/DashboardServiceTests.cs`
- `backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`

**의존성:**
- `xunit 2.9.2`, `xunit.runner.visualstudio 2.8.2`
- `Moq 4.20.72` — IDashboardRepository 모킹
- `Microsoft.NET.Test.Sdk 17.12.0`, `coverlet.collector 6.0.2`

**테스트 케이스 목록:**

| 클래스 | 메서드 | 검증 내용 |
|--------|--------|-----------|
| `DashboardServiceTests` | `GetMonthlySalesAsync_Repository에서_반환된_데이터를_그대로_반환한다` | 반환값 일치, Repository 1회 호출 |
| `DashboardServiceTests` | `GetMonthlySalesAsync_빈_결과도_정상_반환된다` | 빈 IEnumerable 엣지 케이스 |
| `DashboardServiceTests` | `GetDrugTypeSalesAsync_ETC_OTC_두_항목이_반환된다` | ETC/OTC 두 타입 포함 검증 |
| `DashboardServiceTests` | `GetPatientAgeGroupsAsync_연령대_데이터가_반환된다` | Count 값 매핑 검증 |
| `DashboardServiceTests` | `GetHospitalPrescriptionsAsync_TOP6_기관이_반환된다` | 1위 기관명 검증 |
| `DashboardServiceTests` | `GetWholesaleExpensesAsync_도매상별_지출이_반환된다` | Amount 값 검증 |
| `DashboardServiceTests` | `GetDrugCoverageAsync_급여_비급여_두_항목이_반환된다` | 급여 라벨 포함 검증 |
| `DashboardServiceTests` | `GetKpiSummaryAsync_KPI_요약이_반환된다` | 매출·변화율 정확성 검증 |
| `DashboardServiceTests` | `GetKpiSummaryAsync_전월_매출_없을때_변화율은_0이다` | 0 나눗셈 엣지 케이스 |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_안내메시지를_반환한다` | Graceful Degradation |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_Repository_호출하지_않는다` | 불필요한 DB 조회 없음 |
| `AiInsightServiceTests` | `GetInsightAsync_API키_없으면_GeneratedAt이_설정된다` | 타임스탬프 설정 검증 |
| `AiInsightServiceTests` | `GetInsightAsync_두번_호출시_캐시를_반환한다` | IMemoryCache 30분 캐시 히트 |

**실행 결과:**
```
총 테스트 수: 13
     통과: 13
 총 시간: 1.2077 초
```

### T4-2: GitHub Actions CI/CD 파이프라인 재구성

**파일:** `.github/workflows/ci.yml`

**변경 내용:**
- 기존: Python/pytest 템플릿 (프로젝트와 완전 불일치, 동작 불가)
- 변경: .NET 9 + Vue 3 실제 파이프라인

**파이프라인 구조:**
```
push/PR → master, develop
├── backend-test job
│   ├── actions/setup-dotnet@v4 (.NET 9.0.x)
│   ├── dotnet restore (API + Tests)
│   ├── dotnet build --configuration Release
│   ├── dotnet test --verbosity normal
│   └── upload-artifact: TRX 테스트 결과
└── frontend-build job
    ├── actions/setup-node@v4 (Node 20)
    ├── npm ci --prefix frontend
    └── npm run build (VITE_API_BASE_URL 주입)
```

### T4-3: CLAUDE.md AI 컨텍스트 현행화

- 기술 스택: `SQLite` → `PostgreSQL (Supabase), Npgsql`, `.NET Core 8.0+` → `.NET 9.0`
- AI 항목 추가: Google Gemini API, IMemoryCache 캐시, 동적 모델 선택
- DB 스키마: PostgreSQL 함수(`DATE_TRUNC`, `AGE()`) 및 Dapper 타입 매핑 주의사항 명시
- 검증 섹션: 테스트 케이스 수, CI/CD 구성 상세 기재

---

## 완료 조건 (Definition of Done)

- [x] `dotnet test PharmSight.Tests/` — 13개 전체 통과 (로컬 검증 완료)
- [x] CI/CD `.github/workflows/ci.yml` — 프로젝트 실정에 맞는 파이프라인 구성
- [x] `git push origin master` → GitHub Actions 트리거 확인
- [x] `CLAUDE.md` 기술 스택 현행화 완료
- [x] `ROADMAP.md` 단위 테스트 항목 `[x]` 완료 전환

---

## 기술 부채 및 주의사항

- 현재 테스트는 Service 계층만 커버 (Repository는 DB 의존성으로 통합 테스트 필요)
- CI의 `backend-test` job은 DB 연결 없이 단위 테스트만 실행 (Supabase 연결 정보 불필요)
- `PharmSight.Api.csproj`에 `<Compile Remove="PharmSight.Tests\**\*.cs" />` 추가 — MSBuild glob이 하위 디렉토리 .cs 파일을 API 프로젝트에 포함시키는 문제 방지
