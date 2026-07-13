using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;

namespace ResumeAnalyzer.API.Services
{
    public class InMemoryVectorStore : IVectorStore
    {
        private readonly List<VectorDocument> _documents = new();

        public Task AddAsync(VectorDocument document)
        {
            _documents.Add(document);
            return Task.CompletedTask; 
        }

        public Task<List<VectorDocument>> GetAsync()
        {
            return Task.FromResult(_documents);
        }

        public Task ClearAsync()
        {
            _documents.Clear();
            return Task.CompletedTask;
        }
    }
}
