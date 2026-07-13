using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;
using System.Text;

namespace ResumeAnalyzer.API.Services;

public class ResumeParserService : IResumeParserService
{
    private readonly HashSet<string> _knownHeadings = new(StringComparer.OrdinalIgnoreCase)
    {
        "SUMMARY",
        "PROFILE",
        "OBJECTIVE",
        "WORK EXPERIENCE",
        "PROFESSIONAL EXPERIENCE",
        "EXPERIENCE",
        "EDUCATION",
        "SKILLS",
        "TECHNICAL SKILLS",
        "PROJECTS",
        "CERTIFICATIONS",
        "ACHIEVEMENTS",
        "LANGUAGES",
        "INTERNSHIPS"
    };

    public List<ResumeSection> ParseResume(string resumeText)
    {
        var sections = new List<ResumeSection>();

        ResumeSection? currentSection = null;
        var contentBuilder = new StringBuilder();

        var lines = resumeText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach(var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if(IsHeading(line))
            {
                if(currentSection != null)
                {
                    currentSection.Content = contentBuilder.ToString().Trim();
                    sections.Add(currentSection);
                }

                currentSection = new ResumeSection { Title = line };

                contentBuilder.Clear();

            }
            else
            {
                if(currentSection == null)
                {
                    currentSection = new ResumeSection { Title = "General" };
                }

                contentBuilder.AppendLine(line);
            }
        }

        if(currentSection != null)
        {
            currentSection.Content = contentBuilder.ToString().Trim();
            sections.Add(currentSection);
        }

        return sections;
    }

    private bool IsHeading(string line)
    {
        return _knownHeadings.Contains(line.Trim());
    }
}
