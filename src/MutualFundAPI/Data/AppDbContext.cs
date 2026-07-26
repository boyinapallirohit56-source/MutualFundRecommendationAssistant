using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<RiskQuestion> RiskQuestions => Set<RiskQuestion>();
    public DbSet<RiskOption> RiskOptions => Set<RiskOption>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<RiskResponse> RiskResponses => Set<RiskResponse>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<RecommendationAllocation> RecommendationAllocations => Set<RecommendationAllocation>();
    public DbSet<AllocationRule> AllocationRules => Set<AllocationRule>();
    public DbSet<MutualFund> MutualFunds => Set<MutualFund>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<PortfolioHolding> PortfolioHoldings => Set<PortfolioHolding>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<RiskLevel> RiskLevels => Set<RiskLevel>();
    public DbSet<GoalType> GoalTypes => Set<GoalType>();
    public DbSet<FundCategory> FundCategories => Set<FundCategory>();
    public DbSet<AssetClass> AssetClasses => Set<AssetClass>();
    public DbSet<InvestmentType> InvestmentTypes => Set<InvestmentType>();
    public DbSet<StressScenario> StressScenarios => Set<StressScenario>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<FundNAVHistory> FundNAVHistory => Set<FundNAVHistory>();
    public DbSet<FundHolding> FundHoldings => Set<FundHolding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Disable cascade delete for ALL relationships to prevent circular cascade errors
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
        });

        // RiskQuestion -> Options
        modelBuilder.Entity<RiskOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId);

        // RiskAssessment -> Responses
        modelBuilder.Entity<RiskResponse>()
            .HasOne(r => r.Assessment)
            .WithMany(a => a.Responses)
            .HasForeignKey(r => r.AssessmentId);

        // Recommendation -> Allocations
        modelBuilder.Entity<RecommendationAllocation>()
            .HasOne(a => a.Recommendation)
            .WithMany(r => r.Allocations)
            .HasForeignKey(a => a.RecommendationId);

        // Portfolio -> Holdings
        modelBuilder.Entity<PortfolioHolding>()
            .HasOne(h => h.Portfolio)
            .WithMany(p => p.Holdings)
            .HasForeignKey(h => h.PortfolioId);

        // Configure decimal precision for all monetary/percentage fields
        modelBuilder.Entity<MutualFund>(entity =>
        {
            entity.Property(f => f.NAV).HasPrecision(18, 4);
            entity.Property(f => f.CAGR1Y).HasPrecision(10, 2);
            entity.Property(f => f.CAGR3Y).HasPrecision(10, 2);
            entity.Property(f => f.CAGR5Y).HasPrecision(10, 2);
            entity.Property(f => f.ExpenseRatio).HasPrecision(10, 4);
            entity.Property(f => f.AUM).HasPrecision(18, 2);
            entity.Property(f => f.Rating).HasPrecision(5, 2);
            entity.Property(f => f.SharpeRatio).HasPrecision(10, 4);
            entity.Property(f => f.Alpha).HasPrecision(10, 4);
            entity.Property(f => f.Beta).HasPrecision(10, 4);
            entity.Property(f => f.StandardDeviation).HasPrecision(10, 4);
            entity.Property(f => f.ExitLoad).HasPrecision(10, 4);
            entity.Property(f => f.RollingReturns3Y).HasPrecision(10, 2);
        });

        modelBuilder.Entity<PortfolioHolding>(entity =>
        {
            entity.Property(h => h.Units).HasPrecision(18, 4);
            entity.Property(h => h.PurchaseNAV).HasPrecision(18, 4);
            entity.Property(h => h.InvestedAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.Property(p => p.MonthlyIncome).HasPrecision(18, 2);
            entity.Property(p => p.MonthlyExpenses).HasPrecision(18, 2);
            entity.Property(p => p.Savings).HasPrecision(18, 2);
            entity.Property(p => p.Loans).HasPrecision(18, 2);
            entity.Property(p => p.SIPAmount).HasPrecision(18, 2);
            entity.Property(p => p.LumpSumAmount).HasPrecision(18, 2);
            entity.Property(p => p.SWPAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.Property(g => g.TargetAmount).HasPrecision(18, 2);
            entity.Property(g => g.CurrentAmount).HasPrecision(18, 2);
            entity.Property(g => g.MonthlySIP).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AllocationRule>().Property(r => r.Percentage).HasPrecision(10, 2);
        modelBuilder.Entity<RecommendationAllocation>().Property(a => a.Percentage).HasPrecision(10, 2);
        modelBuilder.Entity<FundHolding>().Property(h => h.Percentage).HasPrecision(10, 4);
        modelBuilder.Entity<FundNAVHistory>().Property(n => n.NAV).HasPrecision(18, 4);
        modelBuilder.Entity<StressScenario>().Property(s => s.PercentageChange).HasPrecision(10, 2);
    }
}
