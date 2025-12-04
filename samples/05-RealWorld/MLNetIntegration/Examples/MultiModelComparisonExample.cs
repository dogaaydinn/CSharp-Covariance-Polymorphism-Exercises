using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetIntegration.Models;
using System;
using System.IO;
using System.Linq;

namespace MLNetIntegration.Examples;

/// <summary>
/// Multi-Model Comparison: Compare different ML algorithms
/// Demonstrates how to choose the best model for your problem
/// </summary>
public static class MultiModelComparisonExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           MULTI-MODEL COMPARISON: ALGORITHM SELECTION         ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        var mlContext = new MLContext(seed: 0);

        // Load data
        PrintStep("Loading Data");
        var dataPath = Path.Combine("Data", "housing-data.csv");
        IDataView dataView = mlContext.Data.LoadFromTextFile<HouseData>(
            dataPath, hasHeader: true, separatorChar: ',');

        var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        Console.WriteLine($"✅ Training: {splitData.TrainSet.GetRowCount()} samples");
        Console.WriteLine($"✅ Testing:  {splitData.TestSet.GetRowCount()} samples\n");

        // Define feature engineering (same for all models)
        var featurePipeline = mlContext.Transforms
            .Concatenate("Features",
                nameof(HouseData.Size),
                nameof(HouseData.Bedrooms),
                nameof(HouseData.Bathrooms),
                nameof(HouseData.Age),
                nameof(HouseData.LotSize),
                nameof(HouseData.Garage))
            .Append(mlContext.Transforms.NormalizeMinMax("Features"));

        // Test multiple algorithms
        PrintStep("Training Multiple Algorithms");
        Console.WriteLine();

        // 1. SDCA (Stochastic Dual Coordinate Ascent)
        Console.WriteLine("1️⃣  SDCA (Stochastic Dual Coordinate Ascent)");
        Console.WriteLine("    Fast, works well with large datasets");
        var sdcaModel = TrainAndEvaluate(
            mlContext,
            featurePipeline.Append(mlContext.Regression.Trainers.Sdca(
                labelColumnName: "Label",
                maximumNumberOfIterations: 100)),
            splitData,
            "SDCA");

        // 2. FastTree (Gradient Boosted Trees)
        Console.WriteLine("\n2️⃣  FastTree (Gradient Boosted Decision Trees)");
        Console.WriteLine("    Ensemble method, often very accurate");
        var fastTreeModel = TrainAndEvaluate(
            mlContext,
            featurePipeline.Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                numberOfLeaves: 20,
                numberOfTrees: 100)),
            splitData,
            "FastTree");

        // 3. FastForest (Random Forest)
        Console.WriteLine("\n3️⃣  FastForest (Random Forest)");
        Console.WriteLine("    Robust to overfitting, good default choice");
        var fastForestModel = TrainAndEvaluate(
            mlContext,
            featurePipeline.Append(mlContext.Regression.Trainers.FastForest(
                labelColumnName: "Label",
                numberOfTrees: 100)),
            splitData,
            "FastForest");

        // 4. LightGBM
        Console.WriteLine("\n4️⃣  LightGBM (Light Gradient Boosting Machine)");
        Console.WriteLine("    Fast and accurate, works well with large datasets");
        var lightGbmModel = TrainAndEvaluate(
            mlContext,
            featurePipeline.Append(mlContext.Regression.Trainers.LightGbm(
                labelColumnName: "Label",
                numberOfIterations: 100)),
            splitData,
            "LightGBM");

        // 5. Online Gradient Descent (SGD)
        Console.WriteLine("\n5️⃣  Online Gradient Descent (OGD)");
        Console.WriteLine("    Good for online/streaming scenarios");
        var ogdModel = TrainAndEvaluate(
            mlContext,
            featurePipeline.Append(mlContext.Regression.Trainers.OnlineGradientDescent(
                labelColumnName: "Label")),
            splitData,
            "OGD");

        Console.WriteLine("\n════════════════════════════════════════════════════════════════");
        PrintStep("Comparison Summary");
        Console.WriteLine();

        Console.WriteLine("Algorithm Selection Guide:");
        Console.WriteLine("─────────────────────────────────────────────────────────────");
        Console.WriteLine("📊 BEST ACCURACY:");
        Console.WriteLine("   • Tree-based models (FastTree, FastForest, LightGBM)");
        Console.WriteLine("   • Good for tabular data with complex relationships");
        Console.WriteLine();
        Console.WriteLine("⚡ FASTEST TRAINING:");
        Console.WriteLine("   • SDCA, OGD");
        Console.WriteLine("   • Good for large datasets, real-time scenarios");
        Console.WriteLine();
        Console.WriteLine("🛡️  MOST ROBUST:");
        Console.WriteLine("   • FastForest (Random Forest)");
        Console.WriteLine("   • Less prone to overfitting");
        Console.WriteLine();
        Console.WriteLine("🔄 ONLINE LEARNING:");
        Console.WriteLine("   • OGD (Online Gradient Descent)");
        Console.WriteLine("   • Can update model with new data");
        Console.WriteLine();

        Console.WriteLine("════════════════════════════════════════════════════════════════\n");
    }

    private static (ITransformer Model, RegressionMetrics Metrics, long TrainingTime) TrainAndEvaluate(
        MLContext mlContext,
        IEstimator<ITransformer> pipeline,
        DataOperationsCatalog.TrainTestData splitData,
        string algorithmName)
    {
        // Train
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var model = pipeline.Fit(splitData.TrainSet);
        sw.Stop();

        // Evaluate
        var predictions = model.Transform(splitData.TestSet);
        var metrics = mlContext.Regression.Evaluate(predictions, "Label");

        // Display results
        Console.WriteLine($"    ├─ Training Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"    ├─ R²:            {metrics.RSquared:F4}");
        Console.WriteLine($"    ├─ MAE:           ${metrics.MeanAbsoluteError:F2}k");
        Console.WriteLine($"    └─ RMSE:          ${metrics.RootMeanSquaredError:F2}k");

        return (model, metrics, sw.ElapsedMilliseconds);
    }

    private static void PrintStep(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}
