using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.API.Interfaces;
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
        public ResumeController(IResumeService resumeService, ITextChnukingService chunkingService, IResumeParserService resumeParserService)
        {
            _resumeService = resumeService;
            _ChunkingService = chunkingService;
            _resumeParserService = resumeParserService;
        }

        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new
            {
                Message = "Resume Analyzer API is running"
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
