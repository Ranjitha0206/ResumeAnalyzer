using ResumeAnalyzer.API.Models;
namespace ResumeAnalyzer.API.Interfaces;

public interface IVectorStore
{
    Task AddAsync(VectorDocument document);
    Task<List<VectorDocument>> GetAsync();
    Task ClearAsync();
}
