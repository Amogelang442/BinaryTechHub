using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BinaryTechHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public ChatController(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
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

        var systemInstruction = @"
You are the official AI hardware consultant for 'Binary Tech Hub', South Africa's premier technology and hardware retail platform.
Guidelines:
- All prices are strictly in South African Rand (ZAR / R).
- We offer nationwide 24h express delivery.
- Key categories: Smartphones (Apple iPhone, Samsung Galaxy flagships), Laptops & Computers (MacBooks, gaming laptops), 3D Printers, Audiophile audio/speakers, Gaming consoles, and Peripherals.
- Keep answers direct, friendly, and under 3 to 4 sentences so it displays cleanly in a mobile chat bubble.
- If asked for a product recommendation, suggest 1 or 2 items with realistic ZAR pricing.
";

        var client = _httpClientFactory.CreateClient();
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

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
            var response = await client.PostAsync(requestUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { 
                    reply = $"Google API error ({(int)response.StatusCode}): {responseJson}" 
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