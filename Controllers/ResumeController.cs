using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Services;
using ResumeAnalyzer.API.Models;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ResumeAnalyzer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : Controller
    {
       
        private readonly IResumeService _resumeService;
        private readonly ITextChnukingService _ChunkingService;
        private readonly IResumeParserService _resumeParserService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorStore _vectorStore;
        public ResumeController(IResumeService resumeService, ITextChnukingService chunkingService, IResumeParserService resumeParserService, IEmbeddingService embeddingService, IVectorStore vectorStore)
        {
            _resumeService = resumeService;
            _ChunkingService = chunkingService;
            _resumeParserService = resumeParserService;
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
        }

        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new
            {
                Message = "Resume Analyzer API is running"
            });
        }

        [HttpGet("test-embedding")]
        public async Task<IActionResult> TestEmbedding()
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(
                "Experienced .NET Developer with ASP.NET Core and SQL");

            return Ok(new
            {
                Count = embedding.Count,
                Sample = embedding.Take(10)
            });
        }

        [HttpGet("test-store")]
        public async Task<IActionResult> TestStore()
        {
            await _vectorStore.AddAsync(
                new VectorDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    SectionName = "Skills",
                    Content = "C#, ASP.NET Core",
                    Embedding = await _embeddingService.GenerateEmbeddingAsync("C#, ASP.NET Core")
                }
            );

            var docs = await _vectorStore.GetAsync();
            return Ok(new
            {
                Count = docs.Count,
                FirstSection = docs.First().SectionName
            });
        }

        [HttpGet("test-search")]
        public async Task<IActionResult> TestSearch()
        {
            await _vectorStore.ClearAsync();

            await _vectorStore.AddAsync(
                new VectorDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    SectionName = "Skills",
                    Content = "C#, ASP.NET Core",
                    Embedding = await _embeddingService.GenerateEmbeddingAsync("C#, ASP.NET Core")
                }
            );

            await _vectorStore.AddAsync(
                new VectorDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    SectionName = "Education",
                    Content = "Bachelor Of Engineering",
                    Embedding = await _embeddingService.GenerateEmbeddingAsync("Bachelor Of Engineering")
                });

           var questionEmbedding =  await _embeddingService.GenerateEmbeddingAsync("What are the skills of the candidate?");

            var results = await _vectorStore.SearchAsync(questionEmbedding);

            return Ok(results.Select(r => new
            {
                r.SectionName,
                r.Content
            }));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = await _resumeService.UploadResumeAsync(file);

            var extractedText = _resumeService.ExtractText( filePath);

            //var chunks = _ChunkingService.ChunkText(extractedText);
            //var chunks = _ChunkingService.ChunkResume(extractedText);
            extractedText = NormalizeResumeText(extractedText);

            var sections = _resumeParserService.ParseResume(extractedText);
            //return Ok(new
            //{
            //    Message = "File uploaded successfully",
            //    ExtractedText = extractedText,
            //    ChunksCount = chunks.Count,
            //    Chunks = chunks
            //});

            return Ok(
                new
                {
                    extractedText = extractedText,
                    sections = sections
                }
                );
        }

        private string NormalizeResumeText(string text)
        {
            var headings = new[]
            {
                "Summary",
                "Skills",
                "Work Experience",
                "Project Work",
                "Education",
                "Training & Certifications"
            };

            foreach (var heading in headings)
            {
                text = Regex.Replace(
                    text,
                    Regex.Escape(heading.Replace(" ", "")),
                    $"\n{heading}\n",
                    RegexOptions.IgnoreCase);

                text = Regex.Replace(
                    text,
                    Regex.Escape(heading),
                    $"\n{heading}\n",
                    RegexOptions.IgnoreCase);
            }

            return text;

        }
    }
}
