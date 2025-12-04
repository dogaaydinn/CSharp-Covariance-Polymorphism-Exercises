# Path 2 - Month 9 Final Capstone: Micro-Video Platform

**Difficulty**: ⭐⭐⭐⭐⭐ (Expert)
**Estimated Time**: 120-150 hours (4 weeks full-time)
**Prerequisites**: Completed all Path 2 content (Months 1-8)

---

## 🎯 Project Overview

Build a complete production-ready video platform with microservices, demonstrating ALL concepts learned in Path 2. This is your portfolio centerpiece project.

### System Requirements

A full-featured video streaming platform with:
- **5+ Microservices** communicating via gRPC and events
- **API Gateway** as single entry point
- **Event-driven** architecture with message queues
- **High-performance** video processing
- **Complete observability** (logging, tracing, metrics)
- **Production-ready** with CI/CD
- **Fully documented** with architecture diagrams

---

## 📋 Architecture

```
                    ┌──────────────┐
                    │ API Gateway  │
                    │   (REST)     │
                    └──────┬───────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
   ┌────▼─────┐      ┌─────▼────┐     ┌──────▼──────┐
   │   User   │      │  Video   │     │  Analytics  │
   │ Service  │      │ Service  │     │   Service   │
   │ (gRPC)   │      │ (gRPC)   │     │   (gRPC)    │
   └────┬─────┘      └─────┬────┘     └──────┬──────┘
        │                  │                  │
        │            ┌─────▼──────┐           │
        │            │  Storage   │           │
        │            │  Service   │           │
        │            │  (gRPC)    │           │
        │            └─────┬──────┘           │
        │                  │                  │
        └──────────────────┴──────────────────┘
                           │
                  ┌────────▼─────────┐
                  │    RabbitMQ      │
                  │  (Event Bus)     │
                  └────────┬─────────┘
                           │
              ┌────────────┴───────────────┐
              │                            │
       ┌──────▼──────┐           ┌────────▼────────┐
       │Notification │           │  Search/Index   │
       │   Service   │           │    Service      │
       └─────────────┘           └─────────────────┘

Storage:  PostgreSQL, MongoDB, Redis, S3/MinIO
Monitoring: Seq, Jaeger, Prometheus, Grafana
```

---

## 🛠️ Microservices

### 1. API Gateway (ASP.NET Core)
**Responsibilities**:
- HTTP/REST entry point
- JWT authentication
- Request routing to internal services
- Response aggregation
- Rate limiting
- CORS handling

**Endpoints**:
```
POST   /api/auth/login
POST   /api/auth/register
GET    /api/videos
POST   /api/videos/upload
GET    /api/videos/{id}
POST   /api/videos/{id}/view
GET    /api/users/{id}/videos
GET    /api/stats/overview
```

**Tech Stack**:
- ASP.NET Core 8
- YARP (reverse proxy) or Ocelot
- JWT authentication
- Polly for resilience

---

### 2. User Service (gRPC)
**Responsibilities**:
- User registration/authentication
- Profile management
- JWT token generation
- User preferences

**gRPC Services**:
```protobuf
service UserService {
  rpc Register (RegisterRequest) returns (RegisterResponse);
  rpc Login (LoginRequest) returns (LoginResponse);
  rpc GetUser (GetUserRequest) returns (UserResponse);
  rpc UpdateProfile (UpdateProfileRequest) returns (UpdateProfileResponse);
}
```

**Database**: PostgreSQL
**Events Published**:
- UserRegistered
- UserProfileUpdated

---

### 3. Video Service (gRPC)
**Responsibilities**:
- Video metadata management
- Upload coordination
- Video status tracking
- Thumbnail management

**gRPC Services**:
```protobuf
service VideoService {
  rpc UploadVideo (UploadVideoRequest) returns (UploadVideoResponse);
  rpc GetVideo (GetVideoRequest) returns (VideoResponse);
  rpc ListVideos (ListVideosRequest) returns (ListVideosResponse);
  rpc UpdateVideo (UpdateVideoRequest) returns (UpdateVideoResponse);
  rpc DeleteVideo (DeleteVideoRequest) returns (DeleteVideoResponse);
}
```

**Database**: MongoDB
**Events Published**:
- VideoUploaded
- VideoProcessed
- VideoDeleted

---

### 4. Storage Service (gRPC)
**Responsibilities**:
- Actual file storage (MinIO/S3)
- Pre-signed URL generation
- File chunking for large uploads
- CDN integration

**gRPC Services**:
```protobuf
service StorageService {
  rpc GetUploadUrl (GetUploadUrlRequest) returns (GetUploadUrlResponse);
  rpc GetDownloadUrl (GetDownloadUrlRequest) returns (GetDownloadUrlResponse);
  rpc DeleteFile (DeleteFileRequest) returns (DeleteFileResponse);
}
```

**Storage**: MinIO (S3-compatible)

---

### 5. Analytics Service (gRPC)
**Responsibilities**:
- Track video views
- User engagement metrics
- Generate statistics
- Real-time analytics

**gRPC Services**:
```protobuf
service AnalyticsService {
  rpc RecordView (RecordViewRequest) returns (RecordViewResponse);
  rpc GetVideoStats (GetVideoStatsRequest) returns (VideoStatsResponse);
  rpc GetUserStats (GetUserStatsRequest) returns (UserStatsResponse);
  rpc GetTrendingVideos (GetTrendingVideosRequest) returns (TrendingVideosResponse);
}
```

**Database**: MongoDB (time-series collection) or InfluxDB
**Events Consumed**:
- VideoViewed
- VideoLiked
- VideoShared

---

### 6. Notification Service (Event Consumer)
**Responsibilities**:
- Email notifications
- Push notifications
- SMS notifications (optional)
- Notification templates

**Events Consumed**:
- UserRegistered → Send welcome email
- VideoUploaded → Notify subscribers
- VideoProcessed → Notify uploader

**Tech**: Background service with RabbitMQ consumer

---

### 7. Search Service (Optional, HTTP)
**Responsibilities**:
- Full-text search
- Video indexing
- Search suggestions

**Tech**: Elasticsearch or MeiliSearch

---

## 🎯 Key Features to Implement

### High-Performance Video Upload
```csharp
// Use Span<T> for chunked upload
public async Task<string> UploadChunkedAsync(
    Stream stream,
    string fileName,
    CancellationToken ct)
{
    const int chunkSize = 1024 * 1024; // 1MB chunks
    var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);

    try
    {
        var uploadId = Guid.NewGuid().ToString();
        int partNumber = 1;

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
        {
            ReadOnlyMemory<byte> chunk = buffer.AsMemory(0, bytesRead);

            await _s3Client.UploadPartAsync(new UploadPartRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                UploadId = uploadId,
                PartNumber = partNumber++,
                InputStream = new MemoryStream(chunk.ToArray())
            }, ct);

            _logger.LogInformation(
                "Uploaded chunk {PartNumber} of {FileName}",
                partNumber - 1, fileName);
        }

        return uploadId;
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

### Event-Driven Communication
```csharp
// Publish event
public async Task PublishVideoUploadedAsync(VideoUploadedEvent @event)
{
    var message = JsonSerializer.SerializeToUtf8Bytes(@event);

    await _channel.BasicPublishAsync(
        exchange: "video.events",
        routingKey: "video.uploaded",
        mandatory: true,
        basicProperties: new BasicProperties
        {
            DeliveryMode = 2, // Persistent
            ContentType = "application/json",
            MessageId = @event.VideoId.ToString()
        },
        body: message);

    _logger.LogInformation(
        "Published VideoUploaded event for video {VideoId}",
        @event.VideoId);
}

// Consume event
public class VideoUploadedConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var @event = JsonSerializer.Deserialize<VideoUploadedEvent>(body);

            using var activity = _activitySource.StartActivity("ProcessVideoUploaded");
            activity?.SetTag("video.id", @event.VideoId);

            try
            {
                await ProcessVideoUploadedAsync(@event);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process VideoUploaded event");
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(
            queue: "notification.video.uploaded",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, ct);
    }
}
```

---

## 🎯 Implementation Phases

### Phase 1: Foundation (Week 1)
- [ ] Set up solution structure (7 projects)
- [ ] Configure Docker Compose
- [ ] Set up databases (PostgreSQL, MongoDB, Redis)
- [ ] Set up RabbitMQ
- [ ] Implement basic API Gateway

### Phase 2: Core Services (Week 2)
- [ ] Implement User Service (gRPC)
- [ ] Implement Video Service (gRPC)
- [ ] Implement JWT authentication
- [ ] Basic CRUD operations working

### Phase 3: Storage & Events (Week 3)
- [ ] Implement Storage Service (MinIO)
- [ ] Add RabbitMQ event publishing
- [ ] Implement Notification Service
- [ ] Implement Analytics Service

### Phase 4: Observability (Week 3-4)
- [ ] Add Serilog structured logging
- [ ] Add OpenTelemetry tracing
- [ ] Add health checks
- [ ] Set up Prometheus + Grafana
- [ ] Add Seq for log aggregation
- [ ] Add Jaeger for trace visualization

### Phase 5: Polish & Deploy (Week 4)
- [ ] Write comprehensive tests (unit + integration)
- [ ] Add resilience policies (Polly)
- [ ] Performance testing
- [ ] Complete documentation
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Production deployment

---

## ✅ Evaluation Criteria

| Category | Weight | Description |
|----------|--------|-------------|
| **Architecture** | 20% | Microservice design, separation of concerns |
| **Implementation** | 25% | Code quality, SOLID principles, patterns |
| **Performance** | 15% | High-performance code, Span<T>, optimization |
| **Observability** | 15% | Logging, tracing, metrics, dashboards |
| **Testing** | 10% | Unit tests, integration tests, coverage |
| **Documentation** | 10% | Architecture docs, API docs, README |
| **Deployment** | 5% | Docker, CI/CD, production-ready |

**Minimum Pass**: 80% (240/300 points)

---

## 📚 Required Deliverables

1. **Source Code**:
   - GitHub repository with all services
   - Docker Compose configuration
   - CI/CD pipeline

2. **Documentation**:
   - Architecture diagram (C4 model)
   - API documentation (Swagger)
   - Setup/deployment guide
   - Technical decisions document

3. **Tests**:
   - Unit tests (80%+ coverage)
   - Integration tests for key flows
   - Load test results

4. **Monitoring**:
   - Grafana dashboards
   - Sample logs in Seq
   - Traces in Jaeger

5. **Demo**:
   - Video walkthrough (15 minutes)
   - Live demo or screenshots
   - Performance benchmarks

---

## 🎓 Upon Completion

Congratulations! You've completed Path 2 and are now a **Mid-Level .NET Developer**. You can:

- Design and implement microservice architectures
- Write high-performance code
- Build production-ready systems with observability
- Work with event-driven architectures
- Deploy and monitor distributed systems

**Next Steps**:
1. Take Path 2 final certification exam
2. Update resume and LinkedIn
3. Start Path 3 (Senior Developer) OR
4. Begin job search for mid-level positions

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
