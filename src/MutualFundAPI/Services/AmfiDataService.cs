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
    /// </summary>
    public async Task<AmfiSyncResult> SyncNavData()
    {
        var result = new AmfiSyncResult();

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

                // Try to match with existing fund in database using smart matching
                // Normalize names for comparison: lowercase, remove common suffixes
                var normalizedAmfiName = NormalizeFundName(schemeName);

                // Only process Direct Plan Growth schemes (most relevant for investors)
                if (!schemeName.Contains("Direct") || !schemeName.Contains("Growth")) continue;

                var existingFund = _context.MutualFunds.AsEnumerable()
                    .FirstOrDefault(f => IsNameMatch(f.Name, schemeName));

                if (existingFund != null)
                {
                    existingFund.NAV = nav;
                    result.Updated++;
                }

                result.Processed++;
            }

            await _context.SaveChangesAsync();
            result.Success = true;
            _logger.LogInformation("AMFI sync complete. Processed: {Processed}, Updated: {Updated}",
                result.Processed, result.Updated);
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
    /// Smart matching: compares core fund name keywords between our DB and AMFI
    /// "SBI Bluechip Fund" matches "SBI Blue Chip Fund - Direct Plan - Growth"
    /// </summary>
    private static bool IsNameMatch(string dbName, string amfiName)
    {
        var dbNormalized = NormalizeFundName(dbName);
        var amfiNormalized = NormalizeFundName(amfiName);

        // Check if all keywords from DB name exist in AMFI name
        var dbWords = dbNormalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var amfiWords = amfiNormalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // At least 3 core words must match
        int matchCount = 0;
        foreach (var word in dbWords)
        {
            if (word.Length < 3) continue; // Skip short words like "of", "in"
            if (amfiWords.Any(aw => aw.Contains(word) || word.Contains(aw)))
                matchCount++;
        }

        // Require at least 3 meaningful word matches AND first word (AMC name) must match
        return matchCount >= 3 && dbWords.Length > 0 && amfiWords.Length > 0 &&
               (amfiWords[0] == dbWords[0] || amfiWords[0].Contains(dbWords[0]) || dbWords[0].Contains(amfiWords[0]));
    }

    private static string NormalizeFundName(string name)
    {
        return name.ToLower()
            .Replace("bluechip", "blue chip")
            .Replace("-", " ")
            .Replace("direct plan", "")
            .Replace("regular plan", "")
            .Replace("growth", "")
            .Replace("dividend", "")
            .Replace("fund", "")
            .Replace("  ", " ")
            .Trim();
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
}
