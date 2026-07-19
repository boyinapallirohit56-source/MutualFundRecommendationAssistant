using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Data.Seeders;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.RiskQuestions.Any())
            SeedRiskQuestions(context);

        if (!context.AllocationRules.Any())
            SeedAllocationRules(context);

        if (!context.MutualFunds.Any())
            SeedMutualFunds(context);

        if (!context.Users.Any(u => u.Role == "Admin"))
            SeedAdminUser(context);

        context.SaveChanges();
    }

    private static void SeedAdminUser(AppDbContext context)
    {
        context.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@mutualfund.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        });
    }

    private static void SeedRiskQuestions(AppDbContext context)
    {
        var questions = new List<RiskQuestion>
        {
            new()
            {
                QuestionText = "What is your primary investment goal?",
                OrderNumber = 1,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Preserve my capital with minimal risk", Score = 1 },
                    new() { OptionText = "Generate steady income with low risk", Score = 2 },
                    new() { OptionText = "Grow my wealth with moderate risk", Score = 3 },
                    new() { OptionText = "Maximize returns even if it means high risk", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "How long do you plan to keep your money invested?",
                OrderNumber = 2,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Less than 1 year", Score = 1 },
                    new() { OptionText = "1 to 3 years", Score = 2 },
                    new() { OptionText = "3 to 7 years", Score = 3 },
                    new() { OptionText = "More than 7 years", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "If your investment drops 20% in a month, what would you do?",
                OrderNumber = 3,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Sell everything immediately", Score = 1 },
                    new() { OptionText = "Sell some and hold the rest", Score = 2 },
                    new() { OptionText = "Hold and wait for recovery", Score = 3 },
                    new() { OptionText = "Buy more at the lower price", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "What percentage of your monthly income can you invest?",
                OrderNumber = 4,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Less than 10%", Score = 1 },
                    new() { OptionText = "10% to 20%", Score = 2 },
                    new() { OptionText = "20% to 40%", Score = 3 },
                    new() { OptionText = "More than 40%", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "How would you describe your investment experience?",
                OrderNumber = 5,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "No experience at all", Score = 1 },
                    new() { OptionText = "Basic knowledge, invested in FDs/RDs", Score = 2 },
                    new() { OptionText = "Some experience with mutual funds/stocks", Score = 3 },
                    new() { OptionText = "Experienced investor, comfortable with markets", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "How stable is your current income source?",
                OrderNumber = 6,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Unstable / freelance / irregular", Score = 1 },
                    new() { OptionText = "Somewhat stable but uncertain", Score = 2 },
                    new() { OptionText = "Stable salaried job", Score = 3 },
                    new() { OptionText = "Very stable with multiple income sources", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "Do you have an emergency fund that covers 6 months of expenses?",
                OrderNumber = 7,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "No emergency fund", Score = 1 },
                    new() { OptionText = "Covers 1-3 months", Score = 2 },
                    new() { OptionText = "Covers 3-6 months", Score = 3 },
                    new() { OptionText = "Covers more than 6 months", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "What is your age group?",
                OrderNumber = 8,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Above 55 years", Score = 1 },
                    new() { OptionText = "45 to 55 years", Score = 2 },
                    new() { OptionText = "30 to 45 years", Score = 3 },
                    new() { OptionText = "Below 30 years", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "How much loan/debt do you currently have relative to your income?",
                OrderNumber = 9,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "EMIs take more than 50% of my income", Score = 1 },
                    new() { OptionText = "EMIs take 30-50% of my income", Score = 2 },
                    new() { OptionText = "EMIs take less than 30% of my income", Score = 3 },
                    new() { OptionText = "No loans or debt", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "Which statement best describes your risk attitude?",
                OrderNumber = 10,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "I cannot tolerate any loss in my investment", Score = 1 },
                    new() { OptionText = "I can tolerate small losses for slightly better returns", Score = 2 },
                    new() { OptionText = "I can tolerate moderate losses for higher growth", Score = 3 },
                    new() { OptionText = "I am comfortable with significant losses for maximum growth", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "What would you prefer: guaranteed 8% return or a chance of 15% return with risk of -5%?",
                OrderNumber = 11,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Guaranteed 8% always", Score = 1 },
                    new() { OptionText = "Mostly guaranteed with small portion in risky", Score = 2 },
                    new() { OptionText = "Split equally between both", Score = 3 },
                    new() { OptionText = "Go for 15% chance, I can handle the risk", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "How often do you check your investments?",
                OrderNumber = 12,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Daily, and market drops stress me out", Score = 1 },
                    new() { OptionText = "Weekly, I like to stay updated", Score = 2 },
                    new() { OptionText = "Monthly, I trust the long-term process", Score = 3 },
                    new() { OptionText = "Rarely, I set it and forget it", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "What is your financial dependency situation?",
                OrderNumber = 13,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Many people depend on my income", Score = 1 },
                    new() { OptionText = "A few dependents (spouse/children)", Score = 2 },
                    new() { OptionText = "Only myself to support", Score = 3 },
                    new() { OptionText = "No dependents and dual income household", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "If a friend recommends a high-risk high-return investment, how would you react?",
                OrderNumber = 14,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Ignore it completely, too risky", Score = 1 },
                    new() { OptionText = "Research it but probably won't invest", Score = 2 },
                    new() { OptionText = "Invest a small amount to test", Score = 3 },
                    new() { OptionText = "Invest a significant amount if research supports it", Score = 4 }
                }
            },
            new()
            {
                QuestionText = "What best describes your current financial situation?",
                OrderNumber = 15,
                Options = new List<RiskOption>
                {
                    new() { OptionText = "Living paycheck to paycheck", Score = 1 },
                    new() { OptionText = "Comfortable but with limited savings", Score = 2 },
                    new() { OptionText = "Good savings and financially stable", Score = 3 },
                    new() { OptionText = "Financially secure with surplus income", Score = 4 }
                }
            }
        };

        context.RiskQuestions.AddRange(questions);
    }

    private static void SeedAllocationRules(AppDbContext context)
    {
        var rules = new List<AllocationRule>
        {
            // Conservative
            new() { RiskProfile = "Conservative", AssetClass = "Equity", Percentage = 20 },
            new() { RiskProfile = "Conservative", AssetClass = "Debt", Percentage = 50 },
            new() { RiskProfile = "Conservative", AssetClass = "Hybrid", Percentage = 15 },
            new() { RiskProfile = "Conservative", AssetClass = "Gold", Percentage = 10 },
            new() { RiskProfile = "Conservative", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Conservative", AssetClass = "International", Percentage = 0 },

            // Moderate
            new() { RiskProfile = "Moderate", AssetClass = "Equity", Percentage = 40 },
            new() { RiskProfile = "Moderate", AssetClass = "Debt", Percentage = 30 },
            new() { RiskProfile = "Moderate", AssetClass = "Hybrid", Percentage = 15 },
            new() { RiskProfile = "Moderate", AssetClass = "Gold", Percentage = 10 },
            new() { RiskProfile = "Moderate", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Moderate", AssetClass = "International", Percentage = 0 },

            // Aggressive
            new() { RiskProfile = "Aggressive", AssetClass = "Equity", Percentage = 60 },
            new() { RiskProfile = "Aggressive", AssetClass = "Debt", Percentage = 15 },
            new() { RiskProfile = "Aggressive", AssetClass = "Hybrid", Percentage = 10 },
            new() { RiskProfile = "Aggressive", AssetClass = "Gold", Percentage = 5 },
            new() { RiskProfile = "Aggressive", AssetClass = "Liquid", Percentage = 5 },
            new() { RiskProfile = "Aggressive", AssetClass = "International", Percentage = 5 },

            // Very Aggressive
            new() { RiskProfile = "Very Aggressive", AssetClass = "Equity", Percentage = 80 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Debt", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Hybrid", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Gold", Percentage = 5 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "Liquid", Percentage = 0 },
            new() { RiskProfile = "Very Aggressive", AssetClass = "International", Percentage = 5 },
        };

        context.AllocationRules.AddRange(rules);
    }

    private static void SeedMutualFunds(AppDbContext context)
    {
        var funds = new List<MutualFund>
        {
            // Equity - Large Cap
            new() { Name = "SBI Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "SBI", CAGR1Y = 18.5m, CAGR3Y = 14.2m, CAGR5Y = 12.8m, ExpenseRatio = 0.85m, AUM = 45000, Rating = 4.5m },
            new() { Name = "ICICI Prudential Bluechip Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "ICICI", CAGR1Y = 17.8m, CAGR3Y = 13.9m, CAGR5Y = 12.5m, ExpenseRatio = 0.90m, AUM = 38000, Rating = 4 },
            new() { Name = "Mirae Asset Large Cap Fund", Category = "Equity", SubCategory = "Large Cap", AMC = "Mirae Asset", CAGR1Y = 19.2m, CAGR3Y = 15.1m, CAGR5Y = 13.4m, ExpenseRatio = 0.52m, AUM = 35000, Rating = 5 },

            // Equity - Mid Cap
            new() { Name = "Kotak Emerging Equity Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "Kotak", CAGR1Y = 25.3m, CAGR3Y = 20.1m, CAGR5Y = 16.8m, ExpenseRatio = 0.75m, AUM = 28000, Rating = 4.5m },
            new() { Name = "HDFC Mid-Cap Opportunities Fund", Category = "Equity", SubCategory = "Mid Cap", AMC = "HDFC", CAGR1Y = 24.1m, CAGR3Y = 19.5m, CAGR5Y = 15.9m, ExpenseRatio = 0.82m, AUM = 42000, Rating = 4 },

            // Equity - Small Cap
            new() { Name = "Nippon India Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "Nippon", CAGR1Y = 30.2m, CAGR3Y = 28.5m, CAGR5Y = 22.1m, ExpenseRatio = 0.88m, AUM = 32000, Rating = 4.5m },
            new() { Name = "SBI Small Cap Fund", Category = "Equity", SubCategory = "Small Cap", AMC = "SBI", CAGR1Y = 28.7m, CAGR3Y = 25.3m, CAGR5Y = 20.8m, ExpenseRatio = 0.72m, AUM = 18000, Rating = 5 },

            // Debt
            new() { Name = "HDFC Short Term Debt Fund", Category = "Debt", SubCategory = "Short Duration", AMC = "HDFC", CAGR1Y = 7.2m, CAGR3Y = 6.8m, CAGR5Y = 7.1m, ExpenseRatio = 0.35m, AUM = 15000, Rating = 4 },
            new() { Name = "ICICI Prudential All Seasons Bond Fund", Category = "Debt", SubCategory = "Corporate Bond", AMC = "ICICI", CAGR1Y = 7.8m, CAGR3Y = 7.1m, CAGR5Y = 7.5m, ExpenseRatio = 0.42m, AUM = 12000, Rating = 4.5m },
            new() { Name = "SBI Magnum Gilt Fund", Category = "Debt", SubCategory = "Govt Securities", AMC = "SBI", CAGR1Y = 8.1m, CAGR3Y = 6.5m, CAGR5Y = 7.8m, ExpenseRatio = 0.48m, AUM = 8000, Rating = 4 },

            // Hybrid
            new() { Name = "ICICI Prudential Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "ICICI", CAGR1Y = 12.5m, CAGR3Y = 10.8m, CAGR5Y = 11.2m, ExpenseRatio = 0.95m, AUM = 52000, Rating = 4.5m },
            new() { Name = "HDFC Balanced Advantage Fund", Category = "Hybrid", SubCategory = "Balanced Advantage", AMC = "HDFC", CAGR1Y = 13.1m, CAGR3Y = 11.2m, CAGR5Y = 11.8m, ExpenseRatio = 0.88m, AUM = 62000, Rating = 4 },

            // Gold
            new() { Name = "SBI Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "SBI", CAGR1Y = 15.2m, CAGR3Y = 12.8m, CAGR5Y = 11.5m, ExpenseRatio = 0.50m, AUM = 2500, Rating = 4 },
            new() { Name = "HDFC Gold Fund", Category = "Gold", SubCategory = "Gold ETF", AMC = "HDFC", CAGR1Y = 14.8m, CAGR3Y = 12.5m, CAGR5Y = 11.2m, ExpenseRatio = 0.45m, AUM = 2000, Rating = 4 },

            // Liquid
            new() { Name = "HDFC Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "HDFC", CAGR1Y = 6.8m, CAGR3Y = 5.5m, CAGR5Y = 5.8m, ExpenseRatio = 0.20m, AUM = 55000, Rating = 4.5m },
            new() { Name = "SBI Liquid Fund", Category = "Liquid", SubCategory = "Liquid Fund", AMC = "SBI", CAGR1Y = 6.7m, CAGR3Y = 5.4m, CAGR5Y = 5.7m, ExpenseRatio = 0.22m, AUM = 48000, Rating = 4 },

            // International
            new() { Name = "Motilal Oswal Nasdaq 100 Fund", Category = "International", SubCategory = "International Equity", AMC = "Motilal Oswal", CAGR1Y = 22.5m, CAGR3Y = 18.2m, CAGR5Y = 20.1m, ExpenseRatio = 0.50m, AUM = 5000, Rating = 4.5m },
            new() { Name = "Franklin India Feeder - US Opportunities Fund", Category = "International", SubCategory = "International Equity", AMC = "Franklin", CAGR1Y = 18.3m, CAGR3Y = 15.8m, CAGR5Y = 16.2m, ExpenseRatio = 0.55m, AUM = 3500, Rating = 4 },
        };

        context.MutualFunds.AddRange(funds);
    }
}
