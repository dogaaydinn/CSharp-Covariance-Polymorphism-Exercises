# Real-World Problem: Cascading Failures in Microservices

## The Situation

**Date:** Friday, 5:45 PM  
**From:** On-Call Engineer  
**PagerDuty Alert:**
> "🚨 P1: Order Service DOWN - 500 errors spiking. Payment Service responding slow (10s timeout). Affecting checkout flow. Revenue impact: CRITICAL"

**What you discover:**
```
Order Service → calls → Payment Service
                         ↓ (timeout after 10s)
                         Payment Service → calls → Bank API
                                                    ↓ (Bank API is down!)
                                                    Bank API: 503 Service Unavailable
```

**The problem:**
- Bank API is down (not your fault)
- Your Payment Service waits 10 seconds per request (blocking)
- Your Order Service waits for Payment Service (cascading failure)
- 100 requests queued → 100 threads blocked → server runs out of threads
- **Entire checkout flow is DOWN!**

**Current Code (Order Service):**
```csharp
public async Task<IActionResult> CreateOrder(OrderRequest request)
{
    // This call can take 10 seconds if Payment Service is slow!
    var paymentResult = await _httpClient.PostAsync("http://payment-service/charge", ...);
    
    if (paymentResult.IsSuccessStatusCode)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    return BadRequest("Payment failed"); // User sees error, order not created
}
```

---

## The Business Context

**Your Microservice Architecture:**
```
Frontend → API Gateway → Order Service → Payment Service → Bank API
                        ↓
                        Inventory Service
                        ↓
                        Notification Service
```

**SLA Requirements:**
- Order creation: 95% success rate
- Response time: <2 seconds
- Even when downstream services fail!

**Current Reality:**
- Payment Service down → Order Service down
- Order Service down → Users can't checkout
- **Revenue loss: $10,000/hour**

---

## Your Task

Implement resilience patterns to:
1. Prevent cascading failures
2. Degrade gracefully (create order even if payment service slow)
3. Retry failed requests intelligently
4. Circuit break when service is down

**Time:** You have until Monday to fix (it's Friday evening)

---

## Solutions

1. **SOLUTION-A.md** - Retry with Exponential Backoff (Polly)
2. **SOLUTION-B.md** - Circuit Breaker Pattern
3. **SOLUTION-C.md** - Timeout + Fallback + Queue
4. **DECISION-TREE.md** - Which resilience pattern for which scenario?

**Next:** Read SOLUTION-A.md for immediate fixes.
