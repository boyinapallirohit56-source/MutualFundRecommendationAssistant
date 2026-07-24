using Microsoft.Extensions.Logging;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Data.Seeders;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context, ILogger logger, string environment)
    {
        logger.LogInformation("Starting database seeding for environment: {Environment}", environment);

        // 1. Production Seeders (always run)
        SeedRoles(context, logger);
        SeedRiskLevels(context, logger);
        SeedGoalTypes(context, logger);
        SeedFundCategories(context, logger);
        SeedAssetClasses(context, logger);
        SeedInvestmentTypes(context, logger);
        SeedStressScenarios(context, logger);
        SeedRiskQuestions(context, logger);
        SeedAllocationRules(context, logger);
        SeedMutualFunds(context, logger);

        // 2. Dev/Test Seeders (only in Development)
        if (environment == "Development")
        {
            SeedDemoUsers(context, logger);
            SeedSamplePortfolios(context, logger);
            SeedTestRecommendations(context, logger);
            SeedFundHoldings(context, logger);
            SeedNAVHistory(context, logger);
            SeedGoals(context, logger);
        }

        context.SaveChanges();
        logger.LogInformation("Database seeding completed successfully");
    }


    // --- PRODUCTION SEEDERS ---

    private static void SeedRoles(AppDbContext context, ILogger logger)
    {
        if (context.Users.Any(u => u.Role == "Admin")) return;
        logger.LogInformation("Seeding: Admin user");

        context.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@mutualfund.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        });
    }

    private static void SeedRiskLevels(AppDbContext context, ILogger logger)
    {
        if (context.RiskLevels.Any()) return;
        logger.LogInformation("Seeding: Risk Levels");

        context.RiskLevels.AddRange(
            new RiskLevel { Name = "Conservative", MinScore = 0, MaxScore = 25, Description = "Prefers capital preservation with low risk" },
            new RiskLevel { Name = "Moderate", MinScore = 26, MaxScore = 50, Description = "Balanced approach, moderate risk tolerance" },
            new RiskLevel { Name = "Aggressive", MinScore = 51, MaxScore = 75, Description = "Growth-oriented, higher risk tolerance" },
            new RiskLevel { Name = "Very Aggressive", MinScore = 76, MaxScore = 100, Description = "Maximum growth, highest risk appetite" }
        );
    }

    private static void SeedGoalTypes(AppDbContext context, ILogger logger)
    {
        if (context.GoalTypes.Any()) return;
        logger.LogInformation("Seeding: Goal Types");

        context.GoalTypes.AddRange(
            new GoalType { Name = "Wealth Creation", Description = "Long-term wealth building through compounding" },
            new GoalType { Name = "Retirement", Description = "Building a retirement corpus" },
            new GoalType { Name = "Tax Saving", Description = "Save taxes under Section 80C via ELSS" },
            new GoalType { Name = "Child Education", Description = "Saving for children's higher education" },
            new GoalType { Name = "Home Purchase", Description = "Saving for a house down payment" },
            new GoalType { Name = "Emergency Fund", Description = "Building a safety net for unexpected expenses" }
        );
    }

    private static void SeedFundCategories(AppDbContext context, ILogger logger)
    {
        if (context.FundCategories.Any()) return;
        logger.LogInformation("Seeding: Fund Categories");

        context.FundCategories.AddRange(
            new FundCategory { Name = "Equity", Description = "Funds investing primarily in stocks" },
            new FundCategory { Name = "Debt", Description = "Funds investing in bonds and fixed-income instruments" },
            new FundCategory { Name = "Hybrid", Description = "Funds investing in both equity and debt" },
            new FundCategory { Name = "Gold", Description = "Funds investing in gold ETFs or gold-related instruments" },
            new FundCategory { Name = "Liquid", Description = "Funds investing in very short-term money market instruments" },
            new FundCategory { Name = "International", Description = "Funds investing in international equity markets" }
        );
    }

    private static void SeedAssetClasses(AppDbContext context, ILogger logger)
    {
        if (context.AssetClasses.Any()) return;
        logger.LogInformation("Seeding: Asset Classes");

        context.AssetClasses.AddRange(
            // Equity sub-classes
            new AssetClass { Name = "Large Cap", Category = "Equity", Description = "Top 100 companies by market cap" },
            new AssetClass { Name = "Mid Cap", Category = "Equity", Description = "101st to 250th companies by market cap" },
            new AssetClass { Name = "Small Cap", Category = "Equity", Description = "251st and below companies by market cap" },
            new AssetClass { Name = "Multi Cap", Category = "Equity", Description = "Invests across large, mid, and small cap" },
            // Debt sub-classes
            new AssetClass { Name = "Govt Securities", Category = "Debt", Description = "Government bonds and treasury bills" },
            new AssetClass { Name = "Corporate Bonds", Category = "Debt", Description = "Bonds issued by corporations" },
            new AssetClass { Name = "Short Duration", Category = "Debt", Description = "Debt instruments with 1-3 year maturity" },
            // Hybrid
            new AssetClass { Name = "Balanced Advantage", Category = "Hybrid", Description = "Dynamic allocation between equity and debt" },
            new AssetClass { Name = "Aggressive Hybrid", Category = "Hybrid", Description = "65-80% equity with rest in debt" },
            // Gold
            new AssetClass { Name = "Gold ETF", Category = "Gold", Description = "Exchange-traded fund tracking gold price" },
            new AssetClass { Name = "Sovereign Gold Bond", Category = "Gold", Description = "Government-backed gold bonds" },
            // Liquid
            new AssetClass { Name = "Liquid Fund", Category = "Liquid", Description = "Very short-term instruments, high liquidity" },
            new AssetClass { Name = "Overnight Fund", Category = "Liquid", Description = "Matures in 1 day, lowest risk" },
            // International
            new AssetClass { Name = "International Equity", Category = "International", Description = "Invests in foreign stock markets" }
        );
    }


    private static void SeedInvestmentTypes(AppDbContext context, ILogger logger)
    {
        if (context.InvestmentTypes.Any()) return;
        logger.LogInformation("Seeding: Investment Types");

        context.InvestmentTypes.AddRange(
            new InvestmentType { Name = "SIP", Description = "Systematic Investment Plan - fixed monthly investment" },
            new InvestmentType { Name = "Lump Sum", Description = "One-time investment of a large amount" },
            new InvestmentType { Name = "STP", Description = "Systematic Transfer Plan - transfer from one fund to another" },
            new InvestmentType { Name = "SWP", Description = "Systematic Withdrawal Plan - regular withdrawals from fund" }
        );
    }

    private static void SeedStressScenarios(AppDbContext context, ILogger logger)
    {
        if (context.StressScenarios.Any()) return;
        logger.LogInformation("Seeding: Stress Scenarios");

        context.StressScenarios.AddRange(
            new StressScenario { Name = "10% Market Decline", PercentageChange = -10, Description = "Minor market correction" },
            new StressScenario { Name = "20% Market Decline", PercentageChange = -20, Description = "Significant market correction" },
            new StressScenario { Name = "30% Market Decline", PercentageChange = -30, Description = "Major market crash" },
            new StressScenario { Name = "Bull Market (+20%)", PercentageChange = 20, Description = "Strong market rally" },
            new StressScenario { Name = "Financial Crisis (-50%)", PercentageChange = -50, Description = "2008-style market crash" }
        );
    }

    private static void SeedRiskQuestions(AppDbContext context, ILogger logger)
    {
        if (context.RiskQuestions.Any()) return;
        logger.LogInformation("Seeding: Risk Questions (15 questions with options)");

        var questions = new List<RiskQuestion>
        {
            new() { QuestionText = "What is your primary investment goal?", OrderNumber = 1, Options = new List<RiskOption> {
                new() { OptionText = "Preserve my capital with minimal risk", Score = 1 },
                new() { OptionText = "Generate steady income with low risk", Score = 2 },
                new() { OptionText = "Grow my wealth with moderate risk", Score = 3 },
                new() { OptionText = "Maximize returns even if it means high risk", Score = 4 }
            }},
            new() { QuestionText = "How long do you plan to keep your money invested?", OrderNumber = 2, Options = new List<RiskOption> {
                new() { OptionText = "Less than 1 year", Score = 1 },
                new() { OptionText = "1 to 3 years", Score = 2 },
                new() { OptionText = "3 to 7 years", Score = 3 },
                new() { OptionText = "More than 7 years", Score = 4 }
            }},
            new() { QuestionText = "If your investment drops 20% in a month, what would you do?", OrderNumber = 3, Options = new List<RiskOption> {
                new() { OptionText = "Sell everything immediately", Score = 1 },
                new() { OptionText = "Sell some and hold the rest", Score = 2 },
                new() { OptionText = "Hold and wait for recovery", Score = 3 },
                new() { OptionText = "Buy more at the lower price", Score = 4 }
            }},
            new() { QuestionText = "What percentage of your monthly income can you invest?", OrderNumber = 4, Options = new List<RiskOption> {
                new() { OptionText = "Less than 10%", Score = 1 },
                new() { OptionText = "10% to 20%", Score = 2 },
                new() { OptionText = "20% to 40%", Score = 3 },
                new() { OptionText = "More than 40%", Score = 4 }
            }},
            new() { QuestionText = "How would you describe your investment experience?", OrderNumber = 5, Options = new List<RiskOption> {
                new() { OptionText = "No experience at all", Score = 1 },
                new() { OptionText = "Basic knowledge, invested in FDs/RDs", Score = 2 },
                new() { OptionText = "Some experience with mutual funds/stocks", Score = 3 },
                new() { OptionText = "Experienced investor, comfortable with markets", Score = 4 }
            }},
            new() { QuestionText = "How stable is your current income source?", OrderNumber = 6, Options = new List<RiskOption> {
                new() { OptionText = "Unstable / freelance / irregular", Score = 1 },
                new() { OptionText = "Somewhat stable but uncertain", Score = 2 },
                new() { OptionText = "Stable salaried job", Score = 3 },
                new() { OptionText = "Very stable with multiple income sources", Score = 4 }
            }},
            new() { QuestionText = "Do you have an emergency fund that covers 6 months of expenses?", OrderNumber = 7, Options = new List<RiskOption> {
                new() { OptionText = "No emergency fund", Score = 1 },
                new() { OptionText = "Covers 1-3 months", Score = 2 },
                new() { OptionText = "Covers 3-6 months", Score = 3 },
                new() { OptionText = "Covers more than 6 months", Score = 4 }
            }},
            new() { QuestionText = "What is your age group?", OrderNumber = 8, Options = new List<RiskOption> {
                new() { OptionText = "Above 55 years", Score = 1 },
                new() { OptionText = "45 to 55 years", Score = 2 },
                new() { OptionText = "30 to 45 years", Score = 3 },
                new() { OptionText = "Below 30 years", Score = 4 }
            }},
            new() { QuestionText = "How much loan/debt do you currently have relative to your income?", OrderNumber = 9, Options = new List<RiskOption> {
                new() { OptionText = "EMIs take more than 50% of my income", Score = 1 },
                new() { OptionText = "EMIs take 30-50% of my income", Score = 2 },
                new() { OptionText = "EMIs take less than 30% of my income", Score = 3 },
                new() { OptionText = "No loans or debt", Score = 4 }
            }},
            new() { QuestionText = "Which statement best describes your risk attitude?", OrderNumber = 10, Options = new List<RiskOption> {
                new() { OptionText = "I cannot tolerate any loss in my investment", Score = 1 },
                new() { OptionText = "I can tolerate small losses for slightly better returns", Score = 2 },
                new() { OptionText = "I can tolerate moderate losses for higher growth", Score = 3 },
                new() { OptionText = "I am comfortable with significant losses for maximum growth", Score = 4 }
            }},
            new() { QuestionText = "What would you prefer: guaranteed 8% return or a chance of 15% return with risk of -5%?", OrderNumber = 11, Options = new List<RiskOption> {
                new() { OptionText = "Guaranteed 8% always", Score = 1 },
                new() { OptionText = "Mostly guaranteed with small portion in risky", Score = 2 },
                new() { OptionText = "Split equally between both", Score = 3 },
                new() { OptionText = "Go for 15% chance, I can handle the risk", Score = 4 }
            }},
            new() { QuestionText = "How often do you check your investments?", OrderNumber = 12, Options = new List<RiskOption> {
                new() { OptionText = "Daily, and market drops stress me out", Score = 1 },
                new() { OptionText = "Weekly, I like to stay updated", Score = 2 },
                new() { OptionText = "Monthly, I trust the long-term process", Score = 3 },
                new() { OptionText = "Rarely, I set it and forget it", Score = 4 }
            }},
            new() { QuestionText = "What is your financial dependency situation?", OrderNumber = 13, Options = new List<RiskOption> {
                new() { OptionText = "Many people depend on my income", Score = 1 },
                new() { OptionText = "A few dependents (spouse/children)", Score = 2 },
                new() { OptionText = "Only myself to support", Score = 3 },
                new() { OptionText = "No dependents and dual income household", Score = 4 }
            }},
            new() { QuestionText = "If a friend recommends a high-risk high-return investment, how would you react?", OrderNumber = 14, Options = new List<RiskOption> {
                new() { OptionText = "Ignore it completely, too risky", Score = 1 },
                new() { OptionText = "Research it but probably won't invest", Score = 2 },
                new() { OptionText = "Invest a small amount to test", Score = 3 },
                new() { OptionText = "Invest a significant amount if research supports it", Score = 4 }
            }},
            new() { QuestionText = "What best describes your current financial situation?", OrderNumber = 15, Options = new List<RiskOption> {
                new() { OptionText = "Living paycheck to paycheck", Score = 1 },
                new() { OptionText = "Comfortable but with limited savings", Score = 2 },
                new() { OptionText = "Good savings and financially stable", Score = 3 },
                new() { OptionText = "Financially secure with surplus income", Score = 4 }
            }}
        };

        context.RiskQuestions.AddRange(questions);
    }


    private static void SeedAllocationRules(AppDbContext context, ILogger logger)
    {
        if (context.AllocationRules.Any()) return;
        logger.LogInformation("Seeding: Allocation Rules (4 profiles x 6 asset classes)");

        var rules = new List<AllocationRule>
        {
            new() { RiskProfile = "Conservative", AssetClass = "Equity", Percentage = 20 },
            new() { RiskProfile = "Conservative", AssetClass = "Debt", Percentage = 50 },
            new() { RiskProfile = "Conservative", AssetClass = "Hybrid", Percentage = 15 },
            new() { RiskProfile = "Conservative", AssetClass = "Gold", Percentage = 10 },
            new() { RiskProfile = "Conservative", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Conservative", AssetClass = "International", Percentage = 0 },
            new() { RiskProfile = "Moderate", AssetClass = "Equity", Percentage = 40 },
            new() { RiskProfile = "Moderate", AssetClass = "Debt", Percentage = 30 },
            new() { RiskProfile = "Moderate", AssetClass = "Hybrid", Percentage = 15 },
            new() { RiskProfile = "Moderate", AssetClass = "Gold", Percentage = 10 },
            new() { RiskProfile = "Moderate", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Moderate", AssetClass = "International", Percentage = 0 },
            new() { RiskProfile = "Aggressive", AssetClass = "Equity", Percentage = 60 },
            new() { RiskProfile = "Aggressive", AssetClass = "Debt", Percentage = 15 },
            new() { RiskProfile = "Aggressive", AssetClass = "Hybrid", Percentage = 10 },
            new() { RiskProfile = "Aggressive", AssetClass = "Gold", Percentage = 5 },
            new() { RiskProfile = "Aggressive", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Aggressive", AssetClass = "International", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Equity", Percentage = 80 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Debt", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Hybrid", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Gold", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Liquid", Percentage = 0 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "International", Percentage = 5 },
        };

        context.AllocationRules.AddRange(rules);
    }

    private static void SeedMutualFunds(AppDbContext context, ILogger logger)
    {
        if (context.MutualFunds.Any()) return;
        logger.LogInformation("Seeding: Mutual Fund master data (33 funds across all categories)");

        var funds = new List<MutualFund>
        {
            new() { Name = "SBI Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "SBI", CAGR1Y = 18.5m, CAGR3Y = 14.2m, CAGR5Y = 12.8m, ExpenseRatio = 0.85m, AUM = 45000, FundManager = "Sohini Andani", Rating = 4.5m, SharpeRatio = 1.2m, Alpha = 2.1m, Beta = 0.92m, StandardDeviation = 14.5m, ExitLoad = 1.0m, Benchmark = "Nifty 100", RollingReturns3Y = 13.8m },
            new() { Name = "ICICI Prudential Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "ICICI", CAGR1Y = 17.8m, CAGR3Y = 13.9m, CAGR5Y = 12.5m, ExpenseRatio = 0.90m, AUM = 38000, FundManager = "Rajat Chandak", Rating = 4, SharpeRatio = 1.1m, Alpha = 1.5m, Beta = 0.95m, StandardDeviation = 15.2m, ExitLoad = 1.0m, Benchmark = "Nifty 100", RollingReturns3Y = 13.2m },
            new() { Name = "Mirae Asset Large Cap Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "Mirae Asset", CAGR1Y = 19.2m, CAGR3Y = 15.1m, CAGR5Y = 13.4m, ExpenseRatio = 0.52m, AUM = 35000, FundManager = "Gaurav Misra", Rating = 5, SharpeRatio = 1.4m, Alpha = 3.2m, Beta = 0.88m, StandardDeviation = 13.8m, ExitLoad = 1.0m, Benchmark = "Nifty 100", RollingReturns3Y = 14.5m },
            new() { Name = "Kotak Emerging Equity Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "Kotak", CAGR1Y = 25.3m, CAGR3Y = 20.1m, CAGR5Y = 16.8m, ExpenseRatio = 0.75m, AUM = 28000, FundManager = "Pankaj Tibrewal", Rating = 4.5m, SharpeRatio = 1.3m, Alpha = 4.5m, Beta = 1.05m, StandardDeviation = 18.2m, ExitLoad = 1.0m, Benchmark = "Nifty Midcap 150", RollingReturns3Y = 19.2m },
            new() { Name = "HDFC Mid-Cap Opportunities Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "HDFC", CAGR1Y = 24.1m, CAGR3Y = 19.5m, CAGR5Y = 15.9m, ExpenseRatio = 0.82m, AUM = 42000, FundManager = "Chirag Setalvad", Rating = 4, SharpeRatio = 1.2m, Alpha = 3.8m, Beta = 1.02m, StandardDeviation = 17.5m, ExitLoad = 1.0m, Benchmark = "Nifty Midcap 150", RollingReturns3Y = 18.5m },
            new() { Name = "Nippon India Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "Nippon", CAGR1Y = 30.2m, CAGR3Y = 28.5m, CAGR5Y = 22.1m, ExpenseRatio = 0.88m, AUM = 32000, FundManager = "Samir Rachh", Rating = 4.5m, SharpeRatio = 1.5m, Alpha = 6.2m, Beta = 1.15m, StandardDeviation = 22.0m, ExitLoad = 1.0m, Benchmark = "Nifty Smallcap 250", RollingReturns3Y = 27.0m },
            new() { Name = "SBI Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "SBI", CAGR1Y = 28.7m, CAGR3Y = 25.3m, CAGR5Y = 20.8m, ExpenseRatio = 0.72m, AUM = 18000, FundManager = "R. Srinivasan", Rating = 5, SharpeRatio = 1.4m, Alpha = 5.8m, Beta = 1.10m, StandardDeviation = 20.5m, ExitLoad = 1.0m, Benchmark = "Nifty Smallcap 250", RollingReturns3Y = 24.5m },
            new() { Name = "HDFC Short Term Debt Fund", Category = "Debt", SubCategory = "Short Duration", AMC = "HDFC", CAGR1Y = 7.2m, CAGR3Y = 6.8m, CAGR5Y = 7.1m, ExpenseRatio = 0.35m, AUM = 15000, FundManager = "Anil Bamboli", Rating = 4, SharpeRatio = 0.8m, Alpha = 0.5m, Beta = 0.15m, StandardDeviation = 2.1m, ExitLoad = 0, Benchmark = "CRISIL Short Term Bond", RollingReturns3Y = 6.5m },
            new() { Name = "ICICI Prudential All Seasons Bond Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "ICICI", CAGR1Y = 7.8m, CAGR3Y = 7.1m, CAGR5Y = 7.5m, ExpenseRatio = 0.42m, AUM = 12000, FundManager = "Manish Banthia", Rating = 4.5m, SharpeRatio = 0.9m, Alpha = 0.8m, Beta = 0.12m, StandardDeviation = 1.8m, ExitLoad = 0, Benchmark = "CRISIL Composite Bond", RollingReturns3Y = 6.8m },
            new() { Name = "SBI Magnum Gilt Fund", Category = "Debt", SubCategory = "Govt Securities", AMC = "SBI", CAGR1Y = 8.1m, CAGR3Y = 6.5m, CAGR5Y = 7.8m, ExpenseRatio = 0.48m, AUM = 8000, FundManager = "Dinesh Ahuja", Rating = 4, SharpeRatio = 0.7m, Alpha = 0.3m, Beta = 0.10m, StandardDeviation = 3.5m, ExitLoad = 0, Benchmark = "CRISIL 10yr Gilt", RollingReturns3Y = 6.2m },
            new() { Name = "ICICI Prudential Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "ICICI", CAGR1Y = 12.5m, CAGR3Y = 10.8m, CAGR5Y = 11.2m, ExpenseRatio = 0.95m, AUM = 52000, FundManager = "Sankaran Naren", Rating = 4.5m, SharpeRatio = 1.1m, Alpha = 2.0m, Beta = 0.65m, StandardDeviation = 10.2m, ExitLoad = 1.0m, Benchmark = "CRISIL Hybrid 35+65", RollingReturns3Y = 10.5m },
            new() { Name = "HDFC Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "HDFC", CAGR1Y = 13.1m, CAGR3Y = 11.2m, CAGR5Y = 11.8m, ExpenseRatio = 0.88m, AUM = 62000, FundManager = "Gopal Agrawal", Rating = 4, SharpeRatio = 1.0m, Alpha = 1.8m, Beta = 0.70m, StandardDeviation = 11.0m, ExitLoad = 1.0m, Benchmark = "CRISIL Hybrid 35+65", RollingReturns3Y = 10.8m },
            new() { Name = "SBI Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "SBI", CAGR1Y = 15.2m, CAGR3Y = 12.8m, CAGR5Y = 11.5m, ExpenseRatio = 0.50m, AUM = 2500, FundManager = "Raviprakash Sharma", Rating = 4, SharpeRatio = 0.6m, Alpha = 0, Beta = 0.05m, StandardDeviation = 12.0m, ExitLoad = 0, Benchmark = "Gold Price (MCX)", RollingReturns3Y = 12.0m },
            new() { Name = "HDFC Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "HDFC", CAGR1Y = 14.8m, CAGR3Y = 12.5m, CAGR5Y = 11.2m, ExpenseRatio = 0.45m, AUM = 2000, FundManager = "Krishan Daga", Rating = 4, SharpeRatio = 0.6m, Alpha = 0, Beta = 0.05m, StandardDeviation = 11.8m, ExitLoad = 0, Benchmark = "Gold Price (MCX)", RollingReturns3Y = 11.8m },
            new() { Name = "HDFC Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "HDFC", CAGR1Y = 6.8m, CAGR3Y = 5.5m, CAGR5Y = 5.8m, ExpenseRatio = 0.20m, AUM = 55000, FundManager = "Anil Bamboli", Rating = 4.5m, SharpeRatio = 0, Alpha = 0, Beta = 0, StandardDeviation = 0.3m, ExitLoad = 0, Benchmark = "CRISIL Liquid Fund", RollingReturns3Y = 5.3m },
            new() { Name = "SBI Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "SBI", CAGR1Y = 6.7m, CAGR3Y = 5.4m, CAGR5Y = 5.7m, ExpenseRatio = 0.22m, AUM = 48000, FundManager = "R.P. Agrawal", Rating = 4, SharpeRatio = 0, Alpha = 0, Beta = 0, StandardDeviation = 0.3m, ExitLoad = 0, Benchmark = "CRISIL Liquid Fund", RollingReturns3Y = 5.2m },
            new() { Name = "Motilal Oswal Nasdaq 100 Fund", Category = "International", SubCategory = "International Equity", AMC = "Motilal Oswal", CAGR1Y = 22.5m, CAGR3Y = 18.2m, CAGR5Y = 20.1m, ExpenseRatio = 0.50m, AUM = 5000, FundManager = "Swapnil Mayekar", Rating = 4.5m, SharpeRatio = 1.3m, Alpha = 3.5m, Beta = 1.0m, StandardDeviation = 16.5m, ExitLoad = 1.0m, Benchmark = "Nasdaq 100", RollingReturns3Y = 17.5m },
            new() { Name = "Franklin India Feeder - US Opportunities Fund", Category = "International", SubCategory = "International Equity", AMC = "Franklin", CAGR1Y = 18.3m, CAGR3Y = 15.8m, CAGR5Y = 16.2m, ExpenseRatio = 0.55m, AUM = 3500, FundManager = "Grant Bowers", Rating = 4, SharpeRatio = 1.1m, Alpha = 2.0m, Beta = 0.95m, StandardDeviation = 15.0m, ExitLoad = 1.0m, Benchmark = "S&P 500", RollingReturns3Y = 15.0m },

            // Additional Debt Funds
            new() { Name = "Axis Banking & PSU Debt Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "Axis", CAGR1Y = 7.5m, CAGR3Y = 6.9m, CAGR5Y = 7.3m, ExpenseRatio = 0.32m, AUM = 18000, FundManager = "Devang Shah", Rating = 4.5m, SharpeRatio = 0.85m, Alpha = 0.6m, Beta = 0.11m, StandardDeviation = 1.5m, ExitLoad = 0, Benchmark = "CRISIL Banking PSU Bond", RollingReturns3Y = 6.7m },
            new() { Name = "Kotak Corporate Bond Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "Kotak", CAGR1Y = 7.1m, CAGR3Y = 6.6m, CAGR5Y = 7.0m, ExpenseRatio = 0.38m, AUM = 11000, FundManager = "Deepak Agrawal", Rating = 4, SharpeRatio = 0.75m, Alpha = 0.4m, Beta = 0.13m, StandardDeviation = 1.9m, ExitLoad = 0, Benchmark = "CRISIL Corporate Bond", RollingReturns3Y = 6.4m },
            new() { Name = "Aditya Birla Sun Life Corporate Bond Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "Aditya Birla", CAGR1Y = 7.6m, CAGR3Y = 7.0m, CAGR5Y = 7.4m, ExpenseRatio = 0.40m, AUM = 22000, FundManager = "Kaustubh Gupta", Rating = 4.5m, SharpeRatio = 0.88m, Alpha = 0.7m, Beta = 0.10m, StandardDeviation = 1.6m, ExitLoad = 0, Benchmark = "CRISIL Corporate Bond", RollingReturns3Y = 6.9m },

            // Additional Hybrid Funds
            new() { Name = "Canara Robeco Equity Hybrid Fund", Category = "Hybrid", SubCategory = "Aggressive Hybrid", AMC = "Canara Robeco", CAGR1Y = 14.2m, CAGR3Y = 12.5m, CAGR5Y = 12.8m, ExpenseRatio = 0.65m, AUM = 9500, FundManager = "Shridatta Bhandwaldar", Rating = 4.5m, SharpeRatio = 1.2m, Alpha = 2.5m, Beta = 0.72m, StandardDeviation = 10.8m, ExitLoad = 1.0m, Benchmark = "CRISIL Hybrid 35+65", RollingReturns3Y = 11.8m },
            new() { Name = "Kotak Equity Hybrid Fund", Category = "Hybrid", SubCategory = "Aggressive Hybrid", AMC = "Kotak", CAGR1Y = 13.5m, CAGR3Y = 11.8m, CAGR5Y = 12.1m, ExpenseRatio = 0.72m, AUM = 5200, FundManager = "Harsha Upadhyaya", Rating = 4, SharpeRatio = 1.05m, Alpha = 1.9m, Beta = 0.68m, StandardDeviation = 11.5m, ExitLoad = 1.0m, Benchmark = "CRISIL Hybrid 35+65", RollingReturns3Y = 11.2m },
            new() { Name = "Mirae Asset Hybrid Equity Fund", Category = "Hybrid", SubCategory = "Aggressive Hybrid", AMC = "Mirae Asset", CAGR1Y = 15.1m, CAGR3Y = 13.2m, CAGR5Y = 13.5m, ExpenseRatio = 0.55m, AUM = 7800, FundManager = "Gaurav Misra", Rating = 5, SharpeRatio = 1.3m, Alpha = 3.0m, Beta = 0.75m, StandardDeviation = 10.5m, ExitLoad = 1.0m, Benchmark = "CRISIL Hybrid 35+65", RollingReturns3Y = 12.5m },

            // Additional Gold Funds
            new() { Name = "Kotak Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "Kotak", CAGR1Y = 15.5m, CAGR3Y = 13.1m, CAGR5Y = 11.8m, ExpenseRatio = 0.42m, AUM = 1800, FundManager = "Abhishek Bisen", Rating = 4.5m, SharpeRatio = 0.65m, Alpha = 0.2m, Beta = 0.04m, StandardDeviation = 11.5m, ExitLoad = 0, Benchmark = "Gold Price (MCX)", RollingReturns3Y = 12.5m },
            new() { Name = "Nippon India Gold Savings Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "Nippon", CAGR1Y = 14.5m, CAGR3Y = 12.2m, CAGR5Y = 10.9m, ExpenseRatio = 0.48m, AUM = 1500, FundManager = "Mehul Dama", Rating = 4, SharpeRatio = 0.55m, Alpha = -0.1m, Beta = 0.05m, StandardDeviation = 12.2m, ExitLoad = 0, Benchmark = "Gold Price (MCX)", RollingReturns3Y = 11.5m },
            new() { Name = "Axis Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "Axis", CAGR1Y = 15.8m, CAGR3Y = 13.3m, CAGR5Y = 12.0m, ExpenseRatio = 0.44m, AUM = 1200, FundManager = "Ashish Naik", Rating = 4.5m, SharpeRatio = 0.68m, Alpha = 0.3m, Beta = 0.04m, StandardDeviation = 11.2m, ExitLoad = 0, Benchmark = "Gold Price (MCX)", RollingReturns3Y = 12.8m },

            // Additional Liquid Funds
            new() { Name = "ICICI Prudential Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "ICICI", CAGR1Y = 6.9m, CAGR3Y = 5.6m, CAGR5Y = 5.9m, ExpenseRatio = 0.18m, AUM = 42000, FundManager = "Rahul Goswami", Rating = 4.5m, SharpeRatio = 0, Alpha = 0.1m, Beta = 0, StandardDeviation = 0.2m, ExitLoad = 0, Benchmark = "CRISIL Liquid Fund", RollingReturns3Y = 5.5m },
            new() { Name = "Axis Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "Axis", CAGR1Y = 6.8m, CAGR3Y = 5.5m, CAGR5Y = 5.8m, ExpenseRatio = 0.15m, AUM = 35000, FundManager = "Devang Shah", Rating = 4.5m, SharpeRatio = 0, Alpha = 0, Beta = 0, StandardDeviation = 0.2m, ExitLoad = 0, Benchmark = "CRISIL Liquid Fund", RollingReturns3Y = 5.4m },
            new() { Name = "Kotak Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "Kotak", CAGR1Y = 6.7m, CAGR3Y = 5.4m, CAGR5Y = 5.7m, ExpenseRatio = 0.19m, AUM = 38000, FundManager = "Deepak Agrawal", Rating = 4, SharpeRatio = 0, Alpha = 0, Beta = 0, StandardDeviation = 0.3m, ExitLoad = 0, Benchmark = "CRISIL Liquid Fund", RollingReturns3Y = 5.3m },

            // Additional International Funds
            new() { Name = "ICICI Prudential US Bluechip Equity Fund", Category = "International", SubCategory = "International Equity", AMC = "ICICI", CAGR1Y = 20.1m, CAGR3Y = 16.5m, CAGR5Y = 17.8m, ExpenseRatio = 0.60m, AUM = 3200, FundManager = "Sankaran Naren", Rating = 4, SharpeRatio = 1.2m, Alpha = 2.5m, Beta = 0.92m, StandardDeviation = 14.8m, ExitLoad = 1.0m, Benchmark = "S&P 500", RollingReturns3Y = 15.8m },
            new() { Name = "DSP Global Innovation Fund", Category = "International", SubCategory = "International Equity", AMC = "DSP", CAGR1Y = 24.5m, CAGR3Y = 19.8m, CAGR5Y = 21.5m, ExpenseRatio = 0.65m, AUM = 2100, FundManager = "Jay Kothari", Rating = 4.5m, SharpeRatio = 1.4m, Alpha = 4.0m, Beta = 1.05m, StandardDeviation = 17.2m, ExitLoad = 1.0m, Benchmark = "MSCI World", RollingReturns3Y = 18.5m },
            new() { Name = "Kotak International REIT Fund", Category = "International", SubCategory = "International Equity", AMC = "Kotak", CAGR1Y = 16.8m, CAGR3Y = 14.2m, CAGR5Y = 15.0m, ExpenseRatio = 0.58m, AUM = 1800, FundManager = "Abhishek Bisen", Rating = 4, SharpeRatio = 1.0m, Alpha = 1.5m, Beta = 0.85m, StandardDeviation = 13.5m, ExitLoad = 1.0m, Benchmark = "FTSE NAREIT", RollingReturns3Y = 13.8m }
        };

        context.MutualFunds.AddRange(funds);
    }


    // --- DEV/TEST SEEDERS (Only run in Development environment) ---

    private static void SeedDemoUsers(AppDbContext context, ILogger logger)
    {
        // Seed Rohit demo account (check independently)
        if (!context.Users.Any(u => u.Email == "rohit@wealthai.com"))
        {
            logger.LogInformation("Seeding: Rohit demo account");
            context.Users.Add(new User { Name = "Rohit Boyinapalli", Email = "rohit@wealthai.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rohit@123"), Role = "User" });
            context.SaveChanges();
        }

        // Always ensure Rohit has a profile (even if user was registered manually)
        var rohit = context.Users.First(u => u.Email == "rohit@wealthai.com");
        if (!context.UserProfiles.Any(p => p.UserId == rohit.Id))
        {
            logger.LogInformation("Seeding: Profile for Rohit");
            context.UserProfiles.Add(new UserProfile
            {
                UserId = rohit.Id,
                Age = 24,
                Occupation = "Software Developer",
                Location = "Hyderabad",
                MaritalStatus = "Single",
                Dependents = 0,
                MonthlyIncome = 120000,
                MonthlyExpenses = 45000,
                Savings = 800000,
                Loans = 0,
                ExistingInvestments = "Mutual Funds",
                InvestmentType = "SIP",
                SIPAmount = 50000,
                SIPFrequency = "Monthly",
                SIPDate = 1,
                LumpSumAmount = 0,
                HasSWP = true,
                SWPAmount = 10000,
                DurationInYears = 10,
                Goals = "Wealth Creation,Retirement,Tax Saving,Emergency Fund"
            });
            context.SaveChanges();
        }

        // Seed other demo accounts
        if (!context.Users.Any(u => u.Email == "demo@test.com"))
        {
            logger.LogInformation("Seeding: Demo Users (Development only)");

            var demoUsers = new List<User>
            {
                new() { Name = "Demo User", Email = "demo@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"), Role = "User" },
                new() { Name = "Rahul Sharma", Email = "rahul@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), Role = "User" },
                new() { Name = "Priya Patel", Email = "priya@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), Role = "User" }
            };

            context.Users.AddRange(demoUsers);
            context.SaveChanges();

            // Profile for demo user
            var demo = context.Users.First(u => u.Email == "demo@test.com");
            context.UserProfiles.Add(new UserProfile
            {
                UserId = demo.Id,
                Age = 28,
                Occupation = "Software Engineer",
                Location = "Bangalore",
                MaritalStatus = "Single",
                Dependents = 0,
                MonthlyIncome = 80000,
                MonthlyExpenses = 35000,
                Savings = 500000,
                Loans = 0,
                ExistingInvestments = "FD/RD",
                SIPAmount = 10000,
                DurationInYears = 10,
                Goals = "Wealth Creation,Retirement,Tax Saving"
            });

            var rahul = context.Users.First(u => u.Email == "rahul@test.com");
            context.UserProfiles.Add(new UserProfile
            {
                UserId = rahul.Id,
                Age = 45,
                Occupation = "Business Owner",
                Location = "Mumbai",
                MaritalStatus = "Married",
                Dependents = 2,
                MonthlyIncome = 200000,
                MonthlyExpenses = 80000,
                Savings = 2000000,
                Loans = 50000,
                ExistingInvestments = "Mutual Funds",
                SIPAmount = 50000,
                DurationInYears = 15,
                Goals = "Child Education,Retirement,Wealth Creation"
            });
        }
    }

    private static void SeedSamplePortfolios(AppDbContext context, ILogger logger)
    {
        var demo = context.Users.FirstOrDefault(u => u.Email == "demo@test.com");
        if (demo == null) return;
        if (context.Portfolios.Any(p => p.UserId == demo.Id)) return;
        logger.LogInformation("Seeding: Sample Portfolios (Development only)");

        var portfolio = new Portfolio { UserId = demo.Id, Name = "My Portfolio" };
        context.Portfolios.Add(portfolio);
        context.SaveChanges();

        var sbi = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("SBI Bluechip"));
        var hdfc = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("HDFC Mid-Cap"));
        var icici = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("ICICI Prudential Balanced"));

        var holdings = new List<PortfolioHolding>();

        if (sbi != null)
            holdings.Add(new PortfolioHolding { PortfolioId = portfolio.Id, MutualFundId = sbi.Id, FundName = sbi.Name, Units = 500, PurchaseNAV = 42.5m, InvestedAmount = 21250, PurchaseDate = DateTime.UtcNow.AddMonths(-12) });
        if (hdfc != null)
            holdings.Add(new PortfolioHolding { PortfolioId = portfolio.Id, MutualFundId = hdfc.Id, FundName = hdfc.Name, Units = 300, PurchaseNAV = 55.2m, InvestedAmount = 16560, PurchaseDate = DateTime.UtcNow.AddMonths(-8) });
        if (icici != null)
            holdings.Add(new PortfolioHolding { PortfolioId = portfolio.Id, MutualFundId = icici.Id, FundName = icici.Name, Units = 400, PurchaseNAV = 38.0m, InvestedAmount = 15200, PurchaseDate = DateTime.UtcNow.AddMonths(-6) });

        context.PortfolioHoldings.AddRange(holdings);
    }

    private static void SeedTestRecommendations(AppDbContext context, ILogger logger)
    {
        var demo = context.Users.FirstOrDefault(u => u.Email == "demo@test.com");
        if (demo == null) return;
        if (context.RiskAssessments.Any(a => a.UserId == demo.Id)) return;
        logger.LogInformation("Seeding: Test Recommendations (Development only)");

        // Demo user assessment
        var assessment = new RiskAssessment
        {
            UserId = demo.Id,
            TotalScore = 62,
            RiskProfile = "Aggressive",
            CompletedAt = DateTime.UtcNow.AddDays(-7)
        };
        context.RiskAssessments.Add(assessment);
        context.SaveChanges();

        var recommendation = new Recommendation
        {
            UserId = demo.Id,
            RiskAssessmentId = assessment.Id,
            RiskProfile = "Aggressive",
            GeneratedAt = DateTime.UtcNow.AddDays(-7),
            AIExplanation = "Based on your risk assessment, you have an aggressive risk profile. Your allocation is equity-heavy for maximum growth potential with some diversification into debt and international markets."
        };
        context.Recommendations.Add(recommendation);
        context.SaveChanges();

        context.RecommendationAllocations.AddRange(
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "Equity", Percentage = 60, SuggestedFunds = "SBI Bluechip Fund, Kotak Emerging Equity Fund" },
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "Debt", Percentage = 15, SuggestedFunds = "HDFC Short Term Debt Fund" },
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "Hybrid", Percentage = 10, SuggestedFunds = "ICICI Prudential Balanced Advantage Fund" },
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "Gold", Percentage = 5, SuggestedFunds = "SBI Gold Fund" },
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "Liquid", Percentage = 5, SuggestedFunds = "HDFC Liquid Fund" },
            new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = "International", Percentage = 5, SuggestedFunds = "Motilal Oswal Nasdaq 100 Fund" }
        );

        // Rohit's assessment & recommendation
        var rohit = context.Users.FirstOrDefault(u => u.Email == "rohit@wealthai.com");
        if (rohit != null && !context.RiskAssessments.Any(a => a.UserId == rohit.Id))
        {
            var rohitAssessment = new RiskAssessment
            {
                UserId = rohit.Id,
                TotalScore = 78,
                RiskProfile = "Very Aggressive",
                CompletedAt = DateTime.UtcNow.AddDays(-3)
            };
            context.RiskAssessments.Add(rohitAssessment);
            context.SaveChanges();

            var rohitRecommendation = new Recommendation
            {
                UserId = rohit.Id,
                RiskAssessmentId = rohitAssessment.Id,
                RiskProfile = "Very Aggressive",
                GeneratedAt = DateTime.UtcNow.AddDays(-3),
                AIExplanation = "You have a high risk tolerance and a growth-focused approach. The allocation maximizes equity exposure across large, mid, and small-cap funds for maximum growth potential. International equity adds geographical diversification. This portfolio may experience significant short-term volatility but is positioned for strong long-term returns."
            };
            context.Recommendations.Add(rohitRecommendation);
            context.SaveChanges();

            context.RecommendationAllocations.AddRange(
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "Equity", Percentage = 80, SuggestedFunds = "Mirae Asset Large Cap Fund, Kotak Emerging Equity Fund, Nippon India Small Cap Fund" },
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "Debt", Percentage = 5, SuggestedFunds = "Axis Banking & PSU Debt Fund" },
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "Hybrid", Percentage = 5, SuggestedFunds = "Mirae Asset Hybrid Equity Fund" },
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "Gold", Percentage = 5, SuggestedFunds = "Kotak Gold Fund" },
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "Liquid", Percentage = 0, SuggestedFunds = "" },
                new RecommendationAllocation { RecommendationId = rohitRecommendation.Id, AssetClass = "International", Percentage = 5, SuggestedFunds = "Motilal Oswal Nasdaq 100 Fund" }
            );
        }
    }

    private static void SeedFundHoldings(AppDbContext context, ILogger logger)
    {
        if (context.FundHoldings.Any()) return;
        logger.LogInformation("Seeding: Fund Holdings (top stocks per fund)");

        var sbi = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("SBI Bluechip"));
        if (sbi == null) return;

        context.FundHoldings.AddRange(
            new FundHolding { MutualFundId = sbi.Id, StockName = "HDFC Bank", Sector = "Banking", Percentage = 9.5m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "Reliance Industries", Sector = "Oil & Gas", Percentage = 8.2m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "Infosys", Sector = "IT", Percentage = 7.8m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "ICICI Bank", Sector = "Banking", Percentage = 7.1m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "TCS", Sector = "IT", Percentage = 6.5m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "Bharti Airtel", Sector = "Telecom", Percentage = 5.2m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "ITC", Sector = "FMCG", Percentage = 4.8m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "L&T", Sector = "Engineering", Percentage = 4.5m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "Kotak Mahindra Bank", Sector = "Banking", Percentage = 4.0m },
            new FundHolding { MutualFundId = sbi.Id, StockName = "Axis Bank", Sector = "Banking", Percentage = 3.8m }
        );

        var kotak = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("Kotak Emerging"));
        if (kotak != null)
        {
            context.FundHoldings.AddRange(
                new FundHolding { MutualFundId = kotak.Id, StockName = "Persistent Systems", Sector = "IT", Percentage = 5.5m },
                new FundHolding { MutualFundId = kotak.Id, StockName = "Coforge", Sector = "IT", Percentage = 4.8m },
                new FundHolding { MutualFundId = kotak.Id, StockName = "Supreme Industries", Sector = "Manufacturing", Percentage = 4.2m },
                new FundHolding { MutualFundId = kotak.Id, StockName = "The Phoenix Mills", Sector = "Real Estate", Percentage = 3.9m },
                new FundHolding { MutualFundId = kotak.Id, StockName = "Sundaram Finance", Sector = "Finance", Percentage = 3.5m }
            );
        }
    }

    private static void SeedNAVHistory(AppDbContext context, ILogger logger)
    {
        if (context.FundNAVHistory.Any()) return;
        logger.LogInformation("Seeding: NAV History (last 12 months for SBI Bluechip)");

        var sbi = context.MutualFunds.FirstOrDefault(f => f.Name.Contains("SBI Bluechip"));
        if (sbi == null) return;

        var baseNAV = 65.0m;
        var today = DateTime.UtcNow;

        for (int i = 12; i >= 0; i--)
        {
            var date = today.AddMonths(-i);
            var variation = (decimal)(new Random(i * 7).NextDouble() * 8 - 2); // -2% to +6%
            var nav = baseNAV + (baseNAV * variation / 100) + (i * 0.5m);

            context.FundNAVHistory.Add(new FundNAVHistory
            {
                MutualFundId = sbi.Id,
                NAV = Math.Round(nav, 2),
                Date = new DateTime(date.Year, date.Month, 1)
            });
        }
    }

    private static void SeedGoals(AppDbContext context, ILogger logger)
    {
        // Goals for Rohit (main demo account) — varied SIP amounts, realistic progress
        var rohit = context.Users.FirstOrDefault(u => u.Email == "rohit@wealthai.com");
        if (rohit != null && !context.Goals.Any(g => g.UserId == rohit.Id))
        {
            logger.LogInformation("Seeding: Goals for Rohit (Development only)");
            context.Goals.AddRange(
                new Goal { UserId = rohit.Id, Name = "Wealth Creation", TargetAmount = 5000000, CurrentAmount = 900000, TargetYears = 10, MonthlySIP = 25000 },    // 18%
                new Goal { UserId = rohit.Id, Name = "Retirement", TargetAmount = 10000000, CurrentAmount = 1200000, TargetYears = 30, MonthlySIP = 15000 },        // 12%
                new Goal { UserId = rohit.Id, Name = "Tax Saving", TargetAmount = 150000, CurrentAmount = 52000, TargetYears = 1, MonthlySIP = 12500 },             // 35%
                new Goal { UserId = rohit.Id, Name = "Emergency Fund", TargetAmount = 500000, CurrentAmount = 200000, TargetYears = 2, MonthlySIP = 10000 }         // 40%
            );
            context.SaveChanges();
        }

        // Goals for demo user
        var demo = context.Users.FirstOrDefault(u => u.Email == "demo@test.com");
        if (demo != null && !context.Goals.Any(g => g.UserId == demo.Id))
        {
            logger.LogInformation("Seeding: Goals for Demo User (Development only)");
            context.Goals.AddRange(
                new Goal { UserId = demo.Id, Name = "Retirement", TargetAmount = 5000000, CurrentAmount = 750000, TargetYears = 25, MonthlySIP = 10000 },
                new Goal { UserId = demo.Id, Name = "Wealth Creation", TargetAmount = 2000000, CurrentAmount = 350000, TargetYears = 10, MonthlySIP = 15000 },
                new Goal { UserId = demo.Id, Name = "Tax Saving", TargetAmount = 150000, CurrentAmount = 100000, TargetYears = 1, MonthlySIP = 12500 }
            );
            context.SaveChanges();
        }
    }
}
