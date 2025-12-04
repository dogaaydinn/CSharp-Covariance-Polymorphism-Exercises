# Solution A: Quick Fix - IP-Based In-Memory Rate Limiting

> **Time to implement:** 1 hour  
> **Difficulty:** Beginner  
> **Pros:** Fast to ship, stops the immediate attack  
> **Cons:** Doesn't scale across servers, basic protection only

---

## The Strategy

**"Ship something NOW to stop the bleeding, improve later."**

This solution uses in-memory caching to track request counts per IP address. It's not perfect, but it will stop the attack within the hour.

---

## Implementation

### Step 1: Create Rate Limiting Middleware

```csharp
public class SimpleRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> _requestCounts = new();
    private const int MaxRequestsPerMinute = 100;

    public SimpleRateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Get or create counter for this IP
        var (count, resetTime) = _requestCounts.GetOrAdd(ipAddress, _ => (0, DateTime.UtcNow.AddMinutes(1)));

        // Reset counter if time window expired
        if (DateTime.UtcNow > resetTime)
        {
            _requestCounts[ipAddress] = (1, DateTime.UtcNow.AddMinutes(1));
            await _next(context);
            return;
        }

        // Increment counter
        var newCount = count + 1;
        _requestCounts[ipAddress] = (newCount, resetTime);

        // Check if over limit
        if (newCount > MaxRequestsPerMinute)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.Add("Retry-After", "60");
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Maximum {MaxRequestsPerMinute} requests per minute allowed",
                retryAfter = "60 seconds"
            });
            return;
        }

        // Allow request
        await _next(context);
    }
}
```

### Step 2: Register Middleware in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// ... other services ...

var app = builder.Build();

// Add rate limiting BEFORE routing
app.UseMiddleware<SimpleRateLimitingMiddleware>();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Step 3: Add Cleanup Job (Prevent Memory Leak)

```csharp
// In Program.cs, add background service
app.Services.GetRequiredService<IHostApplicationLifetime>()
    .ApplicationStarted.Register(() =>
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                
                // Clean up old entries
                var now = DateTime.UtcNow;
                var oldEntries = _requestCounts
                    .Where(kvp => kvp.Value.ResetTime < now)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in oldEntries)
                {
                    _requestCounts.TryRemove(key, out _);
                }
            }
        });
    });
```

---

## Testing It

### Test 1: Normal Usage (Should Work)

```bash
# Send 50 requests (under limit)
for i in {1..50}; do
    curl http://localhost:5000/api/products
done

# All should return 200 OK
```

### Test 2: Attack Simulation (Should Block)

```bash
# Send 150 requests (over limit)
for i in {1..150}; do
    curl http://localhost:5000/api/products
done

# First 100 return 200 OK
# Next 50 return 429 Too Many Requests
```

### Test 3: Wait and Retry (Should Reset)

```bash
# Wait 61 seconds
sleep 61

# Try again
curl http://localhost:5000/api/products
# Should return 200 OK (counter reset)
```

---

## Deployment

### 1. Deploy to Production

```bash
# Build and deploy
dotnet publish -c Release
# Deploy to all 3 servers
```

### 2. Monitor Immediately

```bash
# Watch for 429 responses
grep "429" /var/log/api/*.log | wc -l

# Check if legitimate users affected
grep "429" /var/log/api/*.log | grep "UserAgent: MobileApp"
```

### 3. Communicate to Team

**Slack Message:**
> "✅ Rate limiting deployed. Currently blocking IPs that exceed 100 req/min. Monitoring for false positives. If any legitimate users report issues, please ping me immediately."

---

## What This Solves

✅ **Immediate Attack Stopped**
- Attacker's 10,000 req/min now blocked after 100 requests
- Database CPU drops from 98% to 30%
- Legitimate users can access the site again

✅ **Simple & Fast**
- 1 hour to implement and deploy
- No new infrastructure needed
- Easy to understand and debug

---

## What This DOESN'T Solve

❌ **Doesn't Work Across Servers**
- Each of your 3 servers has its own counter
- Attacker can send 100 req/min to EACH server = 300 total
- Load balancer distributes requests, so limits are effectively 3x

❌ **No User-Based Limits**
- Corporate offices (NAT) have same IP for 1000 employees
- All 1000 employees share the 100 req/min limit!
- Premium users can't get higher limits

❌ **No Configurability**
- Limit hardcoded to 100 req/min
- Changing it requires redeploying code
- Can't whitelist monitoring tools

❌ **Memory Grows Over Time**
- Dictionary keeps growing with new IPs
- Cleanup job runs every 5 minutes (could be sooner)
- Under attack, could use 100MB+ RAM

---

## When to Use This Approach

### ✅ Use When:

1. **Emergency situation** - Need something deployed NOW
2. **Single server** - If you only have 1 API server (rare)
3. **Prototypes** - Quick proof-of-concept before building proper solution
4. **Low traffic** - <10,000 users with predictable patterns

### ❌ Don't Use When:

1. **Multiple servers** - Limits are per-server, not global
2. **Large scale** - Millions of users, need centralized tracking
3. **Complex policies** - Different limits per user type
4. **Production system** - Needs proper distributed solution

---

## Evolution Path

**Week 1 (Now):** Deploy this solution to stop the attack  
**Week 2:** Implement SOLUTION-B (Redis-based) for proper distributed rate limiting  
**Month 3:** Consider SOLUTION-C (API Gateway) if scaling further

---

## Real-World Usage

**Where this approach is actually used:**
- Internal admin tools (not internet-facing)
- Development/staging environments
- Quick prototypes before production implementation
- Single-server applications (rare)

**Companies that outgrew this:**
- Twitter: Started with in-memory, moved to Redis, then custom distributed system
- GitHub: Uses Redis-based rate limiting
- Stripe: Custom distributed rate limiting with Consul

---

## Code Quality Improvements

If you have an extra 30 minutes, add:

### 1. Configuration

```csharp
public class RateLimitOptions
{
    public int MaxRequestsPerMinute { get; set; } = 100;
    public int CleanupIntervalMinutes { get; set; } = 5;
}

// In appsettings.json:
{
  "RateLimit": {
    "MaxRequestsPerMinute": 100,
    "CleanupIntervalMinutes": 5
  }
}
```

### 2. Logging

```csharp
if (newCount > MaxRequestsPerMinute)
{
    _logger.LogWarning("Rate limit exceeded for IP {IpAddress}. Count: {Count}", 
        ipAddress, newCount);
    // ... return 429 ...
}
```

### 3. Metrics

```csharp
// Using App Insights or Prometheus
_metrics.IncrementCounter("rate_limit_exceeded", new { ip = ipAddress });
```

---

## Rollback Plan

If this breaks something:

1. **Remove middleware line from Program.cs:**
   ```csharp
   // app.UseMiddleware<SimpleRateLimitingMiddleware>();
   ```

2. **Redeploy:**
   ```bash
   dotnet publish -c Release
   # Deploy to all servers
   ```

3. **Time to rollback:** 5 minutes

---

## Next Steps

1. ✅ Deploy this solution NOW to stop the attack
2. ⏳ Monitor for 24 hours to ensure no false positives
3. 📅 Schedule SOLUTION-B implementation for next sprint
4. 📊 Gather metrics on blocked requests to justify SOLUTION-C

**Read next:** `SOLUTION-B.md` to see the proper production approach.

---

## Key Takeaway

> "Perfect is the enemy of done. Ship the simple solution fast, then improve it based on real data."

This solution isn't perfect, but it solves the immediate business problem (stop the attack) in 1 hour. That's often the right answer in a crisis.
