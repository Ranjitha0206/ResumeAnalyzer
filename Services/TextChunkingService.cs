using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;
using System.Text.RegularExpressions;

namespace ResumeAnalyzer.API.Services;

public class TextChunkingService : ITextChnukingService
{
    public List<string> ChunkText(string text, int chunkSize = 500)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        for (int i=0; i<text.Length; i+= chunkSize)
        {
            int length = Math.Min(chunkSize, text.Length - i);
            chunks.Add(text.Substring(i, length));
        }
        return chunks;
    }

    private readonly List<string> _SectionHeaders = new()
    {
        "Professional Summary",
        "Summary",
        "Profile",
        "Objective",
        "Experience",
        "Work Experience",
        "Professional Experience",
        "Projects",
        "Technical Skills",
        "Skills",
        "Education",
        "Certifications",
        "Achievements",
        "Internship",
        "Internships"
    };

    public List<string> ChunkResumeOld(string resumeText)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(resumeText))
            return chunks;

        string pattern = string.Join("|", _SectionHeaders.Select(Regex.Escape));
        pattern = $"(?=\\b({pattern})\\b)";

        var sections = Regex.Split(resumeText, pattern, RegexOptions.IgnoreCase);
        foreach(var section in sections)
        {
            if(!string.IsNullOrWhiteSpace(section))
            {
                chunks.Add(section.Trim());
            }
        }

        return chunks;
    }

    public List<ResumeSection> ChunkResume(string resumeText)
    {
        var sections = new List<ResumeSection>();

        return sections;
    }
}
