# ML.NET Integration Guide

## Overview

ML.NET is a cross-platform, open-source machine learning framework for .NET developers. This guide covers integrating ML.NET into your C# applications for various machine learning scenarios.

## Table of Contents

- [What is ML.NET?](#what-is-mlnet)
- [Getting Started](#getting-started)
- [Classification](#classification)
- [Regression](#regression)
- [Clustering](#clustering)
- [Anomaly Detection](#anomaly-detection)
- [Time Series Forecasting](#time-series-forecasting)
- [ONNX Runtime Integration](#onnx-runtime-integration)
- [Model Deployment](#model-deployment)
- [Performance Optimization](#performance-optimization)
- [Best Practices](#best-practices)

## What is ML.NET?

ML.NET enables .NET developers to develop and integrate custom machine learning models in their applications without prior ML expertise.

### Key Features
- ✅ **Cross-platform**: Windows, Linux, macOS
- ✅ **Multiple scenarios**: Classification, regression, clustering, anomaly detection, forecasting
- ✅ **AutoML**: Automated machine learning
- ✅ **ONNX support**: Use pre-trained models
- ✅ **Production-ready**: Optimized for .NET performance

### Supported Tasks
- Binary Classification (spam detection, sentiment analysis)
- Multiclass Classification (image classification, categorization)
- Regression (price prediction, forecasting)
- Clustering (customer segmentation)
- Anomaly Detection (fraud detection)
- Ranking (recommendation systems)
- Time Series (forecasting)

## Getting Started

### Installation

```xml
<ItemGroup>
  <!-- Core ML.NET -->
  <PackageReference Include="Microsoft.ML" Version="3.0.1" />

  <!-- Additional components -->
  <PackageReference Include="Microsoft.ML.AutoML" Version="0.21.1" />
  <PackageReference Include="Microsoft.ML.Vision" Version="3.0.1" />
  <PackageReference Include="Microsoft.ML.ImageAnalytics" Version="3.0.1" />
  <PackageReference Include="Microsoft.ML.TimeSeries" Version="3.0.1" />

  <!-- ONNX support -->
  <PackageReference Include="Microsoft.ML.OnnxRuntime" Version="1.16.3" />
  <PackageReference Include="Microsoft.ML.OnnxTransformer" Version="3.0.1" />
</ItemGroup>
```

### Basic Workflow

```csharp
using Microsoft.ML;

public class MLWorkflow
{
    public static void BasicWorkflow()
    {
        // 1. Create ML Context
        var mlContext = new MLContext(seed: 0);

        // 2. Load Data
        var data = mlContext.Data.LoadFromTextFile<ModelInput>("data.csv", separatorChar: ',');

        // 3. Build Pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", "Feature1", "Feature2")
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label"));

        // 4. Train Model
        var model = pipeline.Fit(data);

        // 5. Make Predictions
        var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        var prediction = predictionEngine.Predict(new ModelInput { Feature1 = 1.0f, Feature2 = 2.0f });

        // 6. Save Model
        mlContext.Model.Save(model, data.Schema, "model.zip");
    }
}
```

## Classification

### Binary Classification - Sentiment Analysis

```csharp
using Microsoft.ML;
using Microsoft.ML.Data;

// Input data model
public class SentimentData
{
    [LoadColumn(0)]
    public string Text { get; set; }

    [LoadColumn(1)]
    public bool Sentiment { get; set; }
}

// Prediction model
public class SentimentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    public float Probability { get; set; }

    public float Score { get; set; }
}

public class SentimentAnalysis
{
    private readonly MLContext _mlContext;
    private ITransformer _model;

    public SentimentAnalysis()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public void TrainModel(string dataPath)
    {
        // Load data
        var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(
            dataPath,
            separatorChar: ',',
            hasHeader: true);

        // Split data
        var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        // Build pipeline
        var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(SentimentData.Text))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(SentimentData.Sentiment),
                featureColumnName: "Features"));

        // Train model
        Console.WriteLine("Training model...");
        _model = pipeline.Fit(splitData.TrainSet);

        // Evaluate model
        var predictions = _model.Transform(splitData.TestSet);
        var metrics = _mlContext.BinaryClassification.Evaluate(predictions);

        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
    }

    public SentimentPrediction Predict(string text)
    {
        var predictionEngine = _mlContext.Model
            .CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);

        return predictionEngine.Predict(new SentimentData { Text = text });
    }

    public void SaveModel(string path)
    {
        _mlContext.Model.Save(_model, null, path);
    }

    public void LoadModel(string path)
    {
        _model = _mlContext.Model.Load(path, out _);
    }
}

// Usage
var sentimentAnalyzer = new SentimentAnalysis();
sentimentAnalyzer.TrainModel("sentiment_data.csv");

var prediction = sentimentAnalyzer.Predict("This product is amazing!");
Console.WriteLine($"Sentiment: {(prediction.Prediction ? "Positive" : "Negative")}");
Console.WriteLine($"Confidence: {prediction.Probability:P2}");
```

### Multiclass Classification - Issue Categorization

```csharp
// Input model
public class IssueData
{
    [LoadColumn(0)]
    public string Title { get; set; }

    [LoadColumn(1)]
    public string Description { get; set; }

    [LoadColumn(2)]
    public string Area { get; set; } // Label: bug, feature, question, etc.
}

// Prediction model
public class IssuePrediction
{
    [ColumnName("PredictedLabel")]
    public string Area { get; set; }

    public float[] Score { get; set; }
}

public class IssueClassification
{
    private readonly MLContext _mlContext;

    public IssueClassification()
    {
        _mlContext = new MLContext();
    }

    public ITransformer TrainModel(string dataPath)
    {
        var dataView = _mlContext.Data.LoadFromTextFile<IssueData>(
            dataPath,
            separatorChar: '\t',
            hasHeader: true);

        var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "Label",
                inputColumnName: nameof(IssueData.Area))
            .Append(_mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "TitleFeaturized",
                inputColumnName: nameof(IssueData.Title)))
            .Append(_mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "DescriptionFeaturized",
                inputColumnName: nameof(IssueData.Description)))
            .Append(_mlContext.Transforms.Concatenate(
                "Features",
                "TitleFeaturized",
                "DescriptionFeaturized"))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var model = pipeline.Fit(dataView);

        // Evaluate
        var predictions = model.Transform(dataView);
        var metrics = _mlContext.MulticlassClassification.Evaluate(predictions);

        Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy:P2}");
        Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy:P2}");
        Console.WriteLine($"Log Loss: {metrics.LogLoss:F2}");

        return model;
    }
}
```

## Regression

### Price Prediction

```csharp
// Input model
public class HousingData
{
    [LoadColumn(0)]
    public float Size { get; set; }

    [LoadColumn(1)]
    public float Bedrooms { get; set; }

    [LoadColumn(2)]
    public float Age { get; set; }

    [LoadColumn(3)]
    public float Location { get; set; }

    [LoadColumn(4)]
    public float Price { get; set; } // Label
}

// Prediction model
public class HousingPrediction
{
    [ColumnName("Score")]
    public float Price { get; set; }
}

public class PricePrediction
{
    private readonly MLContext _mlContext;

    public PricePrediction()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public ITransformer TrainModel(IDataView trainingData)
    {
        var pipeline = _mlContext.Transforms.Concatenate(
                "Features",
                nameof(HousingData.Size),
                nameof(HousingData.Bedrooms),
                nameof(HousingData.Age),
                nameof(HousingData.Location))
            .Append(_mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(HousingData.Price),
                featureColumnName: "Features"));

        var model = pipeline.Fit(trainingData);

        // Evaluate
        var predictions = model.Transform(trainingData);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        Console.WriteLine($"R²: {metrics.RSquared:0.##}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:#.##}");
        Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:#.##}");

        return model;
    }

    // Use AutoML for automatic algorithm selection
    public ITransformer AutoTrainModel(IDataView trainingData)
    {
        var experimentSettings = new Microsoft.ML.AutoML.RegressionExperimentSettings
        {
            MaxExperimentTimeInSeconds = 60,
            OptimizingMetric = Microsoft.ML.AutoML.RegressionMetric.RSquared
        };

        var experiment = _mlContext.Auto()
            .CreateRegressionExperiment(experimentSettings);

        var result = experiment.Execute(
            trainingData,
            labelColumnName: nameof(HousingData.Price));

        Console.WriteLine($"Best trainer: {result.BestRun.TrainerName}");
        Console.WriteLine($"R²: {result.BestRun.ValidationMetrics.RSquared:0.##}");

        return result.BestRun.Model;
    }
}
```

## Clustering

### Customer Segmentation

```csharp
// Input model
public class CustomerData
{
    [LoadColumn(0)]
    public float Age { get; set; }

    [LoadColumn(1)]
    public float AnnualIncome { get; set; }

    [LoadColumn(2)]
    public float SpendingScore { get; set; }
}

// Prediction model
public class CustomerSegmentation
{
    [ColumnName("PredictedLabel")]
    public uint ClusterId { get; set; }

    [ColumnName("Score")]
    public float[] Distances { get; set; }
}

public class CustomerClustering
{
    private readonly MLContext _mlContext;

    public CustomerClustering()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public ITransformer TrainModel(IDataView trainingData, int numberOfClusters = 3)
    {
        var pipeline = _mlContext.Transforms.Concatenate(
                "Features",
                nameof(CustomerData.Age),
                nameof(CustomerData.AnnualIncome),
                nameof(CustomerData.SpendingScore))
            .Append(_mlContext.Clustering.Trainers.KMeans(
                featureColumnName: "Features",
                numberOfClusters: numberOfClusters));

        var model = pipeline.Fit(trainingData);

        // Evaluate
        var predictions = model.Transform(trainingData);
        var metrics = _mlContext.Clustering.Evaluate(predictions);

        Console.WriteLine($"Average Distance: {metrics.AverageDistance}");
        Console.WriteLine($"Davies Bouldin Index: {metrics.DaviesBouldinIndex}");

        return model;
    }

    public void AnalyzeClusters(ITransformer model, IDataView data)
    {
        var predictions = model.Transform(data);
        var clusterData = _mlContext.Data
            .CreateEnumerable<CustomerSegmentation>(predictions, reuseRowObject: false);

        // Group by cluster
        var clusters = clusterData
            .GroupBy(x => x.ClusterId)
            .OrderBy(g => g.Key);

        foreach (var cluster in clusters)
        {
            Console.WriteLine($"\nCluster {cluster.Key}:");
            Console.WriteLine($"  Size: {cluster.Count()}");
            Console.WriteLine($"  Avg Distance: {cluster.Average(c => c.Distances[0]):F2}");
        }
    }
}
```

## Anomaly Detection

### Spike and Change Point Detection

```csharp
// Input model
public class SalesData
{
    [LoadColumn(0)]
    public string Month { get; set; }

    [LoadColumn(1)]
    public float Sales { get; set; }
}

// Prediction models
public class SpikeDetection
{
    [VectorType(3)]
    public double[] Prediction { get; set; }
}

public class ChangePointDetection
{
    [VectorType(4)]
    public double[] Prediction { get; set; }
}

public class AnomalyDetector
{
    private readonly MLContext _mlContext;

    public AnomalyDetector()
    {
        _mlContext = new MLContext();
    }

    // Detect sudden spikes
    public void DetectSpikes(IDataView data)
    {
        var pipeline = _mlContext.Transforms.DetectIidSpike(
            outputColumnName: nameof(SpikeDetection.Prediction),
            inputColumnName: nameof(SalesData.Sales),
            confidence: 95,
            pvalueHistoryLength: 30);

        var model = pipeline.Fit(data);
        var transformedData = model.Transform(data);

        var predictions = _mlContext.Data
            .CreateEnumerable<SpikeDetection>(transformedData, reuseRowObject: false);

        var results = predictions.Select((p, i) => new
        {
            Index = i,
            IsSpike = p.Prediction[0] == 1,
            Score = p.Prediction[1],
            PValue = p.Prediction[2]
        }).Where(x => x.IsSpike);

        foreach (var result in results)
        {
            Console.WriteLine($"Spike detected at index {result.Index}");
            Console.WriteLine($"  Score: {result.Score:F2}, P-Value: {result.PValue:F4}");
        }
    }

    // Detect change points (trend changes)
    public void DetectChangePoints(IDataView data)
    {
        var pipeline = _mlContext.Transforms.DetectIidChangePoint(
            outputColumnName: nameof(ChangePointDetection.Prediction),
            inputColumnName: nameof(SalesData.Sales),
            confidence: 95,
            changeHistoryLength: 30);

        var model = pipeline.Fit(data);
        var transformedData = model.Transform(data);

        var predictions = _mlContext.Data
            .CreateEnumerable<ChangePointDetection>(transformedData, reuseRowObject: false);

        var results = predictions.Select((p, i) => new
        {
            Index = i,
            IsChangePoint = p.Prediction[0] == 1,
            Score = p.Prediction[1],
            PValue = p.Prediction[2],
            Martingale = p.Prediction[3]
        }).Where(x => x.IsChangePoint);

        foreach (var result in results)
        {
            Console.WriteLine($"Change point detected at index {result.Index}");
            Console.WriteLine($"  Score: {result.Score:F2}, P-Value: {result.PValue:F4}");
        }
    }
}
```

## Time Series Forecasting

```csharp
// Input model
public class TimeSeriesData
{
    public DateTime Date { get; set; }
    public float Value { get; set; }
}

// Prediction model
public class TimeSeriesPrediction
{
    public float[] ForecastedValues { get; set; }
    public float[] LowerBoundValues { get; set; }
    public float[] UpperBoundValues { get; set; }
}

public class TimeSeriesForecaster
{
    private readonly MLContext _mlContext;

    public TimeSeriesForecaster()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public void ForecastSales(IDataView data, int horizon = 7)
    {
        // Use Singular Spectrum Analysis (SSA)
        var pipeline = _mlContext.Forecasting.ForecastBySsa(
            outputColumnName: nameof(TimeSeriesPrediction.ForecastedValues),
            inputColumnName: nameof(TimeSeriesData.Value),
            windowSize: 30,
            seriesLength: 90,
            trainSize: data.GetRowCount() ?? 0,
            horizon: horizon,
            confidenceLevel: 0.95f,
            confidenceLowerBoundColumn: nameof(TimeSeriesPrediction.LowerBoundValues),
            confidenceUpperBoundColumn: nameof(TimeSeriesPrediction.UpperBoundValues));

        var model = pipeline.Fit(data);

        // Create forecasting engine
        var forecastEngine = model.CreateTimeSeriesEngine<TimeSeriesData, TimeSeriesPrediction>(_mlContext);

        // Generate forecast
        var forecast = forecastEngine.Predict();

        Console.WriteLine("Sales Forecast:");
        for (int i = 0; i < horizon; i++)
        {
            Console.WriteLine($"Day {i + 1}: {forecast.ForecastedValues[i]:F2} " +
                            $"(±{forecast.UpperBoundValues[i] - forecast.ForecastedValues[i]:F2})");
        }

        // Save model for later use
        forecastEngine.CheckPoint(_mlContext, "forecasting_model.zip");
    }
}
```

## ONNX Runtime Integration

### Using Pre-trained ONNX Models

```csharp
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

public class OnnxModelInference
{
    private readonly InferenceSession _session;

    public OnnxModelInference(string modelPath)
    {
        _session = new InferenceSession(modelPath);
    }

    // Image classification example
    public float[] ClassifyImage(float[] imageData, int batchSize, int channels, int height, int width)
    {
        // Create input tensor
        var inputTensor = new DenseTensor<float>(
            imageData,
            new[] { batchSize, channels, height, width });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input", inputTensor)
        };

        // Run inference
        using var results = _session.Run(inputs);

        // Get output
        var output = results.First().AsEnumerable<float>().ToArray();

        return output;
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}

// ML.NET integration with ONNX
public class OnnxWithMLNet
{
    private readonly MLContext _mlContext;

    public OnnxWithMLNet()
    {
        _mlContext = new MLContext();
    }

    public void UseOnnxModel(string modelPath, IDataView data)
    {
        // Create pipeline with ONNX model
        var pipeline = _mlContext.Transforms.ApplyOnnxModel(
            modelFile: modelPath,
            outputColumnName: "output",
            inputColumnName: "input");

        // Transform data through ONNX model
        var transformedData = pipeline.Fit(data).Transform(data);

        // Use predictions
        var predictions = _mlContext.Data
            .CreateEnumerable<OnnxOutput>(transformedData, reuseRowObject: false);
    }
}

public class OnnxOutput
{
    [VectorType]
    public float[] Output { get; set; }
}
```

## Model Deployment

### ASP.NET Core Integration

```csharp
// Startup.cs or Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register ML model as singleton
        builder.Services.AddSingleton<MLContext>(sp =>
        {
            return new MLContext(seed: 0);
        });

        builder.Services.AddSingleton<PredictionEnginePool<SentimentData, SentimentPrediction>>(sp =>
        {
            var mlContext = sp.GetRequiredService<MLContext>();
            var model = mlContext.Model.Load("sentiment_model.zip", out _);
            return new PredictionEnginePool<SentimentData, SentimentPrediction>(mlContext, model);
        });

        builder.Services.AddControllers();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEngine;

    public PredictionController(
        PredictionEnginePool<SentimentData, SentimentPrediction> predictionEngine)
    {
        _predictionEngine = predictionEngine;
    }

    [HttpPost("sentiment")]
    public ActionResult<SentimentPrediction> PredictSentiment([FromBody] SentimentRequest request)
    {
        var input = new SentimentData { Text = request.Text };
        var prediction = _predictionEngine.Predict(input);

        return Ok(new
        {
            sentiment = prediction.Prediction ? "Positive" : "Negative",
            confidence = prediction.Probability
        });
    }
}

public record SentimentRequest(string Text);
```

### Model Versioning

```csharp
public class ModelRegistry
{
    private readonly Dictionary<string, (ITransformer Model, string Version)> _models = new();
    private readonly MLContext _mlContext;

    public ModelRegistry(MLContext mlContext)
    {
        _mlContext = mlContext;
    }

    public void RegisterModel(string name, string modelPath, string version)
    {
        var model = _mlContext.Model.Load(modelPath, out _);
        _models[name] = (model, version);
    }

    public ITransformer GetModel(string name)
    {
        return _models.TryGetValue(name, out var entry) ? entry.Model : null;
    }

    public string GetModelVersion(string name)
    {
        return _models.TryGetValue(name, out var entry) ? entry.Version : null;
    }
}
```

## Performance Optimization

### Batch Predictions

```csharp
public class BatchPrediction
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;

    public BatchPrediction(MLContext mlContext, ITransformer model)
    {
        _mlContext = mlContext;
        _model = model;
    }

    // Efficient batch prediction
    public IEnumerable<SentimentPrediction> PredictBatch(IEnumerable<SentimentData> inputs)
    {
        // Create data view from inputs
        var inputData = _mlContext.Data.LoadFromEnumerable(inputs);

        // Transform all at once (much faster than individual predictions)
        var predictions = _model.Transform(inputData);

        // Return results
        return _mlContext.Data
            .CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
    }
}
```

### GPU Acceleration

```csharp
public class GpuTraining
{
    public ITransformer TrainOnGpu(IDataView trainingData)
    {
        var mlContext = new MLContext(seed: 0);

        // Use GPU-accelerated trainer (requires CUDA)
        var pipeline = mlContext.Transforms.Concatenate("Features", "Feature1", "Feature2")
            .Append(mlContext.Regression.Trainers.LightGbm(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfIterations: 100,
                numberOfLeaves: 31,
                minimumExampleCountPerLeaf: 20,
                learningRate: 0.1,
                useCategoricalSplit: false,
                handleMissingValue: false,
                useSoftmax: false,
                verbose: true,
                device: "gpu")); // Enable GPU

        return pipeline.Fit(trainingData);
    }
}
```

### Caching Predictions

```csharp
using Microsoft.Extensions.Caching.Memory;

public class CachedPredictionService
{
    private readonly IMemoryCache _cache;
    private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEngine;

    public CachedPredictionService(
        IMemoryCache cache,
        PredictionEnginePool<SentimentData, SentimentPrediction> predictionEngine)
    {
        _cache = cache;
        _predictionEngine = predictionEngine;
    }

    public SentimentPrediction Predict(string text)
    {
        var cacheKey = $"prediction_{text.GetHashCode()}";

        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var input = new SentimentData { Text = text };
            return _predictionEngine.Predict(input);
        });
    }
}
```

## Best Practices

### 1. Data Preparation
- ✅ Clean and normalize data
- ✅ Handle missing values
- ✅ Split train/test/validation sets
- ✅ Balance datasets for classification
- ✅ Feature engineering

### 2. Model Training
- ✅ Use cross-validation
- ✅ Try multiple algorithms (or use AutoML)
- ✅ Tune hyperparameters
- ✅ Monitor overfitting
- ✅ Track metrics over time

### 3. Deployment
- ✅ Use PredictionEnginePool for thread-safety
- ✅ Batch predictions when possible
- ✅ Cache frequently requested predictions
- ✅ Version your models
- ✅ Monitor model performance in production

### 4. Performance
- ✅ Use appropriate data types (float vs double)
- ✅ Minimize allocations
- ✅ Consider GPU training for large datasets
- ✅ Profile and benchmark
- ✅ Use Native AOT for deployment

## Integration Example

Complete example integrating ML.NET with the project:

```csharp
// src/AdvancedConcepts.ML/BenchmarkPredictor.cs
namespace AdvancedConcepts.ML;

public class BenchmarkData
{
    public float ArraySize { get; set; }
    public float Iterations { get; set; }
    public float ThreadCount { get; set; }
    public float ExecutionTime { get; set; } // Label
}

public class BenchmarkPrediction
{
    [ColumnName("Score")]
    public float PredictedTime { get; set; }
}

public class BenchmarkPredictor
{
    private readonly MLContext _mlContext;
    private ITransformer _model;

    public BenchmarkPredictor()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public void TrainFromBenchmarkResults(string csvPath)
    {
        var data = _mlContext.Data.LoadFromTextFile<BenchmarkData>(
            csvPath,
            separatorChar: ',',
            hasHeader: true);

        var pipeline = _mlContext.Transforms.Concatenate(
                "Features",
                nameof(BenchmarkData.ArraySize),
                nameof(BenchmarkData.Iterations),
                nameof(BenchmarkData.ThreadCount))
            .Append(_mlContext.Regression.Trainers.FastTree());

        _model = pipeline.Fit(data);

        var predictions = _model.Transform(data);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        Console.WriteLine($"R²: {metrics.RSquared:0.##}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:#.##}ms");
    }

    public float PredictExecutionTime(float arraySize, float iterations, float threadCount)
    {
        var predictionEngine = _mlContext.Model
            .CreatePredictionEngine<BenchmarkData, BenchmarkPrediction>(_model);

        var input = new BenchmarkData
        {
            ArraySize = arraySize,
            Iterations = iterations,
            ThreadCount = threadCount
        };

        var prediction = predictionEngine.Predict(input);
        return prediction.PredictedTime;
    }
}
```

## Resources

- [ML.NET Documentation](https://learn.microsoft.com/en-us/dotnet/machine-learning/)
- [ML.NET Samples](https://github.com/dotnet/machinelearning-samples)
- [ML.NET Model Builder](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet/model-builder)
- [ONNX Runtime](https://onnxruntime.ai/)
- [AutoML.NET](https://github.com/dotnet/machinelearning/blob/main/docs/code/AutoMLAPISweeper.md)

---

**Last Updated:** 2025-12-01
**Version:** 1.0
