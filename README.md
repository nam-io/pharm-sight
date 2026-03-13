# PharmSight AI — 약국 경영 통합 AI 대시보드

약국의 처방·매출·지출 데이터를 한 화면에서 시각화하고, AI가 경영 인사이트를 자동 분석하여 제공하는 통합 대시보드입니다.

[![CI - 빌드 및 테스트](https://github.com/nam-io/pharm-sight/actions/workflows/ci.yml/badge.svg)](https://github.com/nam-io/pharm-sight/actions/workflows/ci.yml)

![PharmSight AI 대시보드 화면](docs/pharm-sight-intro.png)

---

## 문제 정의 — 개인 약국의 생존을 위협하는 데이터 사각지대

### 매년 1,000개 이상의 약국이 문을 닫고 있습니다

대형 드러그스토어와 체인 약국은 POS 데이터 분석으로 재고·마케팅을 최적화합니다. 반면 **국내 25,000개 약국 중 85%를 차지하는 개인 약국은 여전히 감(感)에 의존해 경영**하고 있습니다. 데이터 분석 능력의 격차가 곧 생존 경쟁력의 격차로 이어지는 시대입니다.

**근본 원인: 3개 시스템에 파편화된 경영 데이터**

| 데이터 | 현재 시스템 | 한계 |
|--------|-------------|------|
| 처방·조제 | 건보 청구 프로그램 | 청구 목적만 — 경영 분석 기능 전무 |
| OTC 판매 | POS 단말기 / 수기 | 처방 데이터와 연계 불가 |
| 도매 발주 | 도매상 전용 앱 | 매출 대비 지출 비율 파악 불가 |

이 세 데이터는 서로 연결되지 않습니다. 약사는 **통합 경영 현황을 한 번도 본 적이 없는 상태로** 매일 의사결정을 내리고 있습니다.

**이 파편화가 만드는 실질적 경영 위협:**

1. **병원 폐원 시 매출 급감 무방비**: 매출의 60%가 인근 소아과 1곳에서 오는데 이를 파악하지 못함 — 인근 의원 폐원 시 매출 40~60% 급감 사례 빈번
2. **마진율을 모른 채 발주 반복**: 도매 지출 총액과 매출을 연결해 본 적 없음 — 체인 약국 대비 수익성 열위 심화
3. **데이터 관리 복잡성 폭증**: 코로나 이후 비대면 처방·배달약국 확대로 관리할 채널이 늘었지만, 기존 수작업 방식으로는 한계 도달

**실제 약사 인터뷰 (3인, 2026년 3월 · 서울/경기/인천):**

> *"청구 프로그램은 건보 청구만 돼요. 매출 분석은 따로 엑셀로 하는데, 3~4시간 걸리다가 포기했어요."* — 서울 A약국, 경력 8년차

> *"어느 소아과에서 처방이 많이 오는지 알고 싶은데, 지금 방법으로는 알 수가 없어요."* — 경기 B약국, 개국 5년차

> *"AI가 우리 약국 데이터를 분석해서 요약해 준다면 바로 쓰겠어요."* — 인천 C약국, 경력 12년차

**지금 이 문제가 더 시급한 이유:**
- 약국 폐업률 증가 추세 — 데이터 분석 능력이 **생존의 필수 조건**으로 전환
- 대형 체인·드러그스토어 확장 — 개인 약국과의 데이터 분석 격차 확대
- 보건복지부 2023년 '약국 디지털 전환 지원사업' 시범 시작 — 정책적 수요 확인
- ChatGPT 대중화로 AI 도구 수용성 급격 상승 — 약사들의 즉각 도입 의향 확인

> 시장 규모(TAM/SAM/SOM), 경쟁사 심층 분석, 포지셔닝 맵 상세: [`docs/market-analysis.md`](docs/market-analysis.md)

---

## 사용자가 얻는 핵심 가치 — 경쟁 제품과 무엇이 다른가

### Before vs After: 약사가 체감하는 변화

| 약사 | 지금 (Before) | PharmSight 도입 후 (After) |
|------|--------------|---------------------------|
| 박약사 (38세, 소아과 인근) | 매출의 60%가 한 소아과에서 오는데 모름 — 폐원하면 대비 불가 | **병원별 매출 의존도**를 한눈에 확인, 리스크 사전 대비 |
| 김약사 (52세, 복합상권) | 월말 엑셀 정리 3~4시간, 결국 포기 — 경영 현황 파악 불가 | 접속만 하면 **이번 달 경영 현황을 5분 안에** 파악 |
| 이약사 (45세, 개국 2년차) | 도매 3곳 지출 합계를 모름 — 수익성 관리 불가 | **도매상별 지출 차트**로 비용 구조 즉시 파악 |

### 3-Way 경쟁 비교

| 구분 | 청구 프로그램 | 엑셀 수기 관리 | **PharmSight AI** |
|------|-------------|--------------|-------------------|
| 처방+매출+발주 통합 | X (청구만) | 수동 (월 3~4시간) | **자동 통합** |
| 경영 현황 파악 시간 | 불가 | 3~4시간/월 | **5분 이내** |
| AI 경영 요약 | 없음 | 없음 | **매월 자동 요약 + 추천 액션** |
| 병원 의존도 파악 | 불가 | 직접 계산 필요 | **자동 산출** |
| 모바일 접속 | 불가 (PC 설치형) | 불편 | **브라우저 즉시 접속** |
| 비용 | 월 15~30만원 | 무료 (시간 비용 큼) | **저비용 구독** |

### PharmSight만의 5가지 차별점

1. **흩어진 3개 시스템 데이터를 한 화면에서 확인** — 엑셀 복사-붙여넣기가 더 이상 필요 없음
2. **교육 없이 5분 내 파악 가능한 직관적 화면** — PC, 태블릿, 스마트폰 어디서든 접속
3. **어느 병원에 매출이 의존되는지, 어떤 연령대 고객이 많은지 숫자로 즉시 확인** — 즉각적 경영 액션이 가능한 약국 전용 지표
4. **AI가 매달 경영 상태를 요약하고, 주의사항과 추천 액션을 자동으로 알려줌** — 데이터를 읽을 줄 몰라도 경영 인사이트를 얻을 수 있음
5. **약국 전용 경영 분석 도구는 국내에 아직 없음** — PharmSight AI가 이 시장 공백을 최초로 겨냥

> 국내 약국 25,000개 중 디지털 전환 의향 ~8,000개 — 약국 전용 경영 BI는 국내 미개척 시장 ([상세 시장 분석](docs/market-analysis.md))

---

## 배포 URL

- **프론트엔드:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app) (Vercel)
- **백엔드 API:** [https://pharm-sight.onrender.com](https://pharm-sight.onrender.com) (Render)
- **데이터베이스:** Supabase PostgreSQL (실데이터 연동)

---

## 주요 기능 (구현 완료)

> **라이브 데모:** [https://pharm-sight-frontend.vercel.app](https://pharm-sight-frontend.vercel.app)

| 기능 | 설명 |
|------|------|
| AI 경영 분석 | AI가 이번 달 경영 현황을 2~3문장으로 요약, 주의사항·추천 액션 자동 생성 (30분 캐시) |
| KPI 카드 4종 | 이번 달 매출·조제건수·환자수·발주금액, 전월 대비 변화율 표시 |
| 처방 트렌드 | 월별 매출 및 조제 건수 추이 (기간 필터: 3/6/12개월) |
| 의약품 매출 비중 | 전문의약품(ETC) vs 일반의약품(OTC) 비율 시각화 |
| 고객 연령대 분포 | 방문 환자 연령대별 도넛 차트 |
| 처방 기관 분석 | 처방전 발행 의료기관별 유입 건수 비교 |
| 도매상 지출 현황 | 도매상별 누적 지출, 급여/비급여 비율 |
| CSV 내보내기 | 전체 경영 데이터를 CSV로 즉시 다운로드 (Excel 한글 호환) |
| 반응형 레이아웃 | 모바일~PC 자동 적응 ([테스트 결과](docs/responsive-testing.md)) |

---

## 기술 스택

| 레이어 | 채택 기술 | 선택 근거 |
|--------|---------|---------|
| **Frontend** | Vue 3 + TypeScript + Vite | Composable로 로직 재사용, Vite 즉각 HMR |
| **차트** | Apache ECharts | 복합 차트(바+라인 오버레이) 네이티브 지원, 6종 차트 통일 API |
| **Backend** | C# .NET 9.0 Web API | 강타입 DTO 컴파일 검증, async/await 병렬 쿼리 |
| **DB** | PostgreSQL (Supabase) | DATE_TRUNC·AGE() 집계 함수, 클라우드 영속성 |
| **ORM** | Dapper (순수 SQL) | CTE 3중 집계 1회 왕복, Change Tracking 오버헤드 없음 |
| **AI** | Google Gemini API | 무료 할당량 1M 토큰/월, ListModels 동적 모델 탐색 |
| **Infra** | Vercel + Render + Supabase | 3가지 모두 무료 티어, 해커톤 비용 0원 배포 |

> 패키지별 버전 선택 근거 및 의존성 관리: [`docs/tech-stack.md`](docs/tech-stack.md)

---

## 아키텍처

```
HTTP 요청 → GlobalExceptionMiddleware → DashboardController
                                              │ IDashboardService (인터페이스 DI)
                                              ▼
                                        DashboardService
                                              │ IDashboardRepository (인터페이스 DI)
                                              ▼
                                        DashboardRepository (Dapper + Npgsql)
                                              │
                                              ▼
                                        Supabase PostgreSQL
```

- **Controller → Service → Repository** 계층 분리 (SRP/SoC)
- 인터페이스 기반 의존성 주입 → 테스트 시 Repository Mock 가능
- 전역 예외 처리 미들웨어로 일관된 JSON 오류 응답

> 계층별 코드, Dapper 쿼리 상세, DB 스키마, 프로젝트 구조: [`docs/architecture.md`](docs/architecture.md)

---

## 테스트 전략 및 CI/CD

### 테스트 피라미드: 총 32개 테스트 (백엔드 21 + 프론트엔드 11)

**백엔드 (xUnit 2.9.2 + Moq 4.20.72 + Coverlet 커버리지):**

| 테스트 클래스 | 계층 | 케이스 수 | 검증 내용 |
|---------------|------|-----------|----------|
| `DashboardServiceTests` | Service | 9개 | 7개 메서드 반환값, 빈 결과 엣지, 0 나눗셈 방어 |
| `AiInsightServiceTests` | Service | 4개 | API키 미설정 Graceful Degradation, 캐시 히트 |
| `DashboardRepositoryTests` | Repository | 8개 | 연결문자열 URI 변환 6종, 미설정 예외, 인터페이스 구현 검증 |

**프론트엔드 (Vitest 4.1 + Vue Test Utils + happy-dom):**

| 테스트 파일 | 케이스 수 | 검증 내용 |
|-------------|-----------|----------|
| `useDashboardData.test.ts` | 7개 | Mock 초기 데이터, KPI 카드 생성, 월별 정렬, ETC/OTC 구분 |
| `useAiInsight.test.ts` | 4개 | 초기 상태, API 미설정 시 호출 생략, 반환 속성 검증 |

### CI/CD 파이프라인: 3-Job 자동화

```
push/PR → [backend-test]  .NET 빌드 → xUnit 21개 → Coverlet 커버리지 → TRX 업로드
        → [frontend-test] npm ci → Vitest 11개 → Vite 프로덕션 빌드
        → [e2e-smoke]     API 헬스체크 7개 엔드포인트 + 프론트엔드 응답 확인
```

| Job | 도구 | 실행 내용 |
|-----|------|----------|
| `backend-test` | .NET 9.0 + xUnit + Coverlet | 21개 테스트 + Cobertura XML 커버리지 리포트 생성 |
| `frontend-test` | Node 20 + Vitest | 11개 단위 테스트 + Vite 프로덕션 빌드 검증 |
| `e2e-smoke` | curl | 배포된 서비스 7개 API 엔드포인트 + 프론트엔드 HTTP 상태 확인 |

### 배포 자동화

| 서비스 | 배포 방식 | 트리거 |
|--------|----------|--------|
| Vercel (프론트엔드) | Git 연동 자동 배포 | `master` push 시 자동 빌드·배포 |
| Render (백엔드) | Git 연동 자동 배포 | `master` push 시 자동 빌드·배포 |
| Supabase (DB) | 클라우드 매니지드 | 상시 가동, 별도 배포 불필요 |

> 테스트 코드: [`backend/PharmSight.Tests/`](backend/PharmSight.Tests/) · 프론트엔드 테스트: [`frontend/src/composables/*.test.ts`](frontend/src/composables/) · CI: [`.github/workflows/ci.yml`](.github/workflows/ci.yml)

---

## 개발 이력 (55개 커밋, 5 Phase)

| Phase | 커밋 수 | 핵심 성과 |
|-------|--------|----------|
| Phase 0 — 기반 구축 | 6개 | .NET 9 API, Vue 3, PostgreSQL 스키마 초기화 |
| Phase 1 — 프론트엔드 UI | 7개 | 6종 ECharts 차트, Mock 데이터 Composable, Vercel 배포 |
| Phase 2 — 백엔드 API | 13개 | Dapper 7개 집계 쿼리, Render+Supabase 배포, 연결 버그 3건 수정 |
| Phase 3 — AI 기능 | 13개 | Gemini API 통합, 10단계 디버깅 → ListModels 동적 탐색으로 근본 해결 |
| Phase 4 — 테스트/CI | 16개 | xUnit 13개, GitHub Actions CI, 문서화 강화 |

> 전체 커밋-ROADMAP 매핑: [`docs/CHANGELOG.md`](docs/CHANGELOG.md) · ADR 8개: [`docs/decision-log.md`](docs/decision-log.md) · 스프린트 문서: [`docs/sprint/`](docs/sprint/)

---

## 시작하기

```bash
# 백엔드
cd backend && dotnet restore && dotnet run  # → http://localhost:5000

# 프론트엔드
cd frontend && npm install && npm run dev   # → http://localhost:5173
```

> 환경 변수 설정, 사전 요구사항 등 상세: [`docs/setup-guide.md`](docs/setup-guide.md)
