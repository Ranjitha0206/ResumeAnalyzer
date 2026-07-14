using ResumeAnalyzer.API.Interfaces;

namespace ResumeAnalyzer.API.Services;

public class ResumeSummaryService : IResumeSummaryService
{
    private readonly IVectorStore _vectorStore;
    private readonly IChatService _chatService;

    public ResumeSummaryService(
        IVectorStore vectorStore,
        IChatService chatService)
    {
        _vectorStore = vectorStore;
        _chatService = chatService;
    }

    public async Task<string> GenerateSummaryAsync()
    {
        var documents = await _vectorStore.GetAsync();

        var resumeText = string.Join(
            "\n",
            documents.Select(x => x.Content));

        var prompt = $"""
            Summarize this resume professionally in about 150 words.

            Resume:
            {resumeText}
            """;

        return await _chatService.GenerateAsync(prompt);
    }
}
