using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

public class FileUploadService
{
    private readonly AppDbContext _context;

    public FileUploadService(AppDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<FileUploadResult> ParseCsvFile(Stream fileStream)
    {
        var result = new FileUploadResult();

        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<CsvHoldingRecord>().ToList();

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.FundName)) continue;

            var fundName = record.FundName.Trim();
            var matchedFund = await FindFundByName(fundName);

            if (matchedFund == null)
            {
                result.SkippedFunds.Add(fundName);
                continue;
            }

            result.Holdings.Add(new AddHoldingDTO
            {
                FundName = matchedFund.Name,
                MutualFundId = matchedFund.Id,
                Units = record.Units,
                PurchaseNAV = record.PurchaseNAV,
                InvestedAmount = record.InvestedAmount > 0
                    ? record.InvestedAmount
                    : record.Units * record.PurchaseNAV,
                PurchaseDate = DateTime.TryParse(record.PurchaseDate, out var date)
                    ? date
                    : DateTime.UtcNow
            });
        }

        return result;
    }

    public async Task<FileUploadResult> ParseExcelFile(Stream fileStream)
    {
        var result = new FileUploadResult();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null) return result;

        var rowCount = worksheet.Dimension?.Rows ?? 0;

        for (int row = 2; row <= rowCount; row++)
        {
            var fundName = worksheet.Cells[row, 1].Text?.Trim();
            if (string.IsNullOrWhiteSpace(fundName)) continue;

            var matchedFund = await FindFundByName(fundName);

            if (matchedFund == null)
            {
                result.SkippedFunds.Add(fundName);
                continue;
            }

            var units = decimal.TryParse(worksheet.Cells[row, 2].Text, out var u) ? u : 0;
            var purchaseNAV = decimal.TryParse(worksheet.Cells[row, 3].Text, out var nav) ? nav : 0;
            var investedAmount = decimal.TryParse(worksheet.Cells[row, 4].Text, out var amt) ? amt : 0;
            var purchaseDate = DateTime.TryParse(worksheet.Cells[row, 5].Text, out var date) ? date : DateTime.UtcNow;

            if (investedAmount == 0 && units > 0 && purchaseNAV > 0)
                investedAmount = units * purchaseNAV;

            result.Holdings.Add(new AddHoldingDTO
            {
                FundName = matchedFund.Name,
                MutualFundId = matchedFund.Id,
                Units = units,
                PurchaseNAV = purchaseNAV,
                InvestedAmount = investedAmount,
                PurchaseDate = purchaseDate
            });
        }

        return result;
    }

    private async Task<FundMatch?> FindFundByName(string fundName)
    {
        var lowerName = fundName.ToLower();

        // Try exact match first
        var exactMatch = await _context.MutualFunds
            .Where(f => f.IsActive && f.Name.ToLower() == lowerName)
            .Select(f => new FundMatch { Id = f.Id, Name = f.Name })
            .FirstOrDefaultAsync();

        if (exactMatch != null) return exactMatch;

        // Try contains match (e.g., "SBI Large Cap" matches "SBI Large Cap Fund")
        var allFunds = await _context.MutualFunds
            .Where(f => f.IsActive)
            .ToListAsync();

        var match = allFunds.FirstOrDefault(f =>
            f.Name.ToLower().Contains(lowerName) || lowerName.Contains(f.Name.ToLower()));

        if (match != null)
            return new FundMatch { Id = match.Id, Name = match.Name };

        return null;
    }
}

public class FundMatch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class FileUploadResult
{
    public List<AddHoldingDTO> Holdings { get; set; } = new();
    public List<string> SkippedFunds { get; set; } = new();
}

// CSV mapping class
public class CsvHoldingRecord
{
    public string FundName { get; set; } = string.Empty;
    public decimal Units { get; set; }
    public decimal PurchaseNAV { get; set; }
    public decimal InvestedAmount { get; set; }
    public string PurchaseDate { get; set; } = string.Empty;
}
