using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;
using System.Text;
using System.Text.Json;

namespace BinaryTechHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;

    public ChatController(IConfiguration config, IHttpClientFactory httpClientFactory, AppDbContext db)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> AskAssistant([FromBody] ChatPromptDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            return BadRequest(new { reply = "Please provide a valid question." });
        }

        var apiKey = _config["GEMINI_API_KEY"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            return StatusCode(500, new { reply = "AI assistant service is currently unconfigured (missing GEMINI_API_KEY)." });
        }

        // 1. Fetch current live catalog from Aiven MySQL
        string catalogSummary;
        try
        {
            var products = await _db.Products
                .AsNoTracking()
                .Select(p => $"{p.Name} (ID: {p.ProductId}) - R{p.Price:N0} | Specs: {p.Specs}")
                .ToListAsync();

            catalogSummary = string.Join("\n", products);
        }
        catch
        {
            // Fallback summary in case database query times out
            catalogSummary = "Standard catalog available in store.";
        }

        // 2. Strict system instruction grounding the AI to your exact products
        var systemInstruction = $@"
You are the official shopping consultant for 'Binary Tech Hub', South Africa's tech retailer.
Store Policies:
- Currency: Strictly South African Rand (ZAR / R).
- Delivery: Express 24-hour nationwide delivery across South Africa.
- Rule: ONLY recommend products from the official inventory listed below. Do not invent products or suggest items not in this list.
- When recommending a product, mention its exact name, its price in Rands, and why it matches the customer's budget/needs.
- Keep responses concise (2 to 4 sentences max) for clear display in a mobile chat window.

Current Store Inventory:
{catalogSummary}
";

        var client = _httpClientFactory.CreateClient();
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.8-flash:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new { 
                    role = "user", 
                    parts = new[] { 
                        new { text = $"{systemInstruction}\n\nCustomer question: {dto.Message}" } 
                    } 
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage? response = null;
            string responseJson = string.Empty;
            int maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                response = await client.PostAsync(requestUrl, content);
                responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) break;

                if ((int)response.StatusCode == 503 && attempt < maxRetries)
                {
                    await Task.Delay(1500);
                    continue;
                }

                break;
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                return StatusCode((int)(response?.StatusCode ?? System.Net.HttpStatusCode.BadGateway), new { 
                    reply = $"Service temporarily busy. Please try again." 
                });
            }

            using var doc = JsonDocument.Parse(responseJson);
            
            var replyText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new { reply = replyText });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { reply = "Error communicating with AI service.", error = ex.Message });
        }
    }
}

public class ChatPromptDto
{
    public string Message { get; set; } = string.Empty;
}