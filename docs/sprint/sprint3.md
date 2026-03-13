# Sprint 3: AI 약국 경영 분석 기능 추가

## 스프린트 개요

| 항목 | 내용 |
|------|------|
| **스프린트 번호** | Sprint 3 |
| **연결된 Phase** | Phase 3: AI 경영 분석 기능 |
| **작성일** | 2026-03-13 |
| **담당자** | 1인 풀스택 개발 |
| **상태** | ✅ 완료 |
| **작업 브랜치** | `sprint/sprint3` |

---

## 목표 (Goal)

Claude AI API를 백엔드에 연동하여 실시간 약국 경영 데이터를 분석하고,
대시보드 상단에 AI가 생성한 경영 인사이트를 표시한다.
앱 명칭을 **"약국 경영 통합 AI 대시보드"** 로 리브랜딩한다.

---

## 구현 AI 기능 목록

| 기능 | 설명 |
|------|------|
| AI 경영 요약 | 이번 달 경영 현황을 2~3문장으로 친절하게 요약 |
| AI 핵심 하이라이트 | 긍정적인 주목 포인트 (초록 배지) |
| AI 주의 사항 | 개선 필요 영역 경고 (노란 배지) |
| AI 경영 추천 | 데이터 기반 실용적 조언 1~2문장 |

---

## 작업 분해 (Task Breakdown)

### T3-1: 백엔드 AI 서비스

**파일:**
- `backend/Models/AiInsightModels.cs`
- `backend/Services/Interfaces/IAiInsightService.cs`
- `backend/Services/AiInsightService.cs`
- `backend/Controllers/AiInsightController.cs`

**구현 내용:**
- `IDashboardRepository`에서 KPI·월별 매출·약품 유형·병원 데이터 수집
- Anthropic Claude API(`claude-haiku-4-5-20251001`) 호출로 JSON 형식 인사이트 생성
- `IMemoryCache` 30분 캐시 — API 비용 절감 및 응답 속도 개선
- `IHttpClientFactory`로 Named HttpClient 등록 (`"Anthropic"`)
- API 키 미설정 시 안내 메시지 반환 (Graceful Degradation)

**API 엔드포인트:** `GET /api/ai/insight`

**응답 형식:**
```json
{
  "summary": "이번 달 약국 경영은 전반적으로...",
  "highlights": ["매출 전월 대비 증가", "조제 건수 호조"],
  "warnings": ["발주 지출 점검 권장"],
  "recommendation": "전문의약품 비중을 활용하여...",
  "generatedAt": "2026-03-13T..."
}
```

### T3-2: 프론트엔드 AI 인사이트 패널

**파일:**
- `frontend/src/types/api.ts` — `AiInsight` 인터페이스 추가
- `frontend/src/composables/useAiInsight.ts` — AI 인사이트 fetch composable
- `frontend/src/components/AiInsightPanel.vue` — AI 패널 컴포넌트
- `frontend/src/views/DashboardView.vue` — 패널 삽입 및 앱 명칭 변경

**UI 설계:**
```
┌──────────────────────────────────────────────────────────┐
│ ✨ PharmSight AI 경영 분석                  [갱신 시각]   │
│                                                          │
│  "이번 달 귀 약국 경영 현황은 전반적으로 긍정적입니다..."  │
│                                                          │
│  [✓ 매출 전월比 증가]  [✓ 조제 건수 호조]  [⚠ 발주 점검] │
│                                                          │
│  💡 전문의약품 처방 연계 강화를 위해...                    │
└──────────────────────────────────────────────────────────┘
```
- 로딩 스켈레톤 UI (AI 응답 대기 중)
- API 키 미설정 시 graceful fallback 메시지

### T3-3: 인프라 설정

- `backend/Program.cs`: `IMemoryCache`, Named HttpClient(`"Anthropic"`) DI 등록
- `backend/appsettings.json`: `"Anthropic": { "ApiKey": "" }` 섹션 추가
- Render 환경변수: `Anthropic__ApiKey` 등록 필요 (수동)

---

## 완료 조건 (Definition of Done)

- [x] `GET /api/ai/insight` 200 응답 및 JSON 형식 인사이트 반환
- [x] 대시보드 상단에 AI 인사이트 패널 표시
- [x] 로딩 중 스켈레톤 UI 표시
- [x] API 키 미설정 시 에러 없이 안내 메시지 표시
- [x] 헤더 타이틀 "약국 경영 통합 AI 대시보드" 반영
- [x] Vercel 재배포 완료

---

## 기술 부채 및 주의사항

- Anthropic API 키는 반드시 Render 환경변수로만 관리 (코드/Git 커밋 금지)
- 캐시 30분 → 데이터 갱신 원할 시 수동 서버 재시작 필요 (추후 캐시 무효화 엔드포인트 고려)
