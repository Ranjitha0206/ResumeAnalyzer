using Google.GenAI;
using Microsoft.Extensions.Options;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;

namespace ResumeAnalyzer.API.Services;

public class AIEmbeddingService : IEmbeddingService
{

    private readonly GeminiSettings _geminiSettings;
    private readonly Client _client;

    public AIEmbeddingService(IOptions<GeminiSettings> geminiSettings)
    {
        _geminiSettings = geminiSettings.Value;
        _client = new Client(apiKey : _geminiSettings.ApiKey);
    }
    public async Task<IReadOnlyList<double>> GenerateEmbeddingAsync(string text)
    {
        var response = await _client.Models.EmbedContentAsync(
            model: _geminiSettings.EmbeddingModel,
            contents: text
        );

        if (response.Embeddings is null || response.Embeddings.Count == 0)
        {
            throw new Exception("No embedding returned from Gemini.");
        }

        return response.Embeddings[0].Values;
    }
}
