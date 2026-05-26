using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CineTrack.Infrastructure.Services
{
    /// <summary>
    /// Service for AI-powered recommendations using Groq API
    /// </summary>
    public class GroqAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GroqAiService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public GroqAiService(HttpClient httpClient, IConfiguration configuration, ILogger<GroqAiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = configuration["GroqApi:ApiKey"] ?? throw new InvalidOperationException("Groq API key not configured");
            _model = configuration["GroqApi:Model"] ?? "mixtral-8x7b-32768";
        }

        public async Task<string> GetMovieRecommendationsAsync(string userMood, List<string> favoriteGenres, int count = 5)
        {
            try
            {
                var prompt = $@"Based on the following preferences, recommend {count} movies:
- Current mood: {userMood}
- Favorite genres: {string.Join(", ", favoriteGenres)}

Provide recommendations in JSON format with movie titles, brief descriptions, and why they match the mood.";

                return await GetAiResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AI recommendation error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GenerateChatbotResponseAsync(string userMessage, string context = "")
        {
            try
            {
                var prompt = $@"You are a helpful movie recommendation chatbot for CineTrack.
{(string.IsNullOrEmpty(context) ? "" : $"Context: {context}")}

User message: {userMessage}

Provide a helpful response about movies or recommendations.";

                return await GetAiResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Chatbot error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GenerateReviewSummaryAsync(List<string> reviews)
        {
            try
            {
                var prompt = $@"Summarize the following movie reviews in 2-3 sentences highlighting key points:
{string.Join("\n", reviews)}";

                return await GetAiResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Review summary error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetAiResponseAsync(string prompt)
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 500
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response generated";
        }
    }

    public interface IAiService
    {
        Task<string> GetMovieRecommendationsAsync(string userMood, List<string> favoriteGenres, int count = 5);
        Task<string> GenerateChatbotResponseAsync(string userMessage, string context = "");
        Task<string> GenerateReviewSummaryAsync(List<string> reviews);
    }
}