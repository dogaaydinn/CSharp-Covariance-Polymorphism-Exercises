#!/bin/bash
# Bombardier Load Test Script for AspireVideoService API
# Bombardier is a fast HTTP benchmarking tool written in Go
# Install: go install github.com/codesenberg/bombardier@latest
# Or: brew install bombardier (macOS)

set -e

# Configuration
API_URL="${API_URL:-http://localhost:5000}"
DURATION="${DURATION:-60s}"
CONNECTIONS="${CONNECTIONS:-100}"
TIMEOUT="${TIMEOUT:-10s}"
OUTPUT_DIR="./results"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Create output directory
mkdir -p "$OUTPUT_DIR"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Bombardier Load Test - AspireVideoService${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""
echo -e "${YELLOW}Configuration:${NC}"
echo -e "  API URL: ${GREEN}$API_URL${NC}"
echo -e "  Duration: ${GREEN}$DURATION${NC}"
echo -e "  Connections: ${GREEN}$CONNECTIONS${NC}"
echo -e "  Timeout: ${GREEN}$TIMEOUT${NC}"
echo ""

# Check if bombardier is installed
if ! command -v bombardier &> /dev/null; then
    echo -e "${RED}Error: bombardier is not installed${NC}"
    echo -e "${YELLOW}Install with: go install github.com/codesenberg/bombardier@latest${NC}"
    echo -e "${YELLOW}Or on macOS: brew install bombardier${NC}"
    exit 1
fi

# Check if API is running
echo -e "${YELLOW}Checking API health...${NC}"
if ! curl -s -f "$API_URL/health" > /dev/null; then
    echo -e "${RED}Error: API is not responding at $API_URL${NC}"
    echo -e "${YELLOW}Start the API with: dotnet run --configuration Release${NC}"
    exit 1
fi
echo -e "${GREEN}✓ API is healthy${NC}"
echo ""

# Test 1: GET /health endpoint (baseline)
echo -e "${BLUE}Test 1: Health Check Endpoint${NC}"
echo -e "${YELLOW}Testing: GET $API_URL/health${NC}"
bombardier -c "$CONNECTIONS" -d "$DURATION" -t "$TIMEOUT" \
    --fasthttp \
    --print r \
    --format json \
    "$API_URL/health" > "$OUTPUT_DIR/health-check.json"

# Parse and display results
echo -e "${GREEN}✓ Health check test completed${NC}"
echo ""

# Test 2: GET /api/videos endpoint (read-heavy)
echo -e "${BLUE}Test 2: Get All Videos (Read-Heavy)${NC}"
echo -e "${YELLOW}Testing: GET $API_URL/api/videos${NC}"
bombardier -c "$CONNECTIONS" -d "$DURATION" -t "$TIMEOUT" \
    --fasthttp \
    --print r \
    --format json \
    --header "Accept: application/json" \
    "$API_URL/api/videos" > "$OUTPUT_DIR/get-videos.json"

echo -e "${GREEN}✓ Get videos test completed${NC}"
echo ""

# Test 3: GET /api/videos/1 endpoint (single resource)
echo -e "${BLUE}Test 3: Get Single Video (Cache Test)${NC}"
echo -e "${YELLOW}Testing: GET $API_URL/api/videos/1${NC}"
bombardier -c "$CONNECTIONS" -d "$DURATION" -t "$TIMEOUT" \
    --fasthttp \
    --print r \
    --format json \
    --header "Accept: application/json" \
    "$API_URL/api/videos/1" > "$OUTPUT_DIR/get-single-video.json"

echo -e "${GREEN}✓ Get single video test completed${NC}"
echo ""

# Test 4: POST /api/videos endpoint (write-heavy)
echo -e "${BLUE}Test 4: Create Video (Write-Heavy)${NC}"
echo -e "${YELLOW}Testing: POST $API_URL/api/videos${NC}"

# Create temporary JSON payload
PAYLOAD='{"title":"Bombardier Load Test Video","description":"Performance testing with Bombardier","duration":120}'

bombardier -c "$CONNECTIONS" -d "$DURATION" -t "$TIMEOUT" \
    --method POST \
    --body "$PAYLOAD" \
    --fasthttp \
    --print r \
    --format json \
    --header "Content-Type: application/json" \
    "$API_URL/api/videos" > "$OUTPUT_DIR/post-videos.json"

echo -e "${GREEN}✓ Create video test completed${NC}"
echo ""

# Test 5: Mixed workload (70% reads, 30% writes simulation)
echo -e "${BLUE}Test 5: Mixed Workload Simulation${NC}"
echo -e "${YELLOW}Running concurrent read and write operations...${NC}"

# Start read operations in background
bombardier -c 70 -d "$DURATION" -t "$TIMEOUT" \
    --fasthttp \
    --print r \
    --format json \
    --header "Accept: application/json" \
    "$API_URL/api/videos" > "$OUTPUT_DIR/mixed-reads.json" &
READ_PID=$!

# Start write operations in background
bombardier -c 30 -d "$DURATION" -t "$TIMEOUT" \
    --method POST \
    --body "$PAYLOAD" \
    --fasthttp \
    --print r \
    --format json \
    --header "Content-Type: application/json" \
    "$API_URL/api/videos" > "$OUTPUT_DIR/mixed-writes.json" &
WRITE_PID=$!

# Wait for both to complete
wait $READ_PID
wait $WRITE_PID

echo -e "${GREEN}✓ Mixed workload test completed${NC}"
echo ""

# Generate summary report
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Generating Summary Report...${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Function to extract key metrics from JSON
extract_metrics() {
    local file=$1
    local test_name=$2

    if [ ! -f "$file" ]; then
        echo -e "${RED}Error: Result file not found: $file${NC}"
        return
    fi

    echo -e "${YELLOW}$test_name:${NC}"

    # Extract metrics using jq if available, otherwise use grep/sed
    if command -v jq &> /dev/null; then
        local rps=$(jq -r '.result.rps.mean' "$file" 2>/dev/null || echo "N/A")
        local latency_mean=$(jq -r '.result.latency.mean' "$file" 2>/dev/null || echo "N/A")
        local latency_p50=$(jq -r '.result.latency.percentiles."50"' "$file" 2>/dev/null || echo "N/A")
        local latency_p95=$(jq -r '.result.latency.percentiles."95"' "$file" 2>/dev/null || echo "N/A")
        local latency_p99=$(jq -r '.result.latency.percentiles."99"' "$file" 2>/dev/null || echo "N/A")
        local req_total=$(jq -r '.result.req1xx + .result.req2xx + .result.req3xx + .result.req4xx + .result.req5xx' "$file" 2>/dev/null || echo "N/A")
        local req_2xx=$(jq -r '.result.req2xx' "$file" 2>/dev/null || echo "0")
        local req_errors=$(jq -r '.result.req4xx + .result.req5xx' "$file" 2>/dev/null || echo "0")

        # Calculate error rate
        if [ "$req_total" != "N/A" ] && [ "$req_total" != "0" ]; then
            local error_rate=$(awk "BEGIN {printf \"%.2f\", ($req_errors / $req_total) * 100}")
        else
            local error_rate="N/A"
        fi

        echo -e "  ${GREEN}Throughput:${NC}        ${rps} req/s"
        echo -e "  ${GREEN}Total Requests:${NC}    ${req_total}"
        echo -e "  ${GREEN}Successful (2xx):${NC}  ${req_2xx}"
        echo -e "  ${GREEN}Errors (4xx/5xx):${NC} ${req_errors}"
        echo -e "  ${GREEN}Error Rate:${NC}        ${error_rate}%"
        echo -e "  ${GREEN}Latency (mean):${NC}    ${latency_mean}"
        echo -e "  ${GREEN}Latency (p50):${NC}     ${latency_p50}"
        echo -e "  ${GREEN}Latency (p95):${NC}     ${latency_p95}"
        echo -e "  ${GREEN}Latency (p99):${NC}     ${latency_p99}"
    else
        echo -e "  ${YELLOW}Install jq for detailed metrics: brew install jq${NC}"
        echo -e "  ${YELLOW}Raw results available in: $file${NC}"
    fi

    echo ""
}

# Extract metrics for all tests
extract_metrics "$OUTPUT_DIR/health-check.json" "Health Check"
extract_metrics "$OUTPUT_DIR/get-videos.json" "Get All Videos"
extract_metrics "$OUTPUT_DIR/get-single-video.json" "Get Single Video"
extract_metrics "$OUTPUT_DIR/post-videos.json" "Create Video"

echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}All tests completed successfully!${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""
echo -e "${YELLOW}Results saved in: $OUTPUT_DIR${NC}"
echo -e "${YELLOW}View detailed results:${NC}"
echo "  - Health check:      $OUTPUT_DIR/health-check.json"
echo "  - Get videos:        $OUTPUT_DIR/get-videos.json"
echo "  - Get single video:  $OUTPUT_DIR/get-single-video.json"
echo "  - Create video:      $OUTPUT_DIR/post-videos.json"
echo "  - Mixed reads:       $OUTPUT_DIR/mixed-reads.json"
echo "  - Mixed writes:      $OUTPUT_DIR/mixed-writes.json"
echo ""

# Generate markdown report
REPORT_FILE="$OUTPUT_DIR/PERFORMANCE_REPORT.md"
cat > "$REPORT_FILE" << 'EOF'
# Performance Test Report - AspireVideoService API

**Generated:** $(date)
**Tool:** Bombardier
**API URL:** $API_URL
**Test Duration:** $DURATION per test
**Concurrent Connections:** $CONNECTIONS

## Test Results

### 1. Health Check Endpoint
- **Endpoint:** `GET /health`
- **Purpose:** Baseline performance measurement
- **Results:** See `health-check.json`

### 2. Get All Videos
- **Endpoint:** `GET /api/videos`
- **Purpose:** Read-heavy workload with caching
- **Results:** See `get-videos.json`

### 3. Get Single Video
- **Endpoint:** `GET /api/videos/1`
- **Purpose:** Cache effectiveness validation
- **Results:** See `get-single-video.json`

### 4. Create Video
- **Endpoint:** `POST /api/videos`
- **Purpose:** Write-heavy workload
- **Results:** See `post-videos.json`

### 5. Mixed Workload
- **Workload:** 70% reads, 30% writes
- **Purpose:** Real-world traffic simulation
- **Results:** See `mixed-reads.json` and `mixed-writes.json`

## Analysis

### Throughput
- **Expected:** 50-100 req/s under normal load
- **Achieved:** See individual test results

### Latency
- **Target p95:** < 500ms
- **Target p99:** < 1000ms
- **Achieved:** See individual test results

### Error Rate
- **Target:** < 5%
- **Achieved:** See individual test results

## Recommendations

Based on the test results:

1. **If p95 latency > 500ms:** Consider adding more cache layers or optimizing database queries
2. **If error rate > 5%:** Check logs for failure patterns and improve error handling
3. **If throughput < expected:** Profile the application to identify bottlenecks
4. **If cache hit rate < 80%:** Review caching strategy and TTL values

## Next Steps

- [ ] Run tests with different load profiles (spike, stress, soak)
- [ ] Compare results with previous runs to detect regressions
- [ ] Optimize identified bottlenecks
- [ ] Re-run tests to validate improvements

---

**Test Environment:**
- OS: $(uname -s)
- Bombardier Version: $(bombardier --version 2>&1 | head -1)
- API Version: See API /health endpoint
EOF

echo -e "${GREEN}Performance report generated: $REPORT_FILE${NC}"
echo ""
echo -e "${BLUE}Done! 🚀${NC}"
