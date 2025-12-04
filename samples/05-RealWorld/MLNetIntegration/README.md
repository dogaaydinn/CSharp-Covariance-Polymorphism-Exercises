# ML.NET Integration - Real-World Machine Learning

> **Level:** Real-World  
> **Prerequisites:** C# fundamentals, basic ML concepts  
> **Estimated Time:** 1-2 hours

## 📚 Overview

This real-world example demonstrates how to integrate ML.NET into production applications for sentiment analysis using binary classification. Learn the complete ML workflow: data loading, training, evaluation, and prediction.

## 🎯 Learning Objectives

- ✅ Load and prepare data with ML.NET
- ✅ Build ML pipelines with transformations
- ✅ Train binary classification models
- ✅ Evaluate model performance (accuracy, AUC, F1)
- ✅ Make predictions on new data
- ✅ Save and load trained models
- ✅ Apply ML.NET best practices

## 🚀 Quick Start

```bash
cd samples/05-RealWorld/MLNetIntegration
dotnet run
```

## 📊 What This Example Does

**Problem:** Sentiment Analysis (Positive/Negative classification)

**Pipeline:**
```
Input Text
  ↓ (Text Featurization - TF-IDF)
Feature Vector
  ↓ (Binary Classification - SDCA)
Prediction (0 or 1)
  ↓
Sentiment: Positive 😊 or Negative 😞
```

## 🔑 Key Concepts

### 1. MLContext
```csharp
var mlContext = new MLContext(seed: 0);  // Fixed seed for reproducibility
```

### 2. Data Loading
```csharp
IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
    dataPath, hasHeader: true, separatorChar: ',');
```

### 3. Train/Test Split
```csharp
var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
// 80% training, 20% testing
```

### 4. Pipeline Building
```csharp
var pipeline = mlContext.Transforms.Text
    .FeaturizeText("Features", nameof(SentimentData.SentimentText))
    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
```

### 5. Model Training
```csharp
var model = pipeline.Fit(splitData.TrainSet);
```

### 6. Model Evaluation
```csharp
var metrics = mlContext.BinaryClassification.Evaluate(predictions);
Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
```

### 7. Predictions
```csharp
var predictionEngine = mlContext.Model
    .CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

var result = predictionEngine.Predict(new SentimentData 
{ 
    SentimentText = "This is great!" 
});
```

## 📈 Performance Metrics

**Accuracy:** Percentage of correct predictions (e.g., 95%)  
**AUC (Area Under ROC Curve):** 0.5 = random, 1.0 = perfect  
**F1 Score:** Balance between precision and recall

**Target Metrics for Production:**
- Accuracy: >90%
- AUC: >0.85
- F1 Score: >0.85

## 🎯 Real-World Use Cases

**1. Customer Feedback Analysis**
- Classify reviews as positive/negative
- Route negative feedback to support team
- Track sentiment trends over time

**2. Social Media Monitoring**
- Analyze brand mentions
- Detect PR crises early
- Measure campaign effectiveness

**3. Email Classification**
- Spam detection
- Priority inbox
- Auto-categorization

## ✅ Best Practices

### DO:
- ✅ Use fixed seed for reproducibility
- ✅ Split data into train/test sets
- ✅ Evaluate on unseen test data
- ✅ Save trained models
- ✅ Monitor model performance in production
- ✅ Retrain models periodically

### DON'T:
- ❌ Train on entire dataset (overfitting!)
- ❌ Forget to evaluate metrics
- ❌ Use PredictionEngine for batch predictions (slow!)
- ❌ Ignore data imbalance
- ❌ Deploy without testing

## 🔧 Production Deployment

### Option 1: REST API
```csharp
app.MapPost("/predict", (SentimentData data) =>
{
    var prediction = predictionEngine.Predict(data);
    return new { sentiment = prediction.Prediction ? "positive" : "negative" };
});
```

### Option 2: Azure Functions
```csharp
[Function("SentimentAnalysis")]
public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
{
    var data = await req.ReadFromJsonAsync<SentimentData>();
    var prediction = _predictionEngine.Predict(data);
    return req.CreateResponse(prediction);
}
```

### Option 3: gRPC Service
```csharp
public override Task<SentimentReply> AnalyzeSentiment(
    SentimentRequest request, ServerCallContext context)
{
    var prediction = _predictionEngine.Predict(
        new SentimentData { SentimentText = request.Text });
    return Task.FromResult(new SentimentReply { IsPositive = prediction.Prediction });
}
```

## 📚 Further Reading

- [ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [ML.NET Samples](https://github.com/dotnet/machinelearning-samples)
- [Model Builder](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet/model-builder)

---

**Happy Machine Learning! 🤖**
