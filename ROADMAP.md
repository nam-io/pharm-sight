# 🗺️ PharmSight 프로젝트 로드맵

## 개요
- **목표:** 약국의 처방·매출·지출 데이터를 한 화면에서 시각화하는 경영 통합 대시보드 구축
- **전체 예상 기간:** 약 6시간 (초단기 해커톤 1인 집중 스프린트)
- **배포 전략:** 프론트엔드(Vue 3) Mock 데이터 기반 Vercel 정적 배포 (보너스 10점 타겟)
- **현재 진행 단계:** Phase 0 진행 예정 (프로젝트 인프라 셋업)

---

## 📊 프로젝트 현황 대시보드

| 항목 | 현황 |
|------|------|
| 전체 진행률 | 0% (0/3 Phase 완료) |
| 현재 Phase | Phase 0: 프로젝트 인프라 셋업 |
| 다음 마일스톤 | M1 — Vercel 배포 완료 및 README URL 업데이트 |
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

### 📋 Phase 0: 뼈대 구축 및 인프라 셋업 (0h ~ 1h)
**상태:** 📋 예정

#### 목표
개발 환경 기반 완성: .NET + Vue 3 프로젝트 생성, SQLite 스키마 초기화

#### 작업 목록
- [ ] **[.NET 백엔드]**: `dotnet new webapi -n PharmSight.Api`, `Dapper`, `Microsoft.Data.Sqlite` 설치
- [ ] **[DB 초기화]**: `database/schema.sql` (6개 테이블 DDL) 생성
- [ ] **[Vue 3 프론트엔드]**: `npm create vite@latest frontend -- --template vue-ts`, Tailwind CSS, ECharts 설치

---

### 📋 Phase 1: 프론트엔드 UI 개발 및 Vercel 배포 (1h ~ 3h) 🌟 [보너스 10점 타겟]
**상태:** 📋 예정

#### 목표
Mock 데이터 기반으로 대시보드 UI를 완성하고, Vercel에 즉시 배포하여 데모 URL 확보

#### 작업 목록
- [ ] **[UI 컴포넌트]**: AppLayout 및 6개 ECharts 패널(처방 트렌드, 약품 지출 등) 구현
- [ ] **[Mock 데이터]**: `useDashboardData.ts` 작성
- [ ] **[Vercel 배포]**: `npm install -g vercel`, Frontend 폴더에서 `vercel` 명령어 실행
- [ ] **https://www.wordhippo.com/what-is/the-meaning-of/korean-word-4f72dd68a6b9dbb556794ea1b973edf4068599dd.html**: 발급받은 배포 URL을 `README.md` 최상단에 기록

---

### 📋 Phase 2: 백엔드 API 구현 및 테스트 (3h ~ 6h)
**상태:** 📋 예정

#### 목표
SQLite 실제 데이터 기반 Dapper API 구현 및 단위 테스트 작성 (코드 품질/검증 계획 25점 타겟)

#### 작업 목록
- [ ] **[Dapper Repository]**: 통계 쿼리(CTE, GROUP BY) 구현
- [ ] **[Service & Controller]**: 비즈니스 로직 및 전역 에러 처리(try-catch) 구현
- [ ] **[단위 테스트]**: `PharmSight.Tests` (xUnit, Moq) 테스트 작성
- [ ] **[최종 점검]**: `sprint-close`를 통한 최종 커밋 및 제출물(.zip) 패키징

---

## 🎯 MoSCoW 우선순위
| 분류 | 항목 |
|------|------|
| **Must Have** | Phase 0, 1 완성 및 Vercel 배포 (UI 시연 가능 상태) |
| **Should Have** | Phase 2 (백엔드 API 구현 및 xUnit 테스트) |
| **Could Have** | 프론트엔드-백엔드 실제 연동 (시간 내 가능 시) |
| **Won't Have** | 로그인/인증, 실시간 데이터 동기화 |

---

## 🔮 향후 계획 (Backlog)

- **기간 필터**: 날짜 범위(월/분기/연도) 선택 시 차트 데이터 재조회
- **데이터 내보내기**: 차트 데이터 CSV/Excel 다운로드
- **인쇄 최적화**: 대시보드 인쇄용 CSS 미디어 쿼리
- **다크 모드**: Tailwind `dark:` 클래스 기반 다크 테마
- **멀티 약국 지원**: 약국 선택 드롭다운, 계정 기반 데이터 분리
- **PostgreSQL 마이그레이션**: 규모 확장 시 SQLite → PostgreSQL 전환