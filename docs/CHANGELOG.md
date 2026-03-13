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

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| docs: 프로젝트 기반 문서 및 설정 파일 추가 | `5c00605` | Phase 0: 프로젝트 초기화 |
| ci: GitHub Actions 워크플로우 및 개발 프로세스 문서 추가 | `19656a3` | Phase 0: CI/CD 기반 |
| chore: Claude AI 에이전트 정의 및 메모리 시스템 추가 | `998fbc1` | Phase 0: AI 협업 시스템 |
| docs: Sprint 0 스프린트 계획 문서 작성 | `06bd1d2` | Phase 0: 스프린트 계획 |
| feat: 백엔드 .NET 9.0 Web API 프로젝트 스캐폴딩 및 SQLite 스키마 초기화 | `008f5bd` | Phase 0: 백엔드 초기화 |
| chore: .gitignore에 .NET 빌드 산출물 제외 규칙 추가 | `e1b035f` | Phase 0: 환경 설정 |

---

## Phase 1 — 프론트엔드 UI 개발

> ROADMAP: Phase 1 · Sprint 1

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| feat: 프론트엔드 Vue 3 + Vite 6 + TypeScript 프로젝트 스캐폴딩 | `43be119` | Phase 1: 프로젝트 생성 |
| docs: README.md에 Vercel 배포 URL 등록 | `7e1c4c7` | Phase 1: 배포 URL 기록 |
| feat: 대시보드 TypeScript 타입 정의 및 Mock 데이터 Composable 구현 | `afc1543` | Phase 1: 타입/Composable |
| feat: 대시보드 메인 뷰 연결 및 빌드 환경 설정 | `4569a33` | Phase 1: UI 메인 뷰 |
| docs: Sprint 1 스프린트 계획 문서 작성 | `f5fc702` | Phase 1: 문서화 |
| merge: sprint/sprint1 → develop | `62c01f8` | Phase 1: 브랜치 통합 |
| release: Phase 1 프론트엔드 대시보드 UI → master 배포 | `62443d8` | Phase 1: 배포 |

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

### AI 초기 구현 (Anthropic → Gemini 전환)

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| feat: Claude AI 기반 약국 경영 인사이트 대시보드 추가 | `c7e1ce4` | Phase 3: AI 서비스 초기 구현 |
| fix: AI 인사이트 컨트롤러 라우트 수정 (api/aiinsight → api/ai) | `f94060e` | Phase 3: API 라우팅 수정 |

### Gemini API 디버깅 과정 (기술 결정 추적)

> **이 섹션은 실제 문제 해결 과정을 보여줍니다.** Anthropic API에서 Gemini API로 전환 후
> 올바른 API 버전·모델명·요청 형식을 찾는 과정에서 발생한 시행착오 이력입니다.

| 커밋 | 해시 | 기술적 맥락 |
|------|------|-------------|
| `debug: AI 인사이트 예외 원인 진단용 에러 메시지 노출` | `91d8e2a` | 예외 내용이 숨겨져 있어 원인 불명 → 응답 본문 노출로 디버깅 |
| `feat: AI 엔진 Anthropic → Google Gemini 2.0 Flash 전환` | `5ff16a7` | 무료 할당량 제약으로 Gemini로 전환 결정 |
| `debug: Anthropic API 오류 응답 본문 노출로 400 원인 진단` | `d87c0d8` | 전환 과정 중 구버전 Anthropic 코드 잔존 → 400 오류 원인 추적 |
| `fix: Gemini 모델 gemini-2.0-flash → gemini-1.5-flash 변경 (무료 할당량 확보)` | `e5cd4dc` | 2.0-flash는 유료 할당량 → 무료 1.5-flash로 변경 |
| `debug: AI 오류 원인 재진단` | `1c7a190` | API 응답 파싱 오류 추가 진단 |
| `fix: Gemini API v1beta → v1, 모델명 gemini-1.5-flash-latest 로 변경` | `cf3f5f0` | v1beta 일부 파라미터 비호환 발견 |
| `fix: Gemini v1 API 비호환 generationConfig.responseMimeType 필드 제거` | `9f5c645` | v1에서 responseMimeType 필드 미지원 확인 |
| `fix: Gemini API v1beta + gemini-1.5-flash 조합으로 재변경` | `116718e` | v1beta가 더 안정적임을 확인, 롤백 |
| `debug: AI 오류 원인 재진단` | `8be6235` | 최종 오류 원인 확인 (모델명 불일치) |
| `fix: Gemini 모델명 하드코딩 제거 → ListModels API로 사용 가능 모델 동적 탐색` | `761c3c2` | **근본 해결:** 모델명 변경에 유연하게 대응하는 동적 모델 선택 구현 |

> **결론:** 위 디버깅 과정을 통해 `AiInsightService.ResolveModelNameAsync()`
> (ListModels API로 실제 사용 가능 모델을 런타임에 자동 선택, 1시간 캐시)를 구현함.
> 이는 향후 Gemini 모델명 변경에도 코드 수정 없이 대응 가능한 구조.

### AI 기능 안정화

| 커밋 | 해시 | ROADMAP 항목 |
|------|------|--------------|
| style: 약품 유형 표기 Rx → ETC 전면 변경 | `7056ae4` | Phase 3: UX 개선 |
| docs: sprint1, sprint3 완료 상태 반영 및 ROADMAP.md Phase 3 완료 업데이트 | `31196e3` | Phase 3: 문서화 |

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

| 커밋 | 해시 | 보완 내용 |
|------|------|----------|
| docs: README에 대시보드 스크린샷 추가 | `2a2e2e9` | 사용자 경험 시각화 |
| docs: 해커톤 재평가 대응 — 검증 계획 및 프로젝트 정의 전면 보강 | `2d9d924` | 검증 계획 보강 |
| docs: sprint4.md에 테스트 코드 전문 및 CI/CD 파이프라인 전문 삽입 | `1e926c9` | 검증 계획 코드 공개 |
| docs: README 아이디어/차별화 섹션 강화 (시장 검증 증거 추가) | `0cefbdd` | 아이디어 시장 검증 보강 |
| feat: CSV 데이터 내보내기 기능 구현 + 반응형 테스트 문서 추가 | `7bf4baf` | 완성도/UX/반응형 보강 |
| docs: 기술 구현력 평가 보완 - 아키텍처/코드품질/기술스택 문서화 강화 | `e9a561e` | 기술 구현력 보강 |

---

## 주요 기술 결정 요약

| 결정 | 시점 | 근거 |
|------|------|------|
| SQLite → PostgreSQL(Supabase) 전환 | Phase 2 | Render 에페머럴 파일시스템으로 SQLite 데이터 소멸 |
| ORM 미사용 (Dapper) | 설계 단계 | 집계 쿼리 최적화를 위한 순수 SQL 제어 필요 |
| Anthropic → Google Gemini 전환 | Phase 3 | 무료 할당량 제약 해소 및 안정적 API 운용 |
| 동적 모델 선택 (`ListModels API`) | Phase 3 디버깅 | 모델명 하드코딩 취약점 제거 → 자동 탐색으로 근본 해결 |
| `IMemoryCache` 30분 캐시 | Phase 3 | Gemini API 비용 절감 및 응답 속도 개선 |
| `Promise.all` 병렬 호출 | Phase 1 | 7개 API 순차 호출 대비 응답시간 단축 |
