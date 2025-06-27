using Microsoft.ML;
using SesnsitiveDataScan.Models;

namespace SesnsitiveDataScan.Services
{
    public class ContentAnalysisService
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<SentimentData, SentimentPrediction> _sentimentPredictionEngine;

        public ContentAnalysisService()
        {
            _mlContext = new MLContext();
            InitializeSentimentAnalysis();
        }

        private void InitializeSentimentAnalysis()
        {
            var dataPath = "sentiment_model.zip";
            ITransformer mlModel;
            try
            {
                var modelPath = Path.Combine(FileSystem.AppDataDirectory, dataPath);

                if (File.Exists(modelPath))
                {
                    mlModel = _mlContext.Model.Load(modelPath, out _);
                }
                else
                {
                    var sampleData = new List<SentimentData>
                        {
                            new SentimentData { Text = "This is a great product!", Label = true },
                            new SentimentData { Text = "I hate this service.", Label = false },
                            new SentimentData { Text = "The quality was excellent", Label = true },
                            new SentimentData { Text = "Very disappointed with the purchase", Label = false }
                        };

                    var dataView = _mlContext.Data.LoadFromEnumerable(sampleData);
                    var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Text")
                        .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
                    mlModel = pipeline.Fit(dataView);
                    _mlContext.Model.Save(mlModel, dataView.Schema, modelPath);
                }

                _sentimentPredictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(mlModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sentiment model: {ex.Message}");
                var sampleData = new List<SentimentData>
                    {
                        new SentimentData { Text = "This is a great product!", Label = true },
                        new SentimentData { Text = "I hate this service.", Label = false }
                    };

                var dataView = _mlContext.Data.LoadFromEnumerable(sampleData);
                var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Text")
                    .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

                mlModel = pipeline.Fit(dataView);
                _sentimentPredictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(mlModel);
            }
        }

        public async Task<ContentAnalysisResult> AnalyzeContentAsync(string content)
        {
            return await Task.Run(() => AnalyzeContent(content));
        }

        private ContentAnalysisResult AnalyzeContent(string content)
        {
            var result = new ContentAnalysisResult();

            // Basic document statistics
            result.CharacterCount = content.Length;
            result.WordCount = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            result.LineCount = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            // Extract sample text (first 1000 chars for sentiment analysis)
            var sampleText = content.Length > 1000 ? content.Substring(0, 1000) : content;

            // Perform sentiment analysis
            var sentimentPrediction = _sentimentPredictionEngine.Predict(new SentimentData { Text = sampleText });
            result.SentimentScore = sentimentPrediction.Probability;
            result.SentimentLabel = sentimentPrediction.Prediction ? "Positive" : "Negative";

            var words = content.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in words)
            {
                string cleanWord = word.Trim().ToLowerInvariant();
                if (cleanWord.Length > 4 && !IsStopWord(cleanWord)) // Filter short words and common stop words
                {
                    if (wordFrequency.ContainsKey(cleanWord))
                        wordFrequency[cleanWord]++;
                    else
                        wordFrequency[cleanWord] = 1;
                }
            }
            result.KeyPhrases = wordFrequency
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => x.Key)
                .ToList();

            return result;
        }

        private bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "the", "and", "a", "to", "of", "in", "is", "that", "it", "with",
                "for", "as", "was", "on", "are", "this", "be", "by", "have", "or"
            };

            return stopWords.Contains(word);
        }
    }
}
