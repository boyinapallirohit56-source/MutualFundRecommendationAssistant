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
            SIPAmount = profile.SIPAmount,
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
        profile.SIPAmount = dto.SIPAmount;
        profile.DurationInYears = dto.DurationInYears;
        profile.Goals = dto.Goals;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return dto;
    }
}
