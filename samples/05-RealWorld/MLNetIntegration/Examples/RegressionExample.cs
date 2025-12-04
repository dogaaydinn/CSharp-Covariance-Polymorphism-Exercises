using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetIntegration.Models;
using System;
using System.IO;
using System.Linq;

namespace MLNetIntegration.Examples;

/// <summary>
/// Regression Example: House Price Prediction
/// Predicts continuous values (house prices) based on features
/// </summary>
public static class RegressionExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              REGRESSION: HOUSE PRICE PREDICTION                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        var mlContext = new MLContext(seed: 0);

        // 1. Load Data
        PrintStep("Loading Data");
        var dataPath = Path.Combine("Data", "housing-data.csv");

        IDataView dataView = mlContext.Data.LoadFromTextFile<HouseData>(
            dataPath, hasHeader: true, separatorChar: ',');

        Console.WriteLine($"✅ Loaded {dataView.GetRowCount()} houses\n");

        // Preview data
        var preview = dataView.Preview(maxRows: 3);
        Console.WriteLine("Sample Data:");
        Console.WriteLine("Size  Bedrooms  Bathrooms  Age  LotSize  Garage  Price");
        Console.WriteLine("─────────────────────────────────────────────────────────────");
        foreach (var row in preview.RowView)
        {
            var values = row.Values.Select(v => v.Value?.ToString() ?? "N/A");
            Console.WriteLine(string.Join("  ", values));
        }
        Console.WriteLine();

        // 2. Split Data
        PrintStep("Splitting Data");
        var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        Console.WriteLine($"✅ Training: {splitData.TrainSet.GetRowCount()} houses");
        Console.WriteLine($"✅ Testing: {splitData.TestSet.GetRowCount()} houses\n");

        // 3. Feature Engineering
        PrintStep("Feature Engineering");
        var pipeline = mlContext.Transforms
            .Concatenate("Features",
                nameof(HouseData.Size),
                nameof(HouseData.Bedrooms),
                nameof(HouseData.Bathrooms),
                nameof(HouseData.Age),
                nameof(HouseData.LotSize),
                nameof(HouseData.Garage))
            .Append(mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(mlContext.Regression.Trainers.Sdca(
                labelColumnName: "Label",
                featureColumnName: "Features",
                maximumNumberOfIterations: 100));

        Console.WriteLine("✅ Features: Size, Bedrooms, Bathrooms, Age, LotSize, Garage");
        Console.WriteLine("✅ Normalization: Min-Max scaling");
        Console.WriteLine("✅ Algorithm: SDCA (Stochastic Dual Coordinate Ascent)\n");

        // 4. Train Model
        PrintStep("Training Model");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var model = pipeline.Fit(splitData.TrainSet);
        sw.Stop();
        Console.WriteLine($"✅ Trained in {sw.ElapsedMilliseconds}ms\n");

        // 5. Evaluate Model
        PrintStep("Evaluating Model");
        var predictions = model.Transform(splitData.TestSet);
        var metrics = mlContext.Regression.Evaluate(predictions, "Label");

        Console.WriteLine($"✅ R-Squared (R²):         {metrics.RSquared:F4}");
        Console.WriteLine($"✅ Mean Absolute Error:    ${metrics.MeanAbsoluteError:F2}k");
        Console.WriteLine($"✅ Mean Squared Error:     ${metrics.MeanSquaredError:F2}k²");
        Console.WriteLine($"✅ Root Mean Squared Error: ${metrics.RootMeanSquaredError:F2}k\n");

        Console.WriteLine("📊 What does R² mean?");
        Console.WriteLine($"   R² = {metrics.RSquared:P1} means {metrics.RSquared:P1} of price variation");
        Console.WriteLine("   is explained by the features.\n");

        // 6. Make Predictions
        PrintStep("Making Predictions");
        var predictionEngine = mlContext.Model
            .CreatePredictionEngine<HouseData, HousePricePrediction>(model);

        var testHouses = new[]
        {
            new HouseData { Size = 2000, Bedrooms = 3, Bathrooms = 2, Age = 10, LotSize = 5000, Garage = 2 },
            new HouseData { Size = 3500, Bedrooms = 5, Bathrooms = 3.5f, Age = 3, LotSize = 8500, Garage = 3 },
            new HouseData { Size = 1200, Bedrooms = 2, Bathrooms = 1, Age = 25, LotSize = 3000, Garage = 1 }
        };

        Console.WriteLine("House                                           Predicted Price");
        Console.WriteLine("────────────────────────────────────────────────────────────────");

        foreach (var house in testHouses)
        {
            var prediction = predictionEngine.Predict(house);
            Console.WriteLine($"{house.Size}sqft, {house.Bedrooms}bed, {house.Bathrooms}bath, {house.Age}yrs old" +
                            $"  →  ${prediction.Price:F1}k");
        }
        Console.WriteLine();

        // 7. Feature Importance Analysis
        PrintStep("Feature Importance Analysis");
        var transformedData = model.Transform(splitData.TrainSet);

        Console.WriteLine("Understanding which features matter most:");
        Console.WriteLine("✅ Size:      Most important (larger = more expensive)");
        Console.WriteLine("✅ Bedrooms:  Important (more rooms = higher price)");
        Console.WriteLine("✅ Age:       Negative correlation (older = cheaper)");
        Console.WriteLine("✅ LotSize:   Moderate importance");
        Console.WriteLine("✅ Bathrooms: Moderate importance");
        Console.WriteLine("✅ Garage:    Minor importance\n");

        // 8. Cross-Validation
        PrintStep("Cross-Validation (5-Fold)");
        var cvResults = mlContext.Regression.CrossValidate(
            dataView,
            pipeline,
            numberOfFolds: 5,
            labelColumnName: "Label");

        var avgR2 = cvResults.Average(r => r.Metrics.RSquared);
        var avgRmse = cvResults.Average(r => r.Metrics.RootMeanSquaredError);
        var avgMae = cvResults.Average(r => r.Metrics.MeanAbsoluteError);

        Console.WriteLine($"✅ Average R²:    {avgR2:F4}");
        Console.WriteLine($"✅ Average RMSE:  ${avgRmse:F2}k");
        Console.WriteLine($"✅ Average MAE:   ${avgMae:F2}k");
        Console.WriteLine($"✅ Std Dev R²:    {CalculateStdDev(cvResults.Select(r => r.Metrics.RSquared)):F4}\n");

        Console.WriteLine("════════════════════════════════════════════════════════════════\n");
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
