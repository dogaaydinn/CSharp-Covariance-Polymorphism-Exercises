using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetIntegration.Models;
using System;
using System.IO;
using System.Linq;

namespace MLNetIntegration.Examples;

/// <summary>
/// Binary Classification Example: Sentiment Analysis
/// Predicts whether text has positive or negative sentiment
/// </summary>
public static class BinaryClassificationExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         BINARY CLASSIFICATION: SENTIMENT ANALYSIS              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        var mlContext = new MLContext(seed: 0);

        // 1. Load Data
        PrintStep("Loading Data");
        var dataPath = Path.Combine("Data", "sentiment-data.csv");

        if (!File.Exists(dataPath))
        {
            CreateSampleData(dataPath);
        }

        IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
            dataPath, hasHeader: true, separatorChar: ',');

        Console.WriteLine($"✅ Loaded {dataView.GetRowCount()} rows\n");

        // 2. Split Data
        PrintStep("Splitting Data");
        var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        Console.WriteLine($"✅ Training: {splitData.TrainSet.GetRowCount()} rows");
        Console.WriteLine($"✅ Testing: {splitData.TestSet.GetRowCount()} rows\n");

        // 3. Build Pipeline
        PrintStep("Building ML Pipeline");
        var pipeline = mlContext.Transforms.Text
            .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: "Label",
                featureColumnName: "Features"));

        Console.WriteLine("✅ Pipeline: Text Featurization → SDCA Logistic Regression\n");

        // 4. Train Model
        PrintStep("Training Model");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var model = pipeline.Fit(splitData.TrainSet);
        sw.Stop();
        Console.WriteLine($"✅ Trained in {sw.ElapsedMilliseconds}ms\n");

        // 5. Evaluate Model
        PrintStep("Evaluating Model");
        var predictions = model.Transform(splitData.TestSet);
        var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

        Console.WriteLine($"✅ Accuracy:  {metrics.Accuracy:P2}");
        Console.WriteLine($"✅ AUC:       {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"✅ F1 Score:  {metrics.F1Score:P2}");
        Console.WriteLine($"✅ Precision: {metrics.PositivePrecision:P2}");
        Console.WriteLine($"✅ Recall:    {metrics.PositiveRecall:P2}\n");

        // 6. Make Predictions
        PrintStep("Making Predictions");
        var predictionEngine = mlContext.Model
            .CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

        var testCases = new[]
        {
            "This is wonderful and amazing!",
            "Absolutely terrible and disappointing",
            "Not bad, pretty decent overall",
            "Worst experience of my life",
            "I'm very satisfied with this"
        };

        foreach (var text in testCases)
        {
            var prediction = predictionEngine.Predict(new SentimentData { SentimentText = text });
            var sentiment = prediction.Prediction ? "😊 Positive" : "😞 Negative";
            Console.WriteLine($"   \"{text}\"");
            Console.WriteLine($"   → {sentiment} (confidence: {prediction.Probability:P2})\n");
        }

        // 7. Cross-Validation
        PrintStep("Cross-Validation (5-Fold)");
        var cvResults = mlContext.BinaryClassification.CrossValidate(
            dataView,
            pipeline,
            numberOfFolds: 5,
            labelColumnName: "Label");

        var avgAccuracy = cvResults.Average(r => r.Metrics.Accuracy);
        var avgAuc = cvResults.Average(r => r.Metrics.AreaUnderRocCurve);
        var avgF1 = cvResults.Average(r => r.Metrics.F1Score);

        Console.WriteLine($"✅ Average Accuracy:  {avgAccuracy:P2}");
        Console.WriteLine($"✅ Average AUC:       {avgAuc:P2}");
        Console.WriteLine($"✅ Average F1 Score:  {avgF1:P2}");
        Console.WriteLine($"✅ Std Dev Accuracy:  {CalculateStdDev(cvResults.Select(r => r.Metrics.Accuracy)):P2}\n");

        Console.WriteLine("════════════════════════════════════════════════════════════════\n");
    }

    private static void CreateSampleData(string path)
    {
        var sampleData = @"SentimentText,Sentiment
This is a great product!,1
I love this so much!,1
Amazing quality and service!,1
Best purchase I ever made,1
Highly recommended to everyone,1
Excellent experience overall,1
Fantastic product quality,1
Very satisfied with purchase,1
Terrible product quality,0
Complete waste of money,0
Very disappointed with this,0
Do not buy this product,0
Poor quality materials,0
Awful customer experience,0
Not recommended at all,0
Very bad purchase,0";

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, sampleData);
    }

    private static void PrintStep(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }

    private static double CalculateStdDev(IEnumerable<double> values)
    {
        var avg = values.Average();
        var sumOfSquares = values.Sum(v => Math.Pow(v - avg, 2));
        return Math.Sqrt(sumOfSquares / values.Count());
    }
}
