using Google.GenAI;
using Microsoft.Extensions.Options;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;
using ResumeAnalyzer.API.Prompts;

namespace ResumeAnalyzer.API.Services;

public class GeminiChatService : IChatService
{
    private readonly GeminiSettings _settings;
    private readonly Client _client;

    public GeminiChatService(IOptions<GeminiSettings> settings)
    {
        _settings = settings.Value;
        _client = new Client(apiKey: _settings.ApiKey);
    }
    public async Task<string> AskAsync(string question, IEnumerable<string> context)
    {
        var prompt = ResumePromptBuilder.Build(question, context);

        var response = await _client.Models.GenerateContentAsync(
                model: _settings.ChatModel,
                contents: prompt
            );

        if (response == null || string.IsNullOrWhiteSpace(response.Text))
        {
            throw new Exception("No response returned from Gemini.");
        }

        return response.Text;
    }
}
