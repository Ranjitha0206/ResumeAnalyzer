using Google.GenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;
using ResumeAnalyzer.API.Services;
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
        private readonly IResumeIndexingService _resumeIndexingService;
        private readonly IChatService _chatService;
        private readonly IConfiguration _configuration;
        private readonly IResumeQueryService _resumeQueryService;
        private readonly IResumeSummaryService _resumeSummaryService;
        private readonly IResumeSkillsService _resumeSkillsService;
        public ResumeController(IResumeService resumeService, ITextChnukingService chunkingService, IResumeParserService resumeParserService, IEmbeddingService embeddingService, IVectorStore vectorStore, IResumeIndexingService resumeIndexingService, IChatService chatService, IConfiguration configuration, IResumeQueryService resumeQueryService, IResumeSummaryService resumeSummaryService, IResumeSkillsService resumeSkillsService)
        {
            _resumeService = resumeService;
            _ChunkingService = chunkingService;
            _resumeParserService = resumeParserService;
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
            _resumeIndexingService = resumeIndexingService;
            _chatService = chatService;
            _configuration = configuration;
            _resumeQueryService = resumeQueryService;
            _resumeSummaryService = resumeSummaryService;
            _resumeSkillsService = resumeSkillsService;
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

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(query);

            var results = await _vectorStore.SearchAsync(embedding);

            return Ok(results.Select(r => new
            {
                r.SectionName,
                r.Content
            }));
        }

        [HttpGet("index-status")]
        public async Task<IActionResult> IndexStatus()
        {
            var docs = await _vectorStore.GetAsync();

            return Ok(new
            {
                Count = docs.Count,
                Sections = docs.Select(d => new
                {
                    d.SectionName,
                    Preview = d.Content.Length > 80
                        ? d.Content[..80]
                        : d.Content
                })
            });
        }

        [HttpPost("test-chat")]
        public async Task<IActionResult> TestChat()
        {
            var context = new List<string>
            {
                "The candidate has 3 years of experience in ASP.NET Core.",
                "The candidate has worked with C#, REST APIs, MySQL, JavaScript, AWS S3 and Git.",
                "The candidate has experience building web applications."
            };

            var answer = await _chatService.AskAsync(
                "What are the candidate's skills?",
                context);

            return Ok(new
            {
                Answer = answer
            });
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
            await _resumeIndexingService.IndexResumeAsync(extractedText);
            //var sections = _resumeParserService.ParseResume(extractedText);
            //return Ok(new
            //{
            //    Message = "File uploaded successfully",
            //    ExtractedText = extractedText,
            //    ChunksCount = chunks.Count,
            //    Chunks = chunks
            //});

            //return Ok(
            //    new
            //    {
            //        extractedText = extractedText,
            //        sections = sections
            //    }
            //    );

            return Ok("File uploaded and indexed successfully.");
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

        [HttpGet("models")]
        public async Task<IActionResult> GetModels()
        {
            var client = new Client(apiKey: _configuration["Gemini:ApiKey"]);

            var models = new List<string>();

            var pager = await client.Models.ListAsync();

            await foreach (var model in pager)
            {
                models.Add(model.Name);
            }

            return Ok(models);

        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(AskQuestionRequest request)
        {
            var answer = await _resumeQueryService.AskAsync(request.Question);
            return Ok(new AskQuestionResponse
            {
                Answer = answer
            });
        }

        [HttpPost("summary")]
        public async Task<IActionResult> Summary()
        {
            var summary = await _resumeSummaryService.GenerateSummaryAsync();

            return Ok(new
            {
                Summary = summary
            });
        }

        [HttpPost("skills")]
        public async Task<IActionResult> GetSkills()
        {
            var result = await _resumeSkillsService.GetSkillsAsync();
            return Ok(result);
        }
    }
}
