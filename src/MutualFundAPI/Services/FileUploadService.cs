using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

public class FileUploadService
{
    public FileUploadService()
    {
        // EPPlus license for non-commercial use
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<AddHoldingDTO>> ParseCsvFile(Stream fileStream)
    {
        var holdings = new List<AddHoldingDTO>();

        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<CsvHoldingRecord>();

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.FundName)) continue;

            holdings.Add(new AddHoldingDTO
            {
                FundName = record.FundName.Trim(),
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

        return holdings;
    }

    public async Task<List<AddHoldingDTO>> ParseExcelFile(Stream fileStream)
    {
        var holdings = new List<AddHoldingDTO>();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null) return holdings;

        var rowCount = worksheet.Dimension?.Rows ?? 0;

        // Expect headers in row 1: FundName, Units, PurchaseNAV, InvestedAmount, PurchaseDate
        for (int row = 2; row <= rowCount; row++)
        {
            var fundName = worksheet.Cells[row, 1].Text?.Trim();
            if (string.IsNullOrWhiteSpace(fundName)) continue;

            var units = decimal.TryParse(worksheet.Cells[row, 2].Text, out var u) ? u : 0;
            var purchaseNAV = decimal.TryParse(worksheet.Cells[row, 3].Text, out var nav) ? nav : 0;
            var investedAmount = decimal.TryParse(worksheet.Cells[row, 4].Text, out var amt) ? amt : 0;
            var purchaseDate = DateTime.TryParse(worksheet.Cells[row, 5].Text, out var date) ? date : DateTime.UtcNow;

            if (investedAmount == 0 && units > 0 && purchaseNAV > 0)
                investedAmount = units * purchaseNAV;

            holdings.Add(new AddHoldingDTO
            {
                FundName = fundName,
                Units = units,
                PurchaseNAV = purchaseNAV,
                InvestedAmount = investedAmount,
                PurchaseDate = purchaseDate
            });
        }

        return holdings;
    }
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
