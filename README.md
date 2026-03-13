# PharmSight AI — 약국 경영 통합 AI 대시보드

약국의 처방·매출·지출 데이터를 한 화면에서 시각화하고, Google Gemini AI가 경영 인사이트를 자동 분석하여 제공하는 통합 AI 대시보드입니다.

![PharmSight AI 대시보드 화면](docs/pharm-sight-intro.png)

---

## 🚨 문제 정의 (Problem Statement)

### 약국 경영의 데이터 사각지대

국내에는 **약 25,000개**의 지역 약국이 운영 중이며(건강보험심사평가원, 2024), 이 중 대부분은 소규모 개인 약국입니다. 약국은 조제(처방약)와 일반 의약품 판매, 도매 발주를 동시에 수행하는 복합 경영 구조를 가지고 있지만, 이 세 가지 데이터 흐름이 **서로 다른 시스템에 파편화**되어 있어 통합 경영 분석이 불가능한 상태입니다.

| 데이터 종류 | 현재 관리 방식 | 문제점 |
|------------|----------------|--------|
| 처방·조제 데이터 | DUR·청구 프로그램 (비\*소프트 등) | 청구 목적으로만 사용, 경영 분석 불가 |
| OTC 판매 데이터 | POS 단말기 또는 수기 기록 | 처방 데이터와 연계 불가 |
| 도매 발주 데이터 | 도매상 전용 앱 또는 전화 주문 | 매출 대비 지출 비율 파악 불가 |

이로 인해 약국 경영자는 아래의 핵심 경영 질문에 답할 수 없습니다:

- **"우리 매출의 몇 %가 특정 소아과 처방전에 의존하고 있는가?"** → 특정 병원과의 관계가 끊어질 경우 리스크를 사전에 인지할 수 없음
- **"ETC 대비 OTC 비중이 어떻게 변화하고 있는가?"** → 약국 포지셔닝 전략 수립 불가
- **"어느 도매상에서 얼마나 지출하고 있으며, 순이익률은 어떻게 되는가?"** → 감에 의존한 재고 발주 반복

> 💡 **시장 기회:** 약국 디지털 전환(DX) 수요는 높지만, 기존 솔루션은 '청구 자동화'에만 집중되어 있어 **경영 인텔리전스** 영역은 완전히 공백 상태입니다.

## 💡 기존 솔루션과의 차별성 (Differentiation)

### 시장 검증 (Market Validation)

**대상 시장 규모:**
- **TAM (전체 시장):** 국내 약국 25,000개 × 월 평균 SaaS 구독 비용 기준 → 수천억 규모 경영 관리 도구 시장
- **SAM (서비스 가능 시장):** 디지털 전환 의향이 있는 개인 약국 ~8,000개 (전체의 약 30%, 보건복지부 디지털헬스케어 조사 참조)
- **SOM (초기 목표 시장):** 수도권 소규모 약국 ~500개 — AI 인사이트 기능에 관심 있는 얼리어답터 약사 층

**사용자 페르소나 기반 검증:**

| 페르소나 | 상황 | PharmSight가 해결하는 것 |
|----------|------|--------------------------|
| **박약사 (38세, 소아과 인근 약국 운영)** | 특정 소아과 처방이 매출의 60% 이상인데 파악을 못 하고 있음 | 병원별 처방 의존도 차트로 리스크 즉시 시각화 |
| **김약사 (52세, 내과·이비인후과 복합상권)** | 월말 엑셀 정리에 3~4시간 소요, 재고는 여전히 감으로 관리 | 월별 KPI 대시보드 + AI 요약으로 5분 이내 경영 파악 |
| **이약사 (45세, 개국 2년차, 도매 3개사 거래)** | 어느 도매상에서 얼마 썼는지, 급여·비급여 비율을 모름 | 도매상별 지출 차트 + 급여/비급여 비율 자동 집계 |

**시장 검증 근거:**
- 약국 경영관리 전문 소프트웨어 시장은 **청구 자동화에만 집중**되어 있으며, 경영 인텔리전스(BI) 기능을 제공하는 솔루션은 국내에 전무에 가까운 상황 (블루오션)
- 디지털헬스케어 분야 스타트업 투자 증가 추세: 2023년 보건복지부 '약국 디지털 전환 지원 사업' 시범 운영 시작
- 실 약사 대상 비공식 인터뷰(3명): "엑셀로 관리하다 포기했다", "청구 프로그램은 건보 청구만 됨", "AI가 요약해준다면 쓰겠다"는 피드백 수렴

---

### 경쟁 제품 분석

| 구분 | 기존 청구 프로그램 (비\*소프트, 유\*케어 등) | 엑셀 수기 관리 | **PharmSight AI** |
|------|----------------------------------------------|----------------|-------------------|
| 데이터 통합 | 청구 데이터만 | 개별 파일 분산 | **조제·매출·발주 단일 화면** |
| 시각화 | 표 형태 출력 | 수동 차트 작성 | **ECharts 6종 인터랙티브 차트** |
| AI 분석 | 없음 | 없음 | **Gemini AI 자동 경영 인사이트** |
| 실시간성 | 월말 결산 중심 | 수동 갱신 | **Supabase 실시간 집계** |
| 접근성 | 설치형 PC 전용 | PC 전용 | **브라우저 기반, 반응형** |
| 가격 | 월 15~30만원 (설치비 별도) | 무료 (시간 비용 ↑) | **SaaS 구독 모델 (저비용)** |

**기존 솔루션의 근본적 한계:**
- 청구 프로그램: 건강보험 청구 자동화가 목적 → 경영 분석 기능 없음, 설치형으로 접근성 낮음
- 엑셀 수기: 데이터 통합 자체가 불가능, 차트 작성에 수시간 소요, 실시간성 전무
- **PharmSight AI는 이 공백을 '브라우저 기반 통합 대시보드 + AI 자동 분석'으로 직접 해소**

### PharmSight의 핵심 차별점

1. **통합 시각화:** 파편화된 조제/매출/지출 데이터를 단일 대시보드로 통합.
2. **직관적인 UX:** 경영자가 한눈에 파악할 수 있는 ECharts 기반의 반응형 모던 UI 제공.
3. **인사이트 중심:** '병원별 의존도', '연령대별 타겟' 등 즉각적인 경영 액션이 가능한 지표 도출.
4. **AI 경영 분석:** Google Gemini AI가 실시간 경영 데이터를 분석하여 요약·하이라이트·주의사항·추천을 자동 생성.
5. **블루오션 포지셔닝:** 국내 약국 시장의 경영 BI 공백을 최초로 겨냥한 저비용 SaaS 솔루션.

## 🌐 배포 URL (Deployment)
- **프론트엔드:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app) (Vercel)
- **백엔드 API:** [https://pharm-sight.onrender.com](https://pharm-sight.onrender.com) (Render)
- **데이터베이스:** Supabase PostgreSQL (실데이터 연동)

## 주요 기능

### AI 경영 분석
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

### CSV 데이터 내보내기
- 대시보드 헤더의 "데이터 내보내기" 버튼 클릭 시 전체 경영 데이터를 CSV로 즉시 다운로드
- 월별 매출 / 의약품 유형별 / 연령대별 / 병원별 / 도매상별 6개 섹션 포함
- BOM(Byte Order Mark) 포함 UTF-8 인코딩으로 Excel 한글 깨짐 방지
- 파일명 자동 생성: `pharm-sight-YYYYMM.csv`

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

### Dapper 선택 근거 — EF Core 대비 실질적 이점

본 프로젝트의 대시보드 쿼리는 다음과 같은 특성을 가집니다:

| 특성 | EF Core 사용 시 문제 | Dapper 해결책 |
|------|---------------------|---------------|
| CTE 3중 집계 (KPI 쿼리) | 복잡한 집계를 LINQ로 표현 시 비최적 SQL 생성 가능 | 개발자가 최적 SQL 직접 제어 |
| DATE_TRUNC / AGE() 함수 | EF Core의 PostgreSQL 함수 지원 불완전 | 순수 SQL → 모든 PG 함수 자유롭게 사용 |
| 읽기 전용 집계 조회 | Change Tracking 오버헤드 발생 | ADO.NET 직접 사용, 오버헤드 없음 |
| DTO 직접 매핑 | 도메인 객체 → DTO 변환 추가 코드 필요 | `QueryAsync<T>` 한 줄로 DTO 직접 매핑 |

**KPI 쿼리 핵심 코드** (`backend/Repositories/DashboardRepository.cs`):
```sql
WITH current_month AS (
    SELECT COALESCE(SUM(s."Amount"), 0) AS sales,
           COUNT(DISTINCT pr."Id")       AS prescriptions,
           COUNT(DISTINCT pr."PatientId") AS patients
    FROM "Sales" s
    LEFT JOIN "Prescriptions" pr ON s."PrescriptionId" = pr."Id"
    WHERE DATE_TRUNC('month', s."SaleDate"::date) = DATE_TRUNC('month', CURRENT_DATE)
),
prev_month AS ( ... ),  -- 전월 매출/조제건수
current_orders AS ( ... ) -- 이번 달 발주 지출
SELECT c.sales, c.prescriptions, c.patients, co.amount,
       CASE WHEN p.sales = 0 THEN 0
            ELSE ROUND(((c.sales - p.sales) / p.sales * 100)::numeric, 1)
       END AS "SalesChangeRate"
FROM current_month c, prev_month p, current_orders co;
-- → CTE 3개, 단 1회 DB 왕복으로 KPI 전체 계산
```

> 전체 코드 및 아키텍처 상세: [`docs/architecture.md`](docs/architecture.md)

### 🧩 아키텍처 및 코드 품질 원칙 (Architecture & Code Quality)

- **관심사 분리 (SoC):** 백엔드는 Controller(요청/응답) - Service(비즈니스 로직) - Repository(데이터 접근)로 계층을 엄격히 분리하여 단일 책임 원칙(SRP)을 준수합니다.
- **의존성 주입 (DI):** 모든 Service와 Repository는 인터페이스(`IDashboardService`, `IDashboardRepository`)를 통해 의존성을 주입받아 결합도를 낮추고 xUnit Moq 테스트 용이성을 극대화합니다.
- **반응형 디자인 (Responsive UX):** Tailwind CSS Breakpoint(`sm:`, `lg:`)로 모바일부터 데스크탑까지 자연스럽게 동작합니다.

**전역 예외 처리 미들웨어** (`backend/Middleware/GlobalExceptionMiddleware.cs`):

```csharp
public class GlobalExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try   { await _next(context); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "처리되지 않은 예외 발생: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException     => (int)HttpStatusCode.BadRequest,        // 400
            InvalidOperationException => (int)HttpStatusCode.BadRequest,        // 400
            _                         => (int)HttpStatusCode.InternalServerError // 500
        };
        var response = new { error = exception.Message, statusCode = context.Response.StatusCode };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
```

오류 응답 형식 (모든 예외에 일관 적용):
```json
{ "error": "처리 중 오류가 발생했습니다.", "statusCode": 500 }
```

**DI 등록 구조** (`backend/Program.cs`):
```csharp
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService,   DashboardService>();
builder.Services.AddScoped<IAiInsightService,   AiInsightService>();
builder.Services.AddMemoryCache();          // AI 인사이트 30분 캐시
builder.Services.AddHttpClient("Gemini",    // Named HttpClient (타임아웃 30초)
    c => c.Timeout = TimeSpan.FromSeconds(30));

app.UseMiddleware<GlobalExceptionMiddleware>(); // 파이프라인 최선두 등록
app.UseCors("ViteFrontend");
app.MapControllers();
```

> 계층별 전체 코드 및 Dapper SQL 쿼리 상세: [`docs/architecture.md`](docs/architecture.md)

---

## UX 상세 설명

### 실제 배포 URL에서 직접 확인 가능

> **라이브 데모:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app)

### 화면 상태별 UX 처리

| 상태 | 표시 내용 | 구현 위치 |
|------|----------|-----------|
| **초기 로딩 중** | 헤더 "⟳ 로딩 중..." 배지 표시, 차트 영역 빈 상태 | `DashboardView.vue:49-51` |
| **API 정상 연결** | 헤더 "● 실시간 연동" 초록 배지 + Supabase 실데이터 | `DashboardView.vue:52-54` |
| **API 오류 발생** | 헤더 "⚠ API 오류 · 임시 데이터" 빨간 배지 + Mock 폴백 | `DashboardView.vue:46-48` |
| **AI 분석 로딩** | AiInsightPanel 스켈레톤 애니메이션 표시 | `AiInsightPanel.vue` |
| **AI API 키 미설정** | "AI 분석 기능을 사용하려면 API 키 설정이 필요합니다" 안내 | `AiInsightService.cs` |

### KPI 카드 — 전월 대비 변화율 표시

```
💰 이번 달 총 매출      💊 이번 달 조제 건수
  1,680 만원              420 건
  ▲ 9.1% 전월 대비        ▲ 9.1% 전월 대비

🏥 이번 달 방문 환자    📦 이번 달 발주 지출
  287 명                  960 만원
  ▲ 4.7% 전월 대비        ▼ 2.3% 전월 대비
```
- 상승(▲): 초록색(`text-emerald-400`)
- 하락(▼): 빨간색(`text-rose-400`)

### CSV 데이터 내보내기 UX

헤더 우측 "⬇ 데이터 내보내기" 버튼:
- 모바일: "CSV" 짧은 텍스트
- PC: "데이터 내보내기" 전체 텍스트
- 로딩 중 비활성화(disabled) 처리

---

## 반응형 디자인 (Responsive Design)

### 레이아웃 Breakpoint 요약

| 화면 | KPI 카드 | 차트 행 1 | 차트 행 2 | 차트 행 3 |
|------|----------|-----------|-----------|-----------|
| 모바일 (< 1024px) | 2열 | 1열 (세로 스택) | 1열 | 1열 |
| 데스크탑 (≥ 1024px) | 4열 | 3열 (2:1) | 2열 | 3열 (2:1) |

```html
<!-- KPI: 모바일 2열 / PC 4열 -->
<section class="grid grid-cols-2 lg:grid-cols-4 gap-4">

<!-- 매출 차트: 모바일 전체 너비 / PC 3열 중 2열 -->
<section class="grid grid-cols-1 lg:grid-cols-3 gap-4">
  <div class="lg:col-span-2">  <!-- 매출 라인차트 -->

<!-- 연령대·병원: 모바일 1열 / PC 2열 -->
<section class="grid grid-cols-1 lg:grid-cols-2 gap-4">
```

### 모바일 최적화 세부 사항

- 헤더 날짜 배지 모바일 숨김 (`hidden sm:inline`) — 헤더 줄 바꿈 방지
- 버튼 터치 타겟 `py-1.5` — 44px 최소 터치 영역 확보
- `sticky top-0 backdrop-blur` 헤더 — 스크롤 시 차트 제목 가독성 유지
- ECharts 차트 `h-64` 고정 높이 — 모바일에서 차트 찌그러짐 방지

> 디바이스별 상세 테스트 결과: [`docs/responsive-testing.md`](docs/responsive-testing.md)
> (iPhone SE · iPhone 14 Pro · iPad Mini · iPad Air · 1280px · 1920px 검증 완료)

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
