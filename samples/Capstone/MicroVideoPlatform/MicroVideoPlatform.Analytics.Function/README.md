# 🤖 Analytics.Function - ML.NET Video Intelligence

Azure Functions + ML.NET serverless analytics for video recommendations and sentiment analysis.

## 🎯 Features

### 1. Video Recommendation Engine
**Algorithm:** Content-based filtering with TF-IDF and Cosine Similarity

- Analyzes video metadata (title, description, tags)
- Calculates similarity scores between videos
- Provides personalized recommendations based on watch history
- Updates model every 5 minutes with latest videos

**Endpoints:**
```bash
# Get recommendations for a video
GET /api/recommendations/{videoId}?topN=10

# Get personalized recommendations
POST /api/recommendations/personalized
{
  "userId": "user123",
  "watchedVideoIds": ["vid1", "vid2", "vid3"],
  "topN": 10
}
```

### 2. Sentiment Analysis
**Algorithm:** Binary classification with ML.NET

- Analyzes comment sentiment (positive/negative)
- Batch processing for multiple comments
- Confidence scores for each prediction
- Overall sentiment statistics for videos

**Endpoints:**
```bash
# Analyze single comment
POST /api/analyze/comment
{
  "text": "This video is amazing!"
}

# Analyze batch of comments
POST /api/analyze/comments
{
  "videoId": "vid123",
  "comments": [
    {
      "commentId": "c1",
      "text": "Great video!",
      "authorId": "user1",
      "createdAt": "2025-12-02T10:00:00Z"
    }
  ]
}
```

### 3. Background Processing
**Trigger:** Timer (every 5 minutes)

- Fetches latest videos from database
- Retrains recommendation model
- Keeps recommendations fresh

## 🚀 Local Development

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools (`npm install -g azure-functions-core-tools@4`)
- PostgreSQL (running on localhost:5432)

### Setup

1. **Restore packages:**
```bash
dotnet restore
```

2. **Configure local settings:**
Copy `local.settings.json.example` to `local.settings.json` and update:
```json
{
  "Values": {
    "PostgreSQL_ConnectionString": "Host=localhost;Port=5432;Database=microvideo;Username=postgres;Password=postgres"
  }
}
```

3. **Run locally:**
```bash
func start
```

Functions will be available at:
- http://localhost:7071/api/recommendations/{videoId}
- http://localhost:7071/api/recommendations/personalized
- http://localhost:7071/api/analyze/comment
- http://localhost:7071/api/analyze/comments

## 📊 ML.NET Architecture

### Recommendation Pipeline
```
Input: Video metadata
  ↓
Text Featurization (TF-IDF)
  ↓
Normalization (MinMax)
  ↓
Feature Vectors
  ↓
Cosine Similarity Calculation
  ↓
Top N Recommendations
```

### Sentiment Analysis Pipeline
```
Input: Comment text
  ↓
Text Featurization (TF-IDF)
  ↓
Binary Classification (SDCA Logistic Regression)
  ↓
Prediction + Probability
  ↓
Sentiment (Positive/Negative) + Confidence
```

## 🧪 Testing

Run tests:
```bash
cd ../MicroVideoPlatform.Analytics.Tests
dotnet test
```

Test scenarios:
- Video recommendation accuracy
- Sentiment prediction accuracy
- Edge cases (empty data, unknown videos)
- Performance benchmarks

## 📈 Model Training

### Using ML.NET (C#)
Models are trained automatically when functions start with sample data. For production:

1. Load real data from database
2. Train model with larger dataset
3. Save model to blob storage
4. Load model on function startup

### Using Python (scikit-learn)
For advanced training:

```bash
cd training
pip install -r requirements.txt
python train_sentiment_model.py
```

This generates:
- `sentiment_model.pkl` (scikit-learn model)
- `sentiment_model.onnx` (for ML.NET integration)

## 🔧 Configuration

### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `PostgreSQL_ConnectionString` | Database connection | `Host=localhost;Port=5432;...` |
| `Redis_ConnectionString` | Cache connection | `localhost:6379` |
| `ServiceBus_ConnectionString` | Event bus | (optional) |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Monitoring | (optional) |

### Model Parameters

**Recommendation:**
- Max features: 5000 (TF-IDF)
- Similarity threshold: 0.3
- Default top N: 10

**Sentiment Analysis:**
- Training/test split: 80/20
- Algorithm: SDCA Logistic Regression
- Regularization: C=1.0

## 📦 Dependencies

```xml
<!-- ML.NET -->
<PackageReference Include="Microsoft.ML" Version="3.0.1" />
<PackageReference Include="Microsoft.ML.Recommender" Version="0.21.1" />

<!-- Azure Functions -->
<PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.21.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http" Version="3.1.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Timer" Version="4.3.0" />

<!-- Database -->
<PackageReference Include="Npgsql" Version="8.0.1" />
<PackageReference Include="Dapper" Version="2.1.28" />
```

## 🚀 Deployment

### Azure Portal
1. Create Function App (Consumption or Premium plan)
2. Configure app settings (connection strings)
3. Deploy using VS Code Azure Functions extension or:

```bash
func azure functionapp publish <function-app-name>
```

### Terraform
See `../infrastructure/analytics-function.tf` for IaC deployment.

## 📊 Monitoring

### Application Insights
Automatic telemetry for:
- Function executions
- ML model performance
- HTTP request/response times
- Custom metrics (prediction accuracy, model version)

### Logs
View logs:
```bash
func azure functionapp logstream <function-app-name>
```

## 🔒 Security

- Function-level authorization (API keys)
- Connection strings in Key Vault
- Managed Identity for Azure resources
- Input validation on all endpoints

## 📚 Learn More

**ML.NET Resources:**
- [ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [Text Classification Tutorial](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis)
- [Recommendation Tutorial](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/movie-recommendation)

**Azure Functions:**
- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [Timer Triggers](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer)

## 💡 Future Enhancements

- [ ] Collaborative filtering with matrix factorization
- [ ] Deep learning models (TensorFlow integration)
- [ ] A/B testing for recommendation algorithms
- [ ] Real-time model updates
- [ ] Multi-language sentiment analysis
- [ ] Video content analysis (ML.NET Vision)
