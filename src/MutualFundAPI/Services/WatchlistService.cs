using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class WatchlistService
{
    private readonly AppDbContext _context;

    public WatchlistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WatchlistItemDTO>> GetWatchlist(int userId)
    {
        return await _context.WatchlistItems
            .Where(w => w.UserId == userId)
            .Include(w => w.MutualFund)
            .OrderByDescending(w => w.AddedAt)
            .Select(w => new WatchlistItemDTO
            {
                Id = w.Id,
                MutualFundId = w.MutualFundId,
                FundName = w.MutualFund.Name,
                Category = w.MutualFund.Category,
                AMC = w.MutualFund.AMC,
                NAV = w.MutualFund.NAV,
                CAGR3Y = w.MutualFund.CAGR3Y,
                Rating = w.MutualFund.Rating,
                AddedAt = w.AddedAt
            })
            .ToListAsync();
    }

    public async Task<WatchlistItemDTO?> AddToWatchlist(int userId, int fundId)
    {
        // Check if already in watchlist
        var exists = await _context.WatchlistItems
            .AnyAsync(w => w.UserId == userId && w.MutualFundId == fundId);

        if (exists) return null;

        var fund = await _context.MutualFunds.FindAsync(fundId);
        if (fund == null) return null;

        var item = new WatchlistItem
        {
            UserId = userId,
            MutualFundId = fundId
        };

        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync();

        return new WatchlistItemDTO
        {
            Id = item.Id,
            MutualFundId = fund.Id,
            FundName = fund.Name,
            Category = fund.Category,
            AMC = fund.AMC,
            NAV = fund.NAV,
            CAGR3Y = fund.CAGR3Y,
            Rating = fund.Rating,
            AddedAt = item.AddedAt
        };
    }

    public async Task<bool> RemoveFromWatchlist(int userId, int itemId)
    {
        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.Id == itemId && w.UserId == userId);

        if (item == null) return false;

        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsInWatchlist(int userId, int fundId)
    {
        return await _context.WatchlistItems
            .AnyAsync(w => w.UserId == userId && w.MutualFundId == fundId);
    }
}
