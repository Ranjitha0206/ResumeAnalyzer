using ResumeAnalyzer.API.Models;
namespace ResumeAnalyzer.API.Interfaces;

public interface IVectorStore
{
    Task AddAsync(VectorDocument document);
    Task<List<VectorDocument>> GetAsync();
    Task ClearAsync();

    Task<List<VectorDocument>> SearchAsync(IReadOnlyList<double> embedding, int topK = 3);
}
