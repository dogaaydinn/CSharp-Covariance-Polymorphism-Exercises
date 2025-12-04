using MLNetIntegration.Examples;
using System;

namespace MLNetIntegration;

/// <summary>
/// ML.NET Integration - Comprehensive Machine Learning Examples
///
/// This sample demonstrates:
/// 1. Binary Classification (Sentiment Analysis)
/// 2. Regression (House Price Prediction)
/// 3. Multi-Model Comparison (Algorithm Selection)
/// 4. Cross-Validation
/// 5. Feature Engineering
/// 6. Model Evaluation & Metrics
///
/// ML.NET is Microsoft's open-source machine learning framework for .NET developers.
/// It enables you to build custom ML models without deep ML expertise.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        while (true)
        {
            PrintMenu();

            Console.Write("\nEnter your choice (1-4, or 0 to exit): ");
            var choice = Console.ReadLine();

            Console.Clear();

            switch (choice)
            {
                case "1":
                    BinaryClassificationExample.Run();
                    break;
                case "2":
                    RegressionExample.Run();
                    break;
                case "3":
                    MultiModelComparisonExample.Run();
                    break;
                case "4":
                    ShowMLNetGuide();
                    break;
                case "0":
                    Console.WriteLine("\n👋 Thanks for exploring ML.NET!\n");
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice. Please try again.\n");
                    continue;
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }

    static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("║           ML.NET INTEGRATION - COMPREHENSIVE DEMO             ║");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("║  Real-world machine learning examples with Microsoft ML.NET   ║");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n📚 EXAMPLES:");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("  1️⃣   Binary Classification: Sentiment Analysis");
        Console.WriteLine("      ├─ Predicts positive/negative sentiment from text");
        Console.WriteLine("      ├─ Includes cross-validation");
        Console.WriteLine("      └─ Evaluation metrics: Accuracy, AUC, F1-Score");
        Console.WriteLine();

        Console.WriteLine("  2️⃣   Regression: House Price Prediction");
        Console.WriteLine("      ├─ Predicts continuous values (house prices)");
        Console.WriteLine("      ├─ Feature engineering & normalization");
        Console.WriteLine("      └─ Evaluation metrics: R², MAE, RMSE");
        Console.WriteLine();

        Console.WriteLine("  3️⃣   Multi-Model Comparison");
        Console.WriteLine("      ├─ Compares 5 different algorithms");
        Console.WriteLine("      ├─ SDCA, FastTree, FastForest, LightGBM, OGD");
        Console.WriteLine("      └─ Algorithm selection guidance");
        Console.WriteLine();

        Console.WriteLine("  4️⃣   ML.NET Best Practices Guide");
        Console.WriteLine("      ├─ When to use ML.NET");
        Console.WriteLine("      ├─ Common pitfalls to avoid");
        Console.WriteLine("      └─ Production deployment tips");
        Console.WriteLine();

        Console.WriteLine("  0️⃣   Exit");
        Console.WriteLine();
    }

    static void ShowMLNetGuide()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                 ML.NET BEST PRACTICES GUIDE                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        PrintSection("When to Use ML.NET");
        Console.WriteLine("✅ Classification problems (spam detection, sentiment analysis)");
        Console.WriteLine("✅ Regression problems (price prediction, forecasting)");
        Console.WriteLine("✅ Anomaly detection (fraud detection, equipment failure)");
        Console.WriteLine("✅ Recommendation systems (product recommendations)");
        Console.WriteLine("✅ Image classification (object detection)");
        Console.WriteLine("✅ When you need ML in .NET applications");
        Console.WriteLine();

        PrintSection("ML.NET vs Other Options");
        Console.WriteLine("ML.NET Advantages:");
        Console.WriteLine("  • Native .NET integration (no Python interop)");
        Console.WriteLine("  • Production-ready performance");
        Console.WriteLine("  • Easy deployment (just .NET runtime)");
        Console.WriteLine("  • Great for tabular data & text");
        Console.WriteLine();
        Console.WriteLine("Consider Alternatives When:");
        Console.WriteLine("  • You need cutting-edge deep learning (use PyTorch/TensorFlow)");
        Console.WriteLine("  • You have Python-heavy ML pipeline already");
        Console.WriteLine("  • You need extensive model interpretability tools");
        Console.WriteLine();

        PrintSection("Common Pitfalls to Avoid");
        Console.WriteLine("❌ DON'T train on all data (causes overfitting)");
        Console.WriteLine("   ✅ DO split data into train/test sets");
        Console.WriteLine();
        Console.WriteLine("❌ DON'T ignore data quality and balance");
        Console.WriteLine("   ✅ DO check for missing values, outliers, class imbalance");
        Console.WriteLine();
        Console.WriteLine("❌ DON'T use PredictionEngine for batch predictions");
        Console.WriteLine("   ✅ DO use Transform() for batches (much faster)");
        Console.WriteLine();
        Console.WriteLine("❌ DON'T forget feature engineering");
        Console.WriteLine("   ✅ DO normalize, encode categoricals, create features");
        Console.WriteLine();
        Console.WriteLine("❌ DON'T pick first algorithm that works");
        Console.WriteLine("   ✅ DO compare multiple algorithms (like example #3)");
        Console.WriteLine();

        PrintSection("Production Deployment");
        Console.WriteLine("1. Model Versioning:");
        Console.WriteLine("   • Save models with version numbers");
        Console.WriteLine("   • Track which data was used for training");
        Console.WriteLine();
        Console.WriteLine("2. Monitoring:");
        Console.WriteLine("   • Track prediction latency");
        Console.WriteLine("   • Monitor prediction distribution");
        Console.WriteLine("   • Set up alerts for model drift");
        Console.WriteLine();
        Console.WriteLine("3. Retraining:");
        Console.WriteLine("   • Schedule periodic retraining");
        Console.WriteLine("   • Retrain when performance degrades");
        Console.WriteLine("   • Use A/B testing for new models");
        Console.WriteLine();

        PrintSection("Performance Tips");
        Console.WriteLine("⚡ For Single Predictions:");
        Console.WriteLine("   • Use PredictionEngine<TIn, TOut>");
        Console.WriteLine("   • Cache the engine (thread-safe)");
        Console.WriteLine();
        Console.WriteLine("⚡ For Batch Predictions:");
        Console.WriteLine("   • Use ITransformer.Transform()");
        Console.WriteLine("   • 10-100x faster than PredictionEngine");
        Console.WriteLine();
        Console.WriteLine("⚡ For Large Datasets:");
        Console.WriteLine("   • Use SDCA or OGD trainers");
        Console.WriteLine("   • Consider data sampling for exploration");
        Console.WriteLine();

        PrintSection("Useful Resources");
        Console.WriteLine("📖 ML.NET Documentation:");
        Console.WriteLine("   https://docs.microsoft.com/dotnet/machine-learning/");
        Console.WriteLine();
        Console.WriteLine("📖 ML.NET Samples:");
        Console.WriteLine("   https://github.com/dotnet/machinelearning-samples");
        Console.WriteLine();
        Console.WriteLine("📖 ML.NET Model Builder (Visual Studio):");
        Console.WriteLine("   Auto-generates ML.NET code from your data");
        Console.WriteLine();

        Console.WriteLine("════════════════════════════════════════════════════════════════\n");
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}
