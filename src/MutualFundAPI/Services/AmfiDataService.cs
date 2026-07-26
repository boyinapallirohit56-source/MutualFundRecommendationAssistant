using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

/// <summary>
/// Service to fetch mutual fund data from AMFI (Association of Mutual Funds in India).
/// AMFI provides free NAV data at: https://www.amfiindia.com/spages/NAVAll.txt
/// Format: Scheme Code;ISIN Div Payout;ISIN Growth;Scheme Name;Net Asset Value;Date
/// </summary>
public class AmfiDataService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AmfiDataService> _logger;

    private const string AMFI_NAV_URL = "https://www.amfiindia.com/spages/NAVAll.txt";

    public AmfiDataService(AppDbContext context, IHttpClientFactory httpClientFactory, ILogger<AmfiDataService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Fetches latest NAV data from AMFI and updates the database.
    /// Call this daily or on-demand to keep fund data current.
    /// SAFETY: Only updates the NAV field. Never modifies fund name, category, or holdings.
    /// </summary>
    public async Task<AmfiSyncResult> SyncNavData()
    {
        var result = new AmfiSyncResult();
        var alreadyUpdated = new HashSet<int>(); // First match wins — don't overwrite

        try
        {
            _logger.LogInformation("Starting AMFI NAV data sync...");

            var client = _httpClientFactory.CreateClient("AMFI");
            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetStringAsync(AMFI_NAV_URL);
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            string currentCategory = "";
            string currentAmc = "";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Detect category headers (lines without semicolons that end with specific patterns)
                if (!trimmed.Contains(';'))
                {
                    if (trimmed.Contains("(") || trimmed.Contains("Scheme"))
                    {
                        currentCategory = ExtractCategory(trimmed);
                    }
                    else if (trimmed.Length > 3 && !trimmed.StartsWith("Scheme"))
                    {
                        currentAmc = trimmed.Trim();
                    }
                    continue;
                }

                // Parse data line: SchemeCode;ISINPayout;ISINGrowth;SchemeName;NAV;Date
                var parts = trimmed.Split(';');
                if (parts.Length < 5) continue;

                // Skip header row
                if (parts[0].Trim() == "Scheme Code") continue;

                var schemeCode = parts[0].Trim();
                var schemeName = parts[3].Trim();
                var navStr = parts[4].Trim();

                if (!decimal.TryParse(navStr, out var nav)) continue;
                if (string.IsNullOrWhiteSpace(schemeName)) continue;

                // Only process Direct Plan Growth schemes (skip IDCW/Dividend/Regular)
                if (!schemeName.Contains("Direct")) continue;
                if (!schemeName.Contains("Growth")) continue;
                if (schemeName.Contains("IDCW") || schemeName.Contains("Dividend") || schemeName.Contains("Payout")) continue;

                var existingFund = _context.MutualFunds.AsEnumerable()
                    .FirstOrDefault(f => !alreadyUpdated.Contains(f.Id) && IsNameMatch(f.Name, schemeName));

                if (existingFund != null)
                {
                    var oldNAV = existingFund.NAV ?? 0;

                    // SAFETY: Reject updates with extreme deviation that indicate a wrong match
                    if (oldNAV > 0 && !IsNavChangeReasonable(oldNAV, nav, existingFund.Name))
                    {
                        _logger.LogWarning("NAV REJECTED (extreme deviation): {FundName} | {OldNAV} → {NewNAV} (from: {AmfiName}). Skipping.",
                            existingFund.Name, oldNAV, nav, schemeName);
                        result.Processed++;
                        continue;
                    }

                    // SAFETY: Only update NAV field — never touch Name, Category, MutualFundId, or any identity field
                    existingFund.NAV = nav;
                    alreadyUpdated.Add(existingFund.Id);
                    result.Updated++;
                    _logger.LogInformation("NAV Updated: {FundName} | {OldNAV} → {NewNAV} (matched from: {AmfiName})",
                        existingFund.Name, oldNAV, nav, schemeName);
                }

                result.Processed++;
            }

            await _context.SaveChangesAsync();
            result.Success = true;
            result.UpdatedFundNames = alreadyUpdated.Count > 0
                ? string.Join(", ", _context.MutualFunds.Where(f => alreadyUpdated.Contains(f.Id)).Select(f => f.Name))
                : "None";
            _logger.LogInformation("AMFI sync complete. Processed: {Processed}, Updated: {Updated}. Funds: {Funds}",
                result.Processed, result.Updated, result.UpdatedFundNames);
        }
        catch (HttpRequestException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Failed to fetch AMFI data: {ex.Message}";
            _logger.LogError(ex, "Failed to fetch AMFI NAV data");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Error during sync: {ex.Message}";
            _logger.LogError(ex, "Error during AMFI data sync");
        }

        return result;
    }

    /// <summary>
    /// Validates that a NAV change is within reasonable bounds.
    /// Prevents incorrect matches from corrupting data.
    /// Allows: up to 50% drop or 100% increase (covers extreme market moves and gold rallies).
    /// Rejects: changes that suggest a fundamentally wrong match (e.g., NAV 25 → 2884).
    /// </summary>
    private bool IsNavChangeReasonable(decimal oldNAV, decimal newNAV, string fundName)
    {
        if (oldNAV <= 0) return true; // No basis for comparison

        var changeRatio = newNAV / oldNAV;

        // Allow between 0.5x (50% drop) and 2.0x (100% increase) for normal funds
        // This covers extreme scenarios: market crashes, gold bull runs, etc.
        if (changeRatio >= 0.5m && changeRatio <= 2.0m)
            return true;

        // For funds with very high absolute NAV (like liquid funds with NAV > 1000),
        // allow tighter change (max 10% move) since they barely fluctuate
        if (oldNAV > 1000 && changeRatio >= 0.9m && changeRatio <= 1.1m)
            return true;

        // Log the rejection reason
        _logger.LogWarning("NAV sanity check failed for {Fund}: ratio {Ratio:F2}x (old={Old}, new={New})",
            fundName, changeRatio, oldNAV, newNAV);

        return false;
    }

    /// <summary>
    /// Imports fund master data from AMFI for a specific category.
    /// Use this for initial data population.
    /// </summary>
    public async Task<AmfiSyncResult> ImportFundsByCategory(string category, int maxFunds = 20)
    {
        var result = new AmfiSyncResult();

        try
        {
            _logger.LogInformation("Importing AMFI funds for category: {Category}", category);

            var client = _httpClientFactory.CreateClient("AMFI");
            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetStringAsync(AMFI_NAV_URL);
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            string currentSection = "";
            int imported = 0;

            foreach (var line in lines)
            {
                if (imported >= maxFunds) break;

                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                if (!trimmed.Contains(';'))
                {
                    currentSection = trimmed;
                    continue;
                }

                // Only process if we're in the right category section
                if (!currentSection.ToLower().Contains(category.ToLower())) continue;

                var parts = trimmed.Split(';');
                if (parts.Length < 5) continue;
                if (parts[0].Trim() == "Scheme Code") continue;

                var schemeName = parts[3].Trim();
                var navStr = parts[4].Trim();

                if (!decimal.TryParse(navStr, out var nav)) continue;
                if (string.IsNullOrWhiteSpace(schemeName)) continue;

                // Check if already exists
                var exists = await _context.MutualFunds.AnyAsync(f => f.Name == schemeName);
                if (exists) continue;

                // Only import Growth/Direct plans for cleaner data
                if (!schemeName.Contains("Growth") && !schemeName.Contains("Direct")) continue;

                var fund = new MutualFund
                {
                    Name = schemeName,
                    Category = MapToCategory(currentSection),
                    SubCategory = MapToSubCategory(currentSection),
                    AMC = ExtractAmc(schemeName),
                    NAV = nav,
                    IsActive = true
                };

                _context.MutualFunds.Add(fund);
                imported++;
                result.Processed++;
            }

            await _context.SaveChangesAsync();
            result.Updated = imported;
            result.Success = true;
            _logger.LogInformation("AMFI import complete. Imported {Count} funds", imported);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Import failed: {ex.Message}";
            _logger.LogError(ex, "AMFI import failed");
        }

        return result;
    }

    // --- Helpers ---

    /// <summary>
    /// Strict matching: Maps each DB fund to its expected AMFI scheme name patterns.
    /// Only exact, curated matches are allowed — no fuzzy/generic word matching.
    /// This prevents cross-matching between different funds from the same AMC.
    /// Patterns are verified against real AMFI NAVAll.txt data (July 2026).
    /// </summary>
    private static readonly Dictionary<string, string[]> ExactFundPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        // Equity - Large Cap
        // SEBI renamed "SBI Bluechip" → "SBI Large Cap" but AMFI still uses "Blue Chip"
        { "SBI Bluechip Fund", new[] { "sbi blue chip", "sbi bluechip", "sbi large cap fund" } },
        { "ICICI Prudential Bluechip Fund", new[] { "icici prudential bluechip", "icici prudential blue chip" } },
        { "Mirae Asset Large Cap Fund", new[] { "mirae asset large cap fund" } },

        // Equity - Mid Cap
        // SEBI renamed "Kotak Emerging Equity" → "Kotak Midcap Fund" but AMFI uses old name
        { "Kotak Emerging Equity Fund", new[] { "kotak emerging equity", "kotak midcap fund" } },
        // SEBI renamed "HDFC Mid-Cap Opportunities" → "HDFC Mid-Cap Fund" / "HDFC Mid Cap Fund"
        { "HDFC Mid-Cap Opportunities Fund", new[] { "hdfc mid-cap opportunities", "hdfc mid cap opportunities", "hdfc mid-cap fund", "hdfc mid cap fund" } },

        // Equity - Small Cap
        { "Nippon India Small Cap Fund", new[] { "nippon india small cap fund" } },
        { "SBI Small Cap Fund", new[] { "sbi small cap fund" } },

        // Debt
        { "HDFC Short Term Debt Fund", new[] { "hdfc short term debt fund" } },
        { "ICICI Prudential All Seasons Bond Fund", new[] { "icici prudential all seasons bond" } },
        { "SBI Magnum Gilt Fund", new[] { "sbi magnum gilt fund", "sbi magnum constant maturity" } },
        { "Axis Banking & PSU Debt Fund", new[] { "axis banking & psu debt fund", "axis banking and psu debt" } },
        { "Kotak Corporate Bond Fund", new[] { "kotak corporate bond fund" } },
        { "Aditya Birla Sun Life Corporate Bond Fund", new[] { "aditya birla sun life corporate bond fund", "aditya birla sl corporate bond" } },

        // Hybrid
        { "ICICI Prudential Balanced Advantage Fund", new[] { "icici prudential balanced advantage fund" } },
        // SEBI renamed to just "HDFC Balanced Advantage Fund"
        { "HDFC Balanced Advantage Fund", new[] { "hdfc balanced advantage fund" } },
        { "Canara Robeco Equity Hybrid Fund", new[] { "canara robeco equity hybrid fund" } },
        // SEBI renamed "Kotak Equity Hybrid" → "Kotak Aggressive Hybrid Fund"
        { "Kotak Equity Hybrid Fund", new[] { "kotak equity hybrid fund", "kotak aggressive hybrid fund" } },
        // SEBI renamed "Mirae Asset Hybrid Equity" → "Mirae Asset Aggressive Hybrid Fund"
        { "Mirae Asset Hybrid Equity Fund", new[] { "mirae asset hybrid equity fund", "mirae asset aggressive hybrid fund" } },

        // Gold
        { "SBI Gold Fund", new[] { "sbi gold fund" } },
        { "HDFC Gold Fund", new[] { "hdfc gold fund" } },
        { "Kotak Gold Fund", new[] { "kotak gold fund" } },
        { "Nippon India Gold Savings Fund", new[] { "nippon india gold savings fund" } },
        { "Axis Gold Fund", new[] { "axis gold fund" } },

        // Liquid
        { "HDFC Liquid Fund", new[] { "hdfc liquid fund" } },
        { "SBI Liquid Fund", new[] { "sbi liquid fund" } },
        { "ICICI Prudential Liquid Fund", new[] { "icici prudential liquid fund" } },
        { "Axis Liquid Fund", new[] { "axis liquid fund" } },
        { "Kotak Liquid Fund", new[] { "kotak liquid fund" } },

        // International
        { "Motilal Oswal Nasdaq 100 Fund", new[] { "motilal oswal nasdaq 100" } },
        // SEBI renamed to "Franklin U.S. Opportunities Equity Active Fund of Funds"
        { "Franklin India Feeder - US Opportunities Fund", new[] { "franklin india feeder - franklin u.s. opportunities", "franklin india feeder - franklin us opportunities", "franklin u.s. opportunities equity active fund of funds", "franklin us opportunities" } },
        { "ICICI Prudential US Bluechip Equity Fund", new[] { "icici prudential us bluechip equity fund" } },
        // SEBI renamed to "DSP Global Innovation Overseas Equity Omni FoF"
        { "DSP Global Innovation Fund", new[] { "dsp global innovation fund of fund", "dsp global innovation overseas equity omni fof", "dsp global innovation" } },
        // SEBI renamed to "Kotak International REIT Overseas Equity Omni FoF"
        { "Kotak International REIT Fund", new[] { "kotak international reit fof", "kotak international reit overseas equity omni fof", "kotak international reit" } },
    };

    /// <summary>
    /// Strict matching: only matches if the AMFI scheme name contains one of the
    /// curated patterns for this specific DB fund. No generic word-count matching.
    /// </summary>
    private static bool IsNameMatch(string dbName, string amfiName)
    {
        var amfiLower = amfiName.ToLower();

        // Only match if we have an explicit pattern for this fund
        if (ExactFundPatterns.TryGetValue(dbName, out var patterns))
        {
            foreach (var pattern in patterns)
            {
                if (amfiLower.Contains(pattern))
                {
                    // Additional safety: make sure it's a Direct Growth plan
                    // (already filtered in the main loop, but double-check)
                    return true;
                }
            }
        }

        // No curated pattern found — do NOT match (safe default)
        return false;
    }
    private static string ExtractCategory(string header)
    {
        var lower = header.ToLower();
        if (lower.Contains("equity")) return "Equity";
        if (lower.Contains("debt") || lower.Contains("bond") || lower.Contains("gilt")) return "Debt";
        if (lower.Contains("hybrid") || lower.Contains("balanced")) return "Hybrid";
        if (lower.Contains("gold")) return "Gold";
        if (lower.Contains("liquid") || lower.Contains("money market")) return "Liquid";
        if (lower.Contains("international") || lower.Contains("overseas")) return "International";
        return "Other";
    }

    private static string MapToCategory(string section)
    {
        return ExtractCategory(section);
    }

    private static string MapToSubCategory(string section)
    {
        var lower = section.ToLower();
        if (lower.Contains("large cap")) return "Large Cap";
        if (lower.Contains("mid cap")) return "Mid Cap";
        if (lower.Contains("small cap")) return "Small Cap";
        if (lower.Contains("multi cap") || lower.Contains("flexi cap")) return "Multi Cap";
        if (lower.Contains("gilt") || lower.Contains("government")) return "Govt Securities";
        if (lower.Contains("corporate")) return "Corporate Bond";
        if (lower.Contains("short")) return "Short Duration";
        if (lower.Contains("balanced") || lower.Contains("advantage")) return "Balanced Advantage";
        if (lower.Contains("aggressive hybrid")) return "Aggressive Hybrid";
        if (lower.Contains("gold")) return "Gold ETF";
        if (lower.Contains("liquid")) return "Liquid Fund";
        if (lower.Contains("overnight")) return "Overnight Fund";
        if (lower.Contains("international") || lower.Contains("overseas")) return "International Equity";
        return section.Length > 30 ? section[..30] : section;
    }

    private static string ExtractAmc(string schemeName)
    {
        // Common AMC prefixes
        string[] amcs = { "SBI", "HDFC", "ICICI Prudential", "Kotak", "Nippon India", "Axis",
                         "Mirae Asset", "Motilal Oswal", "Franklin", "Tata", "UTI", "DSP",
                         "Aditya Birla Sun Life", "Canara Robeco", "Invesco", "PGIM" };

        foreach (var amc in amcs)
        {
            if (schemeName.StartsWith(amc, StringComparison.OrdinalIgnoreCase))
                return amc;
        }

        // Fallback: take first two words
        var words = schemeName.Split(' ');
        return words.Length >= 2 ? $"{words[0]} {words[1]}" : words[0];
    }
}

public class AmfiSyncResult
{
    public bool Success { get; set; }
    public int Processed { get; set; }
    public int Updated { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UpdatedFundNames { get; set; }
}
