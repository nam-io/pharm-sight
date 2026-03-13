# PharmSight — 약국 경영 통합 대시보드

약국의 처방·매출·지출 데이터를 한 화면에서 시각화하여 경영 의사결정을 지원하는 통합 대시보드입니다.

---

## 🚨 문제 정의 (Problem Statement)
현재 대부분의 로컬 약국은 처방(조제) 데이터와 일반매약(OTC) 데이터, 그리고 도매상 발주 데이터가 서로 다른 시스템에 파편화되어 있습니다. 이로 인해 약국 경영자는 **"우리 약국이 특정 소아과 처방전에 얼마나 의존하고 있는지"**, **"실질적인 수익을 내는 약품군은 무엇인지"**를 직관적인 데이터로 파악하기 어렵고, 감에 의존한 재고 관리를 하고 있는 실정입니다.

## 💡 기존 솔루션과의 차별성 (Differentiation)
- **기존 청구 프로그램의 한계:** 건강보험 청구 목적에 치중되어 있어 경영 지표 시각화(UI/UX)가 매우 부족함.
- **PharmSight의 차별점:** 1. **통합 시각화:** 파편화된 조제/매출/지출 데이터를 단일 대시보드로 통합.
  2. **직관적인 UX:** 경영자가 한눈에 파악할 수 있는 ECharts 기반의 화려하고 반응형인 모던 UI 제공.
  3. **인사이트 중심:** 단순 데이터 나열이 아닌, '병원별 의존도', '연령대별 타겟' 등 즉각적인 경영 액션이 가능한 지표 도출.

## 🌐 배포 URL (Deployment)
- **프론트엔드 (데모 시연용):** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app)
- *비고: 해커톤 데모 시연을 위해 프론트엔드는 Vercel에 배포되며, 백엔드 로컬 DB(SQLite) 제약으로 인해 데모 환경에서는 Mock 데이터가 동작하도록 구성됩니다.*

## 주요 기능

### 처방 트렌드 분석
- 월별 총 매출 및 조제 건수 추이 (라인 차트)
- 조제약(Rx) vs 일반의약품(OTC) 매출 비중 시각화 (파이 차트)

### 고객 및 처방 기관 분석
- 방문 환자 연령대 분포 (도넛 차트)
- 처방전 발행 의료기관별 유입 건수 비교 (바 차트)

### 의약품 지출 분석
- 도매상별 누적 지출 현황 (바 차트)
- 약품 특성별(급여/비급여, 전문의약품/일반의약품) 지출 비율 (파이 차트)

---

## 기술 스택

| 레이어 | 기술 |
|--------|------|
| **Frontend** | Vue 3 (Composition API, `<script setup>`), TypeScript, Vite, Tailwind CSS, Apache ECharts |
| **Backend** | C# .NET Core 8.0+ Web API |
| **Database** | SQLite (`Microsoft.Data.Sqlite`), Dapper |
| **Architecture** | Controller → Service → Repository 계층 분리 (SRP 준수) |

> ⚠️ Entity Framework는 사용하지 않습니다. 모든 DB 접근은 Dapper를 통한 순수 SQL로 처리합니다.

### 🧩 아키텍처 및 코드 품질 원칙 (Architecture & Code Quality)
- **관심사 분리 (SoC):** 백엔드는 Controller(요청/응답) - Service(비즈니스 로직) - Repository(데이터 접근)로 계층을 엄격히 분리하여 단일 책임 원칙(SRP)을 준수합니다.
- **의존성 주입 (DI):** 모든 Service와 Repository는 인터페이스(`IService`, `IRepository`)를 통해 의존성을 주입받아 모듈 간 결합도를 낮추고 테스트 용이성을 극대화합니다.
- **반응형 디자인 (Responsive UX):** Tailwind CSS의 Breakpoint(`sm:`, `md:`, `lg:`)를 적극 활용하여, 약국 카운터의 PC 모니터뿐만 아니라 약사의 모바일/태블릿 환경에서도 UI가 자연스럽게 동작하는 직관적인 사용자 경험(UX)을 제공합니다.

---

## 데이터베이스 스키마

```
Patients        (Id, DateOfBirth)
Hospitals       (Id, Name)
Drugs           (Id, Name, Type[Rx/OTC], IsCovered)
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
│   │   ├── components/     # 재사용 UI 컴포넌트
│   │   ├── composables/    # Vue Composables (useDashboardData.ts 등)
│   │   ├── views/          # 페이지 컴포넌트
│   │   └── types/          # TypeScript 타입 정의
│   ├── package.json
│   └── vite.config.ts
├── backend/                # .NET Core 8.0 Web API 백엔드
│   ├── Controllers/        # HTTP 요청 처리
│   ├── Services/           # 비즈니스 로직
│   ├── Repositories/       # DB 접근 (Dapper)
│   ├── Models/             # DTO 및 도메인 모델
│   └── PharmSight.Tests/   # xUnit 단위 테스트
├── database/
│   ├── pharm-sight.db      # SQLite 데이터베이스
│   └── schema.sql          # DDL 스크립트
├── docs/
│   ├── sprint/             # 스프린트 계획/완료 문서
│   └── deploy-history/     # 배포 이력 아카이브
├── .github/workflows/      # CI/CD 파이프라인
├── CLAUDE.md               # AI 협업 가이드
└── ROADMAP.md              # 프로젝트 로드맵
```

---

## 시작하기

### 사전 요구사항
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
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

### 데이터베이스 초기화

```bash
# SQLite DB 스키마 생성 (최초 1회)
sqlite3 database/pharm-sight.db < database/schema.sql
```

### 환경 변수 설정

```bash
cp .env.example .env
# .env 파일을 열어 필요한 값 수정
```

---

## 검증 계획

### 백엔드 단위 테스트 (xUnit)

```bash
cd backend
dotnet test PharmSight.Tests/ -v normal
```

- Service 계층 메서드별 단위 테스트
- Repository는 Mocking 처리 (`Moq` 라이브러리 사용)

### 개발 진행 추적

- 기능 단위로 Git 커밋 이력 관리
- 스프린트별 `docs/sprint/sprint{N}.md` 문서에 진행 내역 기록

---

## 개발 워크플로우

이 프로젝트는 AI 에이전트 기반 Agile 워크플로우를 따릅니다.

| 에이전트 | 역할 |
|----------|------|
| `prd-to-roadmap` | PRD → ROADMAP.md 자동 생성 |
| `sprint-planner` | ROADMAP 기반 스프린트 계획 수립 |
| `sprint-close` | 스프린트 완료 처리 및 상태 업데이트 |
| `hotfix-close` | 긴급 버그 수정 마무리 |

자세한 개발 프로세스는 [`docs/dev-process.md`](docs/dev-process.md)를 참조하세요.
