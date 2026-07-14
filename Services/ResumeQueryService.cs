using Microsoft.Extensions.Logging;
using ResumeAnalyzer.API.Interfaces;

namespace ResumeAnalyzer.API.Services;

public class ResumeQueryService : IResumeQueryService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly IChatService _chatService;
    private readonly ILogger<ResumeQueryService> _logger;
    public ResumeQueryService(
        IEmbeddingService embeddingService,
        IVectorStore vectorStore,
        IChatService chatService,
        ILogger<ResumeQueryService> logger)
    {
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
        _chatService = chatService;
        _logger = logger;
    }

    public async Task<string> AskAsync(string question)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question required");

            _logger.LogInformation("Received question: {Question}", question);
            var embedding =
                await _embeddingService.GenerateEmbeddingAsync(question);

            var documents =
                await _vectorStore.SearchAsync(embedding, 3);

            _logger.LogInformation("Retrieved {Count} relevant chunks", documents.Count);
            var context =
                documents.Select(d => d.Content);

            var answer = await _chatService.AskAsync(question, context);

            _logger.LogInformation("Generated answer successfully.");

            return answer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Resume query failed");

            throw;
        }
    }
}
