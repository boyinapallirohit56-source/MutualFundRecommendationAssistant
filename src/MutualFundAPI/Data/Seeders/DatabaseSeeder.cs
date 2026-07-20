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
        logger.LogInformation("Seeding: Mutual Fund master data (18 funds across all categories)");

        var funds = new List<MutualFund>
        {
            new() { Name = "SBI Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "SBI", CAGR1Y = 18.5m, CAGR3Y = 14.2m, CAGR5Y = 12.8m, ExpenseRatio = 0.85m, AUM = 45000, FundManager = "Sohini Andani", Rating = 4.5m },
            new() { Name = "ICICI Prudential Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "ICICI", CAGR1Y = 17.8m, CAGR3Y = 13.9m, CAGR5Y = 12.5m, ExpenseRatio = 0.90m, AUM = 38000, FundManager = "Rajat Chandak", Rating = 4 },
            new() { Name = "Mirae Asset Large Cap Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "Mirae Asset", CAGR1Y = 19.2m, CAGR3Y = 15.1m, CAGR5Y = 13.4m, ExpenseRatio = 0.52m, AUM = 35000, FundManager = "Gaurav Misra", Rating = 5 },
            new() { Name = "Kotak Emerging Equity Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "Kotak", CAGR1Y = 25.3m, CAGR3Y = 20.1m, CAGR5Y = 16.8m, ExpenseRatio = 0.75m, AUM = 28000, FundManager = "Pankaj Tibrewal", Rating = 4.5m },
            new() { Name = "HDFC Mid-Cap Opportunities Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "HDFC", CAGR1Y = 24.1m, CAGR3Y = 19.5m, CAGR5Y = 15.9m, ExpenseRatio = 0.82m, AUM = 42000, FundManager = "Chirag Setalvad", Rating = 4 },
            new() { Name = "Nippon India Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "Nippon", CAGR1Y = 30.2m, CAGR3Y = 28.5m, CAGR5Y = 22.1m, ExpenseRatio = 0.88m, AUM = 32000, FundManager = "Samir Rachh", Rating = 4.5m },
            new() { Name = "SBI Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "SBI", CAGR1Y = 28.7m, CAGR3Y = 25.3m, CAGR5Y = 20.8m, ExpenseRatio = 0.72m, AUM = 18000, FundManager = "R. Srinivasan", Rating = 5 },
            new() { Name = "HDFC Short Term Debt Fund", Category = "Debt", SubCategory = "Short Duration", AMC = "HDFC", CAGR1Y = 7.2m, CAGR3Y = 6.8m, CAGR5Y = 7.1m, ExpenseRatio = 0.35m, AUM = 15000, FundManager = "Anil Bamboli", Rating = 4 },
            new() { Name = "ICICI Prudential All Seasons Bond Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "ICICI", CAGR1Y = 7.8m, CAGR3Y = 7.1m, CAGR5Y = 7.5m, ExpenseRatio = 0.42m, AUM = 12000, FundManager = "Manish Banthia", Rating = 4.5m },
            new() { Name = "SBI Magnum Gilt Fund", Category = "Debt", SubCategory = "Govt Securities", AMC = "SBI", CAGR1Y = 8.1m, CAGR3Y = 6.5m, CAGR5Y = 7.8m, ExpenseRatio = 0.48m, AUM = 8000, FundManager = "Dinesh Ahuja", Rating = 4 },
            new() { Name = "ICICI Prudential Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "ICICI", CAGR1Y = 12.5m, CAGR3Y = 10.8m, CAGR5Y = 11.2m, ExpenseRatio = 0.95m, AUM = 52000, FundManager = "Sankaran Naren", Rating = 4.5m },
            new() { Name = "HDFC Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "HDFC", CAGR1Y = 13.1m, CAGR3Y = 11.2m, CAGR5Y = 11.8m, ExpenseRatio = 0.88m, AUM = 62000, FundManager = "Gopal Agrawal", Rating = 4 },
            new() { Name = "SBI Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "SBI", CAGR1Y = 15.2m, CAGR3Y = 12.8m, CAGR5Y = 11.5m, ExpenseRatio = 0.50m, AUM = 2500, FundManager = "Raviprakash Sharma", Rating = 4 },
            new() { Name = "HDFC Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "HDFC", CAGR1Y = 14.8m, CAGR3Y = 12.5m, CAGR5Y = 11.2m, ExpenseRatio = 0.45m, AUM = 2000, FundManager = "Krishan Daga", Rating = 4 },
            new() { Name = "HDFC Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "HDFC", CAGR1Y = 6.8m, CAGR3Y = 5.5m, CAGR5Y = 5.8m, ExpenseRatio = 0.20m, AUM = 55000, FundManager = "Anil Bamboli", Rating = 4.5m },
            new() { Name = "SBI Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "SBI", CAGR1Y = 6.7m, CAGR3Y = 5.4m, CAGR5Y = 5.7m, ExpenseRatio = 0.22m, AUM = 48000, FundManager = "R.P. Agrawal", Rating = 4 },
            new() { Name = "Motilal Oswal Nasdaq 100 Fund", Category = "International", SubCategory = "International Equity", AMC = "Motilal Oswal", CAGR1Y = 22.5m, CAGR3Y = 18.2m, CAGR5Y = 20.1m, ExpenseRatio = 0.50m, AUM = 5000, FundManager = "Swapnil Mayekar", Rating = 4.5m },
            new() { Name = "Franklin India Feeder - US Opportunities Fund", Category = "International", SubCategory = "International Equity", AMC = "Franklin", CAGR1Y = 18.3m, CAGR3Y = 15.8m, CAGR5Y = 16.2m, ExpenseRatio = 0.55m, AUM = 3500, FundManager = "Grant Bowers", Rating = 4 }
        };

        context.MutualFunds.AddRange(funds);
    }


    // --- DEV/TEST SEEDERS (Only run in Development environment) ---

    private static void SeedDemoUsers(AppDbContext context, ILogger logger)
    {
        if (context.Users.Any(u => u.Email == "demo@test.com")) return;
        logger.LogInformation("Seeding: Demo Users (Development only)");

        var demoUsers = new List<User>
        {
            new() { Name = "Demo User", Email = "demo@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"), Role = "User" },
            new() { Name = "Rahul Sharma", Email = "rahul@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), Role = "User" },
            new() { Name = "Priya Patel", Email = "priya@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), Role = "User" }
        };

        context.Users.AddRange(demoUsers);
        context.SaveChanges();

        // Add profiles for demo users
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

        // Create a pre-computed risk assessment
        var assessment = new RiskAssessment
        {
            UserId = demo.Id,
            TotalScore = 62,
            RiskProfile = "Aggressive",
            CompletedAt = DateTime.UtcNow.AddDays(-7)
        };
        context.RiskAssessments.Add(assessment);
        context.SaveChanges();

        // Create a pre-computed recommendation
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
    }
}
