# 🤖 AI Assistant Context & Guidelines for PharmSight

이 문서는 PharmSight(약국 경영 통합 대시보드) 프로젝트에서 Claude Code와 개발자가 협업(Co-work)하기 위한 핵심 규칙과 에이전트 설정, 기술 스택 가이드를 정의합니다. AI는 모든 작업을 수행할 때 이 문서의 규칙을 최우선으로 준수해야 합니다.

## 1. 🛠 기술 스택 및 아키텍처 (Tech Stack)
- **Frontend:** Vue 3 (Composition API, `<script setup>`), TypeScript, Vite, Tailwind CSS, Apache ECharts.
- **Backend:** C# .NET 9.0 (Web API).
- **Database & ORM:** PostgreSQL (Supabase), Npgsql, Dapper. (⚠️ 절대 Entity Framework를 사용하지 마세요).
- **AI:** Google Gemini API — `AiInsightService`로 경영 데이터 분석, `IMemoryCache` 30분 캐시, 동적 모델 선택(`ResolveModelNameAsync`).
- **Infra:** Vercel (Frontend 배포) · Render (Backend 배포) · Supabase (PostgreSQL 클라우드 DB).
- **Architecture Pattern:** Controller - Service - Repository 계층 분리를 통한 단일 책임 원칙(SRP) 준수.

## 2. 🔄 AI 에이전트 워크플로우 (AI Agent Workflow)
이 프로젝트는 `.claude/agents/` 디렉토리에 정의된 특화 에이전트 프로세스를 따릅니다. 사용자의 지시에 따라 적절한 에이전트 역할을 수행하세요.

1. **`prd-to-roadmap` (로드맵 수립):** `README.md`의 요구사항을 분석하여 Agile/스크럼 기반의 `ROADMAP.md`를 자동 생성합니다.
2. **`sprint-planner` (스프린트 계획):** `ROADMAP.md`를 바탕으로 다음 개발 목표를 설정하고 `docs/sprint/sprint{N}.md` 문서를 작성합니다.
3. **`sprint-close` (스프린트 완료):** 구현이 완료되면 `ROADMAP.md`의 상태를 업데이트(진행 중 -> 완료)하고, 테스트 검증 및 Conventional Commits 형식의 커밋 메시지를 제안합니다.
4. **`hotfix-close` (핫픽스 완료):** 긴급 버그 수정 후 리뷰 및 메인 브랜치 반영 계획을 수립합니다.

## 3. ✍️ 코드 품질 및 문서화 규칙 (Code Quality Rules)
- **언어 규칙:** 모든 응답과 주석, 커밋 메시지, 문서는 반드시 **한국어(Korean)**로 작성합니다.
- **주석 (Comments):** - Backend: 모든 Service와 Repository 클래스, public 메서드 상단에 `<summary>` XML 주석을 필수로 작성하여 가독성을 높입니다.
  - Frontend: 복잡한 로직이나 Vue Composables(`useDashboardData.ts` 등)에 JSDoc을 작성합니다.
- **에러 처리 및 예외 방어 (Error Handling):** (⚠️ 중요 평가 항목)
  - Backend: `try-catch`를 적극 사용하고, 예외 발생 시 전역 `GlobalExceptionMiddleware`를 통해 `{"error": "...", "statusCode": 500}` 형태의 일관된 HTTP 표준 응답을 반환합니다.
  - Frontend: Axios Interceptor 또는 `try-catch`를 통해 API 호출 실패 시 서버가 죽지 않고 사용자에게 친화적인 에러 알림(Toast UI 등)을 보여주도록 방어 로직을 구현합니다.
- **개발 진행 기록 (Commit History):** (⚠️ 중요 평가 항목)
  - `sprint-close` 에이전트 수행 시, 커밋 메시지는 반드시 `Conventional Commits` 형식(예: `feat: 대시보드 매출 차트 컴포넌트 추가`, `fix: DB 쿼리 파라미터 매핑 오류 수정`)을 따릅니다.
  - 커밋 메시지 본문에는 해당 작업이 `ROADMAP.md`의 어떤 이슈를 해결했는지 추적 가능하도록 상세히 기록합니다.

## 4. 🗄 데이터베이스 스키마 컨텍스트 (Database Schema Reference)
통계 및 대시보드 지표 추출용 쿼리를 작성할 때 다음 PostgreSQL(Supabase) 테이블 구조를 반드시 참고하세요.
- `Patients` (Id, DateOfBirth)
- `Hospitals` (Id, Name)
- `Drugs` (Id, Name, Type [ETC/OTC], IsCovered)
- `Prescriptions` (Id, PatientId, HospitalId, DispenseDate)
- `Orders` (Id, WholesaleName, DrugId, Amount, OrderDate)
- `Sales` (Id, Amount, SaleDate, PrescriptionId [nullable])

> **주의:** PostgreSQL은 `DATE_TRUNC`, `TO_CHAR`, `AGE()` 함수를 사용합니다. COUNT()는 bigint(long), ROUND()는 numeric(decimal)로 Dapper 매핑합니다.

## 5. 🧪 검증 및 CI/CD 파이프라인 (Testing & CI)
- **단위 테스트:** `backend/PharmSight.Tests/` — `xUnit` + `Moq`로 Service 계층 테스트 (Repository Mocking). 총 13개 테스트 케이스.
  - `DashboardServiceTests`: 7개 메서드별 동작 검증, 빈 결과 엣지 케이스 포함
  - `AiInsightServiceTests`: API 키 미설정 Graceful Degradation, 캐시 동작 검증
- **CI/CD:** `.github/workflows/ci.yml` — GitHub Actions로 push/PR 시 자동 실행
  - `backend-test` job: .NET 9.0 빌드 + xUnit 단위 테스트 + TRX 결과 업로드
  - `frontend-build` job: Node 20 + `npm ci` + Vite 프로덕션 빌드 검증
- 로컬 테스트 실행: `cd backend && dotnet test PharmSight.Tests/`

## 6. 📋 기술 스택 변경 이력 (Technology Change History)

> **목적:** 초기 설계 결정과 개발 중 발생한 기술 변경을 명시적으로 기록하여 AI가 현재 상태를 정확히 파악하도록 합니다.

| 변경 항목 | 초기 결정 | 최종 결정 | 변경 이유 | 변경 시점 |
|----------|---------|---------|---------|---------|
| **데이터베이스** | SQLite (`Microsoft.Data.Sqlite`) | **PostgreSQL (Supabase)** | Render 무료 플랜의 에페머럴(임시) 파일시스템 → 배포 시 SQLite 데이터 소멸 | Phase 2 |
| **AI API** | Anthropic Claude API | **Google Gemini API** | Anthropic 무료 할당량 소진 → Gemini Flash 무료 티어(1M 토큰/월) 활용 | Phase 3 |
| **.NET 버전** | .NET 8.0 | **.NET 9.0** | 최신 LTS 버전 채택, 성능 개선 | Phase 2 |
| **AI 모델 선택** | 하드코딩 모델명 | **ListModels API 동적 탐색** | 배포 환경별 사용 가능 모델이 달라 NOT_FOUND 오류 → 자동 탐색 구현 | Phase 3 |
| **연결 문자열 형식** | 키=값 형식 | **URI 자동 변환 지원** | Render가 `postgresql://` URI 형식 주입 → `NormalizeConnectionString()` 변환기 구현 | Phase 2 |

**현재 확정된 기술 스택** (위 변경사항 반영 완료):
- DB: PostgreSQL (Supabase) + Dapper — SQLite 및 Entity Framework 미사용
- AI: Google Gemini API + `ResolveModelNameAsync()` 동적 모델 탐색 + `IMemoryCache` 30분 캐시
- Backend: .NET 9.0 Web API (C#)

## 7. 🧠 에이전트 메모리 관리 전략 (Agent Memory Strategy)
본 프로젝트는 단발성 프롬프트가 아닌, 세션 간 지식 유지를 위해 영구 메모리(Persistent Memory) 전략을 사용합니다.
- **메모리 저장소:** `.claude/agent-memory/` 디렉토리 내의 마크다운 파일을 활용하여 에이전트 간 상태와 컨텍스트를 공유합니다.
- **Planner ↔ Close 연계:** `sprint-close` 에이전트가 스프린트를 마감할 때 발생한 기술적 부채나 다음 스프린트의 주의사항을 메모리 파일에 기록하면, 다음 `sprint-planner` 에이전트가 이를 읽고 계획에 반영(Feedback Loop)합니다.
- **문서화 원칙 (Single Source of Truth):** AI의 모든 의사결정과 아키텍처 변경 사항은 휘발성 채팅이 아닌 `ROADMAP.md`와 `docs/sprint/` 문서에 물리적 파일로 기록되어 추적 가능(Traceable)해야 합니다.