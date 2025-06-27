using Microsoft.ML.Data;
using System.Collections.Generic;

namespace SesnsitiveDataScan.Models
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public bool Label { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }
    }

    public class ContentAnalysisResult
    {
        public int CharacterCount { get; set; }
        public int WordCount { get; set; }
        public int LineCount { get; set; }
        public float SentimentScore { get; set; }
        public string SentimentLabel { get; set; }
        public List<string> KeyPhrases { get; set; } = new List<string>();
    }
}