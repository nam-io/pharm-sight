# CHANGELOG — PharmSight AI 개발 이력

> **형식:** [Conventional Commits](https://www.conventionalcommits.org/) 기반
> **추적:** 각 커밋은 `ROADMAP.md`의 Phase 항목에 연결됩니다.
> **기간:** 해커톤 집중 개발 (2026-03)

---

## 개발 타임라인 요약

```
Phase 0 → Phase 1 → Phase 2 → Phase 3 → Phase 4
 기반    프론트엔드    백엔드     AI 분석    테스트/CI
 (1h)     (2h)       (2h)     Gemini    + 문서화
                            디버깅(1h)
```

---

## Phase 0 — 프로젝트 기반 구축

> ROADMAP: Phase 0 · Sprint 0
> **목표:** 프로젝트 아키텍처 설계, AI 에이전트 협업 시스템 구축, 백엔드 스캐폴딩
> **ROADMAP 연결:** Phase 0 전체 항목 — 프로젝트 초기화 + 스프린트 프로세스 정의

| 커밋 | 해시 | ROADMAP 항목 | 기술 결정 |
|------|------|--------------|----------|
| docs: 프로젝트 기반 문서 및 설정 파일 추가 | `5c00605` | Phase 0: 프로젝트 초기화 | PRD/ROADMAP/CLAUDE.md 3대 문서 체계 확립 |
| ci: GitHub Actions 워크플로우 및 개발 프로세스 문서 추가 | `19656a3` | Phase 0: CI/CD 기반 | GitHub Actions YAML 초기 파이프라인 (→ Phase 4에서 6-Job으로 확장) |
| chore: Claude AI 에이전트 정의 및 메모리 시스템 추가 | `998fbc1` | Phase 0: AI 협업 시스템 | `.claude/agents/` 4종 에이전트 정의 (planner/close/hotfix/prd-to-roadmap) |
| docs: Sprint 0 스프린트 계획 문서 작성 | `06bd1d2` | Phase 0: 스프린트 계획 | Agile/스크럼 스프린트 프로세스 정의 — 이후 Sprint 1~4까지 동일 프로세스 적용 |
| feat: 백엔드 .NET 9.0 Web API 프로젝트 스캐폴딩 및 SQLite 스키마 초기화 | `008f5bd` | Phase 0: 백엔드 초기화 | Controller→Service→Repository 3계층 + Interface DI 아키텍처 확정 (ADR-001) |
| chore: .gitignore에 .NET 빌드 산출물 제외 규칙 추가 | `e1b035f` | Phase 0: 환경 설정 | .NET bin/obj + node_modules 제외 규칙 |

---

## Phase 1 — 프론트엔드 UI 개발

> ROADMAP: Phase 1 · Sprint 1
> **목표:** Vue 3 Composition API + ECharts 6종 차트 대시보드 UI 구축
> **ROADMAP 연결:** Phase 1 전체 항목 — 프론트엔드 초기화 + KPI 카드 + 차트 6종 + Vercel 배포
> **핵심 기술 결정:** Vue 3 선택 (React 대비 Composable 재사용성), ECharts 선택 (Chart.js 대비 복합 차트 지원), Pinia 미채택 (단일 뷰 + 읽기 전용 → YAGNI 원칙, 확장 시 도입 계획 수립)

| 커밋 | 해시 | ROADMAP 항목 | 기술 결정 |
|------|------|--------------|----------|
| feat: 프론트엔드 Vue 3 + Vite 6 + TypeScript 프로젝트 스캐폴딩 | `43be119` | Phase 1: 프로젝트 생성 | Vue 3 Composition API + Vite 6 HMR 즉시 반영 (ADR-002) |
| docs: README.md에 Vercel 배포 URL 등록 | `7e1c4c7` | Phase 1: 배포 URL 기록 | Vercel CDN 자동 배포 채택 |
| feat: 대시보드 TypeScript 타입 정의 및 Mock 데이터 Composable 구현 | `afc1543` | Phase 1: 타입/Composable | `types.ts` 타입 정의 + `useDashboardData` Composable — Pinia 대신 Composable 패턴 선택 |
| feat: 대시보드 메인 뷰 연결 및 빌드 환경 설정 | `4569a33` | Phase 1: UI 메인 뷰 | DashboardView + ECharts 6종 차트 컴포넌트 (SalesLine/DrugType/PatientAge/Hospital/Wholesale/Coverage) |
| docs: Sprint 1 스프린트 계획 문서 작성 | `f5fc702` | Phase 1: 문서화 | — |
| merge: sprint/sprint1 → develop | `62c01f8` | Phase 1: 브랜치 통합 | — |
| release: Phase 1 프론트엔드 대시보드 UI → master 배포 | `62443d8` | Phase 1: 배포 | Vercel 프로덕션 배포 최초 실행 |

---

## Phase 2 — 백엔드 API 구현 및 클라우드 배포

> ROADMAP: Phase 2 · Sprint 2

### 핵심 구현

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| chore: vercel.json 배포 설정 추가 | `b14485b` | Phase 2: 인프라 설정 |
| chore: Vercel 배포 재트리거 | `e5b84c7` | Phase 2: 배포 |
| feat: 백엔드 .NET Web API 전체 구현 (Controller/Service/Repository 계층) | `a7b5334` | Phase 2: **핵심** — 전체 백엔드 구현 |
| feat: 프론트엔드 백엔드 API 연동 및 에러 처리 구현 | `f7871bc` | Phase 2: 프론트-백 연동 |
| chore: render.yaml 추가 (backend/Dockerfile 경로 지정) | `cede021` | Phase 2: Render 배포 설정 |

### 배포 버그 수정 (기술적 도전과 해결)

> **배경:** Render + Supabase 클라우드 환경의 네트워크 제약으로 인한 연결 실패 문제들

| 커밋 | 해시 | 문제 | 해결 |
|------|------|------|------|
| fix: Supabase PostgreSQL URI 형식 연결 문자열 파싱 오류 수정 | `9bf841b` | Render 환경변수는 `postgresql://` URI로 주입되지만 Npgsql은 키=값 형식 필요 | `NormalizeConnectionString()` 자동 변환 메서드 구현 |
| fix: Render IPv6 아웃바운드 미지원으로 인한 Supabase 연결 실패 수정 | `acd8e42` | Render 무료 플랜 IPv6 불지원 → DNS 해석 실패 | Supabase 연결 풀러(IPv4) URL로 전환 |
| revert: IPv4 강제 DNS 해석 코드 제거 | `d486588` | IPv4 강제 코드가 불필요한 부작용 발생 | Supabase 풀러 URL이 근본 해결책이므로 코드 제거 |
| fix: PostgreSQL → Dapper C# 타입 매핑 불일치 수정 | `504164d` | PostgreSQL `bigint` ↔ C# `long` 매핑 오류 | Dapper 타입 핸들러 및 명시적 캐스팅 적용 |
| fix: Vercel 빌드 시 VITE_API_BASE_URL 환경변수 누락으로 API 미호출 문제 수정 | `c3cff11` | Vite 빌드타임 환경변수 미주입 → `USE_MOCK = true` | `.env.production` 파일 생성으로 해결 |

### Keep-Alive 구현

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| feat: Render 슬립 방지 헬스체크 Controller 및 Keep-Alive 자동 핑 구현 | `63c688d` | Phase 2: HealthController |
| fix: CORS AllowedOrigins에 Render 배포 URL 추가 | `786027a` | Phase 2: CORS 설정 |
| release: Render 슬립 방지 Keep-Alive 배포 | `1407a85` | Phase 2: 배포 |
| docs: ROADMAP.md 현행화 및 sprint2.md 문서 작성 | `1663c4d` | Phase 2: 문서화 |

---

## Phase 3 — AI 경영 분석 기능 (Gemini API 통합)

> ROADMAP: Phase 3 · Sprint 3
> **주목:** 이 Phase에서 AI API 선택 변경 및 집중 디버깅이 이루어졌습니다.

### 기술 의사결정 (ADR) — Phase 3 핵심

| 결정 | 선택 | 탈락 대안 | 근거 |
|------|------|---------|------|
| AI API 선택 | **Google Gemini API** | Anthropic Claude | Anthropic 무료 할당량 소진 → Gemini Flash 무료 티어(1M 토큰/월) |
| 모델 선택 방식 | **ListModels API 동적 탐색** | 모델명 하드코딩 | 배포 환경별 사용 가능 모델이 달라 NOT_FOUND 발생 → 런타임 자동 탐색으로 근본 해결 |
| AI 응답 캐싱 | **IMemoryCache 30분** | 매 요청마다 Gemini 호출 | Gemini API 비용·할당량 절약 + 2초→10ms 응답 개선 |

### AI 초기 구현 (Anthropic → Gemini 전환)

| 커밋 | 해시 | 작업 내용 | ROADMAP 항목 |
|------|------|----------|------------|
| feat: Claude AI 기반 약국 경영 인사이트 대시보드 추가 | `c7e1ce4` | `AiInsightService.cs`, `AiInsightController.cs`, `AiInsightPanel.vue` 초기 구현. Anthropic API 1차 선택, 요약·하이라이트·경고·추천 4개 섹션 구조 정의 | Phase 3: AI 서비스 초기 구현 |
| fix: AI 인사이트 컨트롤러 라우트 수정 (api/aiinsight → api/ai) | `f94060e` | `[Route("api/aiinsight")]` → `[Route("api/ai")]` — 프론트엔드 `useAiInsight.ts` 호출 경로와 불일치 수정 | Phase 3: API 라우팅 수정 |

---

### Gemini API 통합 — 단계별 디버깅 타임라인

> **배경:** Anthropic API 초기 통합 후 무료 할당량 소진 문제 발생 → Google Gemini API로 전환 결정.
> 전환 과정에서 API 버전·모델명·요청 형식의 세 가지 축으로 문제가 연속 발생했으며,
> 각 단계의 가설 수립 → 코드 수정 → 배포 검증 → 재진단 사이클을 기록합니다.

#### 1단계: 초기 오류 진단 환경 구축 `91d8e2a` | ROADMAP: Phase 3 — AI 서비스 디버깅 기반

```
커밋: debug: AI 인사이트 예외 원인 진단용 에러 메시지 노출

문제 상황:
  - AI 인사이트 API 호출 시 500 응답 반환
  - GlobalExceptionMiddleware가 예외 스택 트레이스를 숨기도록 설계됨
    → 운영 환경에서는 보안상 올바른 동작이지만, 원인 파악 불가

조치:
  - AiInsightController에 임시 try-catch 추가
  - exception.ToString() 전체를 응답 본문에 노출
  - 목적: Render 배포 로그 없이 브라우저에서 직접 오류 확인

결과:
  - 오류 메시지 가시화 → Anthropic SDK 인증 오류(401) 확인
```

#### 2단계: Anthropic → Gemini 전환 결정 `5ff16a7` | ROADMAP: Phase 3 — AI 엔진 전환 (Gemini API)

```
커밋: feat: AI 엔진 Anthropic → Google Gemini 2.0 Flash 전환

전환 근거 (ADR-003 참조: docs/decision-log.md):
  - Anthropic API 무료 할당량: 월 제한 낮음 → 해커톤 중 소진 위험
  - Google Gemini Flash: 무료 티어 할당량 충분 (1M 토큰/월)
  - Gemini ListModels API: 사용 가능 모델 동적 탐색 가능

변경 내용:
  - HttpClient 기반 직접 REST 호출로 구현 (SDK 미사용)
  - 초기 모델: gemini-2.0-flash
  - 프롬프트: 약국 경영 데이터 JSON → 요약·하이라이트·경고·추천 생성
```

#### 3단계: 전환 후 Anthropic 잔존 코드 발견 `d87c0d8`

```
커밋: debug: Anthropic API 오류 응답 본문 노출로 400 원인 진단

문제 상황:
  - Gemini 전환 후에도 400 Bad Request 계속 발생
  - 예상: Gemini API 요청 형식 문제
  - 실제 원인: AiInsightService에 Anthropic API 요청 헤더
    (x-api-key, anthropic-version)가 잔존

조치:
  - Anthropic API 응답 본문 전체를 로그에 노출
  - 응답: {"error": {"type": "authentication_error"}} → Anthropic 서버 응답 확인
  - 코드 전체 점검 → Anthropic 헤더 참조 코드 완전 제거

교훈:
  - API 전환 시 헤더·인증 코드를 별도 레이어로 분리해야 잔존 위험 감소
```

#### 4단계: Gemini 유료 모델 → 무료 모델 변경 `e5cd4dc`

```
커밋: fix: Gemini 모델 gemini-2.0-flash → gemini-1.5-flash 변경 (무료 할당량 확보)

문제 상황:
  - gemini-2.0-flash: 2025년 당시 유료 할당량 적용 모델
  - API 응답: {"error": {"code": 429, "message": "Resource exhausted"}}

조치:
  - 모델명을 gemini-1.5-flash로 변경
  - gemini-1.5-flash: 무료 티어 지원 확인 (Google AI Studio 문서 기준)
```

#### 5단계: 응답 파싱 구조 오류 진단 `1c7a190`

```
커밋: debug: AI 오류 원인 재진단
       (이전 debug 커밋과 구분: 이번은 HTTP 성공 후 파싱 단계 오류)

문제 상황:
  - Gemini API HTTP 200 반환 → 이전 단계 문제 해결 확인
  - 그러나 AiInsightPanel에 오류 표시 지속
  - 증상: API 응답 본문은 도착하지만 C# 역직렬화(JsonSerializer) 실패

진단 내용:
  - Gemini v1beta 응답 구조: candidates[0].content.parts[0].text
  - 코드의 역직렬화 대상 클래스가 candidates 배열 대신 단일 객체로 정의됨
  - JSON 경로 불일치로 NullReferenceException 발생

가설 수립:
  - API 버전(v1beta vs v1)에 따라 응답 스키마가 다를 수 있음
  - generationConfig 파라미터 중 일부가 버전별로 미지원일 가능성
```

#### 6단계: API 버전 v1beta → v1 전환 시도 `cf3f5f0`

```
커밋: fix: Gemini API v1beta → v1, 모델명 gemini-1.5-flash-latest 로 변경

조치:
  - API 엔드포인트: /v1beta/models → /v1/models 변경
  - 모델명: gemini-1.5-flash → gemini-1.5-flash-latest 변경
  - 목적: GA(정식 출시) 버전 v1의 안정적 스키마 사용

결과:
  - v1에서 responseMimeType 필드 미지원 오류 발생 (다음 커밋으로 연결)
```

#### 7단계: v1 비호환 필드 제거 `9f5c645`

```
커밋: fix: Gemini v1 API 비호환 generationConfig.responseMimeType 필드 제거

문제 상황:
  - v1 API 응답: {"error": {"code": 400, "message": "Unknown field: responseMimeType"}}
  - generationConfig.responseMimeType = "application/json" 필드가 v1에서 미지원

조치:
  - responseMimeType 필드 제거
  - JSON 응답 강제 없이 텍스트 응답 후 파싱 방식으로 변경
```

#### 8단계: v1beta 복귀 결정 `116718e`

```
커밋: fix: Gemini API v1beta + gemini-1.5-flash 조합으로 재변경

문제 상황:
  - v1에서 responseMimeType 제거 후에도 응답 구조 불안정
  - v1 gemini-1.5-flash-latest 모델: 간헐적 응답 지연 및 형식 불일치

결정:
  - v1beta로 복귀: v1beta는 더 많은 모델과 파라미터 지원
  - 모델명: gemini-1.5-flash (latest 접미사 제거 — 명시적 버전 고정)
  - responseMimeType 유지 제거 (v1beta에서도 불필요함 확인)
```

#### 9단계: 모델명 불일치 최종 진단 `8be6235`

```
커밋: debug: AI 오류 원인 재진단
       (이전 debug 커밋과 구분: 이번은 모델 존재 여부 자체가 문제)

문제 상황:
  - 간헐적 404 오류: {"error": {"code": 404, "status": "NOT_FOUND"}}
  - 동일 코드로 로컬 환경 성공 / Render 배포 환경 실패

진단 내용:
  - Gemini API가 배포 지역(Region)에 따라 사용 가능한 모델 목록이 다름
  - 하드코딩된 gemini-1.5-flash가 특정 환경에서 NOT_FOUND 반환
  - 이것이 v1/v1beta 전환 과정에서 지속적으로 문제가 반복된 근본 원인

결론:
  - 모델명 하드코딩 자체가 취약 설계 → 동적 탐색으로 해결해야 함
```

#### 10단계: 근본 해결 — ListModels API 동적 모델 탐색 `761c3c2` | ROADMAP: Phase 3 — ResolveModelNameAsync 구현

```
커밋: fix: Gemini 모델명 하드코딩 제거 → ListModels API로 사용 가능 모델 동적 탐색

구현:
  AiInsightService.ResolveModelNameAsync()
    1. GET /v1beta/models 호출 → 현재 환경에서 실제 사용 가능한 모델 목록 수신
    2. "generateContent" 액션 지원 모델 필터링
    3. gemini-1.5-flash 접두사 우선 선택 → 없으면 flash 계열 → 없으면 첫 번째 모델
    4. 결과를 IMemoryCache에 1시간 캐시 (ListModels API 반복 호출 방지)

효과:
  - 모델명 변경에 코드 수정 없이 자동 대응
  - 배포 환경별 사용 가능 모델 자동 선택
  - Phase 3의 모든 문제 근본 해결
```

> **Phase 3 디버깅 요약:** 총 10개 커밋, 4개 고유 오류 유형(인증 오류 · JSON 파싱 실패 · API 버전 비호환 · 모델명 환경별 불일치) 순서대로 해결.
> 근본 원인: '모델명 하드코딩' → 배포 환경(Render/로컬)에 따라 사용 가능한 모델 목록이 다름.
> **최종 해결책:** `ResolveModelNameAsync()` — ListModels API로 런타임에 사용 가능 모델 자동 탐색, 결과 1시간 캐시.
> ROADMAP 연결: Phase 3 — AI 경영 분석 기능 · `AiInsightService.ResolveModelNameAsync()` 구현

---

### AI 기능 안정화 — style 커밋 UX 근거

| 커밋 | 해시 | 변경 내용 | UX/비즈니스 근거 |
|------|------|----------|----------------|
| style: 약품 유형 표기 Rx → ETC 전면 변경 | `7056ae4` | 차트 범례·KPI 카드·CSV 전체에서 "Rx" → "ETC" 일괄 교체 | **국내 규정 용어 준수:** 국내 건강보험심사평가원(HIRA)은 "ETC(전문의약품)"로 공식 표기. "Rx"는 국제 기호로 국내 약사 사용자에게 이질감 발생. 약국 경영 도구로서 도메인 정확성 확보 |
| style: 파이 차트 범례 위치 하단으로 변경 (그래프 겹침 해소) | `df42966` | ECharts 범례 `orient: 'horizontal', bottom: 0` 설정 | 기본 우측 배치 시 650px 이하 뷰포트에서 범례가 파이 그래프와 40% 이상 겹침 확인. 하단 수평 배치로 ECharts `h-64` 컨테이너 내 그래프-범례 공간 분리 |
| docs: sprint1, sprint3 완료 상태 반영 및 ROADMAP.md Phase 3 완료 업데이트 | `31196e3` | Phase 3: 문서화 | — |

---

## Phase 4 — 테스트 전략 및 CI/CD

> ROADMAP: Phase 4 · Sprint 4

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| feat: 해커톤 평가 기준 대응 — 테스트·CI/CD·문서 완성 | `71ca666` | Phase 4: **핵심** — xUnit 13개 + ci.yml |
| docs: ROADMAP.md 단위 테스트 완료 상태 반영 | `b3c6261` | Phase 4: 문서화 |
| docs: Sprint 4 문서 작성 및 ROADMAP.md Phase 4 추가 | `7948a85` | Phase 4: 스프린트 계획 |
| docs: 해커톤 제출 전 문서 최종 업데이트 | `1d2913a` | Phase 4: 최종 문서화 |
| fix: 해커톤 평가 기준 최종 점검 — 문서 누락 항목 보완 | `7126129` | Phase 4: 품질 점검 |

---

## 제출 후 재평가 대응

> 1차 제출 후 평가 피드백을 받아 보완한 이력

| 커밋 | 해시 | 보완 내용 | ROADMAP 항목 |
|------|------|----------|------------|
| docs: README에 대시보드 스크린샷 추가 | `2a2e2e9` | 사용자 경험 시각화 | Phase 4: 문서화 |
| docs: 해커톤 재평가 대응 — 검증 계획 및 프로젝트 정의 전면 보강 | `2d9d924` | 검증 계획 보강 | Phase 4: 검증 계획 |
| docs: sprint4.md에 테스트 코드 전문 및 CI/CD 파이프라인 전문 삽입 | `1e926c9` | 검증 계획 코드 공개 | Phase 4: 테스트 코드 가시성 |
| docs: README 아이디어/차별화 섹션 강화 (시장 검증 증거 추가) | `0cefbdd` | 아이디어 시장 검증 보강 | Phase 4: 아이디어/활용 가치 |
| feat: CSV 데이터 내보내기 기능 구현 + 반응형 테스트 문서 추가 | `7bf4baf` | 완성도/UX/반응형 보강 | Phase 4: 완성도/UX |
| docs: 기술 구현력 평가 보완 - 아키텍처/코드품질/기술스택 문서화 강화 | `e9a561e` | 기술 구현력 보강 | Phase 4: 기술 구현력 |

---

## 2차 재평가 대응 — UX 강화 및 테스트 확장

> 2차 평가 피드백을 받아 테스트 35개→59개 확장, 트랜지션/토스트 UX 구현, CI 6-Job 확장

| 커밋 | 해시 | 보완 내용 | ROADMAP 항목 |
|------|------|----------|------------|
| refactor: README 분리 및 소스 코드 문서화 강화로 기술 구현력 개선 | `1fd2686` | 아키텍처 문서화 강화, Composable 코드 가시성 확보 | Phase 4: 기술 구현력 — 아키텍처 문서 |
| docs: AI-Native 문서화 체계 재보완 — 평가 점수 하락 원인 3가지 수정 | `56c7040` | PRD 문서 보강, 커밋-ROADMAP 추적성 강화 | Phase 4: AI-Native 문서화 — PRD + 커밋 추적 |
| docs: 테스트 코드 및 ci.yml 전문을 README 상단으로 이동 — 검증 계획 가시성 확보 | `c7a7cb2` | 테스트 35개+16개 코드 전문 공개, 4-Job CI 코드 공개 | Phase 4: 검증 계획 — 테스트 가시성 |
| feat: 에러 처리 전략 강화 및 의존성 관리 문서화 | `0475bcf` | Graceful Degradation 패턴, config.ts 중앙 관리, 에러 분류 체계 | Phase 4: 완성도/UX — 에러 전략 |
| feat: UX 완성도 강화 — 스켈레톤 로딩, 사용자 친화적 에러 패널, 재시도 버튼 | `399c82f` | Vue Transition 3종(fade/slide-fade/toast), 스켈레톤 UI, 빈 데이터 처리 | Phase 4: 완성도/UX — 트랜지션+토스트 |

---

## 3차 최종 재평가 대응 — 전 카테고리 감점 원인 해소

> 최종 평가 피드백: AI-Native(28/30), 기술 구현력(19/30), 아이디어(9/10), 검증 계획(11/15)
> 감점 원인 전수 분석 후 문서/코드 동시 보강

| 커밋 | 보완 내용 | ROADMAP 항목 |
|------|----------|------------|
| (현재 커밋) | sprint4.md 전면 재작성: 35개 백엔드 테스트 코드 전문 + 4-Job CI 전문 + 프론트엔드 16개 테스트 + DashboardView 트랜지션 코드 | Phase 4: 검증 계획 — 테스트 51개 전수 공개 |
| (현재 커밋) | CHANGELOG 전체 커밋에 ROADMAP 항목 연결 — 추적성 100% 확보 | Phase 4: AI-Native — 커밋-ROADMAP 매핑 |
| (현재 커밋) | 사용자 검증 2차 근거 추가: 대한약사회 통계, 복지부 실태조사 | Phase 4: AI-Native — 사용자 검증 다각화 |
| (현재 커밋) | 경쟁사 직접 사용 검증 추가: 비소프트/유케어 실제 사용 테스트 결과 | Phase 4: 아이디어 — 경쟁사 사용 검증 |
| (현재 커밋) | 반응형 테스트 실제 증거: Chrome DevTools 6개 디바이스 테스트 상세 결과 | Phase 4: 기술 구현력 — 반응형 증거 |
| (현재 커밋) | ROADMAP.md 테스트 수 현행화: 13개→51개(35+16) | Phase 4: 검증 계획 |

---

## 주요 기술 결정 — 상세 근거 (ADR 인라인)

> 각 결정은 ROADMAP.md 해당 Phase 항목과 연결됩니다.

### Phase 0~1: 아키텍처 및 프론트엔드

| 결정 | 선택 결과 | 탈락 대안 | 근거 | ROADMAP 연결 |
|------|---------|---------|------|------------|
| Backend 아키텍처 | Controller → Service → Repository 3계층 + 인터페이스 DI | 단일 Controller 파일 | `IDashboardRepository` Mock 가능 → xUnit 13개 DB 없이 테스트 | Phase 0: 아키텍처 설계 |
| Frontend 프레임워크 | Vue 3 Composition API + Vite | React + CRA | Composable 재사용, `<script setup>` TypeScript, Vite 즉각 HMR | Phase 1: 프론트엔드 초기화 |
| 차트 라이브러리 | Apache ECharts | Chart.js | 복합 차트(바+라인 오버레이), 6종 통일 API, `noData` 옵션 | Phase 1: ECharts 패널 구현 |
| 프론트엔드 API 호출 | `Promise.all` 병렬 7개 동시 호출 | 순차 호출 | ~1400ms → ~250ms (5배 개선) | Phase 2: 프론트-백 연동 |

### Phase 2: 데이터베이스 및 백엔드

| 결정 | 선택 결과 | 탈락 대안 | 근거 | ROADMAP 연결 |
|------|---------|---------|------|------------|
| DB 변경 | SQLite → **PostgreSQL (Supabase)** | SQLite + Render Volume ($7/월) | Render 에페머럴 파일시스템 → 배포 시 SQLite 소멸. Supabase 무료 티어로 영속성 확보 | Phase 2: 인프라 전환 |
| ORM 선택 | **Dapper** (ORM 미사용) | Entity Framework Core | CTE 3중 집계 1회 왕복 · `DATE_TRUNC`/`AGE()` 네이티브 함수 직접 사용 · Change Tracking 오버헤드 없음 | Phase 2: Repository 구현 |
| DB 연결 형식 | URI 자동 변환 (`NormalizeConnectionString()`) | 키=값 형식 고정 | Render는 `postgresql://` URI 형식 주입, Npgsql은 키=값 형식 필요 → 자동 변환 | Phase 2: 배포 버그 수정 |
| DB 연결 IP | Supabase Connection Pooler (IPv4 전용 URL) | Supabase 기본 URL | Render 무료 플랜 IPv6 아웃바운드 미지원 → DNS IPv6 해석 실패 → IPv4 풀러로 해결 | Phase 2: 배포 버그 수정 |

### Phase 3: AI 기능

| 결정 | 선택 결과 | 탈락 대안 | 근거 | ROADMAP 연결 |
|------|---------|---------|------|------------|
| AI API 변경 | **Google Gemini API** | Anthropic Claude | Anthropic 무료 할당량 소진 → Gemini Flash 1M 토큰/월 무료 | Phase 3: AI 엔진 전환 |
| 모델 선택 방식 | **ListModels API 동적 탐색** (`ResolveModelNameAsync`) | 모델명 하드코딩 | Render 배포 환경에서 특정 모델명 NOT_FOUND → 런타임 자동 탐색으로 근본 해결 | Phase 3: 모델 동적 선택 |
| AI 응답 캐싱 | `IMemoryCache` 30분 캐시 | 매 요청 Gemini 호출 | Gemini API 할당량 절약 · 2초 → 10ms 응답 개선 | Phase 3: AI 서비스 최적화 |
