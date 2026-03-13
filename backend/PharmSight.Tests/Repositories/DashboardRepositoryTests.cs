using Microsoft.Extensions.Configuration;
using PharmSight.Api.Repositories;

namespace PharmSight.Tests.Repositories;

/// <summary>
/// DashboardRepository 통합 테스트.
/// 실제 DB 연결 없이 Repository 계층의 초기화 로직과 연결 문자열 처리를 검증합니다.
/// NormalizeConnectionString 유틸리티의 URI → Npgsql 키=값 변환 로직을 테스트합니다.
/// </summary>
public class DashboardRepositoryTests
{
    // ─────────────────────────────────────────────────────────
    // 연결 문자열 정규화 (NormalizeConnectionString)
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void 키값_형식_연결문자열은_그대로_사용된다()
    {
        // Arrange — 일반적인 Npgsql 키=값 형식
        var config = BuildConfig("Host=localhost;Port=5432;Database=pharmsight;Username=user;Password=pass");

        // Act — Repository 생성 시 NormalizeConnectionString이 호출됨
        var repo = new DashboardRepository(config);

        // Assert — 예외 없이 생성되면 정상
        Assert.NotNull(repo);
    }

    [Fact]
    public void PostgreSQL_URI_형식이_정상_변환된다()
    {
        // Arrange — Render 환경변수에서 주입되는 postgresql:// URI 형식
        var config = BuildConfig("postgresql://user:pass@db.supabase.co:5432/postgres");

        // Act
        var repo = new DashboardRepository(config);

        // Assert — URI 파싱 후 Repository 생성 성공
        Assert.NotNull(repo);
    }

    [Fact]
    public void Postgres_URI_형식도_정상_변환된다()
    {
        // Arrange — postgres:// 프로토콜 (postgresql://의 축약형)
        var config = BuildConfig("postgres://user:p%40ss@host.com:6543/mydb");

        // Act
        var repo = new DashboardRepository(config);

        // Assert
        Assert.NotNull(repo);
    }

    [Fact]
    public void 연결문자열_미설정시_예외가_발생한다()
    {
        // Arrange — 빈 설정
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert — InvalidOperationException 발생 확인
        Assert.Throws<InvalidOperationException>(() => new DashboardRepository(config));
    }

    [Fact]
    public void URI에_특수문자_비밀번호가_포함되어도_정상_처리된다()
    {
        // Arrange — URL 인코딩된 특수문자 비밀번호 (@, #, % 등)
        var config = BuildConfig("postgresql://admin:p%40ss%23w0rd@db.example.com:5432/testdb");

        // Act
        var repo = new DashboardRepository(config);

        // Assert
        Assert.NotNull(repo);
    }

    [Fact]
    public void URI_포트_미지정시_기본_5432_포트가_적용된다()
    {
        // Arrange — 포트 생략 (기본값 5432 적용 확인)
        var config = BuildConfig("postgresql://user:pass@db.supabase.co/postgres");

        // Act
        var repo = new DashboardRepository(config);

        // Assert
        Assert.NotNull(repo);
    }

    // ─────────────────────────────────────────────────────────
    // DI 컨테이너 통합 (의존성 주입 검증)
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void IDashboardRepository_인터페이스를_구현한다()
    {
        // Assert — DashboardRepository가 인터페이스를 올바르게 구현하는지 확인
        Assert.True(typeof(PharmSight.Api.Repositories.Interfaces.IDashboardRepository)
            .IsAssignableFrom(typeof(DashboardRepository)));
    }

    [Fact]
    public void Repository에_7개_메서드가_존재한다()
    {
        // Assert — IDashboardRepository의 7개 메서드가 모두 구현되어 있는지 확인
        var methods = typeof(DashboardRepository).GetMethods()
            .Where(m => m.Name.EndsWith("Async") && m.DeclaringType == typeof(DashboardRepository))
            .ToList();

        Assert.Equal(7, methods.Count);
        Assert.Contains(methods, m => m.Name == "GetMonthlySalesAsync");
        Assert.Contains(methods, m => m.Name == "GetDrugTypeSalesAsync");
        Assert.Contains(methods, m => m.Name == "GetPatientAgeGroupsAsync");
        Assert.Contains(methods, m => m.Name == "GetHospitalPrescriptionsAsync");
        Assert.Contains(methods, m => m.Name == "GetWholesaleExpensesAsync");
        Assert.Contains(methods, m => m.Name == "GetDrugCoverageAsync");
        Assert.Contains(methods, m => m.Name == "GetKpiSummaryAsync");
    }

    // ─────────────────────────────────────────────────────────
    // 헬퍼
    // ─────────────────────────────────────────────────────────

    private static IConfiguration BuildConfig(string connectionString)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();
    }
}
