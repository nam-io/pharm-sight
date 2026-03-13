# Sprint 2: 백엔드 API 구현 및 클라우드 실배포

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 2 |
| **연결된 Phase** | Phase 2: 백엔드 API 구현 및 클라우드 배포 |
| **목표 시간** | 3h ~ 6h |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | ✅ 완료 (배포 버그 수정 포함) |
| **작업 브랜치** | `sprint/sprint2` |
| **백엔드 배포 URL** | https://pharm-sight.onrender.com |
| **프론트엔드 배포 URL** | https://pharm-sight-frontend.vercel.app |

---

## 목표 (Goal)

Supabase PostgreSQL 실데이터 기반의 .NET 백엔드 API를 Render에 배포하고,
프론트엔드와 실연동하여 대시보드에 실제 데이터를 표시한다.

---

## 주요 구현 내역

### T2-1: 인프라 전환 — SQLite → Supabase PostgreSQL + Render

- **DB**: Supabase PostgreSQL 인스턴스 구성
  - `database/schema.sql` SQLite DDL → PostgreSQL DDL 변환 (SERIAL, BOOLEAN, NUMERIC, DATE)
  - `database/seed.sql` 작성 — `generate_series` 기반 100명 환자, 1800건 처방, 2400건 매출
- **백엔드 패키지**: `Microsoft.Data.Sqlite` → `Npgsql` + `Npgsql.DependencyInjection` 전환
- **Render 배포**: Docker 멀티스테이지 빌드 (`sdk:9.0` → `aspnet:9.0`)
  - `backend/Dockerfile` 작성
  - `render.yaml` 작성 — `dockerfilePath: ./backend/Dockerfile`
  - Render 환경변수: `ConnectionStrings__DefaultConnection` (Supabase URI)

### T2-2: 백엔드 Controller / Service / Repository 구현

**아키텍처:** Controller → Service → Repository (SRP 계층 분리)

| 파일 | 설명 |
|------|------|
| `backend/Models/DashboardModels.cs` | 7개 C# record 모델 |
| `backend/Repositories/Interfaces/IDashboardRepository.cs` | Repository 인터페이스 |
| `backend/Repositories/DashboardRepository.cs` | Dapper + NpgsqlConnection 쿼리 구현 |
| `backend/Services/Interfaces/IDashboardService.cs` | Service 인터페이스 |
| `backend/Services/DashboardService.cs` | Service 구현 (ILogger 포함) |
| `backend/Controllers/DashboardController.cs` | 7개 GET 엔드포인트 |
| `backend/Middleware/GlobalExceptionMiddleware.cs` | 전역 예외 처리 → 일관된 JSON 응답 |

**구현된 API 엔드포인트:**
- `GET /api/dashboard/monthly-sales` — 최근 12개월 월별 매출·조제건수
- `GET /api/dashboard/drug-type-sales` — Rx/OTC 매출 비중
- `GET /api/dashboard/patient-ages` — 환자 연령대 분포
- `GET /api/dashboard/hospital-prescriptions` — 의료기관별 처방 유입 TOP 6
- `GET /api/dashboard/wholesale-expenses` — 도매상별 지출 현황
- `GET /api/dashboard/drug-coverage` — 급여/비급여 지출 비율
- `GET /api/dashboard/kpi` — 이번달/전월 대비 KPI 요약 (CTE 쿼리)

### T2-3: Render 슬립 방지 Keep-Alive 구현

- **백엔드**: `HealthController` — `GET /api/health` 응답 (status, timestamp, message)
- **프론트엔드**: `useKeepAlive.ts` composable — 10분 주기 자동 핑, `onUnmounted` 정리

### T2-4: 프론트엔드 실데이터 연동

- `useDashboardData.ts` 수정: `VITE_API_BASE_URL` 환경변수 기반 실API 호출
- `frontend/.env.production` 추가: `VITE_API_BASE_URL=https://pharm-sight.onrender.com`
- KPI 카드 실데이터 연산 (만원 단위 변환, 전월 대비 변화율 표시)
- 대시보드 헤더 배지: 정적 Mock 표기 → 동적 현재 날짜 + "Supabase 연동"

---

## 배포 버그 수정 이력

### BUG-1: `render.yaml` Dockerfile 경로 오류
- **증상**: Render 빌드 시 "Dockerfile not found" 에러
- **원인**: `dockerfilePath` 미설정, Render가 루트에서 Dockerfile을 탐색
- **수정**: `render.yaml`에 `dockerfilePath: ./backend/Dockerfile` 명시

### BUG-2: PostgreSQL URI 형식 연결 문자열 파싱 실패
- **증상**: `/api/dashboard/*` 500 에러 — "Format of initialization string does not conform to specification"
- **원인**: Npgsql이 `postgresql://user:pass@host/db` URI 형식을 직접 파싱하지 못함
- **수정**: `DashboardRepository.NormalizeConnectionString()` — URI를 Npgsql 키=값 형식으로 변환

### BUG-3: Render IPv6 아웃바운드 미지원
- **증상**: `/api/dashboard/*` 500 에러 — "Failed to connect to [IPv6]:5432"
- **원인**: Render 무료 플랜은 IPv6 아웃바운드 미지원. Supabase 호스트 DNS가 AAAA 레코드로 해석됨
- **수정**: `DashboardRepository.CreateConnectionAsync()` — `Dns.GetHostAddressesAsync(host, AddressFamily.InterNetwork)`로 IPv4 주소만 선택적 해석

### BUG-4: Vercel 빌드 시 `VITE_API_BASE_URL` 미주입
- **증상**: 프론트엔드에서 "API 오류 · 임시 데이터" 표시
- **원인**: `.env.production` 파일 없음 → `USE_MOCK = true` → API 호출 없이 Mock 데이터만 사용
- **수정**: `frontend/.env.production` 추가

---

## 기술 결정 사항 (Technical Decisions)

| 결정 | 이유 |
|------|------|
| SQLite → Supabase PostgreSQL | 클라우드 배포 시 파일 시스템 휘발성, 실평가 환경 대응 |
| Render Docker 배포 | .NET 9.0 런타임 지원, 무료 플랜 가용 |
| `render.yaml` 사용 | Render 배포 설정 코드로 관리 (IaC) |
| `NormalizeConnectionString()` | Render 환경변수가 URI 형식이어도 자동 변환 |
| `CreateConnectionAsync()` IPv4 강제 | Render 무료 플랜 IPv6 미지원 대응, 코드 레벨 해결 |
| `frontend/.env.production` Git 커밋 | 공개 API URL이므로 비밀 아님, Vercel 대시보드 설정 불필요 |

---

## 완료 조건 (Definition of Done)

- [x] `GET /api/health` 200 응답 확인
- [x] `GET /api/dashboard/monthly-sales` 200 응답 및 데이터 반환
- [x] `GET /api/dashboard/kpi` 200 응답 및 KPI 데이터 반환
- [x] 프론트엔드 대시보드에서 "● 실시간 데이터 연동" 배지 표시
- [x] 6개 차트 패널에 Supabase 실데이터 표시
- [x] CORS 설정: `https://pharm-sight-frontend.vercel.app` 허용
- [x] GlobalExceptionMiddleware 동작 — 500 에러 시 `{"error":"...", "statusCode":500}` 반환

---

## 산출물 (Deliverables)

| 파일/서비스 | 설명 |
|-------------|------|
| `backend/Dockerfile` | .NET 9.0 멀티스테이지 Docker 이미지 |
| `render.yaml` | Render 자동 배포 설정 |
| `backend/Controllers/DashboardController.cs` | 7개 API 엔드포인트 |
| `backend/Controllers/HealthController.cs` | Keep-Alive 헬스체크 |
| `backend/Services/DashboardService.cs` | 비즈니스 로직 계층 |
| `backend/Repositories/DashboardRepository.cs` | Dapper PostgreSQL 쿼리 |
| `backend/Middleware/GlobalExceptionMiddleware.cs` | 전역 예외 처리 |
| `database/schema.sql` | PostgreSQL DDL (6개 테이블) |
| `database/seed.sql` | 시드 데이터 |
| `frontend/src/composables/useKeepAlive.ts` | 백엔드 Keep-Alive composable |
| `frontend/.env.production` | Vercel 빌드용 API URL 환경변수 |
| Render 배포 | https://pharm-sight.onrender.com |

---

## 기술 부채 및 다음 단계

- **단위 테스트 미구현**: `PharmSight.Tests` (xUnit, Moq) Repository Mocking 테스트 → Backlog
- **Render 콜드 스타트**: 무료 플랜 특성상 첫 요청 시 15~30초 지연 가능 (Keep-Alive로 완화)
- **환경변수 보안 검토**: 운영 확장 시 Vercel 대시보드에서 민감 정보 관리 권장
