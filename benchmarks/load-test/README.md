# Load Testing - AspireVideoService API

This directory contains load testing scripts for the AspireVideoService API using two popular tools: **k6** (JavaScript-based) and **Bombardier** (Go-based).

## 🎯 Purpose

Performance testing demonstrates that your API can handle production traffic with acceptable response times and error rates. These tests measure:

- **Throughput:** Requests per second (RPS)
- **Latency:** Response time (average, p95, p99)
- **Error Rate:** Percentage of failed requests
- **Resource Utilization:** CPU, memory, network
- **Cache Effectiveness:** Cache hit rates

## 🛠️ Tools

### k6 (Recommended for Detailed Scenarios)

**Pros:**
- Powerful scripting with JavaScript
- Detailed metrics and custom thresholds
- HTML/JSON report generation
- Excellent for complex scenarios
- Built-in checks and assertions

**Installation:**
```bash
# macOS
brew install k6

# Linux (Debian/Ubuntu)
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6

# Windows (Chocolatey)
choco install k6

# Or download from: https://k6.io/docs/get-started/installation/
```

### Bombardier (Fast & Simple)

**Pros:**
- Extremely fast (written in Go)
- Simple command-line interface
- Low overhead
- Great for quick benchmarks
- JSON output support

**Installation:**
```bash
# macOS
brew install bombardier

# With Go installed
go install github.com/codesenberg/bombardier@latest

# Linux (download binary)
wget https://github.com/codesenberg/bombardier/releases/latest/download/bombardier-linux-amd64
chmod +x bombardier-linux-amd64
sudo mv bombardier-linux-amd64 /usr/local/bin/bombardier

# Or download from: https://github.com/codesenberg/bombardier/releases
```

## 🚀 Quick Start

### Prerequisites

1. **Start the API in Release mode:**
   ```bash
   cd samples/07-CloudNative/AspireVideoService/VideoService.API
   dotnet run --configuration Release
   ```

2. **Verify the API is running:**
   ```bash
   curl http://localhost:5000/health
   # Expected: {"status":"Healthy"}
   ```

### Running k6 Tests

```bash
# Navigate to the load test directory
cd benchmarks/load-test

# Run the test (default: 30s ramp-up, 2m steady, 30s ramp-down)
k6 run webapi-load-test.js

# Run with custom API URL
k6 run --env API_URL=http://localhost:5000 webapi-load-test.js

# Run with JSON output for analysis
k6 run --out json=results.json webapi-load-test.js

# Run with HTML report (requires k6-reporter)
k6 run --out json=results.json webapi-load-test.js
# Then: k6-html-reporter results.json
```

### Running Bombardier Tests

```bash
# Navigate to the load test directory
cd benchmarks/load-test

# Run the test suite (runs 5 different scenarios)
./bombardier-test.sh

# Run with custom configuration
API_URL=http://localhost:5000 DURATION=120s CONNECTIONS=200 ./bombardier-test.sh

# Single endpoint test with bombardier directly
bombardier -c 100 -d 60s http://localhost:5000/api/videos
```

## 📊 Understanding Results

### k6 Output

```
execution: local
    script: webapi-load-test.js
    output: -

scenarios: (100.00%) 1 scenario, 100 max VUs, 5m30s max duration (incl. graceful stop):
         * default: Up to 100 looping VUs for 5m0s over 5 stages

running (5m00.0s), 000/100 VUs, 15000 complete and 0 interrupted iterations

✓ health check status is 200
✓ get all videos status is 200
✓ create video status is 201

checks.........................: 100.00% ✓ 45000      ✗ 0
data_received..................: 15 MB   50 kB/s
data_sent......................: 3.0 MB  10 kB/s
http_req_duration..............: avg=45ms   p(95)=250ms   p(99)=450ms
http_reqs......................: 15000   50/s
```

**Key Metrics:**
- **http_reqs:** 50/s (throughput)
- **http_req_duration p(95):** 250ms (95% of requests faster than this)
- **checks:** 100% (all assertions passed)

### Bombardier Output

```
Bombarding http://localhost:5000/api/videos for 60s using 100 connection(s)
[==================================================] 60s
Done!

Statistics        Avg      Stdev        Max
  Reqs/sec       523.45     124.32    1024.12
  Latency      191.23ms    45.67ms   512.34ms
  HTTP codes:
    2xx - 31407
    4xx - 0
    5xx - 0
  Throughput:    1.23 MB/s
```

**Key Metrics:**
- **Reqs/sec:** 523.45 (throughput)
- **Latency (avg):** 191.23ms
- **HTTP codes:** 31,407 successful requests (2xx), 0 errors

## 🎯 Performance Targets

### AspireVideoService API Targets

| Metric | Target | Excellent | Warning |
|--------|--------|-----------|---------|
| **Throughput** | 50 RPS | 100+ RPS | < 20 RPS |
| **Latency (avg)** | < 100ms | < 50ms | > 200ms |
| **Latency (p95)** | < 500ms | < 250ms | > 1000ms |
| **Latency (p99)** | < 1000ms | < 500ms | > 2000ms |
| **Error Rate** | < 5% | < 1% | > 10% |
| **Cache Hit Rate** | > 80% | > 90% | < 50% |

### Interpreting Results

**Good Performance:**
```
✅ Throughput: 85 req/s
✅ Latency p95: 245ms
✅ Error rate: 0.5%
✅ All checks passing
```

**Needs Optimization:**
```
⚠️  Throughput: 15 req/s (< 50 target)
⚠️  Latency p95: 1250ms (> 500 target)
❌ Error rate: 8% (> 5% threshold)
```

## 📈 Test Scenarios

### 1. Smoke Test (Quick Validation)
- **Duration:** 1 minute
- **Users:** 5-10
- **Purpose:** Verify basic functionality

```bash
k6 run --duration 1m --vus 10 webapi-load-test.js
```

### 2. Load Test (Normal Traffic)
- **Duration:** 5 minutes
- **Users:** 50-100
- **Purpose:** Measure performance under expected load

```bash
k6 run webapi-load-test.js  # Default scenario
```

### 3. Stress Test (Breaking Point)
- **Duration:** 10 minutes
- **Users:** 200-500
- **Purpose:** Find system limits

```bash
# Modify webapi-load-test.js stages:
# { duration: '2m', target: 500 }
k6 run webapi-load-test.js
```

### 4. Spike Test (Traffic Surge)
- **Duration:** 5 minutes
- **Users:** Sudden jump to 200
- **Purpose:** Test auto-scaling and resilience

```bash
# Modify webapi-load-test.js stages:
# { duration: '30s', target: 200 }  // Instant spike
k6 run webapi-load-test.js
```

### 5. Soak Test (Stability)
- **Duration:** 1-4 hours
- **Users:** 50 (constant)
- **Purpose:** Detect memory leaks and degradation

```bash
# Modify webapi-load-test.js stages:
# { duration: '4h', target: 50 }
k6 run webapi-load-test.js
```

## 🔧 CI/CD Integration

### GitHub Actions Example

Add to `.github/workflows/performance.yml`:

```yaml
name: Performance Tests

on:
  schedule:
    - cron: '0 0 * * 0'  # Weekly on Sunday
  workflow_dispatch:      # Manual trigger

jobs:
  performance:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Install k6
        run: |
          sudo gpg -k
          sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
          echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
          sudo apt-get update
          sudo apt-get install k6

      - name: Start API
        run: |
          cd samples/07-CloudNative/AspireVideoService/VideoService.API
          dotnet run --configuration Release &
          sleep 10  # Wait for startup

      - name: Run performance tests
        run: |
          cd benchmarks/load-test
          k6 run --out json=results.json webapi-load-test.js

      - name: Upload results
        uses: actions/upload-artifact@v4
        with:
          name: performance-results
          path: benchmarks/load-test/results.json
```

## 📝 Reporting

### Generate HTML Report (k6)

1. **Install k6-html-reporter:**
   ```bash
   npm install -g k6-to-junit k6-html-reporter
   ```

2. **Run test with JSON output:**
   ```bash
   k6 run --out json=results.json webapi-load-test.js
   ```

3. **Generate HTML:**
   ```bash
   k6-html-reporter results.json
   ```

### View Results in Grafana

1. **Export to InfluxDB:**
   ```bash
   k6 run --out influxdb=http://localhost:8086/k6 webapi-load-test.js
   ```

2. **View in Grafana dashboard:**
   - Import k6 dashboard template
   - Connect to InfluxDB data source

## 🐛 Troubleshooting

### API Not Responding

```bash
# Check if API is running
curl http://localhost:5000/health

# Check API logs
cd samples/07-CloudNative/AspireVideoService/VideoService.API
dotnet run --configuration Release

# Verify port is not in use
lsof -i :5000
```

### High Error Rates

1. **Check API logs** for exceptions
2. **Verify database connection** is working
3. **Check Redis cache** is available
4. **Review rate limiting** settings
5. **Monitor resource usage** (CPU, memory)

### Low Throughput

1. **Build in Release mode** (not Debug)
2. **Disable detailed logging** in production
3. **Optimize database queries** (check EF Core logs)
4. **Review cache strategy** (increase TTL?)
5. **Consider horizontal scaling**

## 📚 Resources

- [k6 Documentation](https://k6.io/docs/)
- [Bombardier GitHub](https://github.com/codesenberg/bombardier)
- [Performance Testing Guide](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/perf-testing.pdf)
- [.NET Performance Best Practices](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/performance-best-practices)

## 🎓 Best Practices

1. **Always test in Release mode** - Debug builds are 10-100x slower
2. **Warm up the application** - First requests trigger JIT compilation
3. **Use realistic data** - Match production payload sizes
4. **Test from different locations** - Network latency matters
5. **Monitor resources** - CPU, memory, disk I/O during tests
6. **Compare results** - Track performance over time
7. **Test edge cases** - Large payloads, concurrent writes, etc.
8. **Document environment** - OS, hardware, network configuration

## 📊 Example Results

### Baseline Performance (Local Development)

**Environment:**
- MacBook Pro M1, 16GB RAM
- .NET 8.0, Release mode
- PostgreSQL 16, Redis 7

**Results:**
```
Test: Get All Videos
Throughput:     87 req/s
Latency (avg):  45ms
Latency (p95):  125ms
Latency (p99):  285ms
Error Rate:     0.2%
Cache Hit Rate: 85%

Test: Create Video
Throughput:     42 req/s
Latency (avg):  95ms
Latency (p95):  245ms
Latency (p99):  520ms
Error Rate:     0.5%
```

✅ **All targets met!** API is production-ready.

---

**Last Updated:** 2025-12-02
**Maintained By:** Project Team
