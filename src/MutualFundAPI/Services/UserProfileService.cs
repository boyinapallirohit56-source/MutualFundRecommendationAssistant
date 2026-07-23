using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class UserProfileService
{
    private readonly AppDbContext _context;

    public UserProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDTO?> GetProfile(int userId)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return null;

        return new UserProfileDTO
        {
            Age = profile.Age,
            Occupation = profile.Occupation,
            Location = profile.Location,
            MaritalStatus = profile.MaritalStatus,
            Dependents = profile.Dependents,
            MonthlyIncome = profile.MonthlyIncome,
            MonthlyExpenses = profile.MonthlyExpenses,
            Savings = profile.Savings,
            Loans = profile.Loans,
            ExistingInvestments = profile.ExistingInvestments,
            InvestmentType = profile.InvestmentType,
            SIPAmount = profile.SIPAmount,
            SIPFrequency = profile.SIPFrequency,
            SIPDate = profile.SIPDate,
            LumpSumAmount = profile.LumpSumAmount,
            HasSWP = profile.HasSWP,
            SWPAmount = profile.SWPAmount,
            DurationInYears = profile.DurationInYears,
            Goals = profile.Goals
        };
    }

    public async Task<UserProfileDTO> SaveProfile(int userId, UserProfileDTO dto)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile { UserId = userId };
            _context.UserProfiles.Add(profile);
        }

        profile.Age = dto.Age;
        profile.Occupation = dto.Occupation;
        profile.Location = dto.Location;
        profile.MaritalStatus = dto.MaritalStatus;
        profile.Dependents = dto.Dependents;
        profile.MonthlyIncome = dto.MonthlyIncome;
        profile.MonthlyExpenses = dto.MonthlyExpenses;
        profile.Savings = dto.Savings;
        profile.Loans = dto.Loans;
        profile.ExistingInvestments = dto.ExistingInvestments;
        profile.InvestmentType = dto.InvestmentType;
        profile.SIPAmount = dto.SIPAmount;
        profile.SIPFrequency = dto.SIPFrequency;
        profile.SIPDate = dto.SIPDate;
        profile.LumpSumAmount = dto.LumpSumAmount;
        profile.HasSWP = dto.HasSWP;
        profile.SWPAmount = dto.SWPAmount;
        profile.DurationInYears = dto.DurationInYears;
        profile.Goals = dto.Goals;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return dto;
    }
}
