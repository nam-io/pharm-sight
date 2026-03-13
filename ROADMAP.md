# 🗺️ PharmSight 프로젝트 로드맵

## 개요
- **목표:** 약국의 처방·매출·지출 데이터를 한 화면에서 시각화하는 경영 통합 대시보드 구축
- **전체 예상 기간:** 약 6시간 (초단기 해커톤 1인 집중 스프린트)
- **배포 전략:** 프론트엔드(Vue 3) Vercel + 백엔드(.NET) Render + DB Supabase(PostgreSQL)
- **현재 진행 단계:** Phase 3 완료 — AI 경영 분석 기능 구현 및 전 Phase 배포 완료

---

## 📊 프로젝트 현황 대시보드

| 항목 | 현황 |
|------|------|
| 전체 진행률 | 100% (4/4 Phase 완료) |
| 현재 Phase | Phase 4 완료 / 전 Phase 완료 |
| 프론트엔드 배포 | https://pharm-sight-frontend.vercel.app ✅ |
| 백엔드 배포 | https://pharm-sight.onrender.com ✅ |
| DB | Supabase PostgreSQL ✅ |
| 팀 규모 | 1인 (풀스택 개발) |

---

## 진행 상태 범례
- ✅ 완료
- 🔄 진행 중
- 📋 예정
- ⏸️ 보류

---

## 📅 Phase별 상세 계획 (6시간 타임라인)

---

### ✅ Phase 0: 뼈대 구축 및 인프라 셋업 (0h ~ 1h)
**상태:** ✅ 완료

#### 목표
개발 환경 기반 완성: .NET + Vue 3 프로젝트 생성, PostgreSQL 스키마 초기화

#### 작업 목록
- [x] **[.NET 백엔드]**: `dotnet new webapi -n PharmSight.Api`, `Dapper`, `Npgsql` 설치
- [x] **[DB 초기화]**: `database/schema.sql` (6개 테이블 DDL) 생성 — PostgreSQL 문법
- [x] **[DB 시드]**: `database/seed.sql` 100명 환자, 1800건 처방, 2400건 매출 데이터 생성
- [x] **[Vue 3 프론트엔드]**: `npm create vite@latest frontend -- --template vue-ts`, Tailwind CSS v4, ECharts 설치

---

### ✅ Phase 1: 프론트엔드 UI 개발 및 Vercel 배포 (1h ~ 3h) 🌟 [보너스 10점 타겟]
**상태:** ✅ 완료

#### 목표
Mock 데이터 기반으로 대시보드 UI를 완성하고, Vercel에 즉시 배포하여 데모 URL 확보

#### 작업 목록
- [x] **[UI 컴포넌트]**: AppLayout 및 6개 ECharts 패널 구현 (라인/파이/도넛/바 차트)
- [x] **[Mock 데이터]**: `useDashboardData.ts` 작성 (API 연동/Mock 이중 모드)
- [x] **[Vercel 배포]**: https://pharm-sight-frontend.vercel.app 배포 완료
- [x] **[URL 기록]**: `README.md` 최상단에 배포 URL 기록 완료

---

### ✅ Phase 2: 백엔드 API 구현 및 클라우드 배포 (3h ~ 6h)
**상태:** ✅ 완료 (배포 버그 수정 진행 중)

#### 목표
Supabase(PostgreSQL) 실데이터 기반 Dapper API 구현, Render 배포, 프론트엔드 실연동

#### 작업 목록
- [x] **[인프라 전환]**: SQLite → Supabase PostgreSQL, Render Docker 배포
- [x] **[Dapper Repository]**: 7개 통계 쿼리(CTE, GROUP BY, DATE_TRUNC) 구현
- [x] **[Service & Controller]**: 비즈니스 로직, 전역 에러 처리(`GlobalExceptionMiddleware`) 구현
- [x] **[HealthController]**: Render 슬립 방지 Keep-Alive 헬스체크 엔드포인트 구현
- [x] **[프론트엔드 실연동]**: `VITE_API_BASE_URL` 환경변수 기반 실데이터 연동 완료
- [x] **[Keep-Alive]**: `useKeepAlive.ts` composable — 10분 주기 백엔드 핑
- [x] **[배포 버그 수정]**: URI 형식 연결 문자열 파싱 오류, IPv6 연결 실패 수정
- [x] **[단위 테스트]**: `PharmSight.Tests` (xUnit 2.9.2, Moq 4.20.72) — DashboardService 9개 + AiInsightService 4개, 총 13개 통과

---

### ✅ Phase 3: AI 경영 분석 기능 추가 (Sprint 3)
**상태:** ✅ 완료

#### 목표
Google Gemini API를 연동하여 약국 경영 데이터를 자동 분석하고, 대시보드 상단에 친절한 AI 인사이트를 표시한다.

#### 작업 목록
- [x] **[백엔드 AI 서비스]**: `AiInsightService` — Google Gemini API 호출, 동적 모델 선택, 경영 데이터 기반 요약 생성 (30분 캐시)
- [x] **[백엔드 컨트롤러]**: `GET /api/ai/insight` 엔드포인트
- [x] **[프론트엔드 패널]**: `AiInsightPanel.vue` — 요약·하이라이트·경고·추천 표시, 로딩 스켈레톤
- [x] **[앱 리브랜딩]**: "약국 경영 통합 AI 대시보드" 명칭 반영
- [x] **[Render 환경변수]**: `Gemini__ApiKey` 등록

---

### ✅ Phase 4: 테스트 전략 및 CI/CD 파이프라인 (Sprint 4)
**상태:** ✅ 완료

#### 목표
xUnit 단위 테스트 및 GitHub Actions CI 파이프라인을 구축하여 해커톤 "검증 계획" 평가 기준에 대응한다.

#### 작업 목록
- [x] **[xUnit 테스트]**: `PharmSight.Tests` — DashboardService 9개 + AiInsightService 4개, 총 13개 통과
- [x] **[CI/CD 재구성]**: `.github/workflows/ci.yml` — Python 템플릿 → .NET 9 + Vue 3 실제 파이프라인
- [x] **[CLAUDE.md 현행화]**: SQLite→PostgreSQL, .NET 8→9, Gemini AI 항목 반영
- [x] **[push 배포]**: `git push origin master` → GitHub Actions 트리거

---

## 🎯 MoSCoW 우선순위
| 분류 | 항목 | 상태 |
|------|------|------|
| **Must Have** | Phase 0, 1 완성 및 Vercel 배포 | ✅ 완료 |
| **Should Have** | Phase 2 백엔드 API + Render/Supabase 배포 | ✅ 완료 |
| **Could Have** | 프론트엔드-백엔드 실데이터 연동 | ✅ 완료 |
| **Could Have** | xUnit 단위 테스트 | ✅ 완료 (13개 통과) |
| **Could Have** | 기간 필터 (최근 3/6/12개월 선택) | ✅ 완료 |
| **Could Have** | 데이터 내보내기 (CSV 다운로드) | ✅ 완료 |
| **Won't Have** | 로그인/인증, 실시간 데이터 동기화 | — |

---

## 🔮 향후 계획 (Backlog)

- **인쇄 최적화**: 대시보드 인쇄용 CSS 미디어 쿼리
- **다크 모드**: Tailwind `dark:` 클래스 기반 다크 테마
- **멀티 약국 지원**: 약국 선택 드롭다운, 계정 기반 데이터 분리