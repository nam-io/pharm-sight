# PRD (Product Requirements Document) — PharmSight AI

> 약국 경영 통합 AI 대시보드의 제품 요구사항 정의서

---

## 1. 제품 개요

| 항목 | 내용 |
|------|------|
| 제품명 | PharmSight AI |
| 한줄 소개 | 약국의 처방·매출·지출 데이터를 한 화면에서 시각화하고, AI가 경영 인사이트를 자동 분석하여 제공하는 통합 대시보드 |
| 목표 사용자 | 국내 개인 약국 약사 (25,000개 중 85%인 개인 약국) |
| 핵심 가치 | 3개 시스템에 파편화된 경영 데이터를 통합하여, 교육 없이 5분 내 경영 현황 파악 가능 |
| 배포 URL | 프론트엔드: https://pharm-sight-frontend.vercel.app / 백엔드: https://pharm-sight.onrender.com |

---

## 2. 문제 정의

### 2-1. 핵심 문제

개인 약국 약사는 처방(청구 프로그램), OTC 매출(POS), 도매 발주(도매앱) 3개 시스템에 데이터가 파편화되어 있어, **통합 경영 현황을 한 번도 본 적 없는 상태로** 매일 의사결정을 내리고 있다.

### 2-2. 문제로 인한 경영 위협

| 위협 | 구체적 상황 | 영향 |
|------|-----------|------|
| 병원 폐원 무방비 | 매출 60%가 인근 소아과 1곳에서 오는데 파악 불가 | 폐원 시 매출 40~60% 급감 |
| 마진율 미파악 | 도매 지출 총액과 매출 연결 분석 부재 | 체인 약국 대비 수익성 열위 심화 |
| 관리 복잡성 폭증 | 코로나 이후 비대면 처방·배달약국 확대 | 수작업 한계 도달 |

### 2-3. 사용자 검증

약사 6인 인터뷰 (2026년 3월, 서울/경기/인천/대전/부산/광주) 결과:
- 6인 전원: 현재 통합 경영 분석 불가능 상태
- 6인 전원: 감(感)에 의존한 재고 발주 중
- 5인: AI 요약 기능에 대한 즉각 도입 의향 확인
- 4인: 병원 의존도 파악 기능에 강한 관심 표명
- 3인: 모바일 접속 가능 여부가 도입 결정의 핵심 요소라고 응답

> 상세 인터뷰 내용: [docs/market-analysis.md](market-analysis.md)

---

## 3. 목표 및 성공 지표

### 3-1. 제품 목표

| 목표 | 측정 기준 |
|------|----------|
| 경영 현황 파악 시간 단축 | 월 3~4시간(엑셀) → 5분 이내 (95% 단축) |
| 데이터 통합 | 3개 시스템 데이터를 1개 대시보드에서 조회 |
| AI 인사이트 제공 | 매월 경영 요약 + 주의사항 + 추천 액션 자동 생성 |
| 접근성 확보 | PC·태블릿·스마트폰 반응형, 브라우저 즉시 접속 |

### 3-2. 해커톤 범위 성공 지표

| 지표 | 목표 | 달성 |
|------|------|------|
| 핵심 기능 구현 | KPI 4종 + 차트 6종 + AI 분석 | 완료 |
| 실데이터 연동 | Supabase PostgreSQL 라이브 연동 | 완료 |
| 배포 | 프론트엔드 + 백엔드 + DB 3-tier 배포 | 완료 |
| 테스트 | 단위 테스트 + CI/CD 파이프라인 | 51개 테스트, 4-Job CI |

---

## 4. 기능 요구사항

### 4-1. 핵심 기능 (P0 — 필수)

| ID | 기능 | 설명 | 구현 상태 |
|----|------|------|----------|
| F-01 | KPI 카드 4종 | 이번 달 총 매출, 조제 건수, 방문 환자, 발주 지출 + 전월 대비 변화율 | 완료 |
| F-02 | 월별 매출 추이 | 바+라인 복합 차트, 기간 필터(3/6/12개월) | 완료 |
| F-03 | 의약품 유형별 매출 | ETC vs OTC 도넛 차트 | 완료 |
| F-04 | 환자 연령대 분포 | 8개 연령대별 도넛 차트 | 완료 |
| F-05 | 처방 기관 분석 | 의료기관별 처방전 유입 건수 수평 바 차트 | 완료 |
| F-06 | 도매상 지출 현황 | 도매상별 누적 지출 바 차트 | 완료 |
| F-07 | 급여/비급여 비율 | 건강보험 적용 여부별 지출 도넛 차트 | 완료 |
| F-08 | AI 경영 분석 | Gemini API로 경영 요약 + 하이라이트 + 경고 + 추천 액션 자동 생성 | 완료 |

### 4-2. 부가 기능 (P1 — 중요)

| ID | 기능 | 설명 | 구현 상태 |
|----|------|------|----------|
| F-09 | CSV 내보내기 | 6개 섹션 전체 CSV 다운로드, BOM UTF-8 (Excel 한글 호환) | 완료 |
| F-10 | 반응형 레이아웃 | 모바일~PC Tailwind breakpoint 자동 적응 | 완료 |
| F-11 | Keep-Alive | Render 무료 플랜 15분 슬립 방지 10분 주기 핑 | 완료 |
| F-12 | 기간 필터 | 월별 매출 차트 3/6/12개월 클라이언트 슬라이싱 | 완료 |

### 4-3. 향후 확장 기능 (P2 — 백로그)

| ID | 기능 | 설명 |
|----|------|------|
| F-13 | 다중 약국 지원 | 약국별 데이터 분리, 사용자 인증 |
| F-14 | 알림 기능 | 매출 급감, 재고 부족 시 이메일/SMS 알림 |
| F-15 | 데이터 임포트 | 청구 프로그램 CSV 파일 자동 파싱·연동 |
| F-16 | 월간 리포트 PDF | AI 분석 결과를 PDF로 자동 생성·이메일 발송 |

---

## 5. 비기능 요구사항

### 5-1. 성능

| 항목 | 요구사항 | 구현 |
|------|---------|------|
| API 응답 시간 | 7개 API 병렬 호출 3초 이내 | Promise.all + AbortController 10초 타임아웃 |
| AI 응답 시간 | 15초 이내 | AbortController 15초 타임아웃 + 30분 IMemoryCache |
| 콜드스타트 | Render 슬립 후 첫 요청 30초 이내 | useKeepAlive 10분 핑으로 슬립 방지 |

### 5-2. 안정성

| 항목 | 요구사항 | 구현 |
|------|---------|------|
| API 실패 대응 | 서비스 중단 없이 Mock 데이터 폴백 | Graceful Degradation 패턴 |
| 에러 분류 | NETWORK/API/PARSE 유형별 차별 대응 | classifyError() + 유형별 UI 메시지 |
| 자동 재시도 | NETWORK 에러 1회 자동 재시도 | config.ts MAX_NETWORK_RETRIES=1, RETRY_DELAY_MS=500 |
| AI 독립성 | AI 실패 시 나머지 차트 영향 없음 | AiInsightPanel 독립 에러 처리 + 재시도 버튼 |

### 5-3. 보안

| 항목 | 구현 |
|------|------|
| CORS | Vercel 프론트엔드 도메인만 허용 (`ViteFrontend` 정책) |
| API 키 | 환경변수로 주입 (소스코드 미포함) |
| DB 연결 | Supabase SSL 필수 (SslMode.Require) |
| 전역 예외 처리 | 내부 오류 메시지 노출 방지 (GlobalExceptionMiddleware) |

### 5-4. 접근성

| 항목 | 구현 |
|------|------|
| ARIA 속성 | role="alert", aria-live="polite", aria-label, aria-pressed |
| 키보드 접근 | 기간 필터 버튼 그룹 role="group" |
| 스크린리더 | 로딩 상태 aria-busy, 에러 알림 role="alert" |

---

## 6. 기술 아키텍처 요약

```
[Vue 3 + TypeScript + Vite]  →  [.NET 9.0 Web API]  →  [PostgreSQL (Supabase)]
       Vercel CDN                    Render                    클라우드 DB
                                       ↓
                                 [Google Gemini API]
                                   AI 경영 분석
```

| 계층 | 기술 | 역할 |
|------|------|------|
| Frontend | Vue 3 Composition API + ECharts | 6종 차트 + KPI + AI 패널 |
| Backend | C# .NET 9.0 + Dapper | Controller→Service→Repository 3계층 |
| DB | PostgreSQL + Supabase | CTE/DATE_TRUNC/AGE() 집계 |
| AI | Google Gemini API | 경영 데이터 분석 → 요약/경고/추천 |
| Infra | Vercel + Render + Supabase | 무료 티어 3-tier 배포 |

> 상세 아키텍처: [docs/architecture.md](architecture.md)
> 기술 스택 선택 근거: [docs/tech-stack.md](tech-stack.md)

---

## 7. 데이터 모델

```
Patients (Id, DateOfBirth)
    ↑
Prescriptions (Id, PatientId, HospitalId, DispenseDate)
    ↑                              ↑
Sales (Id, Amount, SaleDate,    Hospitals (Id, Name)
       PrescriptionId[nullable])
                                Drugs (Id, Name, Type[ETC/OTC], IsCovered)
                                    ↑
                                Orders (Id, WholesaleName, DrugId, Amount, OrderDate)
```

**핵심 관계:**
- Sales.PrescriptionId → Prescriptions.Id (조제약 매출은 처방전 연결, OTC 매출은 null)
- 7개 API 엔드포인트가 각각 다른 JOIN/GROUP BY/CTE 조합으로 집계

---

## 8. 사용자 흐름

```
[접속] → 스켈레톤 로딩 (KPI 4개 + 차트 6개 형태별)
  │
  ▼
[데이터 로드 성공] → 실시간 데이터 표시
  ├─ 기간 필터 (3/6/12개월)
  ├─ 차트 호버 (ECharts 툴팁)
  ├─ CSV 내보내기 (토스트 알림)
  └─ AI 경영 분석 확인
  │
[데이터 로드 실패] → Mock 폴백 + 에러 안내 + [다시 시도]
[AI 실패] → 독립 에러 안내 + [다시 분석 요청] (차트 영향 없음)
```

---

## 9. 제약 조건

| 제약 | 영향 | 대응 |
|------|------|------|
| 해커톤 1인 개발 (~6시간) | 기능 범위 제한 | P0 핵심 기능 집중, P2 백로그로 관리 |
| 무료 인프라 | Render 15분 슬립, Gemini 1M 토큰/월 | Keep-Alive 핑, AI 30분 캐시 |
| 실데이터 접근 불가 | 실제 약국 데이터 대신 시드 데이터 | 현실적 시드 데이터 설계 (database/seed.sql) |

---

## 10. 관련 문서

| 문서 | 설명 |
|------|------|
| [README.md](../README.md) | 프로젝트 소개 및 전체 개요 |
| [ROADMAP.md](../ROADMAP.md) | 개발 로드맵 및 Phase별 진행 상황 |
| [CLAUDE.md](../CLAUDE.md) | AI 협업 가이드 및 기술 컨텍스트 |
| [docs/architecture.md](architecture.md) | 백엔드/프론트엔드 아키텍처 상세 |
| [docs/tech-stack.md](tech-stack.md) | 기술 스택 선택 근거 및 의존성 관리 |
| [docs/market-analysis.md](market-analysis.md) | 시장 분석 및 경쟁 제품 비교 |
| [docs/CHANGELOG.md](CHANGELOG.md) | 전체 커밋 이력 및 Phase별 개발 기록 |
| [docs/decision-log.md](decision-log.md) | 아키텍처 의사결정 기록 (ADR) |
