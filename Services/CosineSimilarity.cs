namespace ResumeAnalyzer.API.Services
{
    public class CosineSimilarity
    {
        public static double Calculate(IReadOnlyList<double> vector1,
            IReadOnlyList<double> vector2)
        {
            if (vector1.Count != vector2.Count)
                throw new ArgumentException("Vectors must have same dimensions");

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vector1.Count; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitudeA += vector1[i] * vector1[i];
                magnitudeB += vector2[i] * vector2[i];
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
