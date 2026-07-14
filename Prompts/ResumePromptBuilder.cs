namespace ResumeAnalyzer.API.Prompts;

public static class ResumePromptBuilder
{
    public static string Build(
        string question,
        IEnumerable<string> context)
    {
        return $"""
            You are an AI Resume Assistant.

            Answer ONLY using the resume context.

            If information is unavailable,
            say

            "I couldn't find that information in the resume."

            Resume Context

            {string.Join("\n\n", context)}

            Question

            {question}
            """;
    }
}
