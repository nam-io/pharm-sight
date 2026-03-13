# CI/CD 파이프라인 구성

> **파일 경로:** `.github/workflows/ci.yml`
> **트리거:** `master`, `develop` 브랜치 push 및 PR

## 파이프라인 구조

```
push / pull_request (master, develop)
├── backend-test job          ← .NET 9.0 빌드 + xUnit 단위 테스트
└── frontend-build job        ← Node 20 + Vite 프로덕션 빌드 검증
```

## 전체 워크플로우 (`ci.yml`)

```yaml
name: CI - 빌드 및 테스트 자동화

on:
  push:
    branches: [master, develop]
  pull_request:
    branches: [master, develop]

jobs:
  backend-test:
    name: 백엔드 빌드 및 xUnit 단위 테스트
    runs-on: ubuntu-latest
    steps:
      - name: 코드 체크아웃
        uses: actions/checkout@v4

      - name: .NET 9.0 설정
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"

      - name: NuGet 패키지 복원 (API)
        run: dotnet restore backend/PharmSight.Api.csproj

      - name: NuGet 패키지 복원 (Tests)
        run: dotnet restore backend/PharmSight.Tests/PharmSight.Tests.csproj

      - name: 백엔드 빌드 (Release)
        run: dotnet build backend/PharmSight.Api.csproj --no-restore --configuration Release

      - name: xUnit 단위 테스트 실행
        run: dotnet test backend/PharmSight.Tests/PharmSight.Tests.csproj
             --configuration Release --verbosity normal
             --logger "trx;LogFileName=test-results.trx"

      - name: 테스트 결과 업로드
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: backend-test-results
          path: backend/PharmSight.Tests/TestResults/*.trx

  frontend-build:
    name: 프론트엔드 빌드 검증 (Vite)
    runs-on: ubuntu-latest
    steps:
      - name: 코드 체크아웃
        uses: actions/checkout@v4

      - name: Node.js 20 설정
        uses: actions/setup-node@v4
        with:
          node-version: "20"
          cache: "npm"
          cache-dependency-path: frontend/package-lock.json

      - name: NPM 패키지 설치
        run: npm ci --prefix frontend

      - name: 프론트엔드 프로덕션 빌드 검증
        run: npm run build --prefix frontend
        env:
          VITE_API_BASE_URL: https://pharm-sight.onrender.com
```

## 각 Job 설명

### backend-test

| 단계 | 설명 |
|------|------|
| `actions/setup-dotnet@v4` | .NET 9.0.x SDK 설치 |
| `dotnet restore` | NuGet 패키지 복원 (API + Tests 별도) |
| `dotnet build --configuration Release` | Release 빌드로 컴파일 검증 |
| `dotnet test --verbosity normal` | 13개 xUnit 케이스 실행 |
| `upload-artifact` | TRX 포맷 테스트 결과 GitHub Actions artifact 업로드 |

### frontend-build

| 단계 | 설명 |
|------|------|
| `actions/setup-node@v4` | Node.js 20 + npm 캐시 설정 |
| `npm ci --prefix frontend` | package-lock.json 기반 재현 가능 설치 |
| `npm run build` | Vite 프로덕션 빌드 (`VITE_API_BASE_URL` 주입) |

## 보호 대상 브랜치

- **master**: 프로덕션 배포 기준 브랜치
- **develop**: 스프린트 통합 브랜치

PR이 `master` 또는 `develop`로 향할 경우 두 job이 모두 통과해야 병합이 가능합니다.
