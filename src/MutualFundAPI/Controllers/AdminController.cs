using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;
    private readonly AppDbContext _context;

    public AdminController(AdminService adminService, AppDbContext context)
    {
        _adminService = adminService;
        _context = context;
    }

    /// <summary>
    /// Force-creates demo accounts with full data. Call once after setup.
    /// </summary>
    [HttpPost("seed-demo-accounts")]
    public async Task<IActionResult> SeedDemoAccounts()
    {
        var created = new List<string>();

        // --- Rohit: Very Aggressive, SIP 1st ---
        created.Add(await EnsureDemoAccount(
            "Rohit Boyinapalli", "rohit@wealthai.com", "Rohit@123",
            new UserProfile { Age = 24, Occupation = "Software Developer", Location = "Hyderabad", MaritalStatus = "Single", Dependents = 0, MonthlyIncome = 120000, MonthlyExpenses = 45000, Savings = 800000, Loans = 0, ExistingInvestments = "Mutual Funds", InvestmentType = "SIP", SIPAmount = 50000, SIPFrequency = "Monthly", SIPDate = 1, HasSWP = true, SWPAmount = 10000, DurationInYears = 10, Goals = "Wealth Creation,Retirement,Tax Saving,Emergency Fund" },
            new List<Goal> {
                new() { Name = "Wealth Creation", TargetAmount = 5000000, CurrentAmount = 900000, TargetYears = 10, MonthlySIP = 25000 },
                new() { Name = "Retirement", TargetAmount = 10000000, CurrentAmount = 1200000, TargetYears = 30, MonthlySIP = 15000 },
                new() { Name = "Tax Saving", TargetAmount = 150000, CurrentAmount = 52000, TargetYears = 1, MonthlySIP = 12500 },
                new() { Name = "Emergency Fund", TargetAmount = 500000, CurrentAmount = 200000, TargetYears = 2, MonthlySIP = 10000 }
            },
            78, "Very Aggressive",
            "You have a high risk tolerance and a growth-focused approach. The allocation maximizes equity exposure across large, mid, and small-cap funds for maximum growth potential.",
            new Dictionary<string, int> { {"Equity", 80}, {"Debt", 5}, {"Hybrid", 5}, {"Gold", 5}, {"International", 5} }
        ));

        // --- Rahul: Moderate, SIP 5th ---
        created.Add(await EnsureDemoAccount(
            "Rahul Sharma", "rahul@wealthai.com", "Rahul@123",
            new UserProfile { Age = 35, Occupation = "Product Manager", Location = "Mumbai", MaritalStatus = "Married", Dependents = 1, MonthlyIncome = 180000, MonthlyExpenses = 75000, Savings = 1500000, Loans = 30000, ExistingInvestments = "Stocks", InvestmentType = "Both", SIPAmount = 40000, SIPFrequency = "Monthly", SIPDate = 5, LumpSumAmount = 200000, HasSWP = false, SWPAmount = 0, DurationInYears = 15, Goals = "Child Education,Home Purchase,Retirement" },
            new List<Goal> {
                new() { Name = "Child Education", TargetAmount = 3000000, CurrentAmount = 720000, TargetYears = 12, MonthlySIP = 20000 },
                new() { Name = "Home Purchase", TargetAmount = 8000000, CurrentAmount = 1600000, TargetYears = 7, MonthlySIP = 30000 },
                new() { Name = "Retirement", TargetAmount = 20000000, CurrentAmount = 2400000, TargetYears = 25, MonthlySIP = 25000 }
            },
            48, "Moderate",
            "Your moderate risk profile suggests a balanced approach. The allocation splits between equity for growth and debt for stability. Hybrid funds provide automatic rebalancing.",
            new Dictionary<string, int> { {"Equity", 40}, {"Debt", 30}, {"Hybrid", 15}, {"Gold", 10}, {"Liquid", 5} }
        ));

        // --- Priya: Conservative, SIP 10th ---
        created.Add(await EnsureDemoAccount(
            "Priya Patel", "priya@wealthai.com", "Priya@123",
            new UserProfile { Age = 52, Occupation = "Doctor", Location = "Chennai", MaritalStatus = "Married", Dependents = 2, MonthlyIncome = 250000, MonthlyExpenses = 100000, Savings = 3000000, Loans = 0, ExistingInvestments = "Multiple", InvestmentType = "SIP", SIPAmount = 75000, SIPFrequency = "Monthly", SIPDate = 10, HasSWP = true, SWPAmount = 25000, DurationInYears = 8, Goals = "Retirement,Wealth Creation,Tax Saving" },
            new List<Goal> {
                new() { Name = "Retirement", TargetAmount = 30000000, CurrentAmount = 9000000, TargetYears = 8, MonthlySIP = 50000 },
                new() { Name = "Wealth Creation", TargetAmount = 5000000, CurrentAmount = 1750000, TargetYears = 5, MonthlySIP = 25000 },
                new() { Name = "Tax Saving", TargetAmount = 150000, CurrentAmount = 112000, TargetYears = 1, MonthlySIP = 12500 }
            },
            25, "Conservative",
            "Your conservative profile prioritizes capital preservation. The allocation emphasizes debt instruments and gold for stability, with limited equity exposure for modest growth.",
            new Dictionary<string, int> { {"Equity", 20}, {"Debt", 50}, {"Hybrid", 15}, {"Gold", 10}, {"Liquid", 5} }
        ));

        return Ok(new { message = $"Demo accounts ready: {string.Join(", ", created)}" });
    }

    private async Task<string> EnsureDemoAccount(string name, string email, string password,
        UserProfile profileData, List<Goal> goals, int riskScore, string riskProfile,
        string aiExplanation, Dictionary<string, int> allocations)
    {
        // Create or get user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User { Name = name, Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = "User", IsActive = true, IsEmailVerified = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Delete and recreate profile
        var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (existingProfile != null) _context.UserProfiles.Remove(existingProfile);
        profileData.UserId = user.Id;
        _context.UserProfiles.Add(profileData);

        // Delete and recreate goals
        var existingGoals = await _context.Goals.Where(g => g.UserId == user.Id).ToListAsync();
        _context.Goals.RemoveRange(existingGoals);
        foreach (var g in goals) { g.UserId = user.Id; }
        _context.Goals.AddRange(goals);

        // Delete and recreate risk assessment + recommendation
        var existingRecs = await _context.Recommendations.Where(r => r.UserId == user.Id).ToListAsync();
        foreach (var r in existingRecs)
        {
            var allocs = await _context.RecommendationAllocations.Where(a => a.RecommendationId == r.Id).ToListAsync();
            _context.RecommendationAllocations.RemoveRange(allocs);
        }
        _context.Recommendations.RemoveRange(existingRecs);
        var existingAssessments = await _context.RiskAssessments.Where(a => a.UserId == user.Id).ToListAsync();
        _context.RiskAssessments.RemoveRange(existingAssessments);

        var assessment = new RiskAssessment { UserId = user.Id, TotalScore = riskScore, RiskProfile = riskProfile, CompletedAt = DateTime.UtcNow.AddDays(-3) };
        _context.RiskAssessments.Add(assessment);
        await _context.SaveChangesAsync();

        var recommendation = new Recommendation { UserId = user.Id, RiskAssessmentId = assessment.Id, RiskProfile = riskProfile, GeneratedAt = DateTime.UtcNow.AddDays(-3), AIExplanation = aiExplanation };
        _context.Recommendations.Add(recommendation);
        await _context.SaveChangesAsync();

        foreach (var kvp in allocations)
        {
            _context.RecommendationAllocations.Add(new RecommendationAllocation { RecommendationId = recommendation.Id, AssetClass = kvp.Key, Percentage = kvp.Value, SuggestedFunds = "" });
        }

        await _context.SaveChangesAsync();
        return email;
    }

    // --- Users ---

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsers();
        return Ok(users);
    }

    [HttpPut("users/{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(int userId, [FromBody] UpdateUserStatusDTO dto)
    {
        var result = await _adminService.UpdateUserStatus(userId, dto.IsActive);
        if (!result)
            return NotFound(new { message = "User not found" });

        return Ok(new { message = "User status updated" });
    }

    // --- Questions ---

    [HttpPost("questions")]
    public async Task<IActionResult> AddQuestion([FromBody] AdminQuestionDTO dto)
    {
        var question = await _adminService.AddQuestion(dto);
        return Ok(question);
    }

    [HttpPut("questions/{questionId}")]
    public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] AdminQuestionDTO dto)
    {
        var result = await _adminService.UpdateQuestion(questionId, dto);
        if (!result)
            return NotFound(new { message = "Question not found" });

        return Ok(new { message = "Question updated" });
    }

    [HttpDelete("questions/{questionId}")]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        var result = await _adminService.DeleteQuestion(questionId);
        if (!result)
            return NotFound(new { message = "Question not found" });

        return Ok(new { message = "Question deactivated" });
    }

    // --- Funds ---

    [HttpGet("funds")]
    public async Task<IActionResult> GetAllFunds()
    {
        var funds = await _context.MutualFunds
            .OrderBy(f => f.Name)
            .Select(f => new {
                f.Id, f.Name, f.Category, f.SubCategory, f.AMC,
                f.NAV, f.ExpenseRatio, f.CAGR1Y, f.CAGR3Y, f.CAGR5Y,
                f.AUM, f.Rating, f.IsActive
            })
            .ToListAsync();
        return Ok(funds);
    }

    [HttpPost("funds")]
    public async Task<IActionResult> AddFund([FromBody] AdminFundDTO dto)
    {
        var fund = await _adminService.AddFund(dto);
        return Ok(fund);
    }

    [HttpPut("funds/{fundId}")]
    public async Task<IActionResult> UpdateFund(int fundId, [FromBody] AdminFundDTO dto)
    {
        var result = await _adminService.UpdateFund(fundId, dto);
        if (!result)
            return NotFound(new { message = "Fund not found" });

        return Ok(new { message = "Fund updated" });
    }

    [HttpDelete("funds/{fundId}")]
    public async Task<IActionResult> DeleteFund(int fundId)
    {
        var result = await _adminService.DeleteFund(fundId);
        if (!result)
            return NotFound(new { message = "Fund not found" });

        return Ok(new { message = "Fund deactivated" });
    }

    [HttpPut("funds/{fundId}/reactivate")]
    public async Task<IActionResult> ReactivateFund(int fundId)
    {
        var result = await _adminService.ReactivateFund(fundId);
        if (!result)
            return NotFound(new { message = "Fund not found" });

        return Ok(new { message = "Fund reactivated" });
    }

    // --- Analytics ---

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var analytics = await _adminService.GetAnalytics();
        return Ok(analytics);
    }

    // --- AMFI Data Sync ---

    [HttpPost("sync-amfi")]
    public async Task<IActionResult> SyncAmfiData([FromServices] AmfiDataService amfiService)
    {
        var result = await amfiService.SyncNavData();
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { 
            message = $"Sync complete. Processed: {result.Processed}, Updated: {result.Updated}",
            processed = result.Processed,
            updated = result.Updated,
            updatedFunds = result.UpdatedFundNames
        });
    }

    [HttpPost("import-amfi/{category}")]
    public async Task<IActionResult> ImportAmfiByCategory(string category, [FromServices] AmfiDataService amfiService, [FromQuery] int maxFunds = 20)
    {
        var result = await amfiService.ImportFundsByCategory(category, maxFunds);
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = $"Imported {result.Updated} funds for category: {category}" });
    }

    // --- Allocation Rules ---

    [HttpGet("allocation-rules")]
    public async Task<IActionResult> GetAllocationRules()
    {
        var rules = await _adminService.GetAllocationRules();
        return Ok(rules);
    }

    [HttpPut("allocation-rules/{riskProfile}")]
    public async Task<IActionResult> UpdateAllocationRules(string riskProfile, [FromBody] UpdateAllocationRulesDTO dto)
    {
        var result = await _adminService.UpdateAllocationRules(riskProfile, dto.Allocations);
        if (!result)
            return NotFound(new { message = "No rules found for this profile" });

        return Ok(new { message = $"Allocation rules updated for {riskProfile}" });
    }
}
