# Path 2 - Months 5-6 Capstone: Video Streaming Microservice Platform

**Difficulty**: ⭐⭐⭐⭐⭐ (Expert)
**Estimated Time**: 60-70 hours
**Prerequisites**: Months 1-4 of Path 2 completed

---

## 🎯 Project Overview

Build a complete microservice platform with 5+ services communicating via gRPC and message queues, deployed with Docker Compose.

### Learning Objectives

- ✅ Microservice architecture
- ✅ gRPC service-to-service communication
- ✅ REST API Gateway pattern
- ✅ Message queues (RabbitMQ)
- ✅ Docker and containerization
- ✅ Service discovery

---

## 📋 Microservices to Implement

### 1. API Gateway (REST)
- Single entry point for clients
- Routes requests to internal services
- Authentication/Authorization
- Rate limiting
- Response aggregation

### 2. User Service (gRPC)
- User registration and authentication
- Profile management
- JWT token generation
- Stores: PostgreSQL

### 3. Video Service (gRPC)
- Video upload (metadata)
- Video metadata storage
- Thumbnail generation
- Stores: MongoDB

### 4. Analytics Service (gRPC)
- Track video views
- Generate statistics
- Aggregation queries
- Stores: InfluxDB or MongoDB

### 5. Notification Service (Message Consumer)
- Listens to events from RabbitMQ
- Sends emails
- Push notifications
- Stores: None (stateless)

### 6. Search Service (Optional)
- Full-text search
- Elasticsearch integration
- Video search by title, tags

---

## 🏗️ Architecture

```
┌─────────────────┐
│   API Gateway   │ (REST)
│   Port: 5000    │
└────────┬────────┘
         │ (HTTP/gRPC)
    ┌────┴───────────────────┐
    │                        │
┌───▼────┐ ┌───────┐ ┌──────▼──────┐
│  User  │ │ Video │ │  Analytics  │
│ Service│ │Service│ │   Service   │
│ :5001  │ │ :5002 │ │    :5003    │
└───┬────┘ └───┬───┘ └──────┬──────┘
    │          │             │
    └──────────┴─────────────┘
               │
         ┌─────▼──────┐
         │  RabbitMQ  │
         │  :5672     │
         └─────┬──────┘
               │
        ┌──────▼──────────┐
        │  Notification   │
        │    Service      │
        │     :5004       │
        └─────────────────┘
```

---

## 🚀 Key Implementation

### gRPC Service Definition

```protobuf
// user.proto
syntax = "proto3";

service UserService {
  rpc Register (RegisterRequest) returns (RegisterResponse);
  rpc Login (LoginRequest) returns (LoginResponse);
  rpc GetUser (GetUserRequest) returns (UserResponse);
}

message RegisterRequest {
  string username = 1;
  string email = 2;
  string password = 3;
}

message RegisterResponse {
  string user_id = 1;
  bool success = 2;
  string message = 3;
}
```

### gRPC Client in API Gateway

```csharp
public class UserGrpcClient
{
    private readonly UserService.UserServiceClient _client;

    public UserGrpcClient(GrpcChannel channel)
    {
        _client = new UserService.UserServiceClient(channel);
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        return await _client.RegisterAsync(request);
    }
}

// In API Gateway Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserGrpcClient _userClient;

    public UsersController(UserGrpcClient userClient)
    {
        _userClient = userClient;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var response = await _userClient.RegisterAsync(new RegisterRequest
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password
        });

        return response.Success ? Ok(response) : BadRequest(response.Message);
    }
}
```

### Message Queue Publishing

```csharp
public class VideoEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public VideoEventPublisher(IConnectionFactory factory)
    {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("video_events", ExchangeType.Fanout);
    }

    public void PublishVideoUploaded(VideoUploadedEvent @event)
    {
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "video_events",
            routingKey: "",
            basicProperties: null,
            body: body
        );
    }
}
```

### Docker Compose

```yaml
version: '3.8'

services:
  api-gateway:
    build: ./ApiGateway
    ports:
      - "5000:80"
    environment:
      - UserServiceUrl=http://user-service:80
      - VideoServiceUrl=http://video-service:80
    depends_on:
      - user-service
      - video-service

  user-service:
    build: ./UserService
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=users;Username=admin;Password=admin
    depends_on:
      - postgres

  video-service:
    build: ./VideoService
    environment:
      - MongoDB__ConnectionString=mongodb://mongo:27017
      - MongoDB__DatabaseName=videos
    depends_on:
      - mongo
      - rabbitmq

  notification-service:
    build: ./NotificationService
    environment:
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - rabbitmq

  postgres:
    image: postgres:15
    environment:
      - POSTGRES_PASSWORD=admin
    ports:
      - "5432:5432"

  mongo:
    image: mongo:6
    ports:
      - "27017:27017"

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
```

---

## 🎯 Milestones

1. **Week 1-2**: Set up 3 services (API Gateway, User, Video)
2. **Week 3-4**: Add gRPC communication
3. **Week 5-6**: Add RabbitMQ and Notification service
4. **Week 7-8**: Add Analytics service, Docker Compose
5. **Week 9-10**: Testing, monitoring, deployment

---

## ✅ Evaluation

| Criteria | Weight |
|----------|--------|
| Microservice Architecture | 25% |
| gRPC Communication | 20% |
| Message Queue Integration | 20% |
| Docker & Deployment | 20% |
| API Gateway | 10% |
| Tests & Documentation | 5% |

**Pass**: 75%

---

## 📚 Resources

- `samples/07-CloudNative/AspireVideoService/`
- gRPC: https://grpc.io/docs/languages/csharp/
- RabbitMQ: https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
- Microservices: https://microservices.io/

---

*Template Version: 1.0*
