using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class AIChatService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public AIChatService(AppDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _config = config;
        _httpClient = httpClientFactory.CreateClient("OpenAI");
    }

    public async Task<ChatResponseDTO> SendMessage(int userId, string userMessage, string? currentPage = null)
    {
        // Save user message
        _context.ChatMessages.Add(new ChatMessage
        {
            UserId = userId,
            Role = "user",
            Content = userMessage
        });
        await _context.SaveChangesAsync();

        // Get user context for personalized responses
        var userContext = await BuildUserContext(userId, currentPage);

        // Get recent chat history (last 10 messages)
        var history = await _context.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(10)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        // Call OpenAI API
        string reply = await CallOpenAI(userContext, history, userMessage);

        // Save assistant reply
        var chatReply = new ChatMessage
        {
            UserId = userId,
            Role = "assistant",
            Content = reply
        };
        _context.ChatMessages.Add(chatReply);
        await _context.SaveChangesAsync();

        return new ChatResponseDTO
        {
            Reply = reply,
            Timestamp = chatReply.CreatedAt
        };
    }

    public async Task<List<ChatHistoryDTO>> GetChatHistory(int userId, int count = 20)
    {
        return await _context.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatHistoryDTO
            {
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<string> BuildUserContext(int userId, string? currentPage = null)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        var context = "User context: ";
        if (profile != null)
        {
            context += $"Age: {profile.Age}, Occupation: {profile.Occupation}, " +
                       $"Monthly Income: Rs.{profile.MonthlyIncome}, Savings: Rs.{profile.Savings}, " +
                       $"SIP Amount: Rs.{profile.SIPAmount}, Duration: {profile.DurationInYears} years, " +
                       $"Goals: {profile.Goals}. ";
        }
        if (assessment != null)
        {
            context += $"Risk Profile: {assessment.RiskProfile}, Risk Score: {assessment.TotalScore}/100. ";
        }

        // Add current page context for more relevant responses
        if (!string.IsNullOrWhiteSpace(currentPage))
        {
            var pageContext = GetPageContext(currentPage);
            if (!string.IsNullOrEmpty(pageContext))
            {
                context += $"The user is currently on the '{currentPage}' page. {pageContext} ";
            }
        }

        return context;
    }

    private static string GetPageContext(string currentPage)
    {
        return currentPage?.ToLower() switch
        {
            "dashboard" => "They are viewing their investment dashboard with portfolio summary, goals, and recent activity. Prioritize answers about their overall financial health and next steps.",
            "portfolio" => "They are viewing their portfolio holdings and analysis. Prioritize answers about portfolio diversification, rebalancing, and fund performance.",
            "funds" or "fund-list" => "They are browsing mutual funds to discover new investment options. Help them understand fund metrics and selection criteria.",
            "fund-compare" => "They are comparing multiple mutual funds side by side. Help them understand comparison metrics like CAGR, Sharpe ratio, expense ratio, and which fund might suit their profile.",
            "fund-factsheet" => "They are viewing a specific fund's detailed factsheet. Help them understand the fund's metrics, risks, and suitability.",
            "risk-assessment" => "They are taking or reviewing their risk assessment questionnaire. Help them understand risk profiles and what their answers mean for their investments.",
            "sip-calculator" => "They are using the SIP calculator. Help them understand SIP concepts, compounding, and how to choose the right SIP amount.",
            "what-if" => "They are using the what-if scenario simulator. Help them understand how different scenarios affect their investments.",
            "tax-saving" => "They are exploring tax-saving investment options. Help them understand ELSS, Section 80C, and tax-efficient investing.",
            "financial-health" => "They are checking their financial health score. Help them understand what factors affect financial health and how to improve.",
            "stress-test" => "They are running stress tests on their portfolio. Help them understand market risk, drawdowns, and portfolio resilience.",
            "reports" => "They are viewing or generating investment reports. Help them interpret the data and insights in their reports.",
            "watchlist" => "They are managing their fund watchlist. Help them decide which funds to track and when to invest.",
            "onboarding" => "They are setting up or editing their investment profile. Help them understand why each detail matters for recommendations.",
            _ => ""
        };
    }

    private async Task<string> CallOpenAI(string userContext, List<ChatMessage> history, string userMessage)
    {
        var apiKey = _config["OpenAI:ApiKey"];

        // If no API key configured, use fallback responses
        if (string.IsNullOrEmpty(apiKey) || apiKey == "your-openai-api-key-here")
        {
            return GetFallbackResponse(userMessage);
        }

        try
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = "You are a helpful financial education assistant for an Indian mutual fund recommendation platform. " +
                              "You explain investment concepts in simple language. " +
                              "You do NOT give specific buy/sell advice. " +
                              "Always add a disclaimer that your responses are educational and not certified financial advice. " +
                              "Keep responses concise (under 200 words). " +
                              userContext
                }
            };

            // Add conversation history
            foreach (var msg in history.TakeLast(6))
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }

            // Add current message
            messages.Add(new { role = "user", content = userMessage });

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = messages,
                max_tokens = 300,
                temperature = 0.7
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseContent);
                var reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return reply ?? "I apologize, I couldn't generate a response. Please try again.";
            }

            // Log the API error so we can debug
            Console.WriteLine($"[AI Chat] OpenAI API Error {(int)response.StatusCode}: {responseContent}");
            return GetFallbackResponse(userMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AI Chat] Exception calling OpenAI: {ex.Message}");
            return GetFallbackResponse(userMessage);
        }
    }

    private static string GetFallbackResponse(string message)
    {
        var lowerMessage = message.ToLower();

        if (lowerMessage.Contains("sip"))
            return "A SIP (Systematic Investment Plan) is a method of investing a fixed amount regularly in mutual funds. It helps in rupee cost averaging — you buy more units when prices are low and fewer when high. SIPs are great for beginners as they build discipline and reduce the impact of market timing.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("expense ratio"))
            return "Expense ratio is the annual fee charged by a mutual fund to manage your money. It's expressed as a percentage of your investment. For example, a 1% expense ratio means Rs.100 is charged annually for every Rs.10,000 invested. Lower expense ratios mean more returns stay with you.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("nav"))
            return "NAV (Net Asset Value) is the per-unit price of a mutual fund. It's calculated daily by dividing the total value of all assets in the fund by the number of units outstanding. When you invest, you buy units at the current NAV. A higher NAV doesn't mean a fund is expensive — what matters is how much the NAV grows over time.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("risk") || lowerMessage.Contains("conservative") || lowerMessage.Contains("aggressive"))
            return "Risk profiles help determine what mix of investments suits you. Conservative investors prioritize safety (more debt funds), while aggressive investors accept short-term losses for higher long-term growth (more equity). Your risk profile depends on your age, income stability, investment horizon, and personal comfort with market fluctuations.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("small cap") || lowerMessage.Contains("mid cap") || lowerMessage.Contains("large cap") || lowerMessage.Contains("market cap"))
            return "Mutual funds are categorized by the size of companies they invest in:\n\n• **Large Cap**: Top 100 companies (Reliance, TCS, HDFC). Stable, lower risk, moderate returns (12-15% CAGR).\n• **Mid Cap**: Companies ranked 101-250. Higher growth potential, moderate risk (15-20% CAGR).\n• **Small Cap**: Companies ranked 251+. Highest growth potential but volatile (18-25% CAGR in good years, can fall -30% in bad years).\n\nFor beginners, large cap is safest. For long-term (7+ years), a mix of all three works well.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("invest") && (lowerMessage.Contains("plan") || lowerMessage.Contains("india") || lowerMessage.Contains("option") || lowerMessage.Contains("begin") || lowerMessage.Contains("start")))
            return "Popular investment options in India:\n\n• **Mutual Funds (SIP)**: Best for wealth creation. Start with Rs.500/month. Direct Growth plans have lowest fees.\n• **PPF**: 15-year lock-in, tax-free returns (~7.1%), very safe.\n• **Fixed Deposits**: Guaranteed returns (6-7%), but taxable.\n• **NPS**: Retirement-focused, tax benefits under 80CCD.\n• **ELSS**: Tax-saving mutual funds, 3-year lock-in, equity exposure.\n• **Gold (SGBs)**: Sovereign Gold Bonds — 2.5% interest + gold price appreciation.\n\nFor beginners, starting a SIP in a large cap mutual fund is the simplest way to begin.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("mutual fund") && (lowerMessage.Contains("what") || lowerMessage.Contains("explain") || lowerMessage.Contains("how")))
            return "A mutual fund pools money from many investors and invests it in stocks, bonds, or other assets. A professional fund manager decides where to invest. You buy 'units' of the fund at the current NAV (Net Asset Value).\n\n**Why mutual funds?**\n• Professional management\n• Diversification (your money is spread across 50-100 stocks)\n• Start with as little as Rs.500\n• Liquid — you can withdraw anytime (except ELSS)\n• Tax-efficient (LTCG up to Rs.1L is tax-free)\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("hybrid") || lowerMessage.Contains("balanced"))
            return "Hybrid/Balanced funds invest in both equity (stocks) and debt (bonds). They offer a middle ground between growth and stability:\n\n• **Conservative Hybrid**: 75-90% debt + 10-25% equity. Low risk.\n• **Balanced Advantage**: Dynamically shifts between equity and debt based on market conditions. Popular choice.\n• **Aggressive Hybrid**: 65-80% equity + 20-35% debt. Growth-oriented with some cushion.\n\nThey're great for moderate-risk investors who want equity exposure without full volatility.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("direct") && (lowerMessage.Contains("regular") || lowerMessage.Contains("plan") || lowerMessage.Contains("differ")))
            return "**Direct Plan vs Regular Plan:**\n\n• **Direct Plan**: You invest directly with the fund house. No middleman commission. Lower expense ratio (saves 0.5-1.5% annually). Higher returns.\n• **Regular Plan**: Bought through a broker/distributor. Commission is baked into the expense ratio. Lower returns.\n\n**Example (Rs.10L over 10 years):**\n• Direct (14.2% CAGR) → Rs.24.6L\n• Regular (12.8% CAGR) → Rs.22.9L\n• **You save Rs.1.7 Lakhs** with Direct!\n\nAlways choose Direct Growth plans for maximum returns.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("equity") || lowerMessage.Contains("stock"))
            return "Equity mutual funds invest in company stocks. They're categorized by company size: Large Cap (stable, big companies), Mid Cap (growing companies), and Small Cap (smaller companies with high growth potential but more risk). Equity funds are best for long-term goals (5+ years) as they tend to outperform other asset classes over time.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("debt") || lowerMessage.Contains("bond"))
            return "Debt mutual funds invest in government bonds, corporate bonds, and fixed-income instruments. They're safer than equity funds and provide steadier returns (typically 6-8% annually). They're suitable for short-term goals or conservative investors who want stability over high growth.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("gold"))
            return "Gold funds invest in gold through ETFs or fund-of-funds. Gold acts as a hedge against inflation and market crashes — when stock markets fall, gold often rises. A 5-10% allocation to gold provides portfolio diversification and protection during uncertain times.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("tax") || lowerMessage.Contains("elss"))
            return "ELSS (Equity Linked Savings Scheme) funds offer tax benefits under Section 80C of the Income Tax Act. You can save up to Rs.1.5 lakh in taxes annually. ELSS has a mandatory 3-year lock-in period (shortest among 80C options) and invests in equity, offering both tax savings and wealth growth.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        if (lowerMessage.Contains("sharpe"))
            return "The Sharpe Ratio measures how much extra return you get for the risk taken. A higher Sharpe Ratio (above 1) means better risk-adjusted returns. It helps compare funds — if two funds have similar returns, the one with a higher Sharpe Ratio achieved those returns with less volatility.\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";

        return "I can help you understand mutual fund concepts like SIP, NAV, expense ratio, risk profiles, asset allocation, and more. Feel free to ask me anything about investing!\n\n*Disclaimer: This is for educational purposes only, not financial advice.*";
    }
}
