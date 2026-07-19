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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}
