using ResumeAnalyzer.API.Interfaces;

namespace ResumeAnalyzer.API.Services;

public class ResumeSkillsService : IResumeSkillsService
{
    private readonly IVectorStore _vectorStore;
    private readonly IChatService _chatService;

    public ResumeSkillsService(
        IVectorStore vectorStore,
        IChatService chatService)
    {
        _vectorStore = vectorStore;
        _chatService = chatService;
    }

    public async Task<string> GetSkillsAsync()
    {
        var documents = await _vectorStore.GetAsync();

        var resumeText = string.Join(
            "\n",
            documents.Select(x => x.Content));

        var prompt = $"""
            List all technical skills mentioned in this resume.

            Group them into categories like:
            - Languages
            - Frameworks
            - Databases
            - Cloud
            - Tools

            Resume:
            {resumeText}
            """;

        return await _chatService.GenerateAsync(prompt);
    }
}
