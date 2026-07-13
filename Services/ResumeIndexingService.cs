using OpenAI.VectorStores;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;

namespace ResumeAnalyzer.API.Services;

public class ResumeIndexingService : IResumeIndexingService
{
    private readonly IResumeParserService _resumeParserService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorstore;

    public ResumeIndexingService(IResumeParserService resumeParserService, IEmbeddingService embeddingService, IVectorStore vectorstore)
    {
        _resumeParserService = resumeParserService;
        _embeddingService = embeddingService;
        _vectorstore = vectorstore;
    }

    public async Task IndexResumeAsync(string resumeText)
    {
        var sections = _resumeParserService.ParseResume(resumeText);

        await _vectorstore.ClearAsync();

        foreach(var section in sections)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(section.Content);

            await _vectorstore.AddAsync(new VectorDocument
            {
                Id = Guid.NewGuid().ToString(),
                SectionName = section.Title,
                Content = section.Content,
                Embedding = embedding
            });

            Console.WriteLine(
    $"Documents after adding: {(await _vectorstore.GetAsync()).Count}");
        }

    }
}
