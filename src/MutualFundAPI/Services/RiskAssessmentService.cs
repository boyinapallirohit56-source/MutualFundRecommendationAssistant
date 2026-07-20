using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class RiskAssessmentService
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;

    public RiskAssessmentService(AppDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<List<RiskQuestionDTO>> GetQuestions()
    {
        return await _context.RiskQuestions
            .Where(q => q.IsActive)
            .OrderBy(q => q.OrderNumber)
            .Select(q => new RiskQuestionDTO
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OrderNumber = q.OrderNumber,
                Options = q.Options.Select(o => new RiskOptionDTO
                {
                    Id = o.Id,
                    OptionText = o.OptionText
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<RiskAssessmentResultDTO?> SubmitAssessment(int userId, SubmitAssessmentDTO dto)
    {
        // Calculate total score
        var optionIds = dto.Answers.Select(a => a.SelectedOptionId).ToList();
        var options = await _context.RiskOptions
            .Where(o => optionIds.Contains(o.Id))
            .ToListAsync();

        int totalScore = options.Sum(o => o.Score);
        int maxPossibleScore = dto.Answers.Count * 4; // 4 is max score per question
        int normalizedScore = (int)Math.Round((double)totalScore / maxPossibleScore * 100);

        string riskProfile = GetRiskProfile(normalizedScore);

        // Save assessment
        var assessment = new RiskAssessment
        {
            UserId = userId,
            TotalScore = normalizedScore,
            RiskProfile = riskProfile,
            CompletedAt = DateTime.UtcNow
        };

        _context.RiskAssessments.Add(assessment);
        await _context.SaveChangesAsync();

        // Auto-trigger notification
        await _notificationService.CreateNotification(userId,
            "Risk Assessment Complete",
            $"Your risk profile is {riskProfile} (Score: {normalizedScore}/100). View your recommended allocation.",
            "assessment");

        // Save individual responses
        foreach (var answer in dto.Answers)
        {
            _context.RiskResponses.Add(new RiskResponse
            {
                AssessmentId = assessment.Id,
                QuestionId = answer.QuestionId,
                SelectedOptionId = answer.SelectedOptionId
            });
        }
        await _context.SaveChangesAsync();

        return new RiskAssessmentResultDTO
        {
            AssessmentId = assessment.Id,
            TotalScore = totalScore,
            NormalizedScore = normalizedScore,
            RiskProfile = riskProfile,
            Description = GetProfileDescription(riskProfile),
            CompletedAt = assessment.CompletedAt
        };
    }

    public async Task<RiskAssessmentResultDTO?> GetLatestAssessment(int userId)
    {
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        if (assessment == null) return null;

        return new RiskAssessmentResultDTO
        {
            AssessmentId = assessment.Id,
            TotalScore = assessment.TotalScore,
            NormalizedScore = assessment.TotalScore,
            RiskProfile = assessment.RiskProfile,
            Description = GetProfileDescription(assessment.RiskProfile),
            CompletedAt = assessment.CompletedAt
        };
    }

    private static string GetRiskProfile(int normalizedScore)
    {
        return normalizedScore switch
        {
            <= 25 => "Conservative",
            <= 50 => "Moderate",
            <= 75 => "Aggressive",
            _ => "Very Aggressive"
        };
    }

    private static string GetProfileDescription(string profile)
    {
        return profile switch
        {
            "Conservative" => "You prefer capital preservation with low risk. Stable returns are more important to you than high growth.",
            "Moderate" => "You take a balanced approach. You're comfortable with some risk for better returns.",
            "Aggressive" => "You're growth-oriented and comfortable with market ups and downs for higher potential returns.",
            "Very Aggressive" => "You seek maximum growth and are comfortable with significant market volatility.",
            _ => "Unknown profile"
        };
    }
}
