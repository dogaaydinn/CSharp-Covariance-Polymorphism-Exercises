# Quick Start Guide - .NET Aspire Video Service

Get the cloud-native video service running in **5 minutes**.

## Prerequisites

1. **.NET 8.0 SDK** or later
   ```bash
   dotnet --version  # Should be 8.0 or higher
   ```

2. **Docker Desktop** running
   ```bash
   docker info  # Should show server info
   ```

## Run the Application

### Step 1: Navigate to the Project
```bash
cd samples/07-CloudNative/AspireVideoService
```

### Step 2: Start the AppHost
```bash
dotnet run --project VideoService.AppHost
```

That's it! The AppHost will:
- ✅ Start Redis container
- ✅ Start PostgreSQL container
- ✅ Launch API service
- ✅ Launch Web frontend
- ✅ Open Aspire Dashboard

## Access the Services

Once started, you'll see URLs in the terminal. Open:

### 🎯 Aspire Dashboard
```
http://localhost:18888
```
**What to do**: View all services, logs, traces, and metrics in real-time

### 🌐 Blazor Web App
```
http://localhost:5xxx  (check terminal for exact port)
```
**What to do**:
1. Click "Videos" in nav menu
2. Add a new video with title, description, and URL
3. Watch status change from Pending → Processing → Ready
4. Click "Watch" to increment view count

### 🔧 API (Swagger)
```
http://localhost:5xxx/swagger  (check terminal for exact port)
```
**What to do**: Test API endpoints directly

## What to Observe

### 1. Service Discovery (3 min)
1. Add a video in the web app
2. Open **Aspire Dashboard → Traces**
3. Click on the latest trace
4. See the request flow: **Web → API → Processing Service**
5. Notice: No hardcoded URLs! All resolved via service discovery

### 2. Caching in Action (2 min)
1. Add 3-4 videos
2. Open **Aspire Dashboard → Traces**
3. First GET /api/videos: Long trace (database query)
4. Second GET /api/videos: Short trace (Redis cache hit!)
5. Cache expires after 5 minutes

### 3. Distributed Logging (2 min)
1. Open **Aspire Dashboard → Structured Logs**
2. Perform an action (add video)
3. Filter by `TraceId` from the trace
4. See correlated logs across all services

### 4. Metrics & Performance (2 min)
1. Open **Aspire Dashboard → Metrics**
2. View HTTP request rates
3. See .NET runtime stats (GC, CPU, memory)
4. Watch metrics update in real-time

## Troubleshooting

### Port Already in Use
```bash
docker ps
docker stop <container_id>
```

### Docker Not Running
Start Docker Desktop and wait for it to fully initialize

### Services Not Starting
Check Aspire Dashboard → Resources tab for error logs

### Cache Not Working
```bash
# Connect to Redis
docker exec -it <redis-container> redis-cli
# Check keys
KEYS *
# Clear cache
FLUSHALL
```

## Stop the Application

Press `Ctrl+C` in the terminal where AppHost is running.

All containers will be stopped automatically.

## Next Steps

1. **Read the full README.md** for architecture details
2. **Explore the code**:
   - `VideoService.AppHost/Program.cs` - Service orchestration
   - `VideoService.API/Program.cs` - API endpoints with caching
   - `VideoService.ServiceDefaults/Extensions.cs` - Shared config
3. **Modify and experiment**:
   - Add new endpoints
   - Change cache duration
   - Add more microservices

## Key Files

```
AspireVideoService/
├── VideoService.AppHost/Program.cs     ← Start here: orchestration
├── VideoService.API/Program.cs         ← API with Redis & PostgreSQL
├── VideoService.ServiceDefaults/       ← Shared observability setup
└── README.md                           ← Deep dive documentation
```

## Pro Tips

- **Dashboard is the control center**: Everything you need is at `localhost:18888`
- **Logs in real-time**: Dashboard → Console Logs shows live output
- **Trace everything**: Click any trace to see full request flow
- **Hot reload works**: Edit code and it reloads automatically

---

**You're now running a production-grade cloud-native microservices architecture!** 🚀

For questions, check the main README.md or open an issue.
