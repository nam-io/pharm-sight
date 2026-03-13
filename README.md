# PharmSight AI — 약국 경영 통합 AI 대시보드

약국의 처방·매출·지출 데이터를 한 화면에서 시각화하고, Google Gemini AI가 경영 인사이트를 자동 분석하여 제공하는 통합 AI 대시보드입니다.

![PharmSight AI 대시보드 화면](docs/pharm-sight-intro.png)

---

## 🚨 문제 정의 (Problem Statement)
현재 대부분의 로컬 약국은 처방(조제) 데이터와 일반매약(OTC) 데이터, 그리고 도매상 발주 데이터가 서로 다른 시스템에 파편화되어 있습니다. 이로 인해 약국 경영자는 **"우리 약국이 특정 소아과 처방전에 얼마나 의존하고 있는지"**, **"실질적인 수익을 내는 약품군은 무엇인지"**를 직관적인 데이터로 파악하기 어렵고, 감에 의존한 재고 관리를 하고 있는 실정입니다.

## 💡 기존 솔루션과의 차별성 (Differentiation)

### 경쟁 제품 분석

| 구분 | 기존 청구 프로그램 (비\*소프트, 유\*케어 등) | 엑셀 수기 관리 | **PharmSight AI** |
|------|----------------------------------------------|----------------|-------------------|
| 데이터 통합 | 청구 데이터만 | 개별 파일 분산 | **조제·매출·발주 단일 화면** |
| 시각화 | 표 형태 출력 | 수동 차트 작성 | **ECharts 6종 인터랙티브 차트** |
| AI 분석 | 없음 | 없음 | **Gemini AI 자동 경영 인사이트** |
| 실시간성 | 월말 결산 중심 | 수동 갱신 | **Supabase 실시간 집계** |
| 접근성 | 설치형 PC 전용 | PC 전용 | **브라우저 기반, 반응형** |

### PharmSight의 차별점

1. **통합 시각화:** 파편화된 조제/매출/지출 데이터를 단일 대시보드로 통합.
2. **직관적인 UX:** 경영자가 한눈에 파악할 수 있는 ECharts 기반의 반응형 모던 UI 제공.
3. **인사이트 중심:** '병원별 의존도', '연령대별 타겟' 등 즉각적인 경영 액션이 가능한 지표 도출.
4. **AI 경영 분석:** Google Gemini AI가 실시간 경영 데이터를 분석하여 요약·하이라이트·주의사항·추천을 자동 생성.

## 🌐 배포 URL (Deployment)
- **프론트엔드:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app) (Vercel)
- **백엔드 API:** [https://pharm-sight.onrender.com](https://pharm-sight.onrender.com) (Render)
- **데이터베이스:** Supabase PostgreSQL (실데이터 연동)

## 주요 기능

### AI 경영 분석 (신규)
- Google Gemini AI가 이번 달 경영 현황을 2~3문장으로 친절하게 요약
- 긍정적 하이라이트 및 주의 사항 배지 자동 생성
- 데이터 기반 실용적 경영 추천 조언 제공
- 응답 결과 30분 캐시로 API 비용 절감

### 처방 트렌드 분석
- 월별 총 매출 및 조제 건수 추이 (라인 차트)
- 전문의약품(ETC) vs 일반의약품(OTC) 매출 비중 시각화 (파이 차트)

### 고객 및 처방 기관 분석
- 방문 환자 연령대 분포 (도넛 차트)
- 처방전 발행 의료기관별 유입 건수 비교 (바 차트)

### 의약품 지출 분석
- 도매상별 누적 지출 현황 (바 차트)
- 약품 특성별(급여/비급여, 전문의약품/일반의약품) 지출 비율 (파이 차트)

---

## 기술 스택

| 레이어 | 기술 | 선택 근거 |
|--------|------|-----------|
| **Frontend** | Vue 3 (Composition API), TypeScript, Vite, Tailwind CSS, Apache ECharts | Composition API의 로직 재사용성, ECharts의 풍부한 차트 종류 |
| **Backend** | C# .NET 9.0 Web API | 강타입 시스템으로 API 계약 명확, 비동기 처리 성능 우수 |
| **Database** | PostgreSQL (Supabase), Npgsql, Dapper | 클라우드 배포 영속성, Dapper 순수 SQL로 복잡한 집계 쿼리 최적화 |
| **AI** | Google Gemini API | 무료 할당량 제공, 동적 모델 선택으로 API 변경에 유연 대응 |
| **Infra** | Vercel · Render · Supabase | 무료 티어 조합으로 해커톤 제약 내 풀스택 클라우드 배포 달성 |
| **Architecture** | Controller → Service → Repository | SRP 준수, 인터페이스 기반 DI로 테스트 용이성 극대화 |

> ⚠️ Entity Framework는 사용하지 않습니다. 모든 DB 접근은 Dapper를 통한 순수 SQL로 처리합니다.
>
> **SQLite → PostgreSQL 마이그레이션 근거:** Render 무료 플랜은 에페머럴(임시) 파일시스템을 사용하여 배포 시 SQLite 파일이 초기화됩니다. 클라우드 배포 환경의 데이터 영속성을 위해 Supabase PostgreSQL(무료 티어)로 전환하였으며, Dapper + Npgsql 조합으로 ORM 없이 `DATE_TRUNC`, `CTE` 등 PostgreSQL 고급 집계 쿼리를 활용합니다.

### 🧩 아키텍처 및 코드 품질 원칙 (Architecture & Code Quality)
- **관심사 분리 (SoC):** 백엔드는 Controller(요청/응답) - Service(비즈니스 로직) - Repository(데이터 접근)로 계층을 엄격히 분리하여 단일 책임 원칙(SRP)을 준수합니다.
- **의존성 주입 (DI):** 모든 Service와 Repository는 인터페이스(`IService`, `IRepository`)를 통해 의존성을 주입받아 모듈 간 결합도를 낮추고 테스트 용이성을 극대화합니다.
- **반응형 디자인 (Responsive UX):** Tailwind CSS의 Breakpoint(`sm:`, `md:`, `lg:`)를 적극 활용하여, 약국 카운터의 PC 모니터뿐만 아니라 약사의 모바일/태블릿 환경에서도 UI가 자연스럽게 동작하는 직관적인 사용자 경험(UX)을 제공합니다.

---

## 데이터베이스 스키마

```
Patients        (Id, DateOfBirth)
Hospitals       (Id, Name)
Drugs           (Id, Name, Type[ETC/OTC], IsCovered)
Prescriptions   (Id, PatientId → Patients, HospitalId → Hospitals, DispenseDate)
Orders          (Id, WholesaleName, DrugId → Drugs, Amount, OrderDate)
Sales           (Id, Amount, SaleDate, PrescriptionId → Prescriptions [nullable])
```

**주요 관계:**
- `Prescriptions.PatientId` → `Patients.Id`
- `Prescriptions.HospitalId` → `Hospitals.Id`
- `Orders.DrugId` → `Drugs.Id`
- `Sales.PrescriptionId` → `Prescriptions.Id` (조제약 매출은 처방전 연결, 일반의약품 매출은 null)

---

## 프로젝트 구조

```
pharm-sight/
├── frontend/               # Vue 3 + Vite 프론트엔드
│   ├── src/
│   │   ├── components/
│   │   │   ├── AiInsightPanel.vue   # AI 경영 분석 패널
│   │   │   └── charts/             # ECharts 차트 컴포넌트
│   │   ├── composables/
│   │   │   ├── useDashboardData.ts  # 대시보드 데이터 fetch
│   │   │   ├── useAiInsight.ts      # AI 인사이트 fetch
│   │   │   └── useKeepAlive.ts      # Render 슬립 방지 핑
│   │   ├── views/          # 페이지 컴포넌트
│   │   └── types/          # TypeScript 타입 정의
│   ├── .env.production     # Vercel 빌드용 API URL 설정
│   ├── package.json
│   └── vite.config.ts
├── backend/                # .NET 9.0 Web API 백엔드
│   ├── Controllers/        # HTTP 요청 처리
│   ├── Services/
│   │   ├── AiInsightService.cs   # Gemini AI 인사이트 생성
│   │   └── Interfaces/
│   ├── Repositories/       # DB 접근 (Dapper + Npgsql)
│   ├── Models/             # DTO 및 도메인 모델
│   └── PharmSight.Tests/   # xUnit 단위 테스트
├── database/
│   └── schema.sql          # PostgreSQL DDL 스크립트
├── docs/
│   ├── sprint/             # 스프린트 계획/완료 문서 (sprint1~3)
│   └── deploy-history/     # 배포 이력 아카이브
├── .github/workflows/      # CI/CD 파이프라인
├── CLAUDE.md               # AI 협업 가이드
└── ROADMAP.md              # 프로젝트 로드맵
```

---

## 시작하기

### 사전 요구사항
- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

### 백엔드 실행

```bash
cd backend
dotnet restore
dotnet run
# → http://localhost:5000 에서 실행
```

### 프론트엔드 실행

```bash
cd frontend
npm install
npm run dev
# → http://localhost:5173 에서 실행
```

### 환경 변수 설정

백엔드 `appsettings.json` 또는 환경변수:
```
ConnectionStrings__DefaultConnection=<Supabase PostgreSQL 연결 문자열>
Gemini__ApiKey=<Google Gemini API 키>
```

프론트엔드 `.env.production`:
```
VITE_API_BASE_URL=https://pharm-sight.onrender.com
```

---

## 검증 계획

### 백엔드 단위 테스트 (xUnit + Moq)

**테스트 파일:**
- [`backend/PharmSight.Tests/Services/DashboardServiceTests.cs`](backend/PharmSight.Tests/Services/DashboardServiceTests.cs) — 9개 케이스
- [`backend/PharmSight.Tests/Services/AiInsightServiceTests.cs`](backend/PharmSight.Tests/Services/AiInsightServiceTests.cs) — 4개 케이스
- [`backend/PharmSight.Tests/PharmSight.Tests.csproj`](backend/PharmSight.Tests/PharmSight.Tests.csproj) — 프로젝트 설정

**실행:**
```bash
dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj --verbosity normal
```

**실행 결과 (로컬 검증 완료):**
```
총 테스트 수: 13
     통과: 13
    경고 0개  오류 0개
경과 시간: 00:00:05.07
```

| 테스트 클래스 | 케이스 수 | 주요 검증 내용 |
|---------------|-----------|----------------|
| `DashboardServiceTests` | 9개 | 월별매출·약품유형·연령대·병원·도매·급여·KPI 반환값, 빈결과 엣지, 변화율 0 엣지 |
| `AiInsightServiceTests` | 4개 | API키 미설정 Graceful Degradation, Repository 미호출, GeneratedAt 설정, 캐시 히트 |

> 전체 테스트 실행 결과 및 코드 전문: [`docs/test-results.md`](docs/test-results.md)

### CI/CD 자동화 (GitHub Actions)

**파이프라인 파일:** [`.github/workflows/ci.yml`](.github/workflows/ci.yml)

`master` / `develop` 브랜치 push 및 PR 시 자동 실행:

```
push/PR
├── backend-test:  .NET 9.0 setup → dotnet restore → dotnet build → dotnet test → TRX artifact 업로드
└── frontend-build: Node 20 setup → npm ci → Vite 프로덕션 빌드 (VITE_API_BASE_URL 주입)
```

> 파이프라인 전체 YAML 및 구성 설명: [`docs/ci-cd.md`](docs/ci-cd.md)

### 개발 진행 추적

- 51개 Git 커밋, Conventional Commits 형식 (`feat:`, `fix:`, `docs:`, `style:`, `chore:`)
- 스프린트별 `docs/sprint/sprint0.md` ~ `sprint4.md` 문서에 작업 분해·완료 조건 기록
- 브랜치: `sprint/sprint1`, `sprint/sprint2`, `sprint/sprint3_4`

---

## 개발 워크플로우

이 프로젝트는 AI 에이전트 기반 Agile 워크플로우를 따릅니다.

| 에이전트 | 역할 |
|----------|------|
| `prd-to-roadmap` | PRD → ROADMAP.md 자동 생성 |
| `sprint-planner` | ROADMAP 기반 스프린트 계획 수립 |
| `sprint-close` | 스프린트 완료 처리 및 상태 업데이트 |
| `hotfix-close` | 긴급 버그 수정 마무리 |

각 에이전트의 상세 프롬프트는 [`.claude/agents/`](.claude/agents/) 디렉토리에, 스프린트별 진행 기록은 [`docs/sprint/`](docs/sprint/) 디렉토리에서 확인할 수 있습니다.
