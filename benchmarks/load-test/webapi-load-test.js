// k6 Load Test for AspireVideoService API
// Run: k6 run webapi-load-test.js
// Run with reports: k6 run --out json=results.json webapi-load-test.js

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const videosGetTrend = new Trend('videos_get_duration');
const videosPostTrend = new Trend('videos_post_duration');
const healthCheckTrend = new Trend('health_check_duration');
const cacheHits = new Counter('cache_hits');

// Test configuration
export const options = {
    stages: [
        { duration: '30s', target: 20 },  // Ramp-up to 20 users
        { duration: '1m', target: 50 },   // Spike to 50 users
        { duration: '2m', target: 50 },   // Stay at 50 users
        { duration: '30s', target: 100 }, // Spike to 100 users
        { duration: '1m', target: 100 },  // Stay at 100 users
        { duration: '30s', target: 0 },   // Ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(95)<500', 'p(99)<1000'], // 95% of requests under 500ms, 99% under 1s
        http_req_failed: ['rate<0.05'],                  // Error rate must be below 5%
        errors: ['rate<0.05'],                           // Custom error rate below 5%
        videos_get_duration: ['p(95)<300'],              // GET requests under 300ms
        videos_post_duration: ['p(95)<500'],             // POST requests under 500ms
        health_check_duration: ['p(95)<100'],            // Health checks under 100ms
    },
};

const BASE_URL = __ENV.API_URL || 'http://localhost:5000';

// Test data
const testVideos = [
    { title: 'Performance Test Video 1', description: 'Load testing with k6', duration: 120 },
    { title: 'Performance Test Video 2', description: 'API stress testing', duration: 180 },
    { title: 'Performance Test Video 3', description: 'Cache validation test', duration: 240 },
];

export function setup() {
    // Setup phase: Create initial test data
    console.log(`Starting load test against ${BASE_URL}`);

    // Check if API is reachable
    const healthCheck = http.get(`${BASE_URL}/health`);
    if (healthCheck.status !== 200) {
        throw new Error(`API health check failed: ${healthCheck.status}`);
    }

    console.log('API is healthy, starting test scenarios...');
    return { baseUrl: BASE_URL };
}

export default function (data) {
    const baseUrl = data.baseUrl;

    // Scenario 1: Health check (lightweight)
    group('Health Check', () => {
        const start = Date.now();
        const res = http.get(`${baseUrl}/health`);
        const duration = Date.now() - start;

        healthCheckTrend.add(duration);

        const success = check(res, {
            'health check status is 200': (r) => r.status === 200,
            'health check response time < 100ms': () => duration < 100,
        });

        errorRate.add(!success);
    });

    sleep(1);

    // Scenario 2: Get all videos (tests caching)
    group('Get All Videos', () => {
        const start = Date.now();
        const res = http.get(`${baseUrl}/api/videos`, {
            headers: { 'Accept': 'application/json' },
        });
        const duration = Date.now() - start;

        videosGetTrend.add(duration);

        const success = check(res, {
            'get all videos status is 200': (r) => r.status === 200,
            'get all videos response time < 500ms': () => duration < 500,
            'response is JSON': (r) => r.headers['Content-Type']?.includes('application/json'),
            'response has data': (r) => r.body.length > 0,
        });

        // Check for cache header (X-Cache)
        if (res.headers['X-Cache'] === 'HIT') {
            cacheHits.add(1);
        }

        errorRate.add(!success);
    });

    sleep(1);

    // Scenario 3: Create new video (write operation)
    group('Create Video', () => {
        const video = testVideos[Math.floor(Math.random() * testVideos.length)];
        const payload = JSON.stringify(video);

        const start = Date.now();
        const res = http.post(`${baseUrl}/api/videos`, payload, {
            headers: { 'Content-Type': 'application/json' },
        });
        const duration = Date.now() - start;

        videosPostTrend.add(duration);

        const success = check(res, {
            'create video status is 201': (r) => r.status === 201,
            'create video response time < 1000ms': () => duration < 1000,
            'response has id': (r) => {
                try {
                    const body = JSON.parse(r.body);
                    return body.id > 0;
                } catch {
                    return false;
                }
            },
        });

        errorRate.add(!success);

        // If creation succeeded, try to get it (test cache invalidation)
        if (res.status === 201) {
            sleep(0.5);

            try {
                const createdVideo = JSON.parse(res.body);
                const getRes = http.get(`${baseUrl}/api/videos/${createdVideo.id}`);

                check(getRes, {
                    'get created video status is 200': (r) => r.status === 200,
                    'retrieved video matches created': (r) => {
                        try {
                            const retrieved = JSON.parse(r.body);
                            return retrieved.title === video.title;
                        } catch {
                            return false;
                        }
                    },
                });
            } catch (e) {
                console.error('Failed to retrieve created video:', e);
            }
        }
    });

    sleep(2);

    // Scenario 4: Get single video by ID (tests caching)
    group('Get Single Video', () => {
        // Try to get video with ID 1 (should exist after some writes)
        const res = http.get(`${baseUrl}/api/videos/1`);

        const success = check(res, {
            'get single video status is 200 or 404': (r) => r.status === 200 || r.status === 404,
            'get single video response time < 300ms': (r) => r.timings.duration < 300,
        });

        if (res.status === 200 && res.headers['X-Cache'] === 'HIT') {
            cacheHits.add(1);
        }

        errorRate.add(!success);
    });

    sleep(1);
}

export function teardown(data) {
    // Cleanup phase
    console.log('Load test completed');
    console.log(`Total cache hits: ${cacheHits.value || 0}`);
}

export function handleSummary(data) {
    // Custom summary output
    const summary = {
        'stdout': textSummary(data, { indent: ' ', enableColors: true }),
        'summary.json': JSON.stringify(data, null, 2),
        'summary.html': htmlReport(data),
    };

    return summary;
}

function textSummary(data, options) {
    // Generate text summary
    const indent = options?.indent || '';
    const colors = options?.enableColors || false;

    let output = '\n' + indent + '======= Load Test Summary =======\n\n';

    // Overall stats
    if (data.metrics.http_reqs) {
        const totalReqs = data.metrics.http_reqs.values.count;
        const reqRate = data.metrics.http_reqs.values.rate;
        output += indent + `Total Requests: ${totalReqs}\n`;
        output += indent + `Requests/sec: ${reqRate.toFixed(2)}\n\n`;
    }

    // Duration stats
    if (data.metrics.http_req_duration) {
        const duration = data.metrics.http_req_duration.values;
        output += indent + 'Request Duration:\n';
        output += indent + `  avg: ${duration.avg.toFixed(2)}ms\n`;
        output += indent + `  p(95): ${duration['p(95)'].toFixed(2)}ms\n`;
        output += indent + `  p(99): ${duration['p(99)'].toFixed(2)}ms\n\n`;
    }

    // Error rate
    if (data.metrics.http_req_failed) {
        const errorRate = (data.metrics.http_req_failed.values.rate * 100).toFixed(2);
        output += indent + `Error Rate: ${errorRate}%\n\n`;
    }

    // Custom metrics
    if (data.metrics.cache_hits) {
        output += indent + `Cache Hits: ${data.metrics.cache_hits.values.count}\n\n`;
    }

    output += indent + '==================================\n';

    return output;
}

function htmlReport(data) {
    // Generate HTML report
    const totalReqs = data.metrics.http_reqs?.values.count || 0;
    const reqRate = data.metrics.http_reqs?.values.rate || 0;
    const avgDuration = data.metrics.http_req_duration?.values.avg || 0;
    const p95Duration = data.metrics.http_req_duration?.values['p(95)'] || 0;
    const errorRate = (data.metrics.http_req_failed?.values.rate || 0) * 100;

    return `
<!DOCTYPE html>
<html>
<head>
    <title>k6 Load Test Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
        .container { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h1 { color: #333; border-bottom: 3px solid #7d64ff; padding-bottom: 10px; }
        .metric { margin: 20px 0; padding: 15px; background: #f9f9f9; border-left: 4px solid #7d64ff; }
        .metric h3 { margin: 0 0 10px 0; color: #555; }
        .metric .value { font-size: 32px; font-weight: bold; color: #7d64ff; }
        .metric .unit { font-size: 14px; color: #999; }
        .pass { color: #28a745; }
        .fail { color: #dc3545; }
        .warn { color: #ffc107; }
    </style>
</head>
<body>
    <div class="container">
        <h1>🚀 k6 Load Test Report - AspireVideoService API</h1>

        <div class="metric">
            <h3>Total Requests</h3>
            <div class="value">${totalReqs}</div>
        </div>

        <div class="metric">
            <h3>Throughput</h3>
            <div class="value">${reqRate.toFixed(2)} <span class="unit">req/s</span></div>
        </div>

        <div class="metric">
            <h3>Average Response Time</h3>
            <div class="value">${avgDuration.toFixed(2)} <span class="unit">ms</span></div>
        </div>

        <div class="metric">
            <h3>95th Percentile</h3>
            <div class="value ${p95Duration < 500 ? 'pass' : 'warn'}">${p95Duration.toFixed(2)} <span class="unit">ms</span></div>
        </div>

        <div class="metric">
            <h3>Error Rate</h3>
            <div class="value ${errorRate < 5 ? 'pass' : 'fail'}">${errorRate.toFixed(2)} <span class="unit">%</span></div>
        </div>

        <p style="margin-top: 40px; color: #999; font-size: 12px;">
            Generated: ${new Date().toISOString()}<br>
            k6 version: ${__ENV.K6_VERSION || 'unknown'}
        </p>
    </div>
</body>
</html>
    `;
}
