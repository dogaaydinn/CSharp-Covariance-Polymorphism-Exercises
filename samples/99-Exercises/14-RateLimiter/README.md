# Rate Limiter

## Problem
Implement token bucket rate limiter.

## API
```csharp
interface IRateLimiter
{
    bool AllowRequest(string userId);
}
```

## Example
```csharp
var limiter = new TokenBucketLimiter(capacity: 10, refillRate: 1.0); // 1 token/sec

limiter.AllowRequest("user1") => true  // 9 tokens left
limiter.AllowRequest("user1") => true  // 8 tokens left
// ... 8 more requests ...
limiter.AllowRequest("user1") => false // No tokens left

// Wait 1 second, 1 token refilled
limiter.AllowRequest("user1") => true  // Success
```

## Algorithm
1. Each user has a bucket with N tokens
2. Tokens refill at rate R tokens/second
3. Each request consumes 1 token
4. Reject if no tokens available
