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

        public Task<List<VectorDocument>> SearchAsync(IReadOnlyList<double> queryEmbedding, int topK =3)
        {
            var results = _documents.Select(document => new
            {
                Document = document,
                Score = CosineSimilarity.Calculate(queryEmbedding, document.Embedding)
            })
            .OrderByDescending(x=>x.Score)
            .Take(topK)
            .Select(x=>x.Document)
            .ToList();

            Console.WriteLine($"Documents in search: {_documents.Count}");
            return Task.FromResult(results);
        }
    }
}
