# Sprint 0: 뼈대 구축 및 인프라 셋업

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 0 |
| **연결된 Phase** | Phase 0: 뼈대 구축 및 인프라 셋업 |
| **목표 시간** | 0h ~ 1h (60분 이내 완료) |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | 🔄 진행 중 |

---

## 목표 (Goal)

개발을 즉시 시작할 수 있는 실행 가능한 프로젝트 뼈대를 구축한다.

- `.NET Core 8.0 Web API` 프로젝트가 로컬에서 정상 실행 (`dotnet run`)
- `Vue 3 + Vite + TypeScript` 프론트엔드가 로컬에서 정상 실행 (`npm run dev`)
- `database/schema.sql`에 6개 테이블 DDL이 작성되어 SQLite DB 초기화 가능

---

## 범위 (Scope)

### In Scope
- `backend/` 폴더: .NET 8.0 Web API 프로젝트 생성 및 Dapper, SQLite 패키지 설치
- `database/schema.sql`: 6개 테이블 DDL 작성 (Patients, Hospitals, Drugs, Prescriptions, Orders, Sales)
- `frontend/` 폴더: Vue 3 + Vite + TypeScript 스캐폴딩, Tailwind CSS v4 및 Apache ECharts 설치

### Out of Scope
- 실제 API 엔드포인트 구현 → Phase 2에서 처리
- 대시보드 UI 컴포넌트 → Phase 1에서 처리
- 데이터 시딩(seed data) → Phase 2에서 처리
- 프론트엔드-백엔드 실제 연동 → Could Have (선택적)

---

## 작업 분해 (Task Breakdown)

### T0-1: .NET Web API 프로젝트 생성
- **명령어:**
  ```bash
  dotnet new webapi -n PharmSight.Api -o backend
  ```
- **산출물:** `backend/` 폴더에 .NET 8.0 Web API 기본 프로젝트
- **완료 조건:** `cd backend && dotnet run` 실행 시 Swagger UI 또는 기본 응답 확인

### T0-2: NuGet 패키지 설치
- **명령어:**
  ```bash
  cd backend
  dotnet add package Dapper
  dotnet add package Microsoft.Data.Sqlite
  ```
- **완료 조건:** `backend/PharmSight.Api.csproj`에 두 패키지 참조 추가 확인

### T0-3: database/schema.sql 작성
- **파일:** `database/schema.sql`
- **내용:** 6개 테이블 CREATE TABLE 구문 (SQLite 호환, FK 제약 포함)
- **완료 조건:** `sqlite3 database/pharm-sight.db < database/schema.sql` 실행 성공

### T0-4: Vue 3 + Vite + TS 프로젝트 생성
- **명령어:**
  ```bash
  npm create vite@latest frontend -- --template vue-ts
  cd frontend && npm install
  ```
- **산출물:** `frontend/` 폴더에 Vue 3 + TypeScript 기본 프로젝트
- **완료 조건:** `npm run dev` 실행 시 Vite 개발 서버 기동

### T0-5: Tailwind CSS v4 설정
- **명령어:**
  ```bash
  cd frontend
  npm install tailwindcss @tailwindcss/vite
  ```
- `vite.config.ts`에 `@tailwindcss/vite` 플러그인 등록
- `src/style.css` 상단에 `@import "tailwindcss"` 추가
- **완료 조건:** `class="text-blue-500"` 등 Tailwind 유틸리티 클래스가 브라우저에 적용됨

### T0-6: Apache ECharts 설치
- **명령어:**
  ```bash
  cd frontend
  npm install echarts vue-echarts
  ```
- **완료 조건:** `package.json`에 echarts, vue-echarts 의존성 확인

---

## 기술 접근법 (Technical Approach)

### Backend
- `dotnet new webapi` 기본 템플릿 사용 (WeatherForecast 예제 파일 유지 또는 삭제)
- `Program.cs`의 기본 DI 컨테이너 구조 확인 — Phase 2에서 Repository/Service 등록에 활용
- **Dapper + SQLite 연결 방식:** `IDbConnection`을 `Microsoft.Data.Sqlite.SqliteConnection`으로 DI 등록

### Database (SQLite)
- 날짜 필드: `TEXT` 타입으로 ISO 8601 형식(`YYYY-MM-DD`) 저장
- Boolean 필드 (`IsCovered`): `INTEGER` 타입 (0/1)
- FK 참조 무결성: `FOREIGN KEY` 구문 + `PRAGMA foreign_keys = ON` 포함
- 금액 필드 (`Amount`): `REAL` 타입 (소수점 지원)

### Frontend
- Tailwind CSS v4: `tailwind.config.js` 불필요, `@tailwindcss/vite` 플러그인 방식
- ECharts: `vue-echarts` 래퍼 컴포넌트를 통해 Vue 3 반응형 통합
- TypeScript strict 모드 유지

---

## 완료 조건 (Definition of Done)

- [ ] `backend/` 폴더 존재, `dotnet build` 성공 (오류 0건)
- [ ] `backend/PharmSight.Api.csproj`에 Dapper, Microsoft.Data.Sqlite 패키지 참조 포함
- [ ] `database/schema.sql` 파일 존재, 6개 테이블 DDL 완성
- [ ] `frontend/` 폴더 존재, `npm run build` 성공
- [ ] `frontend/package.json`에 tailwindcss, @tailwindcss/vite, echarts, vue-echarts 포함
- [ ] `docs/sprint/sprint0.md` 문서 작성 완료 (현재 파일)

---

## 산출물 (Deliverables)

| 파일/폴더 | 설명 |
|-----------|------|
| `backend/` | .NET 8.0 Web API 프로젝트 (Dapper, SQLite 패키지 포함) |
| `database/schema.sql` | 6개 테이블 DDL 스크립트 |
| `frontend/` | Vue 3 + Vite + TS + Tailwind + ECharts 프로젝트 |
| `docs/sprint/sprint0.md` | 본 스프린트 계획 문서 |

---

## 다음 단계 (Next Phase)

Sprint 0 완료 후 **Phase 1 (1h ~ 3h)** 시작:
- `frontend/src/composables/useDashboardData.ts` 작성 (Mock 데이터)
- 6개 ECharts 대시보드 패널 컴포넌트 구현
- Vercel 배포 및 README URL 업데이트
