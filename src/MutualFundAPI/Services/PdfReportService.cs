using System.Text;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

/// <summary>
/// Generates HTML-based reports that can be printed/saved as PDF from the browser.
/// Uses a clean HTML template that renders well when using Ctrl+P or window.print().
/// </summary>
public class PdfReportService
{
    public string GenerateRiskAssessmentHtml(RiskAssessmentReportDTO report)
    {
        var sb = new StringBuilder();
        sb.Append(GetHtmlHeader("Risk Assessment Report"));
        sb.Append($"<h1>Risk Assessment Report</h1>");
        sb.Append($"<p class='meta'>Generated: {report.GeneratedAt:dd MMM yyyy} | User: {report.UserName}</p>");
        sb.Append($"<div class='score-box'><h2>{report.RiskScore}/100</h2><p>{report.RiskProfile}</p></div>");
        sb.Append($"<p class='desc'>{report.ProfileDescription}</p>");
        sb.Append($"<h3>Summary</h3>");
        sb.Append($"<table><tr><td>Total Questions</td><td>{report.TotalQuestions}</td></tr>");
        sb.Append($"<tr><td>Total Score</td><td>{report.TotalScore} / {report.MaxPossibleScore}</td></tr>");
        sb.Append($"<tr><td>Completed On</td><td>{report.CompletedAt:dd MMM yyyy, hh:mm tt}</td></tr></table>");
        sb.Append($"<h3>Question-wise Breakdown</h3>");
        sb.Append("<table><tr><th>#</th><th>Question</th><th>Your Answer</th><th>Score</th></tr>");
        int i = 1;
        foreach (var r in report.Responses)
        {
            sb.Append($"<tr><td>{i++}</td><td>{r.Question}</td><td>{r.SelectedAnswer}</td><td>{r.Score}/{r.MaxScore}</td></tr>");
        }
        sb.Append("</table>");
        sb.Append(GetHtmlFooter());
        return sb.ToString();
    }

    public string GenerateRecommendationHtml(RecommendationReportDTO report)
    {
        var sb = new StringBuilder();
        sb.Append(GetHtmlHeader("Recommendation Report"));
        sb.Append($"<h1>Mutual Fund Recommendation Report</h1>");
        sb.Append($"<p class='meta'>Generated: {report.GeneratedAt:dd MMM yyyy} | User: {report.UserName}</p>");
        sb.Append($"<h3>Profile Summary</h3>");
        sb.Append($"<table><tr><td>Risk Score</td><td>{report.RiskScore}/100</td></tr>");
        sb.Append($"<tr><td>Risk Profile</td><td><strong>{report.RiskProfile}</strong></td></tr>");
        sb.Append($"<tr><td>Investment Duration</td><td>{report.InvestmentDuration} years</td></tr>");
        sb.Append($"<tr><td>Monthly SIP</td><td>Rs. {report.SIPAmount:N0}</td></tr>");
        sb.Append($"<tr><td>Goals</td><td>{report.Goals}</td></tr></table>");
        sb.Append($"<h3>Recommended Allocation</h3>");
        sb.Append("<table><tr><th>Asset Class</th><th>Allocation %</th><th>Suggested Funds</th></tr>");
        foreach (var a in report.Allocations)
        {
            sb.Append($"<tr><td>{a.AssetClass}</td><td>{a.Percentage}%</td><td>{a.SuggestedFunds ?? "-"}</td></tr>");
        }
        sb.Append("</table>");
        sb.Append($"<h3>AI Explanation</h3><p class='desc'>{report.AIExplanation}</p>");
        sb.Append($"<p class='disclaimer'>{report.Disclaimer}</p>");
        sb.Append(GetHtmlFooter());
        return sb.ToString();
    }

    public string GeneratePortfolioHtml(PortfolioReportDTO report)
    {
        var sb = new StringBuilder();
        sb.Append(GetHtmlHeader("Portfolio Report"));
        sb.Append($"<h1>Portfolio Analysis Report</h1>");
        sb.Append($"<p class='meta'>Generated: {report.GeneratedAt:dd MMM yyyy} | User: {report.UserName}</p>");
        sb.Append($"<h3>Portfolio Summary</h3>");
        sb.Append($"<table><tr><td>Total Invested</td><td>Rs. {report.TotalInvested:N0}</td></tr>");
        sb.Append($"<tr><td>Current Value</td><td>Rs. {report.CurrentValue:N0}</td></tr>");
        sb.Append($"<tr><td>Total Returns</td><td>Rs. {report.TotalReturns:N0} ({report.ReturnsPercentage}%)</td></tr>");
        sb.Append($"<tr><td>Total Holdings</td><td>{report.TotalHoldings}</td></tr></table>");
        sb.Append($"<h3>Holdings</h3>");
        sb.Append("<table><tr><th>Fund</th><th>Invested</th><th>Current</th><th>Returns</th></tr>");
        foreach (var h in report.Holdings)
        {
            sb.Append($"<tr><td>{h.FundName}</td><td>Rs. {h.InvestedAmount:N0}</td><td>Rs. {h.CurrentValue:N0}</td><td>{h.ReturnsPercentage}%</td></tr>");
        }
        sb.Append("</table>");
        if (report.Analysis != null)
        {
            sb.Append($"<h3>Analysis</h3>");
            sb.Append($"<table><tr><td>Portfolio Score</td><td>{report.Analysis.PortfolioScore}/100</td></tr>");
            sb.Append($"<tr><td>Diversification</td><td>{report.Analysis.Diversification.Rating} ({report.Analysis.Diversification.Score}/100)</td></tr>");
            sb.Append($"<tr><td>Risk Alignment</td><td>{report.Analysis.RiskAnalysis.Explanation}</td></tr></table>");
            if (report.Analysis.Insights.Any())
            {
                sb.Append("<h4>Insights</h4><ul>");
                foreach (var insight in report.Analysis.Insights)
                    sb.Append($"<li>{insight}</li>");
                sb.Append("</ul>");
            }
        }
        sb.Append($"<p class='disclaimer'>{report.Disclaimer}</p>");
        sb.Append(GetHtmlFooter());
        return sb.ToString();
    }

    public string GenerateStressTestHtml(StressTestReportDTO report)
    {
        var sb = new StringBuilder();
        sb.Append(GetHtmlHeader("Stress Test Report"));
        sb.Append($"<h1>Stress Test Report</h1>");
        sb.Append($"<p class='meta'>Generated: {report.GeneratedAt:dd MMM yyyy} | User: {report.UserName}</p>");
        foreach (var scenario in report.Scenarios)
        {
            sb.Append($"<h3>{scenario.ScenarioName} ({scenario.MarketChange}%)</h3>");
            sb.Append($"<table><tr><td>Current Value</td><td>Rs. {scenario.PortfolioCurrentValue:N0}</td></tr>");
            sb.Append($"<tr><td>Post-Stress Value</td><td>Rs. {scenario.PortfolioPostStressValue:N0}</td></tr>");
            sb.Append($"<tr><td>Impact</td><td>Rs. {scenario.PortfolioImpact:N0} ({scenario.PortfolioImpactPercentage}%)</td></tr>");
            sb.Append($"<tr><td>Est. Recovery</td><td>{scenario.EstimatedRecoveryMonths} months</td></tr></table>");
            sb.Append("<table><tr><th>Fund</th><th>Category</th><th>Impact %</th></tr>");
            foreach (var h in scenario.HoldingImpacts)
                sb.Append($"<tr><td>{h.FundName}</td><td>{h.Category}</td><td>{h.ImpactPercentage}%</td></tr>");
            sb.Append("</table>");
        }
        sb.Append($"<p class='disclaimer'>{report.Disclaimer}</p>");
        sb.Append(GetHtmlFooter());
        return sb.ToString();
    }

    private static string GetHtmlHeader(string title)
    {
        return $@"<!DOCTYPE html><html><head><meta charset='utf-8'><title>{title}</title>
<style>
body {{ font-family: 'Segoe UI', Arial, sans-serif; padding: 40px; max-width: 900px; margin: 0 auto; color: #333; line-height: 1.6; }}
h1 {{ color: #1e40af; border-bottom: 2px solid #2563eb; padding-bottom: 8px; }}
h3 {{ color: #374151; margin-top: 24px; }}
table {{ width: 100%; border-collapse: collapse; margin: 12px 0 24px; font-size: 14px; }}
th, td {{ padding: 10px 12px; border: 1px solid #e5e7eb; text-align: left; }}
th {{ background: #f3f4f6; font-weight: 600; }}
.meta {{ color: #6b7280; font-size: 13px; }}
.score-box {{ text-align: center; background: #eff6ff; border-radius: 12px; padding: 24px; margin: 16px 0; }}
.score-box h2 {{ font-size: 48px; color: #2563eb; margin: 0; }}
.score-box p {{ color: #1e40af; font-size: 18px; font-weight: 500; margin: 4px 0 0; }}
.desc {{ color: #4b5563; line-height: 1.7; }}
.disclaimer {{ font-size: 12px; color: #9ca3af; font-style: italic; margin-top: 32px; border-top: 1px solid #e5e7eb; padding-top: 12px; }}
ul {{ padding-left: 20px; }}
li {{ margin-bottom: 6px; font-size: 14px; }}
@media print {{ body {{ padding: 20px; }} }}
</style></head><body>";
    }

    private static string GetHtmlFooter()
    {
        return "</body></html>";
    }
}
